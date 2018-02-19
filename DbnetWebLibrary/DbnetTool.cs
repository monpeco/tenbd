using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;  

namespace DbnetWebLibrary
{
    public class asObject
    {
        static asObject()
        {
            //***
        }

        private string[] objeto = new string[100];
        private string[] valor = new string[100];
        private int[] texto = new int[100];
        private int contador = 0;

        public void Set(string objeto, string valor, int texto, string error)
        {
            Exception myException;


            if (valor == "")
            {
                myException = new Exception("El campo " + error + " no puede estar vacio");
                throw myException;
            }

            if (texto == 1)
                valor = valor.ToString().Replace(',', '.');
            string valor2=valor.Replace("-","");
            if (texto == 1 && !(System.Text.RegularExpressions.Regex.IsMatch(valor2, "^\\d{0,20}\\.{0,1}?\\d{1,4}$") ||
                                 System.Text.RegularExpressions.Regex.IsMatch(valor2, "^\\d{0,20}\\,{0,1}?\\d{1,4}$")))
            {
                myException = new Exception("El campo " + error + " debe ser numerico y distinto a vacio.");
                throw myException;
            }

            this.objeto[this.contador] = objeto;
            this.valor[this.contador] = valor;
            this.texto[this.contador] = texto;
            this.contador++;

        }
        public void Set(string objeto, string valor, int texto)
        {
            /*
             *   objeto -> nombre del campo en la base de datos
             * 
             *   valor  -> valor que se asigna
             * 
             *   texto  -> 0 si es texto y lleva ' ' . 
             *             1 si en numerico
             *             2 es fecha
             *             3 NULL
            */
            if (texto == 1)
                valor = valor.ToString().Replace(",", ".");
            if (texto == 1 && valor == "")
                valor = "0";

            string valor2 = valor.Replace("-", "");
            if (texto == 1 && !(System.Text.RegularExpressions.Regex.IsMatch(valor2, "^\\d{0,20}\\.{0,1}?\\d{1,6}$") ||
                                 System.Text.RegularExpressions.Regex.IsMatch(valor2, "^\\d{0,20}\\,{0,1}?\\d{1,6}$")))
            {
                Exception myException;
                myException = new Exception("Esta tratando de ingresar un Caracter en un valor numerico '" + valor + "'");
                throw myException;
            }

            this.objeto[this.contador] = objeto;
            this.valor[this.contador] = valor;
            this.texto[this.contador] = texto;
            this.contador++;
        }

        public string getObjeto(int indice)
        {
            return this.objeto[indice];
        }
        public string getValor(int indice)
        {
            if (this.valor[indice] == "nulo")
            {
                return "NULL";
            }
            else
            {
                if (this.texto[indice] == 1)
                {
                    if (this.valor[indice] == "")
                        return "0";
                    else
                        return this.valor[indice].Replace(',', '.');
                }
                else
                {
                    return "'" + this.valor[indice] + "'";
                }
            }
        }

        public int getContador()
        {
            return this.contador;
        }

        public void Clear()
        {
            for (int x = 0; x < this.contador; x++)
            {
                this.objeto[x] = "";
                this.valor[x] = "";
                this.texto[x] = 0;
            }
            this.contador = 0;
        }
    }

    public class DbnetTool
    {
        private DbnetTool()
        {
            // impedimos que se creen objetos de tipo Ambiente
        }
        public static string SQL(string accion, string tabla, asObject objeto, string condicion)
        {
            /* accion : I -> Insert
             *          U -> Update
             * 
             * tabla   : tabla a la cual afecta
             * 
             * objeto campos
             * 
             * condicion : sin where, ejemplo rut=rutcito and ....
             * */

            string query = "";
            string values = "values (";
            string campos = "(";
            string update = "";

            if (condicion != "")
                condicion = " where " + condicion;

            switch (accion)
            {
                case "I":
                    //retorna consulta insert
                    for (int x = 0; x < objeto.getContador(); x++)
                    {
                        campos += objeto.getObjeto(x) + " ,";
                        values += objeto.getValor(x) + " ,";
                    }
                    values = values.Substring(0, values.Length - 1).ToString() + ")";
                    campos = campos.Substring(0, campos.Length - 1).ToString() + ")";

                    query = "insert into " + tabla + " " + campos + " " + values + " " + condicion;
                    return query;
                case "U":
                    //retorna consulta update
                    for (int x = 0; x < objeto.getContador(); x++)
                    {
                        update += objeto.getObjeto(x) + "=" + objeto.getValor(x) + " ,";
                    }
                    update = update.Substring(0, update.Length - 1).ToString();

                    query = "update " + tabla + " set " + update + " " + condicion;
                    return query;
            }
            return query;
        }

        public static string recuperaMenu(string archivo)
        {
            StreamReader fileLog = new StreamReader(archivo);
            string line, log = "";
            line = fileLog.ReadLine();
            while (line != null)
            {
                log = log + line;
                line = fileLog.ReadLine();
            }
            fileLog.Close();

            return (log);
        }

        public static bool ctrlSqlInjection(HtmlForm _form)
        {
            ControlCollection _control = _form.Controls;
            for (int i = 0; i < _control.Count; i++)
            {
                if (_control[i] is TextBox)
                {
                    if (_control[i].ID != "txtWhere" && _control[i].ID != "Mnsg_erro" && _control[i].ID != "Mens_esta" && _control[i].ID != "SQLC_ALAR" && _control[i].ID != "SQLI_ALAR" && _control[i].ID != "ULTI_ERRO" && _control[i].ID != "Dire_emis" && _control[i].ID != "Nomb_emis" && _control[i].ID != "Nomb_rece" && _control[i].ID != "Dire_rece")
                    {
                        valida_texbox(((TextBox)_control[i]).Text);
                    }
                }
            }
            return true;
        }

        private static void valida_texbox(string _textbox)
        {
            if (_textbox.ToLower().IndexOf(" union select ") != -1)
            {
                if(_textbox.ToLower().Contains("select") && _textbox.ToLower().Contains("from"))
                throw new Exception("union select, no permitido");
            }
            else if (_textbox.ToLower().IndexOf(" or ") != -1)
            {
                if (_textbox.ToLower().Contains("'") || _textbox.ToLower().Contains("(") || _textbox.ToLower().Contains(")"))
                throw new Exception("or, no permitido");
            }
            else if (_textbox.ToLower().IndexOf("select ") != -1 || _textbox.ToLower().IndexOf("select*") != -1)
            {
                if (_textbox.ToLower().Contains("from"))
                throw new Exception("select, no permitido");
            }
            else if (_textbox.ToLower().IndexOf("update ") != -1)
            {
                if (_textbox.ToLower().Contains("set"))
                throw new Exception("update, no permitido");
            }
            else if (_textbox.ToLower().IndexOf("insert ") != -1)
            {
                if (_textbox.ToLower().Contains("values"))
                throw new Exception("insert, no permitido");
            }
            else if (_textbox.ToLower().IndexOf(";") != -1)
            {
                if (_textbox.ToLower().Contains("insert") || _textbox.ToLower().Contains("update") 
                    || _textbox.ToLower().Contains("delete") || _textbox.ToLower().Contains("drop"))
                throw new Exception("punto y coma, no permitido");
            }
            else if (_textbox.ToLower().IndexOf("'") != -1)
            {
                if (_textbox.ToLower().Contains("set") || _textbox.ToLower().Contains("values"))
                throw new Exception("comillas simple, no permitido");
            }
            else if (_textbox.ToLower().IndexOf("\"") != -1)
            {
                if (_textbox.ToLower().Contains("set") || _textbox.ToLower().Contains("values"))
                throw new Exception("comillas doble, no permitido");
            }
            else if (_textbox.ToLower().IndexOf(";delete ") != -1)
            {
                if (_textbox.ToLower().Contains("delete") && _textbox.ToLower().Contains(";"))
                throw new Exception("delete, no permitido");
            }
            else if (_textbox.ToLower().IndexOf(" delete ") != -1)
            {
                if (_textbox.ToLower().Contains("delete") && _textbox.ToLower().Contains(";"))
                throw new Exception("delete, no permitido");
            }
            else if (_textbox.ToLower().IndexOf("delete ") != -1)
            {
                if (_textbox.ToLower().Contains("delete") && _textbox.ToLower().Contains(";"))
                    throw new Exception("delete, no permitido");
            }
            else if (_textbox.ToLower().IndexOf("alter") != -1)
            {
                if (_textbox.ToLower().Contains("table") || _textbox.ToLower().Contains("view") ||
                    _textbox.ToLower().Contains("procedure") || _textbox.ToLower().Contains("function") ||
                    _textbox.ToLower().Contains("database"))
                throw new Exception("alter, no permitido");
            }
            else if (_textbox.ToLower().IndexOf(" alter ") != -1)
            {
                if (_textbox.ToLower().Contains("table") || _textbox.ToLower().Contains("view") ||
                    _textbox.ToLower().Contains("procedure") || _textbox.ToLower().Contains("function") ||
                    _textbox.ToLower().Contains("database"))
                throw new Exception("alter, no permitido");
            }
            else if (_textbox.ToLower().IndexOf(";drop ") != -1)
            {
                if (_textbox.ToLower().Contains("table") || _textbox.ToLower().Contains("view") ||
                    _textbox.ToLower().Contains("procedure") || _textbox.ToLower().Contains("function") ||
                    _textbox.ToLower().Contains("database"))
                throw new Exception("drop, no permitido");
            }
            else if (_textbox.ToLower().IndexOf(" drop ") != -1)
            {
                if (_textbox.ToLower().Contains("table") || _textbox.ToLower().Contains("view") ||
                    _textbox.ToLower().Contains("procedure") || _textbox.ToLower().Contains("function") ||
                    _textbox.ToLower().Contains("database"))
                throw new Exception("drop, no permitido");
            }
        }


            
        public static void MsgError(string mensaje, System.Web.UI.Page pagina)
        {
            /*
            if (mensaje.Trim().Length == 0)
            {
                mensaje = "Respuesta no cargada";
                lbMensaje.Text += "Respuesta no cargada <br>";
            }
             */
            string scr = "<script>alert(\"" + mensaje + "\");</script>";
            pagina.ClientScript.RegisterStartupScript(typeof(Page), "msgError", scr);
        }
        public static void MsgAlerta(string mensaje, System.Web.UI.Page pagina)
        {
            string scr = "<script>alert(\"" + mensaje + "\");</script>";
            pagina.ClientScript.RegisterStartupScript(typeof(Page), "msgError", scr);
        }
        public static string Initcap(string sEntrada)
        {
            int i, sw = 1;
            string sSalida = "";
            char[] cadena;

            if (sEntrada != null)
            {
                cadena = sEntrada.ToCharArray();
                for (i = 0; i < sEntrada.Length; i++)
                {
                    if (sw == 1)
                        sSalida = sSalida + cadena[i].ToString().ToUpper();
                    else
                        sSalida = sSalida + cadena[i].ToString().ToLower();
                    if (cadena[i] == ' ' || cadena[i] == '.' || cadena[i] == ',')
                        sw = 1;
                    else
                        sw = 0;
                }
            }
            return sSalida;
        }

        public static string SelectInto(OleDbConnection dbConn, string query)
        {
            string valor;
            OleDbCommand cmd = new OleDbCommand();
            OleDbDataAdapter da = new OleDbDataAdapter();
            DataTable dt = new DataTable();

            cmd.Connection = dbConn;
            cmd.CommandText = query;
            da.SelectCommand = cmd;
            cmd.CommandTimeout = 900;

            dt.Clear();
            da.Fill(dt);

            DataRow[] currRows = dt.Select("", "");

            if (currRows.Length < 1)
                return "";
            else
            {
                foreach (DataRow myRow in currRows)
                {
                    foreach (DataColumn myCol in dt.Columns)
                    {
                        valor = myRow[myCol].ToString();
                        return valor;
                    }
                }
            }
            return "";
        }

        public static DataTable Ejecuta_Select(OleDbConnection dbConn, string query)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbDataAdapter da = new OleDbDataAdapter();
            DataTable dt = new DataTable();

            cmd.Connection = dbConn;
            cmd.CommandText = query;
            da.SelectCommand = cmd;
            cmd.CommandTimeout = 900;

            dt.Clear();
            da.Fill(dt);

            return dt;
        }

        public static bool valida_certificacion(OleDbConnection dbConn, int codigo_empresa)
        {
            string clave_empr = null;
            string rutt_empr = null;
            OleDbCommand select = new OleDbCommand(@"SELECT RUTT_EMPR,CLAV_ENCR FROM EMPR WHERE CODI_EMPR = " + codigo_empresa, dbConn);
            OleDbDataReader reader = select.ExecuteReader();
            reader.Read();
            try
            {
                if (reader.GetValue(reader.GetOrdinal("RUTT_EMPR")) == System.DBNull.Value)
                {
                    reader.Close();
                    return false;
                }
                else
                {
                    if (reader.GetValue(reader.GetOrdinal("CLAV_ENCR")) == System.DBNull.Value)
                    {
                        reader.Close();
                        return false;
                    }
                    else
                    {
                        rutt_empr = reader.GetValue(reader.GetOrdinal("RUTT_EMPR")).ToString();
                        clave_empr = reader.GetValue(reader.GetOrdinal("CLAV_ENCR")).ToString();

                        reader.Close();

                        //Cambio por Licenciamiento ASP
                        string existe_code_lic = "";
                        OleDbCommand select_lic = new OleDbCommand(@"SELECT CODE_DESC FROM SYS_CODE WHERE DOMAIN_CODE = 0 AND CODE = 'LIC'");
                        select_lic.Connection = dbConn;
                        OleDbDataReader reader_lic = select_lic.ExecuteReader();
                        reader_lic.Read();

                        try
                        {
                            //Extrayendo valor del codigo LIC de la tabla SYS_CODE --> Licencia ASP
                            existe_code_lic = reader_lic.GetValue(reader_lic.GetOrdinal("CODE_DESC")).ToString();
                            reader_lic.Close();
                        }
                        catch
                        {
                            reader_lic.Close();
                            existe_code_lic = "0";
                        }
                        string clave_encrip = "";
                        //Si codigo LIC == 1, entonces clave encryptada es ASP<RUT>
                        //Si codigo LIC <> 1, entonces clave encryptada es DBNeT<RUT>

                        if (existe_code_lic == "1")
                        {
                            //Licenciamiento ASP
                            clave_encrip = DbnetSecurity.encr_vari("ASP" + rutt_empr);
                        }
                        else //if (existe_code_lic == "0")
                        {
                            //Licenciamiento DBNeT
                            clave_encrip = DbnetSecurity.encr_vari("DBNeT" + rutt_empr);
                        }
                        if (clave_encrip == clave_empr)
                        {
                            reader.Close();
                            return true;
                        }
                        else
                        {
                            reader.Close();
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static string SeteaMensaje(string mensaje)
        {
            return SeteaMensaje("N", mensaje);
        }
        public static string SeteaMensaje(string tipo, string mensaje)
        {
            if (tipo == "N")		/* Normal */
                return "<li>" + mensaje;
            else if (tipo == "R")	/* Remarcado */
                return "<b>" + mensaje + "</b>";
            else
                return mensaje;
        }

        public static Int32 Ejecuta_Proceso(string TipoProce, DbnetSesion ctx, string proceso, string parametros)
        {
            int NumeProc = 0;
            string salida = "";
            try
            {
                
                System.Diagnostics.ProcessStartInfo pinfo = new System.Diagnostics.ProcessStartInfo();
                pinfo.UseShellExecute = true;
                pinfo.RedirectStandardOutput = false;

                pinfo.FileName = proceso;

                pinfo.Arguments = parametros;
                pinfo.Arguments += " -h " + DbnetTool.SelectInto(ctx.dbConnection, "select param_value from sys_param where param_name='EGATE_HOME'");

                salida += SeteaMensaje("R", "Proceso Ejecutado:<br>");
                salida += SeteaMensaje("N", pinfo.FileName + pinfo.Arguments);
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pinfo);
                NumeProc = p.Id;
            }
            catch (Exception ex)
            {
                salida += "<br>" + ex.Message;
                NumeProc += 0;
            }
            return NumeProc;
        }
        public static string Ejecuta_Proceso(DbnetSesion ctx, string proceso, string parametros)
        {
            string salida = "";
            try
            {
                System.Diagnostics.ProcessStartInfo pinfo = new System.Diagnostics.ProcessStartInfo();
                pinfo.UseShellExecute = true;
                pinfo.RedirectStandardOutput = false;
                
                pinfo.FileName = proceso;

                pinfo.Arguments += " ";
                pinfo.Arguments += parametros;
                pinfo.Arguments += " -h " + DbnetTool.SelectInto(ctx.dbConnection, "select param_value from sys_param where param_name='EGATE_HOME'");

                salida += SeteaMensaje("R", "Proceso Ejecutado:<br>");
                salida += SeteaMensaje("N", pinfo.FileName + pinfo.Arguments);

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                salida += "<br>" + ex.Message;
            }
            return salida;
        }
        public static string Ejecuta_Proceso(DbnetSesion ctx, string proceso, string parametros, int cola, string so)
        {
            string salida = "";

            try
            {
                parametros += " -h " + DbnetTool.SelectInto(ctx.dbConnection, "select param_value from sys_param where param_name='EGATE_HOME'");
               
                String Query;

                if (so == "unix")
                {
                    proceso = proceso.Replace(@"\", "/");
                    parametros = parametros.Replace(@"\", "/");
                }

                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {

                    Query = "Insert into se_pipe(pipe_stat, pipe_cmd, pipe_codi_usua, cola_proc)" +
                            "values ('P' ," +
                            "'" + proceso + " " + parametros + "'," +
                            "'" + ctx.Codi_usua + "','" + Convert.ToString(cola) + "')";

                    DbnetTool.Ejecuta_Select(ctx.dbConnection, Query);

                }
                else
                {

                    Query = "Insert into se_pipe(pipe_id,pipe_stat, pipe_cmd, pipe_codi_usua, cola_proc)" +
                                "values (seq_se_pipe.nextval(),'P' ," +
                                "'" + proceso + " " + parametros + "'," +
                                "'" + ctx.Codi_usua + "','" + Convert.ToString(cola) + "')";

                    DbnetTool.Ejecuta_Select(ctx.dbConnection, Query);
                }

            }
            catch (Exception)
            {}
            return salida;
        }
        public static string Ejecuta_ProcesoPFX(DbnetSesion ctx, string proceso, string parametros)
        {
            string salida = "";
            try
            {
                System.Diagnostics.ProcessStartInfo pinfo = new System.Diagnostics.ProcessStartInfo();
                pinfo.UseShellExecute = true;
                pinfo.RedirectStandardOutput = false;
                pinfo.FileName = proceso;
                pinfo.Arguments = parametros;
                salida += SeteaMensaje("R", "Proceso Ejecutado:<br>");
                salida += SeteaMensaje("N", pinfo.FileName + pinfo.Arguments);

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pinfo);
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                salida += "<br>" + ex.Message;
            }
            return salida;
        }
        public static string Ejecuta_Proceso_Multiple(DbnetSesion ctx, string proceso, string parametros)
        {
            string salida = "";
            try
            {
                System.Diagnostics.ProcessStartInfo pinfo = new System.Diagnostics.ProcessStartInfo();
                pinfo.UseShellExecute = true;
                pinfo.RedirectStandardOutput = false;
                pinfo.FileName = proceso;

                pinfo.Arguments = parametros;
                pinfo.Arguments += " -h " + DbnetTool.SelectInto(ctx.dbConnection, "select param_value from sys_param where param_name='EGATE_HOME'");

                //salida += SeteaMensaje("R", "Proceso Ejecutado:<br>");
                salida += SeteaMensaje("N", pinfo.FileName + pinfo.Arguments);

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                salida += "<br>" + ex.Message;
            }
            return salida;
        }
        public static string Ejecuta_Proceso_Multiple(DbnetSesion ctx, string proceso, string parametros, int cola, string so)
        {
            string salida = "";

            try
            {
                parametros += " -h " + DbnetTool.SelectInto(ctx.dbConnection, "select param_value from sys_param where param_name='EGATE_HOME'");

                String Query;

                if (so == "unix")
                {
                    proceso = proceso.Replace(@"\", "/");
                    parametros = parametros.Replace(@"\", "/");
                }

                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {

                    Query = "Insert into se_pipe(pipe_stat, pipe_cmd, pipe_codi_usua, cola_proc)" +
                            "values ('P' ," +
                            "'" + proceso + " " + parametros + "'," +
                            "'" + ctx.Codi_usua + "','" + Convert.ToString(cola) + "')";

                    DbnetTool.Ejecuta_Select(ctx.dbConnection, Query);

                }
                else
                {

                    Query = "Insert into se_pipe(pipe_id,pipe_stat, pipe_cmd, pipe_codi_usua, cola_proc)" +
                                "values (seq_se_pipe.nextval(),'P' ," +
                                "'" + proceso + " " + parametros + "'," +
                                "'" + ctx.Codi_usua + "','" + Convert.ToString(cola) + "')";

                    DbnetTool.Ejecuta_Select(ctx.dbConnection, Query);
                }

            }
            catch (Exception)
            {}
            return salida;
        }


        public static string Ejecuta_Proc_Retorna_Error(DbnetSesion ctx, string proceso, string parametros)
        {
            string salida = "";
            try
            {
                System.Diagnostics.ProcessStartInfo pinfo = new System.Diagnostics.ProcessStartInfo();
                pinfo.UseShellExecute = true;
                pinfo.RedirectStandardOutput = false;
                pinfo.FileName = proceso;

                pinfo.Arguments = parametros;
                pinfo.Arguments += " -h " + DbnetTool.SelectInto(ctx.dbConnection, "select param_value from sys_param where param_name='EGATE_HOME'");

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pinfo);
                p.WaitForExit();
                if (p.ExitCode == 1)
                {
                    salida = "El proceso termino con Errores verifique el LOG Asociado en el directorio Log de la Suite";
                }
                else
                {
                    salida += SeteaMensaje("R", "Proceso Ejecutado:<br>");
                    salida += SeteaMensaje("N", pinfo.FileName + pinfo.Arguments);
                }
            }
            catch (Exception ex)
            {
                salida += "<br>" + ex.Message;
            }
            return salida;
        }
        //Ejecuta_Proceso_Espera con division egateDTE
        
        public static string Ejecuta_Proceso_Espera(DbnetSesion ctx, string proceso, string parametros)
        {
            string salida = "";
            try
            {
                System.Diagnostics.ProcessStartInfo pinfo = new System.Diagnostics.ProcessStartInfo();
                pinfo.UseShellExecute = true;
                pinfo.RedirectStandardOutput = false;

                //aca pregunto por el proceso=egateDTE y leo los parametros y busco el. -ts flat y -ts xml y asigno los egateDTE correspondientes
                if (proceso.ToString() == "egateDTE")
                {
                    string[] cadena = new String[5];
                    char[] splitter = { ' ' };
                    bool bandera_te_bd = false;
                    bool bandera_ts_xml = false;
                    bool bandera_te_flat = false;
                    bool bandera_ts_bd = false;
                    
                    cadena = parametros.Split(splitter);
                    for (int x = 0; x < cadena.Length; x++)
                    {
                        if (cadena[x].ToString() == "-te" && cadena[x + 1].ToString() == "bd")
                        { bandera_te_bd = true; }

                        if (cadena[x].ToString() == "-ts" && cadena[x + 1].ToString() == "xml")
                        { bandera_ts_xml = true; }

                        if (cadena[x].ToString() == "-te" && cadena[x + 1].ToString() == "flat")
                        { bandera_te_flat = true; }

                        if (cadena[x].ToString() == "-ts" && cadena[x + 1].ToString() == "bd")
                        { bandera_ts_bd = true; }
                    }

                    if ((bandera_te_bd) && (bandera_ts_xml))
                    {
                        proceso = "egateDTExml";
                    }
                    
                    if ((bandera_te_flat) && (bandera_ts_bd))
                    {
                        proceso = "egateDTEcarga";
                    }
                }
                // hasta aca el proceso de asignar el egateDTE
                pinfo.FileName = proceso.Trim();
                if (!parametros.Substring(0, 1).Equals(" "))
                    parametros = " " + parametros;
                pinfo.Arguments = parametros;
                pinfo.CreateNoWindow = true;
                pinfo.WorkingDirectory = Path.Combine(System.Environment.GetEnvironmentVariable("EGATE_HOME"), "bin"); ;
                pinfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                pinfo.Arguments += " -h " + DbnetTool.SelectInto(ctx.dbConnection, "select param_value from sys_param where param_name='EGATE_HOME'");
                salida += SeteaMensaje("R", "Proceso Ejecutado:<br>");
                salida += SeteaMensaje("N", pinfo.FileName + pinfo.Arguments);
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pinfo);
                p.WaitForExit();
                p.Close();

                p.Dispose();			
			}
			catch (Exception ex)
			{
				salida += "<br>" + ex.Message;
			}
			return salida;
		}
        

        public static string Ejecuta_Proceso_Espera(DbnetSesion ctx, string proceso, string parametros, int cola, string so)
        {
            string salida = "";

            try
            {
                parametros += " -h " + DbnetTool.SelectInto(ctx.dbConnection, "select param_value from sys_param where param_name='EGATE_HOME'");

                String Query;

                if (so == "unix")
                {
                    proceso = proceso.Replace(@"\","/");
                    parametros = parametros.Replace(@"\", "/");
                }


                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {
                    Query = "Insert into se_pipe(pipe_stat, pipe_cmd, pipe_codi_usua, cola_proc)" +
                            "values ('P' ," +
                            "'" + proceso + " " + parametros + "'," +
                            "'" + ctx.Codi_usua + "','" + Convert.ToString(cola) + "')";
                    DbnetTool.Ejecuta_Select(ctx.dbConnection, Query);
                }
                else //ORACLE
                {
                    Query = "Insert into se_pipe(pipe_id,pipe_stat, pipe_cmd, pipe_codi_usua, cola_proc)" +
                                "values (seq_se_pipe.nextval(),'P' ," +
                                "'" + proceso + " " + parametros + "'," +
                                "'" + ctx.Codi_usua + "','" + Convert.ToString(cola) + "')";

                    DbnetTool.Ejecuta_Select(ctx.dbConnection, Query);
                }
            }
            catch (Exception)
            {}
            return salida;
        }


        public static string ConvertXML(XmlDocument InputXMLDocument, string XSLTFilePath , XsltArgumentList XSLTArgs)
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            XslCompiledTransform xslTrans = new XslCompiledTransform();
            xslTrans.Load(XSLTFilePath);
            xslTrans.Transform(InputXMLDocument.CreateNavigator(), XSLTArgs, sw);
            return sw.ToString();
        }

        public static string ProcesaXslt(String _xmlPath, String _xslPath, String _archivoPath)
        {
            try
            {
                /* load the Xml doc */
                XmlDocument doc = new XmlDocument();
                doc.Load(_xmlPath);
                XPathDocument myXPathDoc = new XPathDocument(new StringReader(doc.InnerXml));

                /* load the Xsl */
                XslCompiledTransform myXslTrans = new XslCompiledTransform();
                myXslTrans.Load(_xslPath);

                /* create the output stream */
                XmlTextWriter myWriter = new XmlTextWriter(_archivoPath, Encoding.Default);

                /* do the actual transform of Xml */
                myXslTrans.Transform(myXPathDoc, null, myWriter);
                myWriter.Close();
                return "";
            }
            catch (Exception ex)
            { return "<br>" + ex.Message;  }

        }

        public static string ProcesaXsltString(String _xmlString, String _xslPath, String _archivoPath)
        {
            try
            {
                string sValue = string.Empty;
                /* load the Xml doc */
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_xmlString);
                    XPathDocument myXPathDoc = new XPathDocument(new StringReader(doc.InnerXml));

                    /* load the Xsl */
                    XslCompiledTransform myXslTrans = new XslCompiledTransform();
                    myXslTrans.Load(_xslPath);

                    /* create the output stream */
                    XmlTextWriter myWriter = new XmlTextWriter(_archivoPath, Encoding.Default);

                    /* do the actual transform of Xml */
                    myXslTrans.Transform(myXPathDoc, null, myWriter);
                    myWriter.Close();

                    if (File.Exists(_archivoPath))
                    {
                        foreach (string sLectorHtml in File.ReadAllLines(_archivoPath))
                        {
                            sValue += sLectorHtml;
                        }
                        File.Delete(_archivoPath);
                    }
                    return sValue;
                }
                catch(Exception Ex)
                {
                    sValue = "ProcesaXsltString(): " + Ex.Message;
                    return sValue;
                }
            }
            catch (Exception ex)
            { return "<br>" + ex.Message; }

        }

        public static string Ejecuta_Proceso_Xslt(DbnetSesion ctx, string proceso, string parametros)
        {
            string salida = "";
            try
            {
                System.Diagnostics.ProcessStartInfo pinfo = new System.Diagnostics.ProcessStartInfo();
                pinfo.UseShellExecute = true;
                pinfo.RedirectStandardOutput = false;
                pinfo.FileName = proceso;
                
                pinfo.Arguments = parametros;
                
                salida += SeteaMensaje("R", "Proceso Ejecutado:<br>");
                salida += SeteaMensaje("N", pinfo.FileName + pinfo.Arguments);

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pinfo);
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                salida += "<br>" + ex.Message;
            }
            return salida;
        }
       
        public static string Ejecuta_Proceso_Xslt(DbnetSesion ctx, string proceso, string parametros, int cola, string so)
        {
            string salida = "";
            try
            {

                String Query;
                
                if (so == "unix")
                {
                    proceso = proceso.Replace(@"\", "/");
                    parametros = parametros.Replace(@"\", "/");
                }


                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {

                    Query = "Insert into se_pipe(pipe_stat, pipe_cmd, pipe_codi_usua, cola_proc)" +
                            "values ('P' ," +
                            "'" + proceso + " " + parametros + "'," +
                            "'" + ctx.Codi_usua + "','" + Convert.ToString(cola) + "')";
                    DbnetTool.Ejecuta_Select(ctx.dbConnection, Query);

                }
                else //ORACLE
                {
                    Query = "Insert into se_pipe(pipe_id,pipe_stat, pipe_cmd, pipe_codi_usua, cola_proc)" +
                                "values (seq_se_pipe.nextval(),'P' ," +
                                "'" + proceso + " " + parametros + "'," +
                                "'" + ctx.Codi_usua + "','" + Convert.ToString(cola) + "')";

                    DbnetTool.Ejecuta_Select(ctx.dbConnection, Query);
                }
            }
            catch (Exception)
            {}
            return salida; 
        }

		public static DataTable DataView_To_DataTable(DataView obDataView)
		{
			if (null == obDataView)
			{
				throw new ArgumentNullException	("DataView", "Invalid DataView object specified");
			}
			DataTable obNewDt = obDataView.Table.Clone();
			int idx = 0;
			string [] strColNames = new string[obNewDt.Columns.Count];
			foreach (DataColumn col in obNewDt.Columns)
			{
				strColNames[idx++] = col.ColumnName;
			}

			IEnumerator viewEnumerator = obDataView.GetEnumerator();
			while (viewEnumerator.MoveNext())
			{
				DataRowView drv = (DataRowView)viewEnumerator.Current;
				DataRow dr = obNewDt.NewRow();
				try
				{
					foreach (string strName in strColNames)
					{
						dr[strName] = drv[strName];
					}
				}
				catch
				{
				}
				obNewDt.Rows.Add(dr);
			}
			return obNewDt;
		}

        public static System.Web.UI.WebControls.DataGrid Formatea_Grid(System.Web.UI.WebControls.DataGrid Grilla)
        {
            for (int x = 0; x < Grilla.Columns.Count; x++)
            {
                string caso = (string)Grilla.Columns[x].HeaderText;
                string texte = "";
                switch (caso)
                {
                    case "total":
                        texte = Grilla.Columns[x].ItemStyle.CssClass;// = dbnGrillaDerecha;
                        break;
                    case "precio":
                        texte = (string)Grilla.Columns[x].HeaderText;
                        break;
                    case "monto":
                        texte = (string)Grilla.Columns[x].HeaderText;
                        break;
                }
            }
            return Grilla;
        }

        /// <summary>
        /// retorna el nombre estandar para la generacion de archivo 
        /// </summary>
        /// <param name="cod_empr">codigo empresas</param>
        /// <param name="folio"> folio del documento</param>
        /// <param name="tipodoc">numero del tipo de documento</param>
        /// <returns>retorna el nombre estandar de un archivo especifico</returns>
        public static string Nombre_Estandar(DbnetSesion ctx, string cod_empr, string tipodoc, string folio)
        {
            string rut, tipo, foli, rut_f, tipodoc_f, folio_f, nombre;
            string query = "select rutt_emis,tipo_docu,foli_docu from dte_enca_docu ";
            query += "where codi_empr = " + cod_empr + " and foli_docu = " + folio;
            query += "  and tipo_docu = " + tipodoc;
            nombre = "";

            DataTable dt = DbnetTool.Ejecuta_Select(ctx.dbConnection, query);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                rut = dr["rutt_emis"].ToString();
                tipo = dr["tipo_docu"].ToString();
                foli = dr["foli_docu"].ToString();

                int cerosF = 10 - foli.Length;
                int i = 0;

                rut_f = "E";
                tipodoc_f = "T";
                folio_f = "F";

                if (rut.Length == 9)
                    rut_f += rut;
                else
                    rut_f += "0" + rut;

                if (tipo.Length == 3)
                    tipodoc_f += tipo;
                else
                    tipodoc_f += "0" + tipo;

                while (i < cerosF)
                {
                    folio_f += "0";
                    i++;
                }
                folio_f += foli;
                nombre = rut_f + "" + tipodoc_f + "" + folio_f;
            }
            return nombre;
        }

        public static string Get_Param(DbnetSesion ctx, string ParametroBD, string ParametroEM)
        {
            string Parametro = "";
            string query = "select valo_paem " + " " +
                           "from para_empr " +
                           "where codi_empr = " + ctx.Codi_empr + " " +
                           "and codi_paem = '" + ParametroEM + "' ";

            DataTable dt = DbnetTool.Ejecuta_Select(ctx.dbConnection, query);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                Parametro = dr["VALO_PAEM"].ToString();
            }
            else
            {
                query = "select param_value " + " " +
                        "from sys_param " +
                        "where param_name = '" + ParametroBD + "' ";

                dt = Ejecuta_Select(ctx.dbConnection, query);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    Parametro = dr["PARAM_VALUE"].ToString();
                }
                else
                {
                    Parametro = "%NULL%";
                }
            }
            return Parametro;
        }

        public static void registraPagina(DbnetSesion ctx, string pagina)
        {
            string query;

            string vAuditoria = DbnetTool.SelectInto(ctx.dbConnection, "SELECT PARAM_VALUE FROM sys_param WHERE PARAM_NAME ='SECU_AUDI_MENU'");
            if (vAuditoria == "S")
            {
                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {
                    query = "INSERT INTO sys_sess_prog " +
                            "           (corr_sess, desc_opci, fech_opci, codi_usua) " +
                            "VALUES     (" + ctx.Corr_sess + ", " +
                            "           '" + pagina + "', " +
                            "           GETDATE(), " +
                            "           '" + ctx.Codi_usua + "')";
                }
                else
                {
                    query = "INSERT INTO sys_sess_prog " +
                            "           (corr_sess, desc_opci, fech_opci, codi_usua) " +
                            "VALUES     (" + ctx.Corr_sess + ", " +
                            "           '" + pagina + "', " +
                            "           SYSDATE, " +
                            "           '" + ctx.Codi_usua + "')";
                }
                DbnetTool.Ejecuta_Select(ctx.dbConnection, query);
            }
        }

    }
	public class DbnetProcedure
	{
		private OleDbCommand sp;
		private string[] nombres;
		private string[] valores;
		private string[] tipos;
		private int[] largos;
		private string[] inout;

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1)
		{
			this.nombres = new string[1];
			this.valores = new string[1];
			this.tipos = new string[1];
			this.largos = new int[1];
			this.inout = new string[1];

			this.nombres[0] = nombre1;
			this.valores[0] = valor1;
			this.tipos[0] = tipo1;
			this.largos[0] = largo1;
			this.inout[0] = inout1;
			
			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1, 
			string nombre2, string valor2, string tipo2, int largo2, string inout2)
		{
			this.nombres = new string[2];
			this.valores = new string[2];
			this.tipos = new string[2];
			this.largos = new int[2];
			this.inout = new string[2];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			
			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			
			this.largos[0] = largo1;
			this.largos[1] = largo2;
			
			this.inout[0] = inout1;
			this.inout[1] = inout2;
			
			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1, 
			string nombre2, string valor2, string tipo2, int largo2, string inout2, 
			string nombre3, string valor3, string tipo3, int largo3, string inout3)
		{
			this.nombres = new string[3];
			this.valores = new string[3];
			this.tipos = new string[3];
			this.largos = new int[3];
			this.inout = new string[3];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			
			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			
			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			
			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			
			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1, 
			string nombre2, string valor2, string tipo2, int largo2, string inout2,	
			string nombre3, string valor3, string tipo3, int largo3, string inout3, 
			string nombre4, string valor4, string tipo4, int largo4, string inout4)
		{
			this.nombres = new string[4];
			this.valores = new string[4];
			this.tipos = new string[4];
			this.largos = new int[4];
			this.inout = new string[4];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			
			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			
			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			
			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			
			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1, 
			string nombre2, string valor2, string tipo2, int largo2, string inout2,	
			string nombre3, string valor3, string tipo3, int largo3, string inout3,	
			string nombre4, string valor4, string tipo4, int largo4, string inout4,	
			string nombre5, string valor5, string tipo5, int largo5, string inout5)
		{
			this.nombres = new string[5];
			this.valores = new string[5];
			this.tipos = new string[5];
			this.largos = new int[5];
			this.inout = new string[5];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			
			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			
			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			
			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			
			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1, 
			string nombre2, string valor2, string tipo2, int largo2, string inout2,	
			string nombre3, string valor3, string tipo3, int largo3, string inout3,	
			string nombre4, string valor4, string tipo4, int largo4, string inout4,	
			string nombre5, string valor5, string tipo5, int largo5, string inout5,	
			string nombre6, string valor6, string tipo6, int largo6, string inout6)
		{
			this.nombres = new string[6];
			this.valores = new string[6];
			this.tipos = new string[6];
			this.largos = new int[6];
			this.inout = new string[6];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			this.nombres[5] = nombre6;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			this.valores[5] = valor6;
			
			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			this.tipos[5] = tipo6;
			
			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			this.largos[5] = largo6;
			
			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			this.inout[5] = inout6;
			
			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1,
			string nombre2, string valor2, string tipo2, int largo2, string inout2,
			string nombre3, string valor3, string tipo3, int largo3, string inout3,
			string nombre4, string valor4, string tipo4, int largo4, string inout4,
			string nombre5, string valor5, string tipo5, int largo5, string inout5,
			string nombre6, string valor6, string tipo6, int largo6, string inout6,
			string nombre7, string valor7, string tipo7, int largo7, string inout7)
		{
			this.nombres = new string[7];
			this.valores = new string[7];
			this.tipos = new string[7];
			this.largos = new int[7];
			this.inout = new string[7];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			this.nombres[5] = nombre6;
			this.nombres[6] = nombre7;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			this.valores[5] = valor6;
			this.valores[6] = valor7;
			
			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			this.tipos[5] = tipo6;
			this.tipos[6] = tipo7;
			
			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			this.largos[5] = largo6;
			this.largos[6] = largo7;
			
			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			this.inout[5] = inout6;
			this.inout[6] = inout7;
			
			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1,
			string nombre2, string valor2, string tipo2, int largo2, string inout2,
			string nombre3, string valor3, string tipo3, int largo3, string inout3,
			string nombre4, string valor4, string tipo4, int largo4, string inout4,
			string nombre5, string valor5, string tipo5, int largo5, string inout5,
			string nombre6, string valor6, string tipo6, int largo6, string inout6,
			string nombre7, string valor7, string tipo7, int largo7, string inout7,
			string nombre8, string valor8, string tipo8, int largo8, string inout8)
		{
			this.nombres = new string[8];
			this.valores = new string[8];
			this.tipos = new string[8];
			this.largos = new int[8];
			this.inout = new string[8];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			this.nombres[5] = nombre6;
			this.nombres[6] = nombre7;
			this.nombres[7] = nombre8;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			this.valores[5] = valor6;
			this.valores[6] = valor7;
			this.valores[7] = valor8;
			
			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			this.tipos[5] = tipo6;
			this.tipos[6] = tipo7;
			this.tipos[7] = tipo8;
			
			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			this.largos[5] = largo6;
			this.largos[6] = largo7;
			this.largos[7] = largo8;
			
			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			this.inout[5] = inout6;
			this.inout[6] = inout7;
			this.inout[7] = inout8;
			
			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1,
			string nombre2, string valor2, string tipo2, int largo2, string inout2,
			string nombre3, string valor3, string tipo3, int largo3, string inout3,
			string nombre4, string valor4, string tipo4, int largo4, string inout4,
			string nombre5, string valor5, string tipo5, int largo5, string inout5,
			string nombre6, string valor6, string tipo6, int largo6, string inout6,
			string nombre7, string valor7, string tipo7, int largo7, string inout7,
			string nombre8, string valor8, string tipo8, int largo8, string inout8,
			string nombre9, string valor9, string tipo9, int largo9, string inout9)
		{
			this.nombres = new string[9];
			this.valores = new string[9];
			this.tipos = new string[9];
			this.largos = new int[9];
			this.inout = new string[9];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			this.nombres[5] = nombre6;
			this.nombres[6] = nombre7;
			this.nombres[7] = nombre8;
			this.nombres[8] = nombre9;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			this.valores[5] = valor6;
			this.valores[6] = valor7;
			this.valores[7] = valor8;
			this.valores[8] = valor9;
			
			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			this.tipos[5] = tipo6;
			this.tipos[6] = tipo7;
			this.tipos[7] = tipo8;
			this.tipos[8] = tipo9;
			
			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			this.largos[5] = largo6;
			this.largos[6] = largo7;
			this.largos[7] = largo8;
			this.largos[8] = largo9;
			
			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			this.inout[5] = inout6;
			this.inout[6] = inout7;
			this.inout[7] = inout8;
			this.inout[8] = inout9;
			
			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1,
			string nombre2, string valor2, string tipo2, int largo2, string inout2,
			string nombre3, string valor3, string tipo3, int largo3, string inout3,
			string nombre4, string valor4, string tipo4, int largo4, string inout4,
			string nombre5, string valor5, string tipo5, int largo5, string inout5,
			string nombre6, string valor6, string tipo6, int largo6, string inout6,
			string nombre7, string valor7, string tipo7, int largo7, string inout7,
			string nombre8, string valor8, string tipo8, int largo8, string inout8,
			string nombre9, string valor9, string tipo9, int largo9, string inout9,
			string nombre10, string valor10, string tipo10, int largo10, string inout10)
		{
			this.nombres = new string[10];
			this.valores = new string[10];
			this.tipos = new string[10];
			this.largos = new int[10];
			this.inout = new string[10];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			this.nombres[5] = nombre6;
			this.nombres[6] = nombre7;
			this.nombres[7] = nombre8;
			this.nombres[8] = nombre9;
			this.nombres[9] = nombre10;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			this.valores[5] = valor6;
			this.valores[6] = valor7;
			this.valores[7] = valor8;
			this.valores[8] = valor9;
			this.valores[9] = valor10;

			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			this.tipos[5] = tipo6;
			this.tipos[6] = tipo7;
			this.tipos[7] = tipo8;
			this.tipos[8] = tipo9;
			this.tipos[9] = tipo10;

			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			this.largos[5] = largo6;
			this.largos[6] = largo7;
			this.largos[7] = largo8;
			this.largos[8] = largo9;
			this.largos[9] = largo10;

			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			this.inout[5] = inout6;
			this.inout[6] = inout7;
			this.inout[7] = inout8;
			this.inout[8] = inout9;
			this.inout[9] = inout10;

			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		public DbnetProcedure(OleDbConnection dbConn,string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1,
			string nombre2, string valor2, string tipo2, int largo2, string inout2,
			string nombre3, string valor3, string tipo3, int largo3, string inout3,
			string nombre4, string valor4, string tipo4, int largo4, string inout4,
			string nombre5, string valor5, string tipo5, int largo5, string inout5,
			string nombre6, string valor6, string tipo6, int largo6, string inout6,
			string nombre7, string valor7, string tipo7, int largo7, string inout7,
			string nombre8, string valor8, string tipo8, int largo8, string inout8,
			string nombre9, string valor9, string tipo9, int largo9, string inout9,
			string nombre10, string valor10, string tipo10, int largo10, string inout10,
			string nombre11, string valor11, string tipo11, int largo11, string inout11)
		{
			this.nombres = new string[11];
			this.valores = new string[11];
			this.tipos = new string[11];
			this.largos = new int[11];
			this.inout = new string[11];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			this.nombres[5] = nombre6;
			this.nombres[6] = nombre7;
			this.nombres[7] = nombre8;
			this.nombres[8] = nombre9;
			this.nombres[9] = nombre10;
			this.nombres[10] = nombre11;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			this.valores[5] = valor6;
			this.valores[6] = valor7;
			this.valores[7] = valor8;
			this.valores[8] = valor9;
			this.valores[9] = valor10;
			this.valores[10] = valor11;

			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			this.tipos[5] = tipo6;
			this.tipos[6] = tipo7;
			this.tipos[7] = tipo8;
			this.tipos[8] = tipo9;
			this.tipos[9] = tipo10;
			this.tipos[10] = tipo11;

			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			this.largos[5] = largo6;
			this.largos[6] = largo7;
			this.largos[7] = largo8;
			this.largos[8] = largo9;
			this.largos[9] = largo10;
			this.largos[10] = largo11;

			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			this.inout[5] = inout6;
			this.inout[6] = inout7;
			this.inout[7] = inout8;
			this.inout[8] = inout9;
			this.inout[9] = inout10;
			this.inout[10] = inout11;

			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout =900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

        public DbnetProcedure(OleDbConnection dbConn, string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1,
			string nombre2, string valor2, string tipo2, int largo2, string inout2,
			string nombre3, string valor3, string tipo3, int largo3, string inout3,
			string nombre4, string valor4, string tipo4, int largo4, string inout4,
			string nombre5, string valor5, string tipo5, int largo5, string inout5,
			string nombre6, string valor6, string tipo6, int largo6, string inout6,
			string nombre7, string valor7, string tipo7, int largo7, string inout7,
			string nombre8, string valor8, string tipo8, int largo8, string inout8,
			string nombre9, string valor9, string tipo9, int largo9, string inout9,
			string nombre10, string valor10, string tipo10, int largo10, string inout10,
			string nombre11, string valor11, string tipo11, int largo11, string inout11,
			string nombre12, string valor12, string tipo12, int largo12, string inout12)
		{
			this.nombres = new string[12];
			this.valores = new string[12];
			this.tipos = new string[12];
			this.largos = new int[12];
			this.inout = new string[12];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			this.nombres[5] = nombre6;
			this.nombres[6] = nombre7;
			this.nombres[7] = nombre8;
			this.nombres[8] = nombre9;
			this.nombres[9] = nombre10;
			this.nombres[10] = nombre11;
			this.nombres[11] = nombre12;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			this.valores[5] = valor6;
			this.valores[6] = valor7;
			this.valores[7] = valor8;
			this.valores[8] = valor9;
			this.valores[9] = valor10;
			this.valores[10] = valor11;
			this.valores[11] = valor12;

			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			this.tipos[5] = tipo6;
			this.tipos[6] = tipo7;
			this.tipos[7] = tipo8;
			this.tipos[8] = tipo9;
			this.tipos[9] = tipo10;
			this.tipos[10] = tipo11;
			this.tipos[11] = tipo12;

			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			this.largos[5] = largo6;
			this.largos[6] = largo7;
			this.largos[7] = largo8;
			this.largos[8] = largo9;
			this.largos[9] = largo10;
			this.largos[10] = largo11;
			this.largos[11] = largo12;

			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			this.inout[5] = inout6;
			this.inout[6] = inout7;
			this.inout[7] = inout8;
			this.inout[8] = inout9;
			this.inout[9] = inout10;
			this.inout[10] = inout11;
			this.inout[11] = inout12;

			this.sp = new OleDbCommand(nombre_proc, dbConn);
            sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;            
			param_procedures();
		}

        public DbnetProcedure(OleDbConnection dbConn, string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1,
			string nombre2, string valor2, string tipo2, int largo2, string inout2,
			string nombre3, string valor3, string tipo3, int largo3, string inout3,
			string nombre4, string valor4, string tipo4, int largo4, string inout4,
			string nombre5, string valor5, string tipo5, int largo5, string inout5,
			string nombre6, string valor6, string tipo6, int largo6, string inout6,
			string nombre7, string valor7, string tipo7, int largo7, string inout7,
			string nombre8, string valor8, string tipo8, int largo8, string inout8,
			string nombre9, string valor9, string tipo9, int largo9, string inout9,
			string nombre10, string valor10, string tipo10, int largo10, string inout10,
			string nombre11, string valor11, string tipo11, int largo11, string inout11,
			string nombre12, string valor12, string tipo12, int largo12, string inout12,
			string nombre13, string valor13, string tipo13, int largo13, string inout13)
		{
			this.nombres = new string[13];
			this.valores = new string[13];
			this.tipos = new string[13];
			this.largos = new int[13];
			this.inout = new string[13];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			this.nombres[5] = nombre6;
			this.nombres[6] = nombre7;
			this.nombres[7] = nombre8;
			this.nombres[8] = nombre9;
			this.nombres[9] = nombre10;
			this.nombres[10] = nombre11;
			this.nombres[11] = nombre12;
			this.nombres[12] = nombre13;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			this.valores[5] = valor6;
			this.valores[6] = valor7;
			this.valores[7] = valor8;
			this.valores[8] = valor9;
			this.valores[9] = valor10;
			this.valores[10] = valor11;
			this.valores[11] = valor12;
			this.valores[12] = valor13;

			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			this.tipos[5] = tipo6;
			this.tipos[6] = tipo7;
			this.tipos[7] = tipo8;
			this.tipos[8] = tipo9;
			this.tipos[9] = tipo10;
			this.tipos[10] = tipo11;
			this.tipos[11] = tipo12;
			this.tipos[12] = tipo13;

			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			this.largos[5] = largo6;
			this.largos[6] = largo7;
			this.largos[7] = largo8;
			this.largos[8] = largo9;
			this.largos[9] = largo10;
			this.largos[10] = largo11;
			this.largos[11] = largo12;
			this.largos[12] = largo13;

			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			this.inout[5] = inout6;
			this.inout[6] = inout7;
			this.inout[7] = inout8;
			this.inout[8] = inout9;
			this.inout[9] = inout10;
			this.inout[10] = inout11;
			this.inout[11] = inout12;
			this.inout[12] = inout13;

			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout =900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

        public DbnetProcedure(OleDbConnection dbConn, string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1,
			string nombre2, string valor2, string tipo2, int largo2, string inout2,
			string nombre3, string valor3, string tipo3, int largo3, string inout3,
			string nombre4, string valor4, string tipo4, int largo4, string inout4,
			string nombre5, string valor5, string tipo5, int largo5, string inout5,
			string nombre6, string valor6, string tipo6, int largo6, string inout6,
			string nombre7, string valor7, string tipo7, int largo7, string inout7,
			string nombre8, string valor8, string tipo8, int largo8, string inout8,
			string nombre9, string valor9, string tipo9, int largo9, string inout9,
			string nombre10, string valor10, string tipo10, int largo10, string inout10,
			string nombre11, string valor11, string tipo11, int largo11, string inout11,
			string nombre12, string valor12, string tipo12, int largo12, string inout12,
			string nombre13, string valor13, string tipo13, int largo13, string inout13,
			string nombre14, string valor14, string tipo14, int largo14, string inout14)
		{
			this.nombres = new string[14];
			this.valores = new string[14];
			this.tipos = new string[14];
			this.largos = new int[14];
			this.inout = new string[14];

			this.nombres[0] = nombre1;
			this.nombres[1] = nombre2;
			this.nombres[2] = nombre3;
			this.nombres[3] = nombre4;
			this.nombres[4] = nombre5;
			this.nombres[5] = nombre6;
			this.nombres[6] = nombre7;
			this.nombres[7] = nombre8;
			this.nombres[8] = nombre9;
			this.nombres[9] = nombre10;
			this.nombres[10] = nombre11;
			this.nombres[11] = nombre12;
			this.nombres[12] = nombre13;
			this.nombres[13] = nombre14;
			
			this.valores[0] = valor1;
			this.valores[1] = valor2;
			this.valores[2] = valor3;
			this.valores[3] = valor4;
			this.valores[4] = valor5;
			this.valores[5] = valor6;
			this.valores[6] = valor7;
			this.valores[7] = valor8;
			this.valores[8] = valor9;
			this.valores[9] = valor10;
			this.valores[10] = valor11;
			this.valores[11] = valor12;
			this.valores[12] = valor13;
			this.valores[13] = valor14;

			this.tipos[0] = tipo1;
			this.tipos[1] = tipo2;
			this.tipos[2] = tipo3;
			this.tipos[3] = tipo4;
			this.tipos[4] = tipo5;
			this.tipos[5] = tipo6;
			this.tipos[6] = tipo7;
			this.tipos[7] = tipo8;
			this.tipos[8] = tipo9;
			this.tipos[9] = tipo10;
			this.tipos[10] = tipo11;
			this.tipos[11] = tipo12;
			this.tipos[12] = tipo13;
			this.tipos[13] = tipo14;

			this.largos[0] = largo1;
			this.largos[1] = largo2;
			this.largos[2] = largo3;
			this.largos[3] = largo4;
			this.largos[4] = largo5;
			this.largos[5] = largo6;
			this.largos[6] = largo7;
			this.largos[7] = largo8;
			this.largos[8] = largo9;
			this.largos[9] = largo10;
			this.largos[10] = largo11;
			this.largos[11] = largo12;
			this.largos[12] = largo13;
			this.largos[13] = largo14;

			this.inout[0] = inout1;
			this.inout[1] = inout2;
			this.inout[2] = inout3;
			this.inout[3] = inout4;
			this.inout[4] = inout5;
			this.inout[5] = inout6;
			this.inout[6] = inout7;
			this.inout[7] = inout8;
			this.inout[8] = inout9;
			this.inout[9] = inout10;
			this.inout[10] = inout11;
			this.inout[11] = inout12;
			this.inout[12] = inout13;
			this.inout[13] = inout14;

			this.sp = new OleDbCommand(nombre_proc, dbConn);
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}

		private void param_procedures()
		{
			for(int i=0; i<this.nombres.Length; i++)
			{
				this.sp.Parameters.Add(crea_param(this.nombres[i], this.tipos[i], 
					this.largos[i], this.valores[i], this.inout[i]));
			}	
			this.sp.ExecuteNonQuery();
		}
		private OleDbParameter crea_param(string nombre, string tipo, int largo, string valor, string inout)
		{
			OleDbParameter param = new OleDbParameter();
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				param.ParameterName = "@" + nombre;
			}
			else
			{
				param.ParameterName = nombre;
			}
			
			if (inout.ToUpper() == "OUT")
				param.Direction = ParameterDirection.Output;	
			if (inout.ToUpper() == "INOUT")
				param.Direction = ParameterDirection.InputOutput;			
					
			switch(tipo.ToUpper())
			{
				case "DECIMAL" :
					param.OleDbType = OleDbType.Numeric;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToDecimal(valor);
						}
					}
					break;
				case "SHORT" :
					param.OleDbType = OleDbType.SmallInt;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToInt16(valor);
						}
					}
					break;
				case "INT" :
					param.OleDbType = OleDbType.Integer;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToInt32(valor);
						}
					}
					break;
				case "LONG" :
					param.OleDbType = OleDbType.BigInt;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToInt32(valor);
						}
					}
					break;
				case "BYTE" :
					param.OleDbType = OleDbType.Binary;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToByte(valor);
						}
					}
					break;
				case "SBYTE" :
					param.OleDbType = OleDbType.TinyInt;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToSByte(valor);
						}
					}
					break;
				case "DOUBLE" :
					param.OleDbType = OleDbType.Double;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToDouble(valor);
						}
					}
					break;
				case "USHORT" :
					param.OleDbType = OleDbType.UnsignedSmallInt;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToUInt16(valor);
						}
					}
					break;
				case "UINT" :
					param.OleDbType = OleDbType.UnsignedInt;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToUInt32(valor);
						}
					}
					break;
				case "ULONG" :
					param.OleDbType = OleDbType.UnsignedBigInt;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToUInt64(valor);
						}
					}
					break;
				case "CHAR" :
					param.OleDbType = OleDbType.Char;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToChar(valor);
						}
					}
					break;
				case "STRING" :
					param.OleDbType = OleDbType.VarChar;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = valor;
						}
					}
					break;
				case "VARCHAR" :
					param.OleDbType = OleDbType.VarChar;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = valor;
						}
					}
					break;
				case "BOOL" : 
					param.OleDbType = OleDbType.Boolean;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToBoolean(valor);
						}
					}
					break;
				case "DATE" :
					param.OleDbType = OleDbType.DBDate;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToDateTime(valor);
						}
					}
					break;
				case "TIMESPAN" :
					param.OleDbType = OleDbType.DBTime;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = valor;
						}
					}
					break;
				case "DATETIME" :
					param.OleDbType = OleDbType.DBTimeStamp;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToDateTime(valor);
						}
					}
					break;
				case "OBJECT" :
					param.OleDbType = OleDbType.Variant;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = valor;
						}
					}
					break;
				case "FLOAT" :
					param.OleDbType = OleDbType.Single;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = Convert.ToSingle(valor);
						}
					}
					break;
				default :
					param.OleDbType = OleDbType.Variant;
					if (inout.ToUpper() == "IN" || inout.ToUpper() == "INOUT")
					{
						if (valor == "" || valor == null)
						{
							param.Value = DBNull.Value;
						}
						else
						{
							param.Value = valor;
						}
					}
					break;
			}
			param.Size = largo;
			return param;
		}

		public string return_String(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return this.sp.Parameters[nombre_parametro].Value.ToString();
		}

		public char return_Char(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToChar(this.sp.Parameters[nombre_parametro].Value);
		}

		public short return_Short(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToInt16(this.sp.Parameters[nombre_parametro].Value);
		}

		public int return_Int(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToInt32(this.sp.Parameters[nombre_parametro].Value);
		}

		public long return_Long(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToInt64(this.sp.Parameters[nombre_parametro].Value);
		}

		public double return_Double(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToDouble(this.sp.Parameters[nombre_parametro].Value);
		}

		public decimal return_Decimal(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToDecimal(this.sp.Parameters[nombre_parametro].Value);
		}

		public ushort return_UShort(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToUInt16(this.sp.Parameters[nombre_parametro].Value);
		}

		public uint return_UInt(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToUInt32(this.sp.Parameters[nombre_parametro].Value);
		}

		public ulong return_ULong(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToUInt64(this.sp.Parameters[nombre_parametro].Value);
		}

		public bool return_Bool(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToBoolean(this.sp.Parameters[nombre_parametro].Value);
		}

		public System.DateTime return_Date(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToDateTime(this.sp.Parameters[nombre_parametro].Value);
		}

		public byte return_Byte(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToByte(this.sp.Parameters[nombre_parametro].Value);
		}

		public sbyte return_SByte(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToSByte(this.sp.Parameters[nombre_parametro].Value);
		}

		public float return_Float(string nombre_parametro)
		{
			if (DbnetGlobal.Base_dato == "SQLSERVER")
			{
				nombre_parametro = "@" + nombre_parametro; 
			}
			return Convert.ToSingle(this.sp.Parameters[nombre_parametro].Value);
		}     

	}

    public class DbnetMail
    {
        private MailMessage mensaje;
        public string ipServidor;
        public string from;
        public string to;
        public string cc;
        public string body;
        public string subject;
        public string file1;

        public DbnetMail()
        {
            this.ipServidor = "";
            this.from = "";
            this.to = "";
            this.cc = null;
            this.body = "";
            this.subject = "";
            this.file1 = null;
        }

        public void envioSmtp(out bool status, out string msg)
        {
            try
            {
                SmtpClient ipEmail = new SmtpClient(this.ipServidor);
                mensaje = new MailMessage(from, to, subject, body);
                if (cc != null)
                {
                    MailAddress copy = new MailAddress(cc);
                    mensaje.CC.Add(copy);
                }                
                if (file1 != null)
                {
                    Attachment data = new Attachment(file1, MediaTypeNames.Application.Octet);
                    // Add time stamp information for the file.
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(file1);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(file1);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(file1);
                    // Add the file attachment to this e-mail message.
                    mensaje.Attachments.Add(data);
                }
                //ipEmail.DeliveryMethod = SmtpDeliveryMethod.Network;
                ipEmail.Send(mensaje);
                status = true;
                msg = "OK";
            }
            catch (Exception ex)
            {
                status = false;
                msg = ex.Message;
            }
        }
    }

    public class CompareFileTime : Object, IComparer<FileInfo>
    {
        #region IComparer<FileInfo> Members

        public int Compare(FileInfo x, FileInfo y)
        {
            return x.CreationTime.CompareTo(y.CreationTime);
        }
        #endregion
    } 
}