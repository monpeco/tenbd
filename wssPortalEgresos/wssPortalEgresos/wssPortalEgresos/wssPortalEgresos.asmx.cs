using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;
using System.Configuration;
using conexionBaseDatos;
using Tool;

using System.Xml;
using System.Text;


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

        string ERROR_MARCAR_DTE_TRASPASADO = "7";

        string _sRutaPdf = string.Empty;
        string _sEgateHome = string.Empty;


        #region marcarDTETraspasado
        [WebMethod]
        public Respuesta marcarDTETraspasado(string sCodEmpr, string sTipoDocu, string sFoliDocu, string sRuttEmis)
        {
            Respuesta resp = new Respuesta();
            log logs = new log();
            logs.nombreLog = "marcarDTETraspasado";
            logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);

            if (!validaRut(Convert.ToInt32(sRuttEmis), resp, logs))
            {
                return resp;
            }

            setEgateHome(Convert.ToInt32(sRuttEmis), logs);

            logs.putLog(1, "");
            logs.putLog(1, "INICIO marcarDTETraspasado");

            bdConexion conexion = new bdConexion();
            try
            {
                conexion.egateHome = logs.egateHome;
                conexion.conexionOpen();
                logs.putLog(1, "conexionOpen(): OK");

                marcarDTE(sCodEmpr, sTipoDocu, sFoliDocu, sRuttEmis, conexion, resp, logs);
                logs.putLog(1, "marcarDTE finaliza normalmente");
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: " + logs.nombreLog);
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }

            return resp;
        }

        private void marcarDTE(string sCodEmpr, string sTipoDocu, string sFoliDocu, string sRuttEmis, bdConexion conexion, Respuesta resp, log logs)
        {
            try
            {
                string sql = " UPDATE DTO_ENCA_DOCU_P SET ESTA_TRAS = 'TRA' ";
                sql += " WHERE CODI_EMPR = {0} ";
                sql += " AND TIPO_DOCU = {1} ";
                sql += " AND FOLI_DOCU = {2} ";
                sql += " AND RUTT_EMIS = {3} ";

                if (conexion.EjecutaNonQuery(String.Format(sql, sCodEmpr, sTipoDocu, sFoliDocu, sRuttEmis)) == 0)
                {
                    resp.SCodigo = ERROR_MARCAR_DTE_TRASPASADO;
                    resp.SMensaje = "No se pudo marcar el DTE como traspasado";
                    logs.putLog(1, "-- No se pudo marcar el DTE como traspasado");
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se marca exitosamente DTE como traspasado";
                    logs.putLog(1, "-- Se marca exitosamente DTE como traspasado");
                }
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: marcarDTE");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }
        }

        #endregion

        #region entregarDTEPendientes
        [WebMethod]
        public DTEPendietes entregarDTEPendientes()
        {
            DTEPendietes dtes = new DTEPendietes();
            log logs = new log();
            logs.nombreLog = "entregarDTEPendientes";
            logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);

            setEgateHome(000, logs);
            logs.putLog(1, "");
            logs.putLog(1, "INICIO entregarDTEPendientes");

            bdConexion conexion = new bdConexion();
            try
            {
                conexion.egateHome = logs.egateHome;
                conexion.conexionOpen();
                logs.putLog(1, "conexionOpen(): OK" );
                
                int cantDTERecuperar = Int32.Parse(ConfigurationManager.AppSettings["cantDTE"]);
                logs.putLog(1, "Cantidad de DTE a recuperar: " + cantDTERecuperar);
                
                recuperarDTEs(dtes, cantDTERecuperar, conexion, logs);
                logs.putLog(1, "recuperarDTEs finaliza normalmente");

            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: " + logs.nombreLog);
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }

            return dtes;
        }

        private void recuperarDTEs(DTEPendietes dtes, int cantDTERecuperar, bdConexion conexion, log logs)
        { 
            string sql = string.Empty;
            DataTable result;

            sql = " select distinct dto.rutt_rece, dto.digi_rece, dto.rutt_emis, dto.digi_emis, dto.tipo_docu, dto.foli_docu, dto.fech_emis, dto.mont_neto, dto.mont_exen, dto.mont_tota, dto.corr_docu, dto.codi_empr ";
            sql += " FROM dto_enca_docu_p dto ";
            sql += " LEFT OUTER JOIN dto_docu_refe_p ref ON dto.corr_docu=ref.corr_docu ";
            sql += " where dto.esta_docu in ('INI', 'ERA') ";
            sql += " and dto.tipo_docu in ('33','34') ";
            sql += " and dto.esta_tras is null ";
            sql += " and dto.corr_docu not in ( ";
            sql += "     SELECT dto.corr_docu ";
            sql += "     FROM dto_enca_docu_p dto ";
            sql += "     LEFT OUTER JOIN dto_docu_refe_p ref ON dto.corr_docu=ref.corr_docu ";
            sql += "     where dto.esta_docu in ('INI', 'ERA') ";
            sql += "     and dto.tipo_docu in ('33','34') ";
            sql += "     and ref.TIPO_REFE = 803 and ref.foli_refe = 'COM' ";
            sql += " ) --cantDTEOracle ";

            string sqlCantDTE = cantidadDTE(sql, cantDTERecuperar, conexion.baseDatos);

            result = conexion.EjecutaSelect(sqlCantDTE);
            
            int dte_num = result.Rows.Count;
            logs.putLog(1, "-- Cantidad de DTE a entregar (dte_num): " + dte_num);

            if (dte_num == 0)
            {
                dtes.mensaje = "No hay DTE pendientes por entregar";
                dtes.cantRestantes = 0;
                logs.putLog(1, "-- No hay DTE pendientes por entregar: ");
                logs.putLog(1, "-- cantRestantes: 0");
            }
            else
            {
                for (int i = 0; i < dte_num; i++)
                {
                    string ruttRece = result.Rows[i][0].ToString();
                    string digiRece = result.Rows[i][1].ToString();
                    string ruttEmis = result.Rows[i][2].ToString();
                    string digiEmis = result.Rows[i][3].ToString();
                    string tipoDocu = result.Rows[i][4].ToString();
                    string foliDocu = result.Rows[i][5].ToString();
                    string fechEmis = result.Rows[i][6].ToString();
                    string montNeto = result.Rows[i][7].ToString();
                    string montExen = result.Rows[i][8].ToString();
                    string montTota = result.Rows[i][9].ToString();
                    string corrDocu = result.Rows[i][10].ToString();
                    string codiEmpr = result.Rows[i][11].ToString();

                    List<Referencia> Refencias = new List<Referencia>();
                    Refencias = getRefencias(corrDocu, conexion);
                    string xml = getXML(corrDocu, conexion, logs);

                    string pdf = getPDF(codiEmpr, ruttRece, ruttEmis, tipoDocu, foliDocu, logs);

                    Documento dte_temp = new Documento(ruttRece, digiRece, ruttEmis, digiEmis, tipoDocu, foliDocu, fechEmis, montNeto, montExen, montTota, xml, pdf, Refencias);
                    dtes.DTE.Add(dte_temp);
                    
                    logs.putLog(1, "-- Recuperado DTE Folio: " + foliDocu);
                }
                dtes.cantRestantes = cantDTERestantes(sql, conexion) - dte_num;
                logs.putLog(1, "-- Cantidad restante de DTE (cantRestantes) " + dtes.cantRestantes);
            }
        }

        private List<Referencia> getRefencias(string corrDocu, bdConexion conexion)
        {
            string sql = string.Empty;
            DataTable result;
            List<Referencia> list_refe_temp = new List<Referencia>();

            sql = " select foli_refe, tipo_refe ";
            sql += " from dto_docu_refe_p ";
            sql += " where corr_docu = {0} ";

            result = conexion.EjecutaSelect(String.Format(sql, corrDocu));
            int dte_num = result.Rows.Count;
            for (int i = 0; i < dte_num; i++)
            {
                string foliRefe = result.Rows[i][0].ToString();
                string tipoRefe = result.Rows[i][1].ToString();
                Referencia ref_temp = new Referencia(foliRefe, tipoRefe);
                list_refe_temp.Add(ref_temp);
            }
            return list_refe_temp;
        }

        private int cantDTERestantes(string sql, bdConexion conexion)
        {
            string strCount = string.Empty;
            DataTable result;

            strCount = sql.Replace("select distinct", "select count(*) from (select distinct");
            strCount = strCount.Replace("--cantDTEOracle", " ) ");

            result = conexion.EjecutaSelect(strCount);
            
            return Int32.Parse(result.Rows[0][0].ToString());
        }

        private string cantidadDTE(string sql, int cantDTERecuperar, string tipoBD)
        {
            string result = string.Empty;
            if (tipoBD.ToLower() == "oracle")
            {
                string strCantDTE = string.Format(" ) where rownum <= {0} ", cantDTERecuperar);
                result = sql.Replace("select distinct", "select * from (select distinct");
                result = result.Replace("--cantDTEOracle", strCantDTE);
            }
            else if (tipoBD.ToLower() == "sqlserver")
            {
                //Falta implementar
            }
            return result;
        }

        private string getXML(string corrDocu, bdConexion conexion, log logs)
        {
            string xmlResult = string.Empty;
            string xml = string.Empty;
            string sql = "select clob_docu from dto_docu_lob where corr_docu = {0} and tipo_arch = 'XML'";
            try
            {
                xml = conexion.SelectText(String.Format(sql, corrDocu));
                xml.Trim();
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: getXML");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }
            return xml;
        }

        private string getPDF(string codiEmpr, string _sRutRece, string _sRuttEmis, string sTipoDocu, string sFoliDocu, log logs)
        {
            string sParametros = string.Empty;
            string sSalida = string.Empty;
            string result = string.Empty;
            SetParametros(_sRutRece);
            string sArchivo = FormateaNombre(_sRuttEmis, sTipoDocu, sFoliDocu, false);
            _sRutaPdf += sArchivo;

            if (!File.Exists(_sRutaPdf))
            {
                string sProceso = "egateDTE";
                
                sParametros = GeneraCmdEgateDte("egateDTE ", _sEgateHome, codiEmpr, sTipoDocu, sFoliDocu, _sRuttEmis);
                sSalida = EjecutaProceso(_sEgateHome, sProceso, sParametros, logs);
            }
            else
            {
                //Log.putLog(_sDirectorio, "Pdf se encontraba generado");
                string sProceso = "egateDTE"; //quitar
            }


            if (sSalida != "error")
            {
                if (File.Exists(_sRutaPdf))
                {
                    try
                    {
                        string PDF;
                        //this._Mensaje.vCodigo = "DOK";
                        //this._Mensaje.vMensaje = Transformar(_sRutaPdf, out PDF);
                        //this._Mensaje.vPDF = PDF;
                        string pdf = Transformar(_sRutaPdf, out PDF, logs);
                        result = PDF;
                    }
                    catch (Exception ex)
                    {
                        logs.putLog(1, "Proceso: getPDF");
                        logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
                    }
                }
                else
                {
                    //sBaseSeisCuatro = "Error No se encuentra PDF, no pudo ser generado";
                    //this._Mensaje.vCodigo = "ERR1";
                    string err = "";
                }
            }
            return result;
        }

        public string FormateaNombre(string sRutt, string sTipoDocu, string sFoliDocu, bool bMerito)
        {
            string sArchivo = string.Empty;
            string sDato = string.Empty;
            int iContador = 0;

            sDato = sRutt;
            for (iContador = sDato.Length; iContador < 9; iContador++)
                sDato = "0" + sDato;

            sArchivo = "E" + sDato;

            sDato = sTipoDocu;
            for (iContador = sDato.Length; iContador < 3; iContador++)
                sDato = "0" + sDato;

            sArchivo += "T" + sDato;

            sDato = sFoliDocu;
            for (iContador = sDato.Length; iContador < 10; iContador++)
                sDato = "0" + sDato;

            sArchivo += "F" + sDato;
            if (bMerito)
                sArchivo += "_me";

            sArchivo = sArchivo + ".pdf";

            return sArchivo;
        }
       
        public void SetParametros(string sRut)
        {
            //const int _ICCERO = 0;
            string sRutP = string.Empty;
            string sHome = string.Empty;
            string _sCodiEmex = string.Empty;
            //string _sEgateHome = string.Empty;
            string _sRutaBin = string.Empty;
            string _sDirectorio = string.Empty;
            string _sBaseDir = AppDomain.CurrentDomain.BaseDirectory;
            //string EmpresaPath = Path.Combine(_sBaseDir, "librerias\\empresas.xml");
            //sHome = ConfigurationManager.AppSettings.Get("Entorno");
            if (ConfigurationManager.AppSettings["eHome:" + sRut.ToString()] != null)
                sHome = ConfigurationManager.AppSettings["eHome:" + sRut.ToString()];
            else
                sHome = "EGATE_HOME";
                
            _sDirectorio = Convert.ToString(Environment.GetEnvironmentVariable(sHome));

            _sEgateHome = sHome;
            _sRutaPdf = _sDirectorio + @"\in\pdf\";
            _sRutaBin = _sDirectorio + @"\bin\";
            //Log.putLog(_sDirectorio, "Parametros empresa cargados");
        }

        private string GeneraCmdEgateDte(string sNombreProceso, string sEgateDte, string sCodEmpr, string sTipoDocu, string sFoliDocu, string sRuttEmis)
        {
            string sProceso = string.Empty;
            StringBuilder sbProceso = new StringBuilder();
            //sbProceso.Append(sNombreProceso);
            sbProceso.Append(" -h " + sEgateDte);
            sbProceso.Append(" -empr " + sCodEmpr);
            sbProceso.Append(" -tdte " + sTipoDocu);
            sbProceso.Append(" -fdte " + sFoliDocu);
            sbProceso.Append(" -tl 3 ");
            sbProceso.Append(" -te dto ");
            sbProceso.Append(" -ts html ");
            sbProceso.Append(" -re " + sRuttEmis);
            sProceso = sbProceso.ToString();
            return sProceso;
        }

        public string EjecutaProceso(string sEgateHome, string sProceso, string sParametros, log logs)
        {
            string sSalida = string.Empty;
            try
            {
                //Log.putLog(_sDirectorio, "Generando Archvio");
                System.Diagnostics.ProcessStartInfo pInfo = new System.Diagnostics.ProcessStartInfo();
                pInfo.UseShellExecute = true;
                pInfo.RedirectStandardOutput = false;
                pInfo.FileName = sProceso;

                pInfo.Arguments = sParametros;

                sSalida += "Proceso Ejecutado:<br>";

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pInfo);
                //Log.putLog(_sDirectorio, "Esperando a servidor");
                p.WaitForExit();
                //Log.putLog(_sDirectorio, "Generacion de archivo terminado");
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: EjecutaProceso");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }
            return sSalida;
        }


        private string Transformar(string sNombreArchivo, out string PDF, log logs)
        {
            string sBaseSeisCuatro = string.Empty;
            FileStream fsStream = new FileStream(sNombreArchivo, FileMode.Open);
            BinaryReader brLector = new BinaryReader(fsStream);
            byte[] bBytes = new byte[(int)fsStream.Length];
            try
            {
                brLector.Read(bBytes, 0, bBytes.Length);
                sBaseSeisCuatro = Convert.ToBase64String(bBytes);
                PDF = sBaseSeisCuatro;
                return "OK";
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: Transformar");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
                PDF = "";
                return "Error";
            }
            // Se cierran los archivos para liberar memoria.
            finally
            {
                fsStream.Close();
                fsStream = null;
                brLector = null;
                bBytes = null;
            }
        }
        #endregion


        #region Agregar/Quitar Emisor
        [WebMethod]
        public Respuesta AgregarRutEmisor(int Rut, int digiVeri, string nombre)
        {
            Respuesta resp = new Respuesta();
            log logs = new log();
            logs.nombreLog = "AgregarRutEmisor";
            logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);

            if (!validaRut(Rut, resp, logs))
            {
                return resp;
            }

            setEgateHome(Rut, logs);

            logs.putLog(1, "");
            logs.putLog(1, "INICIO AgregarRutEmisor");

            bdConexion conexion = new bdConexion();
            try
            {
                conexion.egateHome = logs.egateHome;
                conexion.conexionOpen();

                if (existeEmisor(Rut, conexion, resp, logs))
                {
                    habilitarEmisor(Rut, conexion, resp, logs);
                }

            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: " + logs.nombreLog);
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }

            return resp;
        }


        [WebMethod]
        public Respuesta QuitarRutEmisor(int Rut, int digiVeri, string nombre)
        {
            Respuesta resp = new Respuesta();
            log logs = new log();
            logs.nombreLog = "QuitarRutEmisor";
            logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);

            if (!validaRut(Rut, resp, logs))
            {
                return resp;
            }

            setEgateHome(Rut, logs);

            logs.putLog(1, "");
            logs.putLog(1, "INICIO QuitarRutEmisor");

            bdConexion conexion = new bdConexion();
            try
            {
                conexion.egateHome = logs.egateHome;
                conexion.conexionOpen();

                if (existeEmisor(Rut, conexion, resp, logs))
                {
                    deshabilitarEmisor(Rut, conexion, resp, logs);
                }

            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: " + logs.nombreLog);
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }

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

            if (!validaRut(Rut, resp, logs))
            {
                return resp;
            }

            setEgateHome(Rut, logs);

            logs.putLog(1, "");
            logs.putLog(1, "INICIO AgregarRutReceptor");

            bdConexion conexion = new bdConexion();
            try
            {
                conexion.egateHome = logs.egateHome;
                conexion.conexionOpen();

                if (!existeReceptor(Rut, conexion, resp, logs))
                {
                    agregarReceptor(Rut, digiVeri, nombre, conexion, resp, logs);
                }
                else
                {
                    habilitarReceptor(Rut, conexion, resp, logs);
                }

            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: " + logs.nombreLog);
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
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

            if(!validaRut(Rut, resp, logs))
            {
                return resp;
            }

            setEgateHome(Rut, logs);

            logs.putLog(1, "");
            logs.putLog(1, "INICIO QuitarRutReceptor");

            bdConexion conexion = new bdConexion();
            try
            {
                conexion.egateHome = logs.egateHome;
                conexion.conexionOpen();

                if (!existeReceptor(Rut, conexion, resp, logs))
                {
                    quitarReceptor(Rut, digiVeri, nombre, conexion, resp, logs);
                }
                else
                {
                    deshabilitarReceptor(Rut, conexion, resp, logs);
                }

            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: " + logs.nombreLog);
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
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

        private Boolean validaRut(int Rut, Respuesta resp, log logs)
        {
            Boolean result = true;
            if (Rut == 0)
            {
                resp.SCodigo = ERROR_RUT_VACIO;
                resp.SMensaje = "Rut No puede estar vacio";
                result = false;
                logs.putLog(1, "-- Rut No puede estar vacio");
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
                logs.putLog(1, "Se establece home (egateHome): " + logs.egateHome);
            }
        }

        private Boolean existeReceptor(int Rut, bdConexion conexion, Respuesta resp, log logs)
        {
            Boolean result = true;
            string sql = "SELECT COUNT(*) FROM PERSONAS WHERE RUTT_PERS = '{0}'";
            int cantPersonas = Convert.ToInt32(conexion.SelectInto(String.Format(sql, Rut)));
            if(cantPersonas == 0)
            {
                result = false;
                resp.SMensaje = "Rut no se encuentra";
                logs.putLog(1, "-- Rut no se encuentra");
            }
            return result;
        }

        private void agregarReceptor(int Rut, int digiVeri, string nombre, bdConexion conexion, Respuesta resp, log logs)
        {
            try
            {
                string sql = "INSERT INTO PERSONAS (RUTT_PERS, DGTO_PERS, NOMB_PERS, EMPR_PERS, INDI_WSS) VALUES ({0}, {1}, '{2}', 'S', 'S')";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut, digiVeri, nombre)) == 0)
                {
                    resp.SCodigo = ERROR_AGREGAR_RUT_RECE;
                    resp.SMensaje = "No se pudo agregar Rut en receptores";
                    logs.putLog(1, "-- No se pudo agregar Rut en receptores");
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se inserta Rut en receptores";
                    logs.putLog(1, "-- Se inserta Rut en receptores");
                }
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: agregarReceptor");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }
        }

        private void habilitarReceptor(int Rut, bdConexion conexion, Respuesta resp, log logs)
        {
            try
            {
                string sql = "UPDATE PERSONAS SET INDI_WSS = 'S' WHERE RUTT_PERS = {0} ";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut)) == 0)
                {
                    resp.SCodigo = ERROR_HABILITAR_RUT_RECE;
                    resp.SMensaje = "No se pudo habilitar Rut en receptores";
                    logs.putLog(1, "-- No se pudo habilitar Rut en receptores");
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se habilita Rut en receptores";
                    logs.putLog(1, "-- Se habilita Rut en receptores");
                }
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: habilitarReceptor");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }
        }
    
        private void quitarReceptor(int Rut, int digiVeri, string nombre, bdConexion conexion, Respuesta resp, log logs){
            try
            {
                string sql = "INSERT INTO PERSONAS (RUTT_PERS, DGTO_PERS, NOMB_PERS, EMPR_PERS, INDI_WSS) VALUES ({0}, {1}, '{2}', 'S', 'N')";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut, digiVeri, nombre)) == 0)
                {
                    resp.SCodigo = ERROR_QUITAR_RUT_RECE;
                    resp.SMensaje = "No se pudo quitar Rut en receptores";
                    logs.putLog(1, "-- No se pudo quitar Rut en receptores");
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se quita Rut en receptores";
                    logs.putLog(1, "-- Se quita Rut en receptores");
                }
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: quitarReceptor");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }
        }

        private void deshabilitarReceptor(int Rut, bdConexion conexion, Respuesta resp, log logs)
        {
            try
            {
                string sql = "UPDATE PERSONAS SET INDI_WSS = 'N' WHERE RUTT_PERS = {0} ";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut)) == 0)
                {
                    resp.SCodigo = ERROR_DESHABILITAR_RUT_RECE;
                    resp.SMensaje = "No se pudo deshabilitar Rut en receptores";
                    logs.putLog(1, "-- No se pudo deshabilitar Rut en receptores");
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se deshabilita Rut en receptores";
                    logs.putLog(1, "-- Se deshabilita Rut en receptores");
                }
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: deshabilitarReceptor");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }
        }

        private Boolean existeEmisor(int Rut, bdConexion conexion, Respuesta resp, log logs)
        {
            Boolean result = true;
            string sql = "SELECT COUNT(*) FROM EMPR WHERE RUTT_EMPR = '{0}'";
            int cantPersonas = Convert.ToInt32(conexion.SelectInto(String.Format(sql, Rut)));
            if (cantPersonas == 0)
            {
                result = false;
                resp.SCodigo = ERROR_EMISOR_INVALIDO_EMIS;
                resp.SMensaje = "No se encuentra Rut del emisor";
                logs.putLog(1, "-- No se encuentra Rut del emisor");
            }
            return result;
        }

        private void habilitarEmisor(int Rut, bdConexion conexion, Respuesta resp, log logs)
        {
            try
            {
                string sql = "UPDATE EMPR SET INDI_WSS = 'S' WHERE RUTT_EMPR = {0} ";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut)) == 0)
                {
                    resp.SCodigo = ERROR_HABILITAR_RUT_RECE;
                    resp.SMensaje = "No se pudo habilitar Rut en emisores";
                    logs.putLog(1, "-- No se pudo habilitar Rut en emisores");
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se habilita Rut en emisores";
                    logs.putLog(1, "-- Se habilita Rut en emisores");
                }
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: habilitarEmisor");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }
        }

        private void deshabilitarEmisor(int Rut, bdConexion conexion, Respuesta resp, log logs)
        {
            try
            {
                string sql = "UPDATE EMPR SET INDI_WSS = 'N' WHERE RUTT_EMPR = {0} ";

                if (conexion.EjecutaNonQuery(String.Format(sql, Rut)) == 0)
                {
                    resp.SCodigo = ERROR_DESHABILITAR_RUT_RECE;
                    resp.SMensaje = "No se pudo deshabilitar Rut en emisores";
                    logs.putLog(1, "-- No se pudo deshabilitar Rut en emisores");
                }
                else
                {
                    conexion.confirma();
                    resp.SCodigo = CONSULTA_OK;
                    resp.SMensaje = "Se hdesabilita Rut en emisores";
                    logs.putLog(1, "-- Se hdesabilita Rut en emisores");
                }
            }
            catch (Exception ex)
            {
                logs.putLog(1, "Proceso: deshabilitarEmisor");
                logs.putLog(1, "Error: " + Convert.ToString(ex.Message));
            }
        }

        #endregion
    }
}