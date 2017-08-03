using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;
using System.Configuration;
using conexionBaseDatos;
using Tool;

namespace wssPortalEgresos
{
    /// <summary>
    /// Descripción breve de Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        string CONSULTA_OK = "0";
        string ERROR_RUT_VACIO = "1";

        string ERROR_AGREGAR_RUT_RECE = "2";
        string ERROR_HABILITAR_RUT_RECE = "3";
        string ERROR_QUITAR_RUT_RECE = "4";
        string ERROR_DESHABILITAR_RUT_RECE = "5";

        string ERROR_EMISOR_INVALIDO_EMIS = "6";

        [WebMethod]
        public Mensaje SupplierTrasETD(int Rut)
        {
            string sql = "", xml = "", xmlBase64 = "", corr_docu = "", sEmex = "";
            byte[] bytes;
            string codi_empr = "";
            DataTable result;
            Mensaje mens = new Mensaje();
            log logs = new log();
            logs.nombreLog = "WssSupplierETD";
            logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);

            if (Rut == 0)
            {
                mens.Codigo = "ER0";
                mens.Mensajes = "Rut No puede estar vacio";
                logs.putLog(1, mens.Mensajes);
                return mens;
            }

            if (ConfigurationManager.AppSettings["eHome:" + Rut.ToString()] != null)
            {
                logs.egateHome = ConfigurationManager.AppSettings["eHome:" + Rut.ToString()];
            }
            else
            {
                logs.egateHome = "EGATE_HOME";
                logs.putLog(1, "Empresa no registra Home: " + Rut);
            }

            bdConexion conexion = new bdConexion();
            try
            {
                conexion.egateHome = logs.egateHome;
                conexion.conexionOpen();
                sEmex = conexion.Emex;

                if (!string.IsNullOrEmpty(sEmex))
                    sql = "select codi_empr from empr_hold where rutt_empr = {0}";
                else
                    sql = "select codi_empr from empr where rutt_empr = {0} ";

                codi_empr = conexion.SelectInto(String.Format(sql, Rut));
                if (string.IsNullOrEmpty(codi_empr))
                {
                    mens.cantDTE = 0;
                    mens.DTE = "";
                    mens.Codigo = "ER1";
                    mens.Mensajes = "Empresa no se encuentra configurada";
                    return mens;
                }

                sql = "";
                if (conexion.baseDatos.CompareTo("oracle".ToLower()) == 0)
                {
                    sql = "SELECT nvl(rutt_emis, 0), nvl(tipo_docu,0), nvl(foli_docu,0), corr_docu, nvl(digi_emis,'0') ";
                    sql += "FROM  dto_enca_docu_p ";
                    sql += "WHERE codi_empr = {1} ";
                    sql += "AND   esta_docu in ('INI','ERA') ";
                    sql += "AND   esta_tras is null ";
                    sql += "AND   rownum < 2 ";
                    sql += "ORDER BY fech_carg asc";
                }
                else
                {
                    // Si es CLOUD, se rescata desde la tabla HOLD
                    if (!string.IsNullOrEmpty(sEmex))
                    {
                        sql = "SELECT top 1 isnull(rutt_emis, 0), isnull(tipo_docu,0), isnull(foli_docu,0), corr_docu, isnull(digi_emis,'0') ";
                        sql += "FROM  dto_enca_docu_p_hold WITH (NOLOCK) ";
                        sql += "WHERE codi_emex='{0}'  ";
                        sql += "AND   codi_empr={1} ";
                        sql += "AND   esta_docu in ('INI','ERA') ";
                        sql += "AND   esta_tras is null ";
                        sql += "ORDER BY fech_carg asc";
                    }
                    else
                    {
                        sql = "SELECT top 1 isnull(rutt_emis, 0), isnull(tipo_docu,0), isnull(foli_docu,0), corr_docu, isnull(digi_emis,'0') ";
                        sql += "FROM  dto_enca_docu_p WITH (NOLOCK) ";
                        sql += "WHERE codi_empr = {1} ";
                        sql += "AND   esta_docu in ('INI','ERA') ";
                        sql += "AND   esta_tras is null ";
                        sql += "ORDER BY fech_carg asc";
                    }
                }
                result = conexion.EjecutaSelect(String.Format(sql, sEmex, codi_empr));
                if (result.Rows.Count == 0)
                {
                    mens.cantDTE = 0;
                    mens.DTE = "";
                    mens.Codigo = "DON";
                    mens.Mensajes = "No existen documentos a pendientes";
                    return mens;
                }
                else
                {
                    mens.RuttEmis = result.Rows[0][0].ToString() + "-" + result.Rows[0][4].ToString();
                    mens.TipoDoc = result.Rows[0][1].ToString();
                    if (result.Rows[0][2].ToString().Contains(".0"))
                        mens.FolioDoc = result.Rows[0][2].ToString().Remove(result.Rows[0][2].ToString().IndexOf(".0"), 2);
                    else
                        mens.FolioDoc = result.Rows[0][2].ToString();
                    corr_docu = result.Rows[0][3].ToString();

                    if (!string.IsNullOrEmpty(sEmex))
                        sql = "select clob_docu from dto_docu_lob_hold WITH (NOLOCK) where codi_emex = '{0}' and corr_docu = {1} and tipo_arch = 'XML'";
                    else
                        sql = "select clob_docu from dto_docu_lob where corr_docu = {1} and tipo_arch = 'XML'";

                    xml = conexion.SelectText(String.Format(sql, sEmex, corr_docu));
                    xml.Trim();
                    if (!string.IsNullOrEmpty(xml))
                    {
                        mens.Codigo = "DOK";
                        mens.Mensajes = "Documento Retornado OK";
                        if (!EsBase64(xml.TrimEnd()))
                        {
                            bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(xml.TrimEnd());
                            xmlBase64 = Convert.ToBase64String(bytes);
                        }
                        else
                        {
                            xmlBase64 = xml.TrimEnd();
                        }
                        mens.DTE = xmlBase64.TrimEnd();
                    }
                    else
                    {
                        mens.Codigo = "ER2";
                        mens.Mensajes = "Documento NO tiene XML disponible, correlativo: " + corr_docu;
                        mens.DTE = "";
                    }

                    if (!string.IsNullOrEmpty(sEmex))
                        sql = "update dto_enca_docu_p_hold set esta_tras = 'TRA' where codi_emex = '{0}' and corr_docu = {1}";
                    else
                        sql = "update dto_enca_docu_p set esta_tras = 'TRA' where corr_docu = {1}";
                    if (conexion.EjecutaNonQuery(String.Format(sql, sEmex, corr_docu)) == 0)
                    {
                        mens.Codigo = "ER3";
                        mens.Mensajes = "No se puede cambiar estado a Documento Correlativo: " + corr_docu;
                        mens.DTE = "";
                    }
                    else
                    {
                        conexion.confirma();
                    }
                    if (!string.IsNullOrEmpty(sEmex))
                    {
                        sql = "SELECT count(*) ";
                        sql += "FROM  dto_enca_docu_p_hold WITH (NOLOCK) ";
                        sql += "WHERE codi_emex = '{0}' ";
                        sql += "AND   codi_empr = {1} ";
                        sql += "AND   esta_docu in ('INI','ERA') ";
                        sql += "AND   esta_tras is null ";
                    }
                    else
                    {
                        sql = "SELECT count(*) ";
                        sql += "FROM  dto_enca_docu_p /*WITH (NOLOCK)*/ ";
                        sql += "WHERE codi_empr = {1} ";
                        sql += "AND   esta_docu in ('INI','ERA') ";
                        sql += "AND   esta_tras is null ";
                    }
                    mens.cantDTE = Convert.ToInt32(conexion.SelectInto(String.Format(sql, sEmex, codi_empr)));
                }
            }
            catch (Exception ex)
            {
                conexion.rechaza();
                logs.putLog(1, Convert.ToString(ex.Message));
                mens.cantDTE = 0;
                mens.DTE = "";
                mens.Codigo = "ERR";
                mens.Mensajes = "Problemas en el proceso";
            }
            finally
            {
                conexion.closeConexion();
            }
            return mens;
        }

        private static bool EsBase64(string str)
        {
            if (str.IndexOf("<") > 0 && str.IndexOf(">") > 0)
                return false;
            else
                return true;
        }


        #region Agregar/Quitar Emisor
        [WebMethod]
        public Respuesta AgregarRutEmisor(int Rut, int digiVeri, string nombre)
        {
            Respuesta resp = new Respuesta();
            return resp;
        }


        [WebMethod]
        public Respuesta QuitarRutEmisor(int Rut, int digiVeri, string nombre)
        {
            Respuesta resp = new Respuesta();
            return resp;
        }
        #endregion


        #region Agregar/Quitar Receptor
        // Rut -> Mensaje
        [WebMethod]
        public Respuesta AgregarRutReceptor(int Rut, int digiVeri, string nombre)
        {
            string sql = string.Empty;
            string xml = string.Empty;
            string xmlBase64 = string.Empty;
            string corr_docu = string.Empty;
            string sEmex = string.Empty;
            string codi_empr = string.Empty;

            Respuesta resp = new Respuesta();

            log logs = new log();
            logs.nombreLog = "AgregarRutReceptor";
            logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);

            if (!validaRut(Rut, resp))
            {
                return resp;
            }

            setEgateHome(Rut, logs);

            bdConexion conexion = new bdConexion();
            try
            {
                conexion.egateHome = logs.egateHome;
                conexion.conexionOpen();

                /*if (!existeEmpresa(Rut, out codi_empr, conexion, resp))
                {
                    return resp;
                }*/

                if (!existeReceptor(Rut, conexion, resp))
                {
                    agregarReceptor(Rut, digiVeri, nombre, conexion, resp);
                }
                else
                {
                    habilitarReceptor(Rut, conexion, resp);
                }

            }
            catch (Exception ex)
            {
            }
            return resp;
        }

        // Rut -> Mensaje
        [WebMethod]
        public Respuesta QuitarRutReceptor(int Rut, int digiVeri, string nombre)
        {
            string sql = string.Empty;
            string xml = string.Empty;
            string xmlBase64 = string.Empty;
            string corr_docu = string.Empty;
            string sEmex = string.Empty;
            string codi_empr = string.Empty;

            Respuesta resp = new Respuesta();

            log logs = new log();
            logs.nombreLog = "QuitarRutReceptor";
            logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);

            if(!validaRut(Rut, resp))
            {
                return resp;
            }

            setEgateHome(Rut, logs);

            bdConexion conexion = new bdConexion();
            try
            {
                conexion.egateHome = logs.egateHome;
                conexion.conexionOpen();

                /*if (!existeEmpresa(Rut, out codi_empr, conexion, resp))
                {
                    return resp;
                }*/

                if (!existeReceptor(Rut, conexion, resp))
                {
                    quitarReceptor(Rut, digiVeri, nombre, conexion, resp);
                }
                else
                {
                    deshabilitarReceptor(Rut, conexion, resp);
                }

            }
            catch (Exception ex)
            {
            }
            return resp;
        }

        // Rut -> Boolean
        private static bool existeRutReceptor(int rut)
        {
            string sql = string.Empty;
            string codi_empr = string.Empty;
            DataTable result;

            bdConexion conexion = new bdConexion();
            return false;
        }
        #endregion

        #region private methods Receptor
        private static string tipoBD()
        {
            return "";
        }

        private static Boolean Emex()
        {
            return false;
        }

        private Boolean validaRut(int Rut, Respuesta resp)
        {
            Boolean result = true;
            if (Rut == 0)
            {
                resp.SCodigo = ERROR_RUT_VACIO;
                resp.SMensaje = "Rut No puede estar vacio";
                result = false;
                // FIX logs.putLog(1, resp.SMensaje);
                //return resp;
            }
            return result;
        }

        private void setEgateHome(int Rut, log logs)        
        {
            if (ConfigurationManager.AppSettings["eHome:" + Rut.ToString()] != null)
            {
                logs.egateHome = ConfigurationManager.AppSettings["eHome:" + Rut.ToString()];
            }
            else
            {
                logs.egateHome = "EGATE_HOME";
                logs.putLog(1, "Empresa no registra Home: " + Rut);
            }
        }

        private Boolean existeEmpresa(int Rut, out string codi_empr, bdConexion conexion, Respuesta resp)
        {
            Boolean result = true;
            string sql = string.Empty;

            if (!string.IsNullOrEmpty(conexion.Emex))
            {
                sql = "select codi_empr from empr_hold where rutt_empr = {0}";
            }
            else
            {
                sql = "select codi_empr from empr where rutt_empr = {0} ";
            }

            codi_empr = conexion.SelectInto(String.Format(sql, Rut));


            if (string.IsNullOrEmpty(codi_empr))
            {
                resp.SMensaje = "Empresa no se encuentra configurada";
                result = false;
            }
            return result;
        }

        private Boolean existeReceptor(int Rut, bdConexion conexion, Respuesta resp)
        {
            Boolean result = true;
            string sql = "SELECT COUNT(*) FROM PERSONAS WHERE RUTT_PERS = '{0}'";
            int cantPersonas = Convert.ToInt32(conexion.SelectInto(String.Format(sql, Rut)));
            if(cantPersonas == 0)
            {
                result = false;
                resp.SMensaje = "Rut no se encuentra";
            }
            return result;
        }

        private void agregarReceptor(int Rut, int digiVeri, string nombre, bdConexion conexion, Respuesta resp)
        {
            try
            {
                string sql = "INSERT INTO PERSONAS (RUTT_PERS, DGTO_PERS, NOMB_PERS, INDI_WSS) VALUES ({0}, {1}, '{2}', 'S')";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut, digiVeri, nombre)) == 0)
                {
                    resp.SCodigo = ERROR_AGREGAR_RUT_RECE;
                    resp.SMensaje = "No se pudo agregar Rut en receptores";
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se inserta Rut en receptores";
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void habilitarReceptor(int Rut, bdConexion conexion, Respuesta resp)
        {
            try
            {
                string sql = "UPDATE PERSONAS SET INDI_WSS = 'S' WHERE RUTT_PERS = {0} ";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut)) == 0)
                {
                    resp.SCodigo = ERROR_HABILITAR_RUT_RECE;
                    resp.SMensaje = "No se pudo habilitar Rut en receptores";
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se habilita Rut en receptores";
                }
            }
            catch (Exception ex)
            {
            }
        }
    
        private void quitarReceptor(int Rut, int digiVeri, string nombre, bdConexion conexion, Respuesta resp){
            try
            {
                string sql = "INSERT INTO PERSONAS (RUTT_PERS, DGTO_PERS, NOMB_PERS, INDI_WSS) VALUES ({0}, {1}, '{2}', 'N')";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut, digiVeri, nombre)) == 0)
                {
                    resp.SCodigo = ERROR_QUITAR_RUT_RECE;
                    resp.SMensaje = "No se pudo quitar Rut en receptores";
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se quita Rut en receptores";
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void deshabilitarReceptor(int Rut, bdConexion conexion, Respuesta resp)
        {
            try
            {
                string sql = "UPDATE PERSONAS SET INDI_WSS = 'N' WHERE RUTT_PERS = {0} ";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut)) == 0)
                {
                    resp.SCodigo = ERROR_DESHABILITAR_RUT_RECE;
                    resp.SMensaje = "No se pudo deshabilitar Rut en receptores";
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se deshabilita Rut en receptores";
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        #endregion
    }
}