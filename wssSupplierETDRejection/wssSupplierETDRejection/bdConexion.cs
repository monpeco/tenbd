using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Data.OleDb;
using System.Data;
using WssSupplierETDRejection;
using Tool;
using System.Configuration;
using System.Linq;

namespace conexionBaseDatos
{
    class bdConexion
    {
        protected OleDbConnection conn;
        protected OleDbCommand comandos;
        protected OleDbTransaction trans;
        protected OleDbCommand sp;
        protected string[] nombres, valores, tipos, inout;
        protected int[] largos;
        protected String home = "", sLine = "", rutaConn = "", userConn = "", conexion = "", emex = "";
        protected String passConn = "", servConn = "", baseconn = "", svrtemp = "", bd = "", _egateHome = "";
        protected String mensajeError = "";
        String PI_CODI_APPL = "SUITE5";
        #region base
        public bdConexion()
        {}

        public void conexionOpen()
        {
            try
            {
                mensajeError = "";
                home = Environment.GetEnvironmentVariable(_egateHome).ToString();
                rutaConn = home + @"\config\par\egate_config.cfg";                          /* ruta archivo conexion */

                StreamReader leerConn = new StreamReader(rutaConn);                         /* abro el archivo de conexion */
                ArrayList arrText = new ArrayList();
                while (sLine != null)                                                      /* leo el contenido del archivo */
                {
                    sLine = leerConn.ReadLine();
                    if (sLine != null)
                        arrText.Add(sLine);
                }
                leerConn.Close();
                // recorro el array buscando los datos
                char[] x = { ' ' };
                foreach (string sOutput in arrText)
                {
                    string[] arr = sOutput.Split(x);
                    // leo cada linea buscando datos

                    for (int k = 0; k < arr.Length; k++)
                    {
                        if (arr[k].ToLower() == "-usr")
                        {
                            userConn = arr[k + 1];
                        }
                        else if (arr[k].ToLower() == "-psw")
                        {
                            passConn = arr[k + 1];
                        }
                        else if (arr[k].ToLower() == "-srv")
                        {
                            svrtemp = arr[k + 1];
                        }
                        else if ((arr[k].ToLower() == "-emex"))
                        {
                            emex = arr[k + 1];
                        }
                        else if ((arr[k].ToLower() == "-basedatos"))
                        {
                            bd = arr[k + 1].ToLower();
                        }
                    }
                }

                if (bd.ToLower().Equals("oracle"))
                {
                    conexion = @"Provider=MSDAORA.1;Password=" + passConn + ";User ID=" + userConn + ";Data Source="
                                        + svrtemp + ";Persist Security Info=false";
                }
                else
                {
                    string[] svr = svrtemp.Split('.');
                    servConn = svr[0]; baseconn = svr[1];

                    conexion = @"Provider=SQLOLEDB.1;Persist Security Info=False;User ID="
                                        + userConn + ";Password=" + passConn + ";Initial Catalog= "
                                        + baseconn + ";Data Source= " + servConn
                                        + ";Use Procedure for Prepare=1;Auto Translate=True;Packet Size=4096;Use "
                                        + " Encryption for Data=False;Tag with column collation when possible=False";
                }
                conn = new OleDbConnection(conexion);
                conn.Open();
                trans = conn.BeginTransaction();
                comandos = conn.CreateCommand();
                comandos.Connection = conn;
                comandos.Transaction = trans;

                if (!string.IsNullOrEmpty(emex) && bd.ToLower().Equals("sqlserver"))
                {
                    EjectProcedure("dbnet_set_emex", "p_codi_emex", emex, "varchar", 30, "in");
                }
            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                System.Environment.Exit(1);
            }
        }

        public String egateHome
        {
            set { _egateHome = Convert.ToString(value); }
            get
            {
                if (_egateHome == "") { _egateHome = "EGATE_HOME"; }
                return _egateHome;
            }
        }

        public String MensajeError
        {
            get
            {
                return mensajeError;
            }
        }

        public string SelectInto(String query)
        {
            try
            {
                string valor;
                OleDbDataAdapter da = new OleDbDataAdapter();
                DataTable dt = new DataTable();

                mensajeError = "";
                comandos.Connection = conn;
                comandos.Transaction = trans;
                comandos.CommandText = query;
                da.SelectCommand = comandos;
                comandos.CommandTimeout = 0;

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
                            valor = Convert.ToString(myRow[myCol]);
                            return valor;
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                return Convert.ToString(ex).Substring(Convert.ToString(ex).IndexOf("Se terminó la instrucción") + 28, 185);
            }
        }

        public string Get_Param(string ParametroBD, string ParametroEM, int ruttEmis)
        {
            string Parametro = "";
            string query = "select pa.valo_paem " + " " +
              "from para_empr pa, empr em " +
              "where pa.codi_empr = em.codi_empr" +
              " and em.rutt_emis = " + Convert.ToString(ruttEmis) + " " +
              " and pa.codi_paem = '" + ParametroEM + "' ";

            DataTable dt = EjecutaSelect(query);
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

                dt = EjecutaSelect(query);
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

        public DataTable EjecutaSelect(string query)
        {
            OleDbDataAdapter da = new OleDbDataAdapter();
            DataTable dt = new DataTable();

            comandos.Connection = conn;
            comandos.Transaction = trans;
            comandos.CommandText = query;
            da.SelectCommand = comandos;
            comandos.CommandTimeout = 0;

            dt.Clear();
            da.Fill(dt);

            return dt;
        }

        public void confirma()
        {
            trans.Commit();
        }

        public void rechaza()
        {
            trans.Rollback();
        }

        public void closeConexion()
        {
            if (Convert.ToString(conn.State) == "Open")
                conn.Close();
        }
        #endregion

        #region Consultas
        public Boolean validaExistencia(int companyCodeSii, int documentType, int documentNumber)
        {
            String sFolio = documentNumber.ToString();
            try
            {
                mensajeError = "";
                int esta = 0;
                string sql = "";
                switch (bd.ToLower())
                {
                    case "oracle":
                        sql = "select 1 " +
                              " from dto_enca_docu_p" +
                              " WHERE   esta_docu in ('INI','ERA') " +
                              " AND   rutt_emis = {0}" +
                              " AND   tipo_docu = {1}" +
                              " AND   foli_docu = {2}";
                        break;
                    case "sqlserver":
                        sql = "select 1 " +
                              " from dto_enca_docu_p" +
                              " WHERE   esta_docu in ('INI','ERA') " +
                              " AND   rutt_emis = {0}" +
                              " AND   tipo_docu = {1}" +
                              " AND   Convert(numeric,foli_docu) = {2}";
                        break;
                }

                comandos = conn.CreateCommand();
                comandos.Connection = conn;
                comandos.Transaction = trans;

                comandos.CommandText = String.Format(sql, companyCodeSii, documentType, documentNumber);

                OleDbDataReader reader = comandos.ExecuteReader();

                while (reader.Read())
                {esta = 1;}
                reader.Close();
                reader.Dispose();

                if (esta == 0)
                { return false; }
                else
                { return true; }
            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                return false;
            }
        }

        public Boolean validaExistenciaReclamo(string company, string corr_qmsg, out string rutt_empr, out string digi_empr )
        {
            try
            {
                mensajeError = "";
                int esta = 0;
                string sql = "";
                switch (bd.ToLower())
                {
                    case "oracle":
                        sql = "select rutt_empr,digi_empr " +
                              " from dtec_queue_msge " +
                              " where rutt_empr = {0} " +
                              " and corr_qmsg = {1} ";
                        break;
                    case "sqlserver":
                        sql = "select rutt_empr,digi_empr " +
                              " from dtec_queue_msge " +
                              " where rutt_empr = {0} " +
                              " and corr_qmsg = {1} ";
                        break;
                }

                comandos = conn.CreateCommand();
                comandos.Connection = conn;
                comandos.Transaction = trans;

                comandos.CommandText = String.Format(sql, company, corr_qmsg);

                OleDbDataReader reader = comandos.ExecuteReader();

                rutt_empr = "-1";
                digi_empr = "-1";
                while (reader.Read())
                { 
                    esta = 1;
                    rutt_empr = Convert.ToString(reader.GetValue(0));
                    digi_empr = Convert.ToString(reader.GetValue(1));

                }
                reader.Close();
                reader.Dispose();

                if (esta == 0)
                { return false; }
                else
                { return true; }
            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                rutt_empr = "err";
                digi_empr = "e";
                return false;
            }
        }

        public Boolean estadoAprov(string v_codi_esap)
        {
            try
            {
                if (new string[] { "ACD", "ERM", "RCD", "RFP", "RFT", "FRS", "CED", "CCD" }.Contains(v_codi_esap))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                return false;
            }
        }

        public string verificaDocumento(int companyCodeSii, int documentType, int documentNumber)
        {
            string codigo = null;
            try
            {
                mensajeError = "";
                string sql = "";
                switch (bd.ToLower())
                {
                    case "oracle":
                        sql = "select codi_esap from" +
                                        " dto_enca_docu_p" +
                                        " WHERE esta_docu in ('INI','ERA')" +
                                        " AND   rutt_emis = {0}" +
                                        " AND   tipo_docu = {1}" +
                                        " AND   foli_docu = {2}";
                        break;
                    case "sqlserver":
                        sql = "select codi_esap from dto_enca_docu_p" +
                                        " WHERE esta_docu in ('INI','ERA')" +
                                        " AND   rutt_emis = {0}" +
                                        " AND   tipo_docu = {1}" +
                                        " AND   Convert(numeric,foli_docu) = {2}";
                        break;
                }
                comandos = conn.CreateCommand();
                comandos.Connection = conn;
                comandos.Transaction = trans;

                comandos.CommandText = String.Format(sql, companyCodeSii, documentType, documentNumber);
                OleDbDataReader reader = comandos.ExecuteReader();
                while (reader.Read())
                {
                    codigo = Convert.ToString(reader.GetValue(0));
                    if (codigo == "")
                        codigo = "-.-";
                }
                reader.Close();
                reader.Dispose();

            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
            }

            return codigo;
        }

        //public Boolean applyRejection(string company, string digitoCompany, string companyCodeSii, string digitoCompanyCodeSii, int documentType, int documentNumber, string statusCode, string reasonDesc, string pi_codi_erro, string pi_mens_erro, string pi_corr_qmsg)
        public Boolean applyRejection(string company, string digitoCompany, string companyCodeSii, string digitoCompanyCodeSii, int documentType, int documentNumber, string statusCode, string reasonDesc, out string  pi_codi_erro, out string  pi_mens_erro, out string  pi_corr_qmsg)
        {
            bool oData = false;
            //String emex = "PROD_0121";
            try
            {
                /*String pi_codi_appl = "suite no 5.0";
                int pi_rutt_empr = 121;
                String pi_digi_empr = "1";
                String pi_name_msge = "kaka";
                String pi_name_addr = "gfgfgf";
                String pi_curl_para = "gfgfgf";
                int pi_codi_erro = 12;
                String pi_mens_erro = "sksj";
                int pi_corr_qmsg = 12;*/

                pi_codi_erro = "-10";
                pi_mens_erro = "-20";
                pi_corr_qmsg = "-30";

                
                String pi_rutt_empr = String.Empty;
                String pi_digi_empr = String.Empty;
                String pi_name_msge = String.Empty;
                String pi_name_addr = String.Empty;
                String pi_curl_para = String.Empty;

                if (new string[] { "ACD", "ERM", "RCD", "RFP", "RFT",   "FRS" }.Contains(statusCode))    //Si es caso de Recepcion 
                {
                    pi_rutt_empr = company;
                    pi_digi_empr = digitoCompany;

                    pi_name_msge = "MSG_INGREC";
                    pi_name_addr = "AD_INGREC";

                    if (statusCode == "FRS"){
                        pi_name_msge = "MSG_FCHRCP";
                        pi_name_addr = "AD_FCHRCP";
                    }

                }
                else if (new string[] { "CED", "CCD" }.Contains(statusCode))   //Si es caso de Emision 
                {
                    pi_rutt_empr = companyCodeSii;
                    pi_digi_empr = digitoCompanyCodeSii;

                    if (statusCode == "CCD")
                    {
                        pi_name_msge = "MSG_QRYCED";
                        pi_name_addr = "AD_QRYCED";
                    }
                    else    
                    {
                        pi_name_msge = "MSG_LISEVE";
                        pi_name_addr = "AD_LISEVE";
                    }
                }

                /*	SET @curl_para	= '{ "RUT_EMIS" : "' + (select convert(varchar(10),@rutt_emis)) 
					+ '", "DV_EMIS" : "' + @digi_emis 
					+ '", "TIPO_DOC" : "' + (select convert(varchar(4),@tipo_docu))
					+ '", "FOLIO" : "' + (select convert(varchar(10),@foli_docu))
					+ '", "ACCION" : "' + @even_sii
					+ '" }';*/
                pi_curl_para = "{ \"RUT_EMIS\" : \"" + companyCodeSii
                             + "\", \"DV_EMIS\" : \"" + digitoCompanyCodeSii
                             + "\", \"TIPO_DOC\" : \"" + documentType.ToString()
                             + "\", \"FOLIO\" : \"" + documentNumber.ToString()
                             + "\", \"ACCION\" : \"" + statusCode
                             + "\" }";

                EjectProcedure9("PRC_PUT_MESSAGE"
                , "pi_codi_appl", PI_CODI_APPL, "varchar", 40, "in"
                , "pi_rutt_empr", pi_rutt_empr, "DECIMAL", 8, "in"
                , "pi_digi_empr", pi_digi_empr, "varchar", 1, "in"
                , "pi_name_msge", pi_name_msge, "varchar", 20, "in"
                , "pi_name_addr", pi_name_addr, "varchar", 20, "in"
                , "pi_curl_para", pi_curl_para, "varchar", 200, "in"
                , "pi_codi_erro", pi_codi_erro, "DECIMAL", 5, "out"
                , "pi_mens_erro", pi_mens_erro, "varchar", 80, "out"
                , "pi_corr_qmsg", pi_corr_qmsg, "DECIMAL", 22, "out");

                pi_codi_erro = return_String("pi_codi_erro");
                pi_mens_erro = return_String("pi_mens_erro");
                pi_corr_qmsg = return_String("pi_corr_qmsg");

                oData = true;
                
            } 
            catch (Exception ex)
            {
                mensajeError = ex.Message;

                pi_codi_erro = ex.Message;
                pi_mens_erro = ex.Message;
                pi_corr_qmsg = ex.Message;

                oData = false;
            }

            return oData;
        }


        public Boolean recoverRejection(string company, string CodigoSolicitud, string rutt_empr, string digi_empr, out string pi_codi_erro, out string pi_mens_erro, out string pi_resp_msge)
        {
            bool oData = false;
            //String emex = "PROD_0121";
            try
            {
                /*String pi_codi_appl = "suite no 5.0";
                int pi_rutt_empr = 121;
                String pi_digi_empr = "1";
                String pi_name_msge = "kaka";
                String pi_name_addr = "gfgfgf";
                String pi_curl_para = "gfgfgf";
                int pi_codi_erro = 12;
                String pi_mens_erro = "sksj";
                int pi_corr_qmsg = 12;*/

                pi_codi_erro = "-10";
                pi_mens_erro = "-20";
                pi_resp_msge = "-30";




                EjectProcedure7("PRC_GET_MESSAGE"
                , "pi_codi_appl", PI_CODI_APPL, "varchar", 40, "in"
                , "pi_rutt_empr", rutt_empr, "DECIMAL", 8, "in"
                , "pi_digi_empr", digi_empr, "varchar", 1, "in"
                , "pi_name_msge", CodigoSolicitud, "DECIMAL", 22, "in"
                , "pi_codi_erro", pi_codi_erro, "DECIMAL", 5, "out"
                , "pi_mens_erro", pi_mens_erro, "varchar", 80, "out"
                , "pi_corr_qmsg", pi_resp_msge, "varchar", 80000, "out");

                pi_codi_erro = return_String("pi_codi_erro");
                pi_mens_erro = return_String("pi_mens_erro");
                pi_resp_msge = return_String("pi_corr_qmsg");

                oData = true;

            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;

                pi_codi_erro = ex.Message;
                pi_mens_erro = ex.Message;
                pi_resp_msge = ex.Message;

                oData = false;
            }

            return oData;
        }


        public Boolean RegistraTraza(string Servicio, string Tipo, string Estado, int Codi_Empr,
            int Tipo_Docu, int Folio, int Corr_Docu, string Desc)
        {
            try
            {
                mensajeError = "";
                string sql = "insert into dbn_traza_docu (" +
                             "codi_emex,codi_serv, docu_tipo, fech_esta, esta_docu, codi_empr, tipo_docu, foli_docu, corr_refe, obse_esta) " +
                             "values (" +
                             "'{0}','{1}', '{2}', {3}, '{4}', {5}, {6}, '{7}', {8}, '{9}')";

                comandos = conn.CreateCommand();
                comandos.Connection = conn;
                comandos.Transaction = trans;

                comandos.CommandText = String.Format(sql, emex, Servicio, Tipo, (bd.ToLower().Equals("oracle")) ? "sysdate" : "getdate()", Estado, Codi_Empr, Tipo_Docu, Folio, Corr_Docu, Desc);

                if (comandos.ExecuteNonQuery() != 0)
                { return true; }
                else
                { return false; }

            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                return false; 
            }
        }

        public Boolean validaExistenciaRecibo(int companyCodeSii, int documentType, int documentNumber)
        {
            String sFolio = documentNumber.ToString();
            try
            {
                int esta = 0;
                mensajeError = "";
                string sql = "";
                switch (bd.ToLower())
                {
                    case "oracle":
                        sql = "select 1 " +
                              " from dto_enca_docu_p" +
                              " WHERE   esta_docu in ('INI','ERA') " +
                              " AND   rutt_emis = {0}" +
                              " AND   tipo_docu = {1}" +
                              " AND   codi_reme is null " +
                              " AND   foli_docu = {2}";
                        break;
                    case "sqlserver":
                        sql = "select 1 " +
                              " from dto_enca_docu_p" +
                              " WHERE   esta_docu in ('INI','ERA') " +
                              " AND   rutt_emis = {0}" +
                              " AND   tipo_docu = {1}" +
                              " AND   codi_reme is null " +
                              " AND   Convert(numeric,foli_docu) = {2}";
                        break;
                }
                comandos = conn.CreateCommand();
                comandos.Connection = conn;
                comandos.Transaction = trans;

                comandos.CommandText = String.Format(sql, companyCodeSii, documentType, documentNumber);

                OleDbDataReader reader = comandos.ExecuteReader();

                while (reader.Read())
                {
                    esta = 1;
                }
                reader.Close();
                reader.Dispose();

                if (esta == 0)
                { return false; }
                else
                { return true; }
            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                return false;
            }
        }

        public Boolean ActualizaEstadoRecibo(int companyCodeSii, int documentType, int documentNumber, string statusCode, string place)
        {
            bool oData = false;
            try
            {
                mensajeError = "";
                string sql = "";
                switch (bd.ToLower())
                {
                    case "oracle":
                        sql = "UPDATE dto_enca_docu_p" +
                              " SET   codi_reme = '{0}'," +
                              "       fech_reme = sysdate," +
                              "       usua_reme = 'WSS'," +
                              "       reci_rece = '{4}'" +
                              " WHERE rutt_emis = {1}" +
                              " AND   tipo_docu = {2}" +
                              " AND   foli_docu = {3}" +
                              " AND   esta_docu in ('INI', 'ERA')";
                        break;
                    case "sqlserver":
                        sql = "UPDATE dto_enca_docu_p" +
                              " SET   codi_reme = '{0}'," +
                              "       fech_reme = getdate()," +
                              "       usua_reme = 'WSS'," +
                              "       reci_rece = '{4}'" +
                              " WHERE rutt_emis = {1}" +
                              " AND   tipo_docu = {2}" +
                              " AND   Convert(numeric,foli_docu) = {3}" +
                              " AND   esta_docu in ('INI', 'ERA')";
                        break;
                }
                comandos = conn.CreateCommand();
                comandos.Connection = conn;
                comandos.Transaction = trans;
                comandos.CommandText = String.Format(sql, statusCode, companyCodeSii, documentType, documentNumber, place);
                oData = comandos.ExecuteNonQuery() != 0 ? true : false;

                string sqlTraza = "insert into dbn_traza_docu (codi_serv, docu_tipo, esta_docu, fech_esta, codi_empr, tipo_docu, foli_docu, corr_refe, obse_esta) " +
                                  "select 'SupplierETDRejection', 'DTO', codi_reme, getdate(), codi_empr, tipo_Docu, convert(numeric,foli_docu), corr_docu, reci_rece " +
                                  "from dto_enca_docu_p where rutt_emis = {0} and tipo_docu = {1} and convert(numeric,foli_docu) = {2} and esta_docu in ('INI','ERA')";
                comandos = conn.CreateCommand();
                comandos.Connection = conn;
                comandos.Transaction = trans;
                comandos.CommandText = String.Format(sqlTraza, companyCodeSii, documentType, documentNumber);
                oData = comandos.ExecuteNonQuery() != 0 ? true : false;

            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                oData = false;
            }
            return oData;
        }
        #endregion


        #region procedure
        public void EjectProcedure(string nombre_proc, string nombre1, string valor1, string tipo1, int largo1, string inout1)
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

            this.sp = new OleDbCommand(nombre_proc, conn);
            this.sp.Transaction = trans;
            this.sp.CommandTimeout = 0;
            this.sp.CommandType = CommandType.StoredProcedure;

            param_procedures();
        }

        //REjecuta un procedimiento almacenado de 9 parametros. 07-09-2017|AM
		public void EjectProcedure9(string nombre_proc
		, string nombre1, string valor1, string tipo1, int largo1, string inout1,
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

            this.sp = new OleDbCommand(nombre_proc, conn);
            this.sp.Transaction = trans;
            this.sp.CommandTimeout = 900;
			this.sp.CommandType = CommandType.StoredProcedure;
			param_procedures();
		}


        //REjecuta un procedimiento almacenado de 9 parametros. 07-09-2017|AM
        public void EjectProcedure7(string nombre_proc
        , string nombre1, string valor1, string tipo1, int largo1, string inout1,
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

            this.sp = new OleDbCommand(nombre_proc, conn);
            this.sp.Transaction = trans;
            this.sp.CommandTimeout = 900;
            this.sp.CommandType = CommandType.StoredProcedure;
            param_procedures();
        }

        //Recupera parametro de salida en la ejecución de un procedimiento almacenado. 07-09-2017|AM
        public string return_String(string nombre_parametro)
        {
            if (bd.ToLower() == "sqlserver")
            {
                nombre_parametro = "@" + nombre_parametro;
            }
            return this.sp.Parameters[nombre_parametro].Value.ToString();
        }

        private void param_procedures()
        {
            for (int i = 0; i < this.nombres.Length; i++)
            {
                this.sp.Parameters.Add(crea_param(this.nombres[i], this.tipos[i],
                    this.largos[i], this.valores[i], this.inout[i]));
            }

            this.sp.ExecuteNonQuery();
        }

        private OleDbParameter crea_param(string nombre, string tipo, int largo, string valor, string inout)
        {
            OleDbParameter param = new OleDbParameter();
            if (bd.ToLower().CompareTo("sqlserver".ToLower()) == 0)
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


            switch (tipo.ToUpper())
            {
                case "DECIMAL":
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
                case "SHORT":
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
                case "INT":
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
                case "LONG":
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
                case "BYTE":
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
                case "SBYTE":
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
                case "DOUBLE":
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
                case "USHORT":
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
                case "UINT":
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
                case "ULONG":
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
                case "CHAR":
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
                case "STRING":
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
                case "VARCHAR":
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
                case "BOOL":
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
                case "DATE":
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
                case "TIMESPAN":
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
                case "DATETIME":
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
                case "OBJECT":
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
                case "FLOAT":
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
                default:
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

        #endregion
    }
}
