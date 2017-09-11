using System;
using System.Web.Services;
using System.Web.Services.Description;
using conexionBaseDatos;
using System.Configuration;
using Tool;

[WebService(Namespace = "http://www.dbnet.cl/SupplierETDRejection")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[Microsoft.Web.Services3.Policy("DbnetWssPoliticaSeguridad")]
public class SupplierETDRejection : WebService
{
    String ER0 = "ER0";
    String DOK = "DOK";

    [WebMethod(Description = "Metodo que permite realizar el reclamo o aceptacion de un DTE")]
    public Response setRejection(string company, string companyCodeSii, int documentType, int documentNumber, string statusCode)
    {
        string ErrLugar = "";
        ErrLugar="Se crea instancia Mensaje()";
        Response mens = new Response();
        // Seguridad de Servicios Web (Validando presencia de todos los campos)
        if (string.IsNullOrEmpty(company) || string.IsNullOrEmpty(companyCodeSii) || string.IsNullOrEmpty(documentType.ToString()) || string.IsNullOrEmpty(documentNumber.ToString()) || string.IsNullOrEmpty(statusCode)) 
        {
            mens.Codigo = ER0;
            mens.Mensaje = "Todos los parámetros deben contener valor";
            return mens;
        }
        if (!DbnetWssSecurity.DbnetWssAutorizador.validaUsuario(company))
        {
            mens.Codigo = "ER1";
            mens.Mensaje = "Empresa no se encuentra configurada.";
            return mens;
        }

        #region Define Variables
        String Servicio = "";
        String Metodo = "";
        String log_mensaje="";
        log  logs = new log();
        int posicion;
        string companySinDV;
        string companyCodeSiiSinDV;
        string digitoCompanyCodeSii = String.Empty;
        string digitoCompany = String.Empty;
        String LogAplica = "N";
        String ListaParametros = "";
        #endregion Define Variables
        bdConexion conexion = new bdConexion();

        try
        {
            #region Inicializacion Nombre de WSS y Metodo
            ErrLugar = "Inicializacion Nombre de WSS y Metodo";
            ListaParametros = "Parametros : Empresa [" + company + "].";
            ListaParametros = "Parametros : [Emisor - " + companyCodeSii + "][TipoDocumento - " + documentType + "][Folio - " + documentNumber + "][Estado - " + statusCode + "].";
            Servicio = "SupplierETDRejection";
            Metodo = "setRejection";
            #endregion Inicializacion Nombre de WSS y Metodo

            #region Calcula RutEmpresa sin DV
            ErrLugar = "Calcula RutEmpresa sin DV";
            posicion = company.IndexOf("-");
            companySinDV = company;
            if (company.IndexOf("-") >= 0)
            {
                digitoCompany = company.Substring(posicion + 1, 1);
                if (digitoCompany != "")
                {
                    companySinDV = company.Substring(0, posicion);
                }
            }
            #endregion Calcula RutEmpresa sin DV

            // Determinar RutsinDV
            #region Verifica Empresa Autorizada a trabajar con WebService
            ErrLugar = "Verifica Empresa Autorizada a trabajar con WebService";
            mens.Codigo = "INI";
            mens.Mensaje = "Inicializacion. Company ["+companySinDV+"]";
            if (ConfigurationManager.AppSettings["eHome:" +companySinDV] != null) 
            {
                #region Manejo Logs
                ErrLugar = "Manejo Logs";
                logs.nombreLog = Servicio + "_" + Metodo;
                logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);
                logs.egateHome = ConfigurationManager.AppSettings["eHome:" +companySinDV];

                mens.Mensaje = "Rut Empresa : " + company;
                log_mensaje = mens.Mensaje;
                #endregion Manejo Logs
            }
            else
            {
                mens.Codigo = "ER7";
                mens.Mensaje = "Empresa [ " + companySinDV + " ] no autorizada a operar en WebService.";
                return mens;
            }
            #endregion Verifica Empresa Autorizada a trabajar con WebService

            mens.Codigo = "INI";
            mens.Mensaje = "Entra.";

            #region Conexion a BD
            ErrLugar = "Conexion a BD";
            conexion.egateHome = ConfigurationManager.AppSettings["eHome:" +companySinDV];
            conexion.conexionOpen();
            #endregion Conexion a BD

            LogAplica = "S"; // A partir de aca puedo escribir en log

            #region Valida DV RutReceptor
            ErrLugar = "Valida DV RutReceptor";
            if (digitoCompany != "")
            {
                if (!Validaciones.validaRut(company))
                {
                    mens.Codigo = "ER1";
                    mens.Mensaje = "Rut Receptor no Válido";
                    log_mensaje += " - " + mens.Mensaje;
                    logs.putLog(1, log_mensaje);
                    return mens;
                }
                company = companySinDV;
            }
            #endregion Valida Rut Receptor

            #region Valida DV RutEmisor
            ErrLugar = "Valida DV Emisor";
            posicion = companyCodeSii.IndexOf("-");
            companyCodeSiiSinDV = companyCodeSii;
            if (companyCodeSii.IndexOf("-") >= 0)
            {
                digitoCompanyCodeSii = companyCodeSii.Substring(posicion + 1, 1);
                if (digitoCompanyCodeSii != "")
                {
                    if (!Validaciones.validaRut(companyCodeSii))
                    {
                        mens.Codigo = ER0;
                        mens.Mensaje = "DV RutEmisor no corresponde.";
                        log_mensaje += " - " + mens.Mensaje;
                        logs.putLog(1, log_mensaje);
                        return mens;
                    }
                    companyCodeSiiSinDV = companyCodeSii.Substring(0, posicion);
                }
                companyCodeSii = companyCodeSiiSinDV;
            }
            #endregion Valida DV RutEmisor

            if (!conexion.estadoAprov(statusCode) )
            {
                #region Valida Estado de Aprob/Rechazo
                ErrLugar = "Valida Estado de Aprob/Rechazo";
                mens.Codigo = "ER4";
                mens.Mensaje = "Estado no es Válido, Estados Posibles APR: Aprobado, ARE: Aprobado con reparos, REC: Rechazado";
                log_mensaje += " - " + mens.Mensaje;
                logs.putLog(1, log_mensaje);
                return mens;
                #endregion Valida Estado de Aprob/Rechazo
            }
            else
            {
                if (!conexion.validaExistencia(Convert.ToInt32(companyCodeSii),documentType, documentNumber) )
                {
                    #region Documento no existe
                    ErrLugar = "Documento no existe";
                    mens.Codigo = "ER5";
                    mens.Mensaje = "Documento no existe o posee estado que no permite ser aceptado comercialmente.";
                    log_mensaje += " - " + mens.Mensaje + " Emisor : [" + companyCodeSii + "]" + " Tipo : [" + Convert.ToString(documentType) + "]" + " Folio : [" + Convert.ToString(documentNumber) + "].";
                    logs.putLog(1, log_mensaje);
                    return mens;
                    #endregion Documento no existe
                }
                else
                {
                    #region Documento Existe
                    ErrLugar = "Aplica Rejection";
                    string pi_codi_erro = String.Empty;
                    string pi_mens_erro = String.Empty;
                    string pi_corr_qmsg = String.Empty;

                    if (conexion.applyRejection(company, digitoCompany, companyCodeSii, digitoCompanyCodeSii, documentType, documentNumber, statusCode, out pi_codi_erro, out pi_mens_erro, out pi_corr_qmsg))
                    {
                        conexion.confirma();
                        mens.CodigoSolicitud = pi_corr_qmsg ;
                        mens.Codigo = pi_codi_erro;
                        mens.Mensaje = pi_mens_erro;

                        logs.putLog(1, "Se aplica Reclamo");
                        log_mensaje += " - company : [" + company + "-" + digitoCompany + "] companyCodeSii : [" + companyCodeSii + "-" + digitoCompanyCodeSii + 
                                        "] documentType : [" + documentType + "] documentNumber : [" + documentNumber + "] statusCode : [" + statusCode +
                                        "] mens.CodigoSolicitud : [" + mens.CodigoSolicitud + "]" + " mens.Codigo : [" + mens.Codigo + "]" + " mens.Mensaje : [" + mens.Mensaje + "]";
                        logs.putLog(1, log_mensaje);

                        int codi_empr = Convert.ToInt32(ConfigurationManager.AppSettings["codi_empr"]);
                        string codi_emex = ConfigurationManager.AppSettings["codi_emex"];
                        string integracion = ConfigurationManager.AppSettings["integracion"];

                        // Valida si tiene integracion con SE
                        if (integracion == "1")
                        {
                            logs.putLog(1, "Integracion con SE");

                            if (conexion.notificaReclamoASE(codi_empr, codi_emex, statusCode, companyCodeSii, documentType, documentNumber))
                            {
                                logs.putLog(1, "Se actualiza SE con exito");
                            }
                            else
                            {
                                logs.putLog(1, "Fallo al intentar actualizar SE");
                            }


                        }

                        return mens;
                    }
                    else
                    {
                        ErrLugar = "Error al aplicar Reclamo";
                        mens.Codigo = "DON";
                        mens.Mensaje = "Error al aplicar Reclamo";
                        log_mensaje += " - " + mens.Mensaje + " - company : [" + company + "-" + digitoCompany + "] companyCodeSii : [" + companyCodeSii + "-" + digitoCompanyCodeSii +
                                        "] documentType : [" + documentType + "] documentNumber : [" + documentNumber + "] statusCode : [" + statusCode +
                                        "] mens.CodigoSolicitud : [" + mens.CodigoSolicitud + "]" + " mens.Codigo : [" + mens.Codigo + "]" + " mens.Mensaje : [" + mens.Mensaje + "]";
                        logs.putLog(1, log_mensaje);
                        conexion.rechaza();
                    }
                    #endregion Documento Existe
                }
            }
        }
        catch (Exception ex)
        {
            #region Manejo de Exception
            if (LogAplica == "S")
            {
                int largo;
                largo = Convert.ToString(ex.Message).Length;
                if (largo > 100)
                {
                    largo = 100;
                }
                mens.Codigo = "ERR";
                mens.Mensaje = "Se ha producido el error : " + Convert.ToString(ex.Message) + ".";

                log_mensaje = "Rut Empresa : " + company + ". Error en " + ErrLugar + " : " + Convert.ToString(ex.Message).Substring(1, largo) + ". " + ListaParametros;
                logs.putLog(1, log_mensaje);
            }
            else
            {
                mens.Codigo = "ERR";
                mens.Mensaje = "Se ha producido el error : " + ErrLugar + Convert.ToString(ex.Message) + ".";
            }

            return mens;
            #endregion Manejo de Exception
        }
        finally
        {
            if (conexion != null)
            {
                conexion.closeConexion();
            }
        }
        return mens;
    }

    [WebMethod(Description = "Metodo que permite consultar el estado de una solicitud de reclamo")]
    public ResponseGet getStateRejection(string company, string CodigoSolicitud)
    {
        ResponseGet mens = new ResponseGet();
        // Seguridad de Servicios Web (Validando presencia de todos los campos)
        if (string.IsNullOrEmpty(company) || string.IsNullOrEmpty(CodigoSolicitud))
        {
            mens.Codigo = ER0;
            mens.Mensaje = "Todos los parámetros deben contener valor";
            return mens;
        }

        if (!DbnetWssSecurity.DbnetWssAutorizador.validaUsuario(company))
        {
            mens.Codigo = "ER1";
            mens.Mensaje = "Empresa no se encuentra configurada.";
            return mens;
        }

        #region Definicion de Variables
        String Servicio = "";
        String Metodo = "";
        String log_mensaje = "";
        log logs = new log();
        int posicion;
        string companySinDV;
        string companyCodeSiiSinDV;
        string digito = "";
        String LogAplica = "N";
        string ErrLugar = "";
        String ListaParametros = "";
        #endregion Definicion de Variables

        bdConexion conexion = new bdConexion();
        try
        {
            #region Inicializacion Nombre de WSS y Metodo
            ErrLugar = "Inicializacion de WSS y Metodo";
            ListaParametros = "Parametros : [company - " + company + "][CodigoSolicitud - " + CodigoSolicitud + "].";
            Servicio = "SupplierETDRejection";
            Metodo = "getStateRejection";
            #endregion Inicializacion Nombre de WSS y Metodo

            #region Calcula RutEmpresa sin DV
            posicion = company.IndexOf("-");
            companySinDV = company;
            if (company.IndexOf("-") >= 0)
            {
                digito = company.Substring(posicion + 1, 1);
                if (digito != "")
                {
                    companySinDV = company.Substring(0, posicion);
                }
            }
            #endregion Calcula RutEmpresa sin DV

            #region Verifica Empresa Autorizada a trabajar con WebService
            ErrLugar = "Verifica Empresa Autorizada a trabajar con WebService";
            mens.Codigo = "INI";
            mens.Mensaje = "Inicializacion. Company [" + companySinDV + "]";
            if (ConfigurationManager.AppSettings["eHome:" +companySinDV] != null)
            {
                #region Manejo Logs
                logs.nombreLog = Servicio + "_" + Metodo;
                logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);
                logs.egateHome = ConfigurationManager.AppSettings["eHome:" + companySinDV];

                mens.Mensaje = "Rut Empresa : " + company;
                log_mensaje = mens.Mensaje;
                #endregion Manejo Logs
            }
            else
            {
                mens.Codigo = "ER7";
                mens.Mensaje = "Empresa [ " + companySinDV + " ] no autorizada a operar en WebService.";
                return mens;
            }
            #endregion Verifica Empresa Autorizada a trabajar con WebService

            mens.Codigo = "INI";
            mens.Mensaje = "Entra.";

            #region Conexion a BD
            ErrLugar = "Conexion a BD";
            conexion.egateHome = logs.egateHome;
            conexion.conexionOpen();
            #endregion Conexion a BD

            LogAplica = "S"; // A partir de aca puedo escribir en log

            #region Valida DV RutReceptor
            ErrLugar = "Valida DV RutReceptor";
            if (digito != "")
            {
                if (!Validaciones.validaRut(company))
                {
                    mens.Codigo = ER0;
                    mens.Mensaje = "DV RutReceptor no corresponde";
                    log_mensaje += " - " + mens.Mensaje;
                    logs.putLog(1, log_mensaje);
                    return mens;
                }
                company = companySinDV;
            }
            #endregion Valida Rut Receptor


            String rutt_empr = String.Empty;
            String digi_empr = String.Empty;

            if (!conexion.validaExistenciaReclamo(company, CodigoSolicitud, out rutt_empr, out digi_empr))
            {
                #region Documento no existe
                ErrLugar = "Documento no existe";
                mens.Codigo = "ER5";
                mens.Estado = "0";
                mens.Mensaje = "Documento no existe o posee estado que no permite ser reclamado";
                log_mensaje += " - " + mens.Mensaje + " Emisor : [" + company + "]" + " CodigoSolicitud : [" + CodigoSolicitud + "]." ;
                logs.putLog(1, log_mensaje);
                return mens;
                #endregion Documento no existe
            }
            else
            {
                logs.putLog(1, "Documento Existe");
                String pi_codi_erro = String.Empty;
                String pi_mens_erro = String.Empty;
                String pi_resp_msge = String.Empty;
                
                if (conexion.recoverRejection(company, CodigoSolicitud, rutt_empr,  digi_empr, out pi_codi_erro, out pi_mens_erro, out pi_resp_msge))
                {
                    conexion.confirma();
                    mens.Codigo = DOK;
                    mens.Estado = pi_codi_erro;
                    mens.Mensaje = pi_mens_erro;
                    logs.putLog(1, "Se recupera Reclamo");
                    log_mensaje += " - company : [" + company + "] CodigoSolicitud : [" + CodigoSolicitud + 
                                    "] rutt_empr : [" + rutt_empr + "-" + digi_empr + "mens.Codigo : [" + mens.Codigo + "]" + 
                                    " mens.Estado : [" + mens.Estado + "]" + " mens.Mensaje : [" + mens.Mensaje + "]";
                    logs.putLog(1, log_mensaje);

                    return mens;
                }
                else
                {
                    ErrLugar = "Error al aplicar Reclamo";
                    mens.Codigo = "DON";
                    mens.Mensaje = "Error al aplicar Reclamo";
                    log_mensaje += " - " + mens.Mensaje + " - company : [" + company + "] CodigoSolicitud : [" + CodigoSolicitud +
                                    "] rutt_empr : [" + rutt_empr + "-" + digi_empr + "mens.Codigo : [" + mens.Codigo + "]" +
                                    " mens.Estado : [" + mens.Estado + "]" + " mens.Mensaje : [" + mens.Mensaje + "]";
                    logs.putLog(1, log_mensaje);
                    conexion.rechaza();
                }

                //oData = true;
            }

            return mens;
        }
        catch (Exception ex)
        {
            #region Manejo de Exception
            if (LogAplica == "S")
            {
                int largo;
                largo = Convert.ToString(ex.Message).Length;
                if (largo > 100)
                {
                    largo = 100;
                }
                mens.Codigo = "ERR";
                mens.Mensaje = "Se ha producido el error : " + Convert.ToString(ex.Message) + ".";

                log_mensaje = "Rut Empresa : " + company + ". Error en " + ErrLugar + " : " + Convert.ToString(ex.Message) + ". " + ListaParametros;
                logs.putLog(1, log_mensaje);
            }
            else
            {
                mens.Codigo = "ERR";
                mens.Mensaje = "Se ha producido el error : " + ErrLugar + Convert.ToString(ex.Message) + ".";
            }
            return mens;
            #endregion Tratamiento de Error
        }
        finally
        {
            if (conexion != null)
            {
                conexion.closeConexion();
            }
        }
     }

        

}