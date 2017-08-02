using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using System.Data.OracleClient;

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
        protected String passConn = "", servConn = "", baseconn = "", svrtemp = "", bd = "", _egateHome = "", cpu = "";

        #region base
        public bdConexion()
        { }

        public void conexionOpen()
        {
            try
            {
                home = Environment.GetEnvironmentVariable(_egateHome).ToString();
                rutaConn = home + @"\config\par\egate_config.cfg";                          /* ruta archivo conexion */

                StreamReader leerConn = new StreamReader(rutaConn);                         /* abro el archivo de conexion */
                ArrayList arrText = new ArrayList();

                while (sLine != null)                                                       /* leo el contenido del archivo */
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
                        if (arr[k] == "-usr")
                        {
                            userConn = arr[k + 1];
                        }
                        else if (arr[k] == "-psw")
                        {
                            passConn = arr[k + 1];
                        }
                        else if (arr[k] == "-srv")
                        {
                            svrtemp = arr[k + 1];
                        }
                        else if ((arr[k] == "-emex"))
                        {
                            emex = arr[k + 1].ToLower();
                        }
                        else if ((arr[k] == "-baseDatos"))
                        {
                            bd = arr[k + 1].ToLower();
                        }
                        else if ((arr[k] == "-cpu"))
                        {
                            cpu = arr[k + 1].ToLower();
                        }
                    }
                }

                if (bd.CompareTo("oracle".ToLower()) == 0)
                {
                    if (cpu.CompareTo("x64") == 0)
                    {
                        conexion = @"Provider=OraOLEDB.Oracle;Password=" + passConn + ";User ID=" + userConn + ";Data Source=" +
                            svrtemp + ";OLEDB.NET=true";
                    }
                    else
                    {
                        conexion = @"Provider=MSDAORA.1;Password=" + passConn + ";User ID=" + userConn + ";Data Source="
                                            + svrtemp + ";Persist Security Info=false";
                    }
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
            }
            catch (FileNotFoundException ex)
            {
                string error = Convert.ToString(ex);
                throw new bdConexionException("Imposible accesar: Archivo Configuracion");
            }
            catch (Exception ex)
            {
                string error = Convert.ToString(ex);
                throw new bdConexionException("EB0: Imposible Acceder a BD");
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
        public String Emex
        {
            set { emex = Convert.ToString(value); }
            get { return emex; }
        }
        public string SelectInto(String query)
        {
            try
            {
                string valor;
                OleDbDataAdapter da = new OleDbDataAdapter();
                DataTable dt = new DataTable();

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
                return Convert.ToString(ex).Substring(Convert.ToString(ex).IndexOf("Se terminó la instrucción") + 28, 185);
            }
        }
        public string SelectText(String query)
        {
            string valor = "";
            try
            {
                if (bd.CompareTo("oracle".ToLower()) == 0)
                {
                    OracleDataAdapter da = new OracleDataAdapter();
                    OracleCommand cmdOra = new OracleCommand();
                    OracleConnection connOra = new OracleConnection();

                    connOra.ConnectionString = "Data Source=" + svrtemp + ";User Id=" + userConn + ";Password=" + passConn + ";";
                    connOra.Open();

                    cmdOra.Connection = connOra;
                    cmdOra.CommandText = query;
                    da.SelectCommand = cmdOra;
                    cmdOra.CommandTimeout = 0;

                    OracleDataReader dr = cmdOra.ExecuteReader();
                    if (dr.Read())
                        valor = dr.GetString(0);
                    dr.Close();
                    connOra.Close();
                }
                else
                {
                    OleDbDataAdapter da = new OleDbDataAdapter();
                    DataTable dt = new DataTable();

                    comandos.Connection = conn;
                    comandos.Transaction = trans;
                    comandos.CommandText = query;
                    da.SelectCommand = comandos;
                    comandos.CommandTimeout = 0;

                    OleDbDataReader dr = comandos.ExecuteReader();
                    if (dr.Read())
                        valor = dr.GetString(0);
                    dr.Close();
                }
                return valor;
            }
            catch (Exception ex)
            {
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
        public int EjecutaNonQuery(string query)
        {
            int filas = 0;
            comandos.Connection = conn;
            comandos.Transaction = trans;
            comandos.CommandText = query;
            comandos.CommandTimeout = 0;
            filas = comandos.ExecuteNonQuery();
            return filas;
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
            if (conn.State == ConnectionState.Open)
                conn.Close();
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
            if (bd.CompareTo("sqlserver".ToLower()) == 0)
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

        public string baseDatos
        {
            get { return bd; }
            set { bd = value; }
        }
    }
}