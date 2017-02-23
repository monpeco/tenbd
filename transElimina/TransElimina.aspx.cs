using System;
using System.Web.UI.WebControls;
using System.Data.SQLite;
using System.Data;
using System.Configuration;

namespace webTransferenciaSucursal
{
    public partial class TransElimina : System.Web.UI.Page
    {
        static string vPathBD;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                vPathBD = System.Configuration.ConfigurationManager.AppSettings.Get("transBD");
                txtEmpresa.Text = System.Configuration.ConfigurationManager.AppSettings.Get("empresa");
            }
        }
 
        protected void btProcesar_Click(object sender, EventArgs e)
        {
            if (txtEmpresa.Text == "0" || txtEmpresa.Text == "")
                txtEmpresa.Text = "%";
            if (txtTipo.Text == "0" || txtTipo.Text == "")
                txtTipo.Text = "%";

            SQLiteCommand cmd = null;
            SQLiteConnection Conexion = null;
            try
            {
                if (vPathBD == null || vPathBD == "")
                    vPathBD = System.Configuration.ConfigurationManager.AppSettings.Get("transBD");

                Conexion = new SQLiteConnection("Data Source=" + vPathBD + ";Version=3;Pooling=true;ReadOnly=false");
                Conexion.Open();
                string qry =  " delete from dbq_arch " +
                              " where  pk01_tabl = '" + txtEmpresa.Text + "' " +
                              " and    pk02_tabl = '" + txtTipo.Text + "' " +
                              " and    pk03_tabl = '" + txtFolio.Text + "' " +
                              " and    esta_arch not in ('CER');";

                if (chkForzar.Checked)
                {
                    qry = " delete from dbq_arch " +
                          " where  pk01_tabl = '" + txtEmpresa.Text + "' " +
                          " and    pk02_tabl = '" + txtTipo.Text + "' " +
                          " and    pk03_tabl = '" + txtFolio.Text + "' ";
                }

                cmd = new SQLiteCommand(qry, Conexion);
                int reg = cmd.ExecuteNonQuery();

                lblMEnsaje.Text = "Se eliminaron " + reg.ToString() + " registros";

                Conexion.Close();
            }
            catch
            {
                lblMEnsaje.Text = "Error en la ejecución del proceso";
            }
            finally
            {
                Conexion.Dispose();
            }
        }
    }
}