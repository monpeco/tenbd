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
        String tracer = "1"; //am:23/02/17

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
                tracer = "2"; //am:23/02/17
                Conexion = new SQLiteConnection("Data Source=" + vPathBD + ";Version=3;Pooling=true;ReadOnly=false");
                tracer = "3"; //am:23/02/17
                Conexion.Open();
                tracer = "4"; //am:23/02/17
                string qry = " delete from dbq_arch " +
                              " where  pk01_tabl = '" + txtEmpresa.Text + "' " +
                              " and    pk02_tabl = '" + txtTipo.Text + "' " +
                              " and    pk03_tabl = '" + txtFolio.Text + "' " +
                              " and    esta_arch not in ('CER');";
                tracer = "5"; //am:23/02/17
                if (chkForzar.Checked)
                {
                    tracer = "6"; //am:23/02/17
                    qry = " delete from dbq_arch " +
                          " where  pk01_tabl = '" + txtEmpresa.Text + "' " +
                          " and    pk02_tabl = '" + txtTipo.Text + "' " +
                          " and    pk03_tabl = '" + txtFolio.Text + "' ";
                }
                tracer = "7"; //am:23/02/17
                cmd = new SQLiteCommand(qry, Conexion);
                tracer = "8"; //am:23/02/17
                int reg = cmd.ExecuteNonQuery();
                tracer = "9"; //am:23/02/17
                lblMEnsaje.Text = "Se eliminaron " + reg.ToString() + " registros";
                tracer = "10"; //am:23/02/17
                Conexion.Close();
                tracer = "11"; //am:23/02/17
            }
            //catch //am:23/02/17
            //{
            //    lblMEnsaje.Text = "Error en la ejecución del proceso";
            //}
            catch (Exception ex)
            {

                lblMEnsaje.Text = "tracer:[" + tracer + "] <br>" + ex.ToString() + " <br> Error en la ejecución del proceso";
            }
            finally
            {
                Conexion.Dispose();
            }
        }
    }
}