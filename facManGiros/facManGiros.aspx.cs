using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;
using DbnetWebLibrary;

/// <summary>
/// Descripción breve de dbsProceso.
/// </summary>
public partial class facManGiros : DbnetPage
{
    private static int salidaweb = 0;
    private static string modo = "I";
    private static string qryRamo = "";
    private static int opAyuda = 0;
    private static int opError = 0;

    private void mostrar(int ayuda, int error)
    {
        int a = barDescripcion.Text.ToLower().Replace("<br>", "$").Split('$').Length;
        if (a > 2)
            if (a + 8 > 11) a = 11;
        int tamA = Convert.ToInt32(barDescripcion.Height.Value) + (5 * a);
        if (tamA > 40) tamA = 40;


        int b = lbMensaje.Text.ToLower().Replace("<br>", "$").Split('$').Length;
        if (b > 2)
            if (b + 8 > 11) b = 11;
        int tamE = Convert.ToInt32(lbMensaje.Height.Value) + (5 * b);
        if (tamE > 40) tamE = 40;
        ClientScript.RegisterStartupScript(typeof(Page), "bar", "<script type=\"text/javascript\">Error(" + opError + "," + tamE + ");Ayuda(" + opAyuda + "," + tamA + ");</script>");
    }

    private void MsgError(int valor)
    {
        if (valor == 2)
        {
            valor = 1;
            lbMensaje.CssClass = "dbnOK";
        }
        else
            lbMensaje.CssClass = "dbnError";

        opError = valor;
        mostrar(opAyuda, opError);
    }
    private void MsgAyuda(int valor)
    {
        opAyuda = valor;
    }
    private void barAyuda_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        if (opAyuda == 0)
            MsgAyuda(1);
        else
            MsgAyuda(0);
        mostrar(opAyuda, opError);
    }
    private void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            DbnetContext = (DbnetSesion)Session["contexto"];
            if (!IsPostBack)
            {
                lbTitulo.Text = "Mantencion de Giros";


                if (DbnetContext.Val1 == "" && modo != "M")
                    modo = "I";
                else modo = "M";

                switch (modo)
                {

                    case "M":

                        if (DbnetContext.Key1.TrimStart() != "CODI_RAMO")
                            lbMensaje.Text = "Error : Parametros Invalidos";
                        else
                        {
                            qryRamo = "select codi_ramo,nomb_ramo from ramo where codi_empr= " + DbnetContext.Codi_empr.ToString() + " and codi_ramo='" + DbnetContext.Val1.ToString() + "'";

                            DataTable dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, qryRamo);
                            DataRow dr = dt.Rows[0];

                            Codi_ramo.Text = dr["codi_ramo"].ToString();
                            Nomb_ramo.Text = dr["nomb_ramo"].ToString();
                            Codi_ramo.Enabled = false;
                        }
                        break;
                    case "I":
                        break;
                }

                if (DbnetContext.Key5 == "SALIDA_WEB")
                    salidaweb = 1;
                else
                    salidaweb = 0;


                DbnetContext.Key1 = ""; DbnetContext.Val1 = "";
                DbnetContext.Key2 = ""; DbnetContext.Val2 = "";
                DbnetContext.Key3 = ""; DbnetContext.Val3 = "";
                DbnetContext.Key4 = ""; DbnetContext.Val4 = "";
            }
        }
        catch (Exception ex)
        {
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Maestro de Giros", "VarChar", 50, "in",
                                  "pmsaj_erro", ex.Message, "VarChar", 150, "in",
                                  "pbin_erro", "WEB", "VarChar", 50, "in",
                                  "p_mensaje", "", "VarChar", 200, "out");

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = sp.return_String("p_mensaje");
        }
    }

    #region Web Form Designer generated code
    override protected void OnInit(EventArgs e)
    {
        //
        // CODEGEN: llamada requerida por el Diseñador de Web Forms ASP.NET.
        //
        InitializeComponent();
        base.OnInit(e);
    }

    /// <summary>
    /// Método necesario para admitir el Diseñador, no se puede modificar
    /// el contenido del método con el editor de código.
    /// </summary>
    private void InitializeComponent()
    {
        this.barRun.Click += new System.Web.UI.ImageClickEventHandler(this.barRun_Click);
        this.barDel.Click += new System.Web.UI.ImageClickEventHandler(this.barDel_Click);
        this.barExit.Click += new System.Web.UI.ImageClickEventHandler(this.barSalir_Click);
        this.Load += new System.EventHandler(this.Page_Load);
        this.barAyuda.Click += new System.Web.UI.ImageClickEventHandler(this.barAyuda_Click);

    }
    #endregion
    private bool ValidaFormulario()
    {
        lbMensaje.Text = "";

        lbMensaje.Text += DbnetComprobaciones.ValidaCadena("<br>El Codigo", Codi_ramo.Text, 2, "", "=>1;<=12"); //">=5;<=8");
        lbMensaje.Text += DbnetComprobaciones.ValidaCadena("<br>El Nombre del giro ", Nomb_ramo.Text, 0, "'\"", ">=5;<=80"); //">=5;<=8");

        if (lbMensaje.Text == "")
            return true;
        else
        { MsgError(1); return false; }
    }
    private void barRun_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        lbMensaje.Text = "";
        try
        {
            if (ValidaFormulario())
            {
                DataTable dataTable = new DataTable();
                string qryCodiEmpr = String.Empty;
                string qryAllRamos = String.Empty;

                if (modo == "I")
                {
                    qryCodiEmpr = "select codi_empr from empr";
                    dataTable = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, qryCodiEmpr);

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        qryRamo = "Insert into ramo(codi_ramo,nomb_ramo,codi_empr)" +
                                    " values (" +
                                    "'" + Codi_ramo.Text + "'," +
                                    "'" + Nomb_ramo.Text + "'," +
                                    "" + dr[0].ToString() + "); ";
                        qryAllRamos += qryRamo;
                    }

                    DbnetTool.ctrlSqlInjection(this.Page.Form);
                    DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, qryAllRamos);
                    if (salidaweb == 0)
                        DbnetTool.MsgAlerta("Nuevo Giro Agregado!!", this.Page);

                    Codi_ramo.Enabled = false;
                }
                else
                {
                    qryRamo = "update ramo set nomb_ramo='" + Nomb_ramo.Text + "'" +
                         "where codi_ramo='" + Codi_ramo.Text + "' and codi_empr=" + DbnetContext.Codi_empr.ToString();

                    DbnetTool.ctrlSqlInjection(this.Page.Form);
                    DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, qryRamo);
                    if (salidaweb == 0)
                        DbnetTool.MsgAlerta("Cambios Realizados!!", this.Page);
                }
                if (salidaweb != 0)
                {
                    string aux = "";
                    if (modo == "I")
                        aux = "Nuevo Giro Agregado!!";
                    else
                        aux = "Cambios Realizados !!";
                    string pScript = "<script type=\"text/javascript\"> " +
                              "parent.opener.theForm.submit();" +
                              "alert(\"" + aux + "\") </script>";
                    DbnetContext.AuxSql = "giro";
                    ClientScript.RegisterStartupScript(typeof(Page), "Pagina", pScript);
                }
            }
        }
        catch (Exception ex)
        {

            if (ex is System.Data.OleDb.OleDbException)
            {
                DbnetTool.MsgAlerta("Infracción de la restricción PRIMARY KEY", this.Page);
            }

            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Maestro de Giros", "VarChar", 50, "in",
                                  "pmsaj_erro", ex.Message, "VarChar", 150, "in",
                                  "pbin_erro", "WEB", "VarChar", 50, "in",
                                  "p_mensaje", "", "VarChar", 200, "out");

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = sp.return_String("p_mensaje");
        }
    }
    private void barSalir_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        salir();
    }
    private void barDel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        lbMensaje.Text = "";
        try
        {
            //	validar que el codigo no este en uso

            bool accion1 = false;
            bool accion2 = false;
            string query;
            query = "select count(*) numero from empr where " +
                               " codi_ramo='" + Codi_ramo.Text + "'";

            DbnetTool.ctrlSqlInjection(this.Page.Form);
            DataTable dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                if (Convert.ToInt32(dr["numero"].ToString()) > 0)
                    accion1 = true;
                else
                    accion1 = false;
            }

            query = "select count(*) numero from personas where " +
                               " codi_ramo='" + Codi_ramo.Text + "'";

            DbnetTool.ctrlSqlInjection(this.Page.Form);
            dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                if (Convert.ToInt32(dr["numero"].ToString()) > 0)
                    accion2 = true;
                else
                    accion2 = false;
            }
            string mensaje = "";

            if (accion1)
                mensaje = "El Giro esta en uso en la tabla de Personas, no se puede eliminar!!";

            if (accion2)
                mensaje += "\\n El Giro esta en uso en la tabla de Empresas , no se puede eliminar!!";

            if (accion2 == false && accion1 == false)
            {
                query = "delete from ramo where " +
                               "codi_ramo='" + Codi_ramo.Text + "' and " +
                               "nomb_ramo='" + Nomb_ramo.Text + "'";
                DbnetTool.ctrlSqlInjection(this.Page.Form);
                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                DbnetTool.MsgAlerta("Giro Eliminado!!", this.Page);
            }
            else
                DbnetTool.MsgAlerta(mensaje, this.Page);
            salir();
        }
        catch (Exception ex)
        {
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Maestro de Giros", "VarChar", 50, "in",
                                  "pmsaj_erro", ex.Message, "VarChar", 150, "in",
                                  "pbin_erro", "WEB", "VarChar", 50, "in",
                                  "p_mensaje", "", "VarChar", 200, "out");

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = sp.return_String("p_mensaje");
        }
    }
    private void salir()
    {
        string pScript = "";
        switch (salidaweb)
        {
            case 0:
                pScript = "<script type=\"text/javascript\"> " +
                    "window.location.href=\"../dbnWeb/dbnListador.aspx?listado=fac-lisGiros&modo=M \"" +
                    "</script>";
                break;

            default:
                pScript = "<script type=\"text/javascript\"> " +
                     "window.close();" +
                     "</script>";
                break;
        }
        DbnetContext.Key5 = ""; DbnetContext.Val5 = "";

        ClientScript.RegisterStartupScript(typeof(Page), "Pagina", pScript);
    }
}