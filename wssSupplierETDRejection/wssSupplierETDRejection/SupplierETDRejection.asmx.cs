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

    [WebMethod(Description = "Metodo que permite realizar la aceptacion o rechazo comercial, asociado a un DTE")]
    public Mensaje setSupplierETDBusinessState(string company, string companyCodeSii, int documentType, int documentNumber, string statusCode, string reasonDesc)
    {
        string ErrLugar = "";
        ErrLugar="Se crea instancia Mensaje()";
        Mensaje mens = new Mensaje();
        // Seguridad de Servicios Web (Validando Autorización del usuario para ese RUT)
        if (string.IsNullOrEmpty(company))
        {
            mens.Codigo = "ER0";
            mens.Descripcion = "company no puede ser vacio.";
            return mens;
        }
        if (!DbnetWssSecurity.DbnetWssAutorizador.validaUsuario(company))
        {
            mens.Codigo = "ER1";
            mens.Descripcion = "Empresa no se encuentra configurada.";
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
        string digito="";
        String LogAplica = "N";
        String ListaParametros = "";
        #endregion Define Variables
        bdConexion conexion = new bdConexion();

        try
        {
            #region Inicializacion Nombre de WSS y Metodo
            ErrLugar = "Inicializacion Nombre de WSS y Metodo";
            ListaParametros = "Parametros : Empresa [" + company + "].";
            ListaParametros = "Parametros : [Emisor - " + companyCodeSii + "][TipoDocumento - " + documentType + "][Folio - " + documentNumber + "][Estado - " + statusCode + "][Descripcion - " + reasonDesc + "].";
            Servicio = "SupplierETDRejection";
            Metodo   = "setSupplierETDBusinessState";
            #endregion Inicializacion Nombre de WSS y Metodo

            #region Calcula RutEmpresa sin DV
            ErrLugar = "Calcula RutEmpresa sin DV";
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

            // Determinar RutsinDV
            #region Verifica Empresa Autorizada a trabajar con WebService
            ErrLugar = "Verifica Empresa Autorizada a trabajar con WebService";
            mens.Codigo = "INI";
            mens.Descripcion = "Inicializacion. Company ["+companySinDV+"]";
            if (ConfigurationManager.AppSettings["eHome:" +companySinDV] != null) 
            {
                #region Manejo Logs
                ErrLugar = "Manejo Logs";
                logs.nombreLog = Servicio + "_" + Metodo;
                logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);
                logs.egateHome = ConfigurationManager.AppSettings["eHome:" +companySinDV];

                mens.Descripcion = "Rut Empresa : " + company;
                log_mensaje = mens.Descripcion;
                #endregion Manejo Logs
            }
            else
            {
                mens.Codigo = "ER7";
                mens.Descripcion = "Empresa [ " + companySinDV + " ] no autorizada a operar en WebService.";
                return mens;
            }
            #endregion Verifica Empresa Autorizada a trabajar con WebService

            mens.Codigo = "INI";
            mens.Descripcion = "Entra.";

            #region Validacion de Parametros
            ErrLugar = "Validacion de Parametros";
            if (company == "" || companyCodeSii == "" || documentType.ToString() == "" || documentNumber.ToString() == "" || 
                statusCode == "" || reasonDesc == "")
            {
                mens.Codigo = "ER0";
                mens.Descripcion = "Faltan Parámetros";
                log_mensaje += " - " + mens.Descripcion;
                logs.putLog(1, log_mensaje);
                return mens;
            }
            #endregion Validacion de Parametros

            #region Conexion a BD
            ErrLugar = "Conexion a BD";
            conexion.egateHome = ConfigurationManager.AppSettings["eHome:" +companySinDV];
            conexion.conexionOpen();
            #endregion Conexion a BD

            LogAplica = "S"; // A partir de aca puedo escribir en log

            #region Valida DV RutReceptor
            ErrLugar = "Valida DV RutReceptor";
            if (digito != "")
            {
                if (!Validaciones.validaRut(company))
                {
                    mens.Codigo = "ER1";
                    mens.Descripcion = "Rut Receptor no Válido";
                    log_mensaje += " - " + mens.Descripcion;
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
                digito = companyCodeSii.Substring(posicion + 1, 1);
                if (digito != "")
                {
                    if (!Validaciones.validaRut(companyCodeSii))
                    {
                        mens.Codigo = "ER0";
                        mens.Descripcion = "DV RutEmisor no corresponde.";
                        log_mensaje += " - " + mens.Descripcion;
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
                mens.Descripcion = "Estado no es Válido, Estados Posibles APR: Aprobado, ARE: Aprobado con reparos, REC: Rechazado";
                log_mensaje += " - " + mens.Descripcion;
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
                    mens.Descripcion = "Documento no existe o posee estado que no permite ser aceptado comercialmente.";
                    log_mensaje += " - " + mens.Descripcion + " Emisor : [" + companyCodeSii + "]" + " Tipo : [" + Convert.ToString(documentType) + "]" + " Folio : [" + Convert.ToString(documentNumber) + "].";
                    logs.putLog(1, log_mensaje);
                    return mens;
                    #endregion Documento no existe
                }
                else
                {
                    #region Documento Existe
                    ErrLugar = "Actualiza Estado";
                    if (conexion.ActualizaEstado(Convert.ToInt32(companyCodeSii), documentType, documentNumber, statusCode, reasonDesc))
                    {
                        int codi_empr = 0;
                        int corr_docu = 0;

                        ErrLugar = "Registro Traza";
                        if (conexion.RegistraTraza(Metodo, "DTO", statusCode, codi_empr, documentType,
                            documentNumber, corr_docu, Servicio))
                        {
                            mens.Codigo = "DOK";
                            mens.Descripcion = "Respuesta comercial registrada";
                            log_mensaje += " - " + mens.Descripcion + " Emisor : [" + companyCodeSii + "]" + " Tipo : [" + Convert.ToString(documentType) + "]" + " Folio : [" + Convert.ToString(documentNumber) + "].";
                            logs.putLog(1, log_mensaje);
                            conexion.confirma();
                            return mens;
                        }
                        else
                        {
                            ErrLugar = "Error al registrar Traza";
                            mens.Codigo = "DON";
                            mens.Descripcion = "Error al registrar Traza";
                            log_mensaje += " - " + mens.Descripcion + " Emisor : [" + companyCodeSii + "]" + " Tipo : [" + Convert.ToString(documentType) + "]" + " Folio : [" + Convert.ToString(documentNumber) + "].";
                            logs.putLog(1, log_mensaje);
                            conexion.rechaza();
                            return mens;
                        }
                    }
                    else
                    {
                        ErrLugar = "Error al actualizar Documento";
                        mens.Codigo = "DON";
                        mens.Descripcion = "Error al actualizar Documento";
                        log_mensaje += " - " + mens.Descripcion + " Emisor : [" + companyCodeSii + "]" + " Tipo : [" + Convert.ToString(documentType) + "]" + " Folio : [" + Convert.ToString(documentNumber) + "].";
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
                mens.Descripcion = "Se ha producido el error : " + Convert.ToString(ex.Message) + ".";

                log_mensaje = "Rut Empresa : " + company + ". Error en " + ErrLugar + " : " + Convert.ToString(ex.Message).Substring(1, largo) + ". " + ListaParametros;
                logs.putLog(1, log_mensaje);
            }
            else
            {
                mens.Codigo = "ERR";
                mens.Descripcion = "Se ha producido el error : " + ErrLugar + Convert.ToString(ex.Message) + ".";
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

    [WebMethod(Description = "Metodo que permite realizar el recibo de mercaderia, asociado a un DTE")]
    public Mensaje setSupplierETDReception(string company, string companyCodeSii, int documentType, int documentNumber, string statusCode, string place)
    {
        Mensaje mens = new Mensaje();
        // Seguridad de Servicios Web (Validando Autorización del usuario para ese RUT)
        if (string.IsNullOrEmpty(company))
        {
            mens.Codigo = "ER0";
            mens.Descripcion = "company no puede ser vacio.";
            return mens;
        }
        if (!DbnetWssSecurity.DbnetWssAutorizador.validaUsuario(company))
        {
            mens.Codigo = "ER1";
            mens.Descripcion = "Empresa no se encuentra configurada.";
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
            ListaParametros = "Parametros : [Emisor - " + companyCodeSii + "][TipoDocumento - " + documentType + "][Folio - " + documentNumber + "][Estado - "+ statusCode +"][Lugar - "+ place +"].";
            Servicio = "SupplierETDRejection";
            Metodo = "setSupplierETDReception";
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
            mens.Descripcion = "Inicializacion. Company [" + companySinDV + "]";
            if (ConfigurationManager.AppSettings["eHome:" +companySinDV] != null)
            {
                #region Manejo Logs
                logs.nombreLog = Servicio + "_" + Metodo;
                logs.tipoLog = Convert.ToInt32(ConfigurationManager.AppSettings["tl"]);
                logs.egateHome = ConfigurationManager.AppSettings["eHome:" + companySinDV];

                mens.Descripcion = "Rut Empresa : " + company;
                log_mensaje = mens.Descripcion;
                #endregion Manejo Logs
            }
            else
            {
                mens.Codigo = "ER7";
                mens.Descripcion = "Empresa [ " + companySinDV + " ] no autorizada a operar en WebService.";
                return mens;
            }
            #endregion Verifica Empresa Autorizada a trabajar con WebService

            mens.Codigo = "INI";
            mens.Descripcion = "Entra.";

            #region Conexion a BD
            ErrLugar = "Conexion a BD";
            conexion.egateHome = logs.egateHome;
            conexion.conexionOpen();
            #endregion Conexion a BD

            LogAplica = "S"; // A partir de aca puedo escribir en log

            #region Validacion de Parametros
            ErrLugar = "Validacion de Parametros";
            if (company == "" || companyCodeSii == "" || documentType.ToString() == "" || documentNumber.ToString() == "" || statusCode == "" || place == "")
            {
                mens.Codigo = "ER0";
                mens.Descripcion = "Faltan Parámetros";
                log_mensaje += " - " + mens.Descripcion;
                logs.putLog(1, log_mensaje);
                return mens;
            }
            #endregion Validacion de Parametros

            #region Valida DV RutReceptor
            ErrLugar = "Valida DV RutReceptor";
            if (digito != "")
            {
                if (!Validaciones.validaRut(company))
                {
                    mens.Codigo = "ER1";
                    mens.Descripcion = "Rut Receptor no Válido";
                    log_mensaje += " - " + mens.Descripcion;
                    logs.putLog(1, log_mensaje);
                    return mens;
                }
                company = companySinDV;
            }
            #endregion Valida Rut Receptor

            #region Valida DV RutEmisor
            ErrLugar = "Valida DV RutEmisor";
            posicion = companyCodeSii.IndexOf("-");
            companyCodeSiiSinDV = companyCodeSii;
            if (companyCodeSii.IndexOf("-") >= 0)
            {
                digito = companyCodeSii.Substring(posicion + 1, 1);
                if (digito != "")
                {
                    if (!Validaciones.validaRut(companyCodeSii))
                    {
                        mens.Codigo = "ER0";
                        mens.Descripcion = "DV RutEmisor no corresponde.";
                        log_mensaje += " - " + mens.Descripcion;
                        logs.putLog(1, log_mensaje);
                        return mens;
                    }
                    companyCodeSiiSinDV = companyCodeSii.Substring(0, posicion);
                }
                companyCodeSii = companyCodeSiiSinDV;
            }
            #endregion Valida DV RutEmisor


            if (statusCode != "RME")
            {
                mens.Codigo = "ER4";
                mens.Descripcion = "Estado no es Válido, Estados Posibles RME: Recibo Mercaderia";
                logs.putLog(1, statusCode + " - " + mens.Descripcion);
            }
            else
            {
                if (!conexion.validaExistenciaRecibo(Convert.ToInt32(companyCodeSii), documentType, documentNumber))
                {
                    #region Documento no existe
                    ErrLugar = "Documento no existe o mercaderia ya recepcionada";
                    mens.Codigo = "ER5";
                    mens.Descripcion = "Documento no existe o mercaderia ya recepcionada.";
                    log_mensaje += " - " + mens.Descripcion + " Emisor : [" + companyCodeSii + "]" + " Tipo : [" + Convert.ToString(documentType) + "]" + " Folio : [" + Convert.ToString(documentNumber) + "].";
                    logs.putLog(1, log_mensaje);
                    if (conexion.MensajeError != "")
                    {
                        logs.putLog(1, "Error: " + conexion.MensajeError);
                    }
                    return mens;
                    #endregion Documento no existe
                }
                else
                {
                    #region Documento Existe
                    ErrLugar = "Documento Existe";
                    if (conexion.ActualizaEstadoRecibo(Convert.ToInt32(companyCodeSii),documentType, documentNumber, statusCode, place))
                    {
                        int codi_empr = 0;
                        int corr_docu = 0;

                        if (conexion.RegistraTraza(Metodo, "DTO", statusCode, codi_empr, documentType, documentNumber, corr_docu, Servicio))
                        {
                            ErrLugar = "Registra Traza";
                            mens.Codigo = "DOK";
                            mens.Descripcion = "Recepcion de mercaderia registrada ";
                            log_mensaje += " - " + mens.Descripcion + " Emisor : [" + companyCodeSii + "]" + " Tipo : [" + Convert.ToString(documentType) + "]" + " Folio : [" + Convert.ToString(documentNumber) + "].";
                            logs.putLog(1, log_mensaje);
                            conexion.confirma();
                            return mens;
                        }
                        else
                        {
                            ErrLugar = "Error al registrar Traza";
                            mens.Codigo = "DON";
                            mens.Descripcion = "Error al registrar Traza";
                            log_mensaje += " - " + mens.Descripcion + " Emisor : [" + companyCodeSii + "]" + " Tipo : [" + Convert.ToString(documentType) + "]" + " Folio : [" + Convert.ToString(documentNumber) + "].";
                            logs.putLog(1, log_mensaje);
                            logs.putLog(1, "Error: " + conexion.MensajeError);
                            conexion.rechaza();
                            return mens;
                        }
                    }
                    else
                    {
                        ErrLugar = "Error al actualizar Documento";
                        mens.Codigo = "DON";
                        mens.Descripcion = "Error al actualizar Documento";
                        log_mensaje += " - " + mens.Descripcion + " Emisor : [" + companyCodeSii + "]" + " Tipo : [" + Convert.ToString(documentType) + "]" + " Folio : [" + Convert.ToString(documentNumber) + "].";
                        logs.putLog(1, log_mensaje);
                        logs.putLog(1, "Error: " + conexion.MensajeError);
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
                mens.Descripcion = "Se ha producido el error : " + Convert.ToString(ex.Message) + ".";

                log_mensaje = "Rut Empresa : " + company + ". Error en " + ErrLugar + " : " + Convert.ToString(ex.Message) + ". " + ListaParametros;
                logs.putLog(1, log_mensaje);
            }
            else
            {
                mens.Codigo = "ERR";
                mens.Descripcion = "Se ha producido el error : " + ErrLugar + Convert.ToString(ex.Message) + ".";
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
        return mens;
    }

}