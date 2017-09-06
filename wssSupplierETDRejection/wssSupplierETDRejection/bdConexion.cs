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

        public Boolean estadoAprov(string v_codi_esap)
        {
            try
            {
                if (v_codi_esap != "APR")
                    if (v_codi_esap != "ARE")
                        if (v_codi_esap != "REC")
                            return false;
                        else
                            return true;
                    else
                        return true;
                else
                    return true;

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

        public Boolean ActualizaEstado(int companyCodeSii, int documentType, int documentNumber, string statusCode, string reasonDesc)
        {
            bool oData = false;
            try
            {
                string sql = "";
                mensajeError = "";
                switch (bd.ToLower())
                {
                    case "oracle":
                        sql = "UPDATE dto_enca_docu_p" +
                              " SET 	codi_esap = '{0}'," +
                              " razo_esap = '{1}'," +
                              " fech_esap = sysdate " +
                              " WHERE rutt_emis = {2}" +
                              " AND   tipo_docu = {3}" +
                              " AND   esta_docu in ('INI','ERA')" +
                              " AND   foli_docu = {4}";
                        break;
                    case "sqlserver":
                        sql = "UPDATE dto_enca_docu_p" +
                              " SET 	codi_esap = '{0}'," +
                              " razo_esap = '{1}'," +
                              " fech_esap = getdate()" +
                              " WHERE rutt_emis = {2}" +
                              " AND   tipo_docu = {3}" +
                              " AND   esta_docu in ('INI','ERA')" +
                              " AND   Convert(numeric,foli_docu) = {4}";
                        break;
                }

                comandos = conn.CreateCommand();
                comandos.Connection = conn;
                comandos.Transaction = trans;
                comandos.CommandText = String.Format(sql, statusCode, reasonDesc, companyCodeSii, documentType, documentNumber);
                oData = comandos.ExecuteNonQuery() != 0 ? true : false;

                string sqlTraza = "insert into dbn_traza_docu (codi_serv, docu_tipo, esta_docu, fech_esta, codi_empr, tipo_docu, foli_docu, obse_esta) " +
                                  "select 'SupplierETDRejection', 'DTO', codi_esap, fech_esap, codi_empr, tipo_Docu, convert(numeric,foli_docu), razo_esap " +
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
