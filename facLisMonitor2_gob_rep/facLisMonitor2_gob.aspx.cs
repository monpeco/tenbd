using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Util;
using System.Xml;
using DbnetWebLibrary;
using dbnWeb;


public partial class facLisMonitor2_GOB : DbnetPage
{
    //private DataTable this.DbnetContext.dtSelect;
    //private DataTable this.DbnetContext.dtSelectInicial;
    //private DataTable this.DbnetContext.dtGrilla;
    //private static string sortGrilla   = "";
    //private int     this.DbnetContext.nroPagina    = 0;
    //private int     this.DbnetContext.totPagina    = 0;
    //private int     this.DbnetContext.totRegistro  = 0;
    //private int     this.DbnetContext.opBusqueda   = 0;
    //private string  this.DbnetContext.TipoMonitor  = "DTE";
    //private int     this.DbnetContext.regpag       = 0;

    //protected System.Web.UI.WebControls.Label lbMensaje;

    //protected System.Web.UI.WebControls.Label lbEx;
    //protected System.Web.UI.WebControls.CheckBox chkEx;
    //protected System.Web.UI.WebControls.CheckBox chkDespliega;

    //protected System.Web.UI.WebControls.DataGrid Grilla;
    //protected System.Web.UI.WebControls.Label lbRegistro;
    //protected System.Web.UI.WebControls.Label lbReg;
    //protected System.Web.UI.WebControls.Label lbPagina;
    //protected System.Web.UI.WebControls.Label lbPag;
    //protected System.Web.UI.WebControls.ImageButton barBack;
    //protected System.Web.UI.WebControls.ImageButton barNext;
    //protected System.Web.UI.WebControls.Image imgTitulo;
    //protected System.Web.UI.WebControls.Label lbTitulo;
    //protected System.Web.UI.WebControls.ImageButton barFirst;
    //protected System.Web.UI.WebControls.ImageButton barLast;
    //protected System.Web.UI.WebControls.ImageButton btBuscar;
    //protected DbnetWebControl.DbnetWebTextBox_Numero Foli_docu1;
    //protected DbnetWebControl.DbnetWebTextBox_Numero Foli_docu2;
    //protected DbnetWebControl.DbnetWebTextBox_Fecha Fecha_desde;
    //protected DbnetWebControl.DbnetWebTextBox_Fecha Fecha_hasta;
    //protected DbnetWebControl.DbnetWebLabel DbnetWebLabel1;
    //protected DbnetWebControl.DbnetWebLabel DbnetWebLabel2;
    //protected DbnetWebControl.DbnetWebLabel DbnetWebLabel3;
    //protected DbnetWebControl.DbnetWebLabel DbnetWebLabel4;
    //protected DbnetWebControl.DbnetWebLabel DbnetWebLabel5;
    //protected DbnetWebControl.DbnetWebLabel DbnetWebLabel20;
    //protected DbnetWebControl.DbnetWebLabel lbCodi_pers;
    //protected DbnetWebControl.DbnetWebLov lvTipo_docu;
    //protected DbnetWebControl.DbnetWebTextBox_Numero Tipo_docu;
    //protected DbnetWebControl.DbnetWebLov lvCodi_pers;
    //protected DbnetWebControl.DbnetWebTextBox_Numero Codi_pers;
    //protected System.Web.UI.WebControls.RadioButton rdEmision;
    //protected System.Web.UI.WebControls.RadioButton rdRecepcion;
    //protected DbnetWebControl.DbnetWebLabel lbCodi_sucu;
    //protected DbnetWebControl.DbnetWebTextBox_Numero Codi_sucu;
    //protected DbnetWebControl.DbnetWebLov lvCodi_sucu;
    //protected DbnetWebControl.DbnetWebLabel Dbnetweblabel6;
    //protected System.Web.UI.WebControls.CheckBox EstadoAnormal;
    //protected System.Web.UI.WebControls.Label lbRegPag;
    //protected System.Web.UI.WebControls.ImageButton barExcel;
    //protected DbnetWebControl.DbnetWebLov txtRegPag;
    //protected DbnetWebControl.DbnetWebLabel Dbnetweblabel7;
    //protected System.Web.UI.WebControls.CheckBox FormatoRespuesta;
    //protected System.Web.UI.WebControls.CheckBox ChkFoli_clie;
    //protected DbnetWebControl.DbnetWebLabel LbFoli_clie;
    //protected DbnetWebControl.DbnetWebTextBox Foli_erp2;
    //protected DbnetWebControl.DbnetWebTextBox Foli_erp1;

    //protected System.Web.UI.WebControls.ImageButton barAyuda;
    //protected System.Web.UI.WebControls.Label barDescripcion;

    //protected DbnetWebControl.DbnetWebTextBox txtPasaCuenta;
    //protected DbnetWebControl.DbnetWebTextBox txtPasaImp;
    
    private bool sw_buscar = false;
    public double tamano = 0;
    /* Declaracion de parametros que habilitan funcionalidad */
    public bool par_trazos = false;	/* Lapiz Digital */
    public bool par_folio_erp = false; /* Folio ERP */

    private static int opAyuda = 0;
    private static int opError = 0;
    private DbnetListadorXML lis;
    private string query = "";
    private static string lib = "librerias/xml-conf/";
    private static string qryCodi_pers = "";
    private static string qryTipo_docu = "";
    private static string qryCodi_sucu = "";
    private static string qryins = "";

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
        // mostrar(opAyuda, opError); 
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
        //if (Request.Params["par1"] == "DTE")
        //{
        //    rdEmision.Visible = false;
        //    rdRecepcion.Visible = false;
        //    DbnetWebLabel20.Visible = false;
        //}
        //else
        //{
        //   rdEmision.Visible = true;
        //   rdRecepcion.Visible = true;
        //   DbnetWebLabel20.Visible = true;
        //}


        // Carga el contexto de la session
        DbnetContext = (DbnetSesion)Session["contexto"];
        lis = (DbnetListadorXML)Session["monitor"];
        // Introducir aquí el código de usuario para inicializar la página
        if (!IsPostBack)
        {
            string p_listado = "";

            Habilita();//Request.Params["par1"]);
            //switch (Request.Params["par1"])
            //{
            //    case "DTE":
            //        this.DbnetContext.TipoMonitor      = "DTE";
            //        p_listado        = "fac-monDTE";
            //        lbCodi_pers.Text = "Receptor";
            //        break;
            //    case "DTE_ESTA":
            //        this.DbnetContext.TipoMonitor      = "DTE";
            //        p_listado        = "fac-monDTEEstados";
            //        lbCodi_pers.Text = "Receptor";
            //        break;
            //    case "DTE_TOTAL":
            //        this.DbnetContext.TipoMonitor      = "DTE";
            //        p_listado        = "fac-monDTETotales";
            //        lbCodi_pers.Text = "Receptor";
            //        break;
            //    case "DTO":
            this.DbnetContext.TipoMonitor = "DTO";
            p_listado = "fac-monDTO2_GOB";
            lbCodi_pers.Text = "Emisor";
            //        break;
            //    case "DTO_ESTA":
            //        this.DbnetContext.TipoMonitor      = "DTO";
            //        p_listado        = "fac-monDTOEstados";
            //        lbCodi_pers.Text = "Receptor";
            //        break;
            //    case "DTO_TOTAL":
            //        this.DbnetContext.TipoMonitor      = "DTO";
            //        p_listado        = "fac-monDTOTotales";
            //        lbCodi_pers.Text = "Receptor";
            //        break;
            //}

            if (Request.Params["SUCURSAL"] == "S")
            {
                query = "select codi_ofic from usua_sist where codi_usua='" + DbnetContext.Codi_usua + "'";
                Codi_sucu.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
                Codi_sucu.Enabled = false;
                lvCodi_sucu.Enabled = false;
            }

            if (p_listado == null)
                p_listado = "dbnListador";
            p_listado = p_listado + ".xml";

            lis = new DbnetListadorXML(DbnetGlobal.Path + lib + p_listado, "M", DbnetGlobal.Base_dato);
            if (lis.Status)
            {
                this.DbnetContext.regpag = lis.dtRegistros;
                txtRegPag.SelectedValue = this.DbnetContext.regpag.ToString();
                imgTitulo.Width = lis.dtAncho;
                //GDIAZ: 27-02-2012 Se elimina título mayúscula.
                //lbTitulo.Text = lis.dtTitulo.ToUpper();
                lbTitulo.Text = lis.dtTitulo;

                Grilla.Width = lis.dtAncho;
                Grilla.PageSize = this.DbnetContext.regpag;
                //if (this.DbnetContext.TipoMonitor == "DTE")
                //{
                //    Grilla.Columns[6].Visible = true;
                //    Grilla.Columns[7].Visible = true;
                //    Grilla.Columns[8].Visible = true;
                //    if(DbnetTool.SelectInto(DbnetContext.dbConnection,"select param_value from sys_param where param_name = 'EGATE_FOLI_CLIE'")!="1")
                //    {
                //        LbFoli_clie.Visible	 = false;
                //        ChkFoli_clie.Visible = false; 
                //    }
                //    else
                //    {
                //        LbFoli_clie.Visible	 = true;
                //        ChkFoli_clie.Visible = true;
                //    }
                //}
                //else
                //{
                //Grilla.Columns[6].Visible = false;
                //Grilla.Columns[7].Visible = false;
                //Grilla.Columns[8].Visible = false;
                //}

                this.DbnetContext.dtGrilla = new DataTable();

                qryCodi_pers = "Select codi_pers, " + DbnetContext.Auxdbo + "initcap(nomb_pers) from personas union select '0',' Todos' from dual order by 2";
                qryTipo_docu = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec='S' union select 0,' Todos' from dual order by 2";
                qryCodi_sucu = "Select codi_ofic, " + DbnetContext.Auxdbo + "initcap(nomb_ofic) from oficina where codi_empr='" + DbnetContext.Codi_empr + "' union select '0',' Todos' from dual order by 1";


                lvCodi_sucu.Query = qryCodi_sucu;
                lvCodi_sucu.Rescata(DbnetContext.dbConnection);

                lvCodi_pers.Query = qryCodi_pers;
                //Se comento para hacer cargar manual
                //lvCodi_pers.Rescata(DbnetContext.dbConnection);

                lvTipo_docu.Query = qryTipo_docu;
                lvTipo_docu.Rescata(DbnetContext.dbConnection);

                Tipo_docu.Text = "0";
                lvTipo_docu.Selecciona(Tipo_docu.Text);
                Codi_pers.Text = "0";
                //lvCodi_pers.Selecciona(Codi_pers.Text);
                if (this.Codi_sucu.Text == "")
                    this.Codi_sucu.Text = "0";
                lvCodi_sucu.Selecciona(this.Codi_sucu.Text);

                DateTime fecha_desde = DateTime.Today.AddDays(-7);
                try
                {
                    int dias = Convert.ToInt16(DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name = 'EGATE_WEB_NDIAS'")) * -1;
                    fecha_desde = DateTime.Today.AddDays(dias);
                }
                catch
                {
                    fecha_desde = DateTime.Today.AddDays(-7);
                }
                //DateTime fecha_desde = DateTime.Today.AddDays(-360);
                DateTime fecha_hasta = DateTime.Today;

                //Fecha_desde.Text = fecha_desde.Year.ToString() + "-" +
                //                   fecha_desde.Month.ToString() + "-" +
                //                   fecha_desde.Day.ToString();

                //Fecha_hasta.Text = fecha_hasta.Year.ToString() + "-" +
                //    fecha_hasta.Month.ToString() + "-" +
                //    fecha_hasta.Day.ToString();
                Fecha_desde.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                Fecha_hasta.Text = DateTime.Now.ToString("yyyy-MM-dd");

                try
                {
                    Session.Add("monitor", lis);
                }
                catch
                {
                    Session["monitor"] = lis;
                }
            }
            else
            {
                DbnetTool.MsgError("Definicion de Listado no existe", this.Page);
            }
        }
        ActualizacionBar();
        //dbnWebCalendario.verCalendario(this);
    }
    private void Habilita()//string pTipo)
    {
        /* Definicion de parametros que habilitan funcionalidad */
        if (DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name = 'EGATE_FOLI_CLIE'") == "1")
            par_folio_erp = true;
        else
            par_folio_erp = false;
        if (DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name = 'EGATE_ANOTO_DIAS'") != "")
            par_trazos = true;
        else
            par_trazos = false;

        /* Habilitacion o Deshabilitacion de Columnas */
        //if (pTipo == "DTE")
        //{
        //    Grilla.Columns[0].Visible = true;	/* Boton XML */
        //    Grilla.Columns[1].Visible = true;	/* Boton PDF */
        //    Grilla.Columns[2].Visible = true;   /* Boton PDF-Merito */
        //    Grilla.Columns[3].Visible = true;   /* Boton HTML */
        //    Grilla.Columns[4].Visible = true;   /* Boton Archivo */
        //    Grilla.Columns[5].Visible = true;   /* Boton Ayuda */
        //    Grilla.Columns[6].Visible = true;   /* Boton Detalle */
        //    Grilla.Columns[7].Visible = true;   /* Lin */
        //    Grilla.Columns[8].Visible = true;   /* Tipo */
        //    Grilla.Columns[9].Visible = true;  /* Folio */
        //    if (par_folio_erp == true)			/* Folio ERP */
        //    {
        //        LbFoli_clie.Visible	 = true;
        //        ChkFoli_clie.Visible = true; 
        //        Grilla.Columns[10].Visible = true;  
        //    }
        //    else
        //    {
        //        LbFoli_clie.Visible	 = false;
        //        ChkFoli_clie.Visible = false; 
        //        Grilla.Columns[10].Visible = false;
        //    }
        //    Grilla.Columns[11].Visible = true;  /* Rut */
        //    Grilla.Columns[12].Visible = true;  /* Nombre */
        //    Grilla.Columns[13].Visible = true;  /* Total */
        //    Grilla.Columns[14].Visible = true;  /* Emision */
        //    Grilla.Columns[15].Visible = true;  /* Carga */
        //    Grilla.Columns[16].Visible = true;  /* Envio SII */
        //    Grilla.Columns[17].Visible = true;  /* Respuesta SII */
        //    Grilla.Columns[18].Visible = true;  /* Envio Contribuyente */
        //    Grilla.Columns[19].Visible = false; /* Recepcion DTE */
        //    Grilla.Columns[20].Visible = true;  /* Resp.Tecnica */
        //    Grilla.Columns[21].Visible = true;  /* Boton Resp.Tecnica */
        //    if (par_trazos == true)				/* Boton PDF-Trazo */	
        //        Grilla.Columns[22].Visible = true;   
        //    else
        //        Grilla.Columns[22].Visible = false;
        //    Grilla.Columns[23].Visible = true;  /* Resp.Comercial */
        //    Grilla.Columns[24].Visible = true;  /* Boton Resp.Comercial */
        //    Grilla.Columns[25].Visible = true;  /* Estado DTE */
        //    Grilla.Columns[26].Visible = true;  /* Estado Envio */
        //    Grilla.Columns[27].Visible = true;  /* Corr. Envio */
        //    Grilla.Columns[28].Visible = true;  /* Track-ID */
        //}
        //else if (pTipo == "DTO")
        //{
        Grilla.Columns[0].Visible = true;	/* Boton XML */
        Grilla.Columns[1].Visible = true;	/* Boton PDF */
        Grilla.Columns[2].Visible = true;   /* Boton HTML */
        Grilla.Columns[3].Visible = true;   /* Boton Archivo */
        Grilla.Columns[4].Visible = true;   /* Boton Ayuda */
        Grilla.Columns[5].Visible = true;   /* LIN*/
        Grilla.Columns[6].Visible = false;   /* IMP*/
        Grilla.Columns[7].Visible = false;   /* CON*/
        Grilla.Columns[8].Visible = true;   /* Tipo */
        Grilla.Columns[9].Visible = true;  /* Folio */
        Grilla.Columns[10].Visible = false; /* Folio ERP */
        Grilla.Columns[11].Visible = true;  /* Rut */
        Grilla.Columns[12].Visible = true;  /* Nombre */
        Grilla.Columns[13].Visible = true;  /* Total */
        Grilla.Columns[14].Visible = true;  /* Emision */
        Grilla.Columns[15].Visible = false; /* Carga */
        Grilla.Columns[16].Visible = true; /* Envio SII */
        Grilla.Columns[17].Visible = false; /* Respuesta SII */
        Grilla.Columns[18].Visible = false; /* Envio Contribuyente */
        Grilla.Columns[19].Visible = true;  /* Recepcion DTE */
        Grilla.Columns[20].Visible = true;  /* Resp.Tecnica */
        Grilla.Columns[21].Visible = false; /* Boton Resp.Tecnica */
        Grilla.Columns[22].Visible = false; /* Boton PDF-Trazo */
        Grilla.Columns[23].Visible = true;  /* Resp.Comercial */
        Grilla.Columns[24].Visible = false; /* Boton Resp.Comercial */
        Grilla.Columns[25].Visible = true;  /* Estado DTE */
        Grilla.Columns[26].Visible = false; /* Estado Envio */
        Grilla.Columns[27].Visible = false; /* Corr. Envio */
        Grilla.Columns[28].Visible = false; /* Track-ID */        
        //}
    }
    private void ActualizacionBar()
    {
        this.DbnetContext.regpag = Convert.ToInt16(txtRegPag.SelectedValue);
        Grilla.PageSize = this.DbnetContext.regpag;
        lbPagina.Text = this.DbnetContext.nroPagina.ToString() + "/" + this.DbnetContext.totPagina.ToString();
        lbRegistro.Text = this.DbnetContext.totRegistro.ToString();
        //tamano=Grilla.Items.Count % 2;
    }
    private void buscar()
    {
        lbMensaje.Text = "";
        string str_defe = "1=1";
        try
        {
            query = lis.dtQuery;
            query = query.Replace(":P_CODI_EMPR", DbnetContext.Codi_empr.ToString());
            query = query.Replace(":P_CODI_PERS", DbnetContext.Codi_pers.ToString());
            query = query.Replace(":P_CODI_CECO", DbnetContext.Codi_ceco.ToString());
            query = query.Replace(":P_CODI_USUA", DbnetContext.Codi_usua.ToString());
            query = query.Replace(":P_CODI_MODU", DbnetContext.Codi_modu.ToString());

            /* Filtro de Fecha Desde */
            if (Fecha_desde.Text != "")
            {
                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {
                    //if (Request.Params["par1"] == "DTO")
                    //{
                    //if (rdEmision.Checked==true)
                    //{
                    //    query = query.Replace(":FECHA_RECE1", "Convert(DateTime,DTE.FECH_EMIS,120) >= Convert(DateTime,'" + Fecha_desde.Text + "',120)");                                
                    //    query = query.Replace(":FECHA_DOCU1", str_defe);
                    //}
                    //if (rdRecepcion.Checked==true)
                    //{
                    query = query.Replace(":FECHA_RECE1", "Convert(DateTime,DTE.FECH_RECE,120) >= Convert(DateTime,'" + Fecha_desde.Text + "',120)");
                    query = query.Replace(":FECHA_DOCU1", str_defe);
                    //}
                    //}
                    //else  // DTE
                    //{
                    //    query = query.Replace(":FECHA_EMIS1", "Convert(DateTime,DTE.FECH_EMIS,120) >= Convert(DateTime,'" + Fecha_desde.Text + "',120)");
                    //    query = query.Replace(":FECHA_RECE1", "Convert(DateTime,DTE.FECH_RECE,120) >= Convert(DateTime,'" + Fecha_desde.Text + "',120)");
                    //    query = query.Replace(":FECHA_DOCU1", "Convert(DateTime,DTE.FECH_DOCU,120) >= Convert(DateTime,'" + Fecha_desde.Text + "',120)");
                    //}
                }
                else  // Oracle
                {
                    //if (Request.Params["par1"] == "DTO")
                    //{
                    //    if (rdEmision.Checked==true)
                    //    {
                    //        query = query.Replace(":FECHA_RECE1", "to_date(DTE.FECH_EMIS,'YYYY-MM-DD') >= to_date('" + Fecha_desde.Text + "','yyyy-mm-dd')");                                                                                                
                    //        query = query.Replace(":FECHA_DOCU1", str_defe);
                    //    }
                    //    if (rdRecepcion.Checked==true)
                    //    {
                    query = query.Replace(":FECHA_RECE1", "to_date(DTE.FECH_RECE,'YYYY-MM-DD') >= to_date('" + Fecha_desde.Text + "','yyyy-mm-dd')");
                    query = query.Replace(":FECHA_DOCU1", str_defe);
                    //    }
                    //}
                    //else // DTE
                    //{
                    //    query = query.Replace(":FECHA_EMIS1", "to_date(DTE.FECH_EMIS,'YYYY-MM-DD') >= to_date('" + Fecha_desde.Text + "','yyyy-mm-dd')");
                    //    query = query.Replace(":FECHA_RECE1", "to_date(DTE.FECH_RECE,'YYYY-MM-DD') >= to_date('" + Fecha_desde.Text + "','yyyy-mm-dd')");
                    //    query = query.Replace(":FECHA_DOCU1", "to_date(DTE.FECH_DOCU,'YYYY-MM-DD') >= to_date('" + Fecha_desde.Text + "','yyyy-mm-dd')");
                    //}
                }
            }
            else // sin filtro de fecha desde
            {
                query = query.Replace(":FECHA_EMIS1", str_defe);
                query = query.Replace(":FECHA_RECE1", str_defe);
                query = query.Replace(":FECHA_DOCU1", str_defe);
            }

            query = query.Replace(":FECHA_EMIS1", str_defe);
            /* Filtro de Fecha Hasta */
            if (Fecha_hasta.Text != "")
            {
                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {
                    //if (Request.Params["par1"] == "DTO")
                    //{
                    //    if (rdEmision.Checked==true)
                    //    {
                    //        query = query.Replace(":FECHA_RECE2", "Convert(DateTime,DTE.FECH_EMIS,120) <= Convert(DateTime,'" + Fecha_hasta.Text + "',120)");
                    //        query = query.Replace(":FECHA_RECE2", str_defe);
                    //        query = query.Replace(":FECHA_DOCU2", str_defe);
                    //    }
                    //    if (rdRecepcion.Checked==true)
                    //    {
                    query = query.Replace(":FECHA_RECE2", "Convert(DateTime,DTE.FECH_RECE,120) <= Convert(DateTime,'" + Fecha_hasta.Text + "',120)");
                    query = query.Replace(":FECHA_EMIS2", str_defe);
                    query = query.Replace(":FECHA_DOCU2", str_defe);
                    //    }
                    //}
                    //else  //DTE
                    //{
                    //    query = query.Replace(":FECHA_EMIS2", "Convert(DateTime,DTE.FECH_EMIS,120) <= Convert(DateTime,'" + Fecha_hasta.Text + "',120)");
                    //    query = query.Replace(":FECHA_RECE2", "Convert(DateTime,DTE.FECH_RECE,120) <= Convert(DateTime,'" + Fecha_hasta.Text + "',120)");
                    //    query = query.Replace(":FECHA_DOCU2", "Convert(DateTime,DTE.FECH_DOCU,120) <= Convert(DateTime,'" + Fecha_hasta.Text + "',120)");
                    //}
                }
                else // Oracle
                {
                    //if (Request.Params["par1"] == "DTO")
                    //{
                    //    if (rdEmision.Checked==true)
                    //    {
                    //        query = query.Replace(":FECHA_RECE2", "to_date(DTE.FECH_EMIS,'YYYY-MM-DD') <= to_date('" + Fecha_hasta.Text + "','yyyy-mm-dd')");
                    //        query = query.Replace(":FECHA_RECE2", str_defe);
                    //        query = query.Replace(":FECHA_DOCU2", str_defe);
                    //    }
                    //    if (rdRecepcion.Checked==true)
                    //    {
                    query = query.Replace(":FECHA_RECE2", "to_date(DTE.FECH_RECE,'YYYY-MM-DD') <= to_date('" + Fecha_hasta.Text + "','yyyy-mm-dd')");
                    query = query.Replace(":FECHA_EMIS2", str_defe);
                    query = query.Replace(":FECHA_DOCU2", str_defe);

                    //    }
                    //}
                    ////else  // DTE
                    //{
                    //    query = query.Replace(":FECHA_EMIS2", "to_date(DTE.FECH_EMIS,'YYYY-MM-DD') <= to_date('" + Fecha_hasta.Text + "','yyyy-mm-dd')");
                    //    query = query.Replace(":FECHA_RECE2", "to_date(DTE.FECH_RECE,'YYYY-MM-DD') <= to_date('" + Fecha_hasta.Text + "','yyyy-mm-dd')");
                    //    query = query.Replace(":FECHA_DOCU2", "to_date(DTE.FECH_DOCU,'YYYY-MM-DD') <= to_date('" + Fecha_hasta.Text + "','yyyy-mm-dd')");
                    //}
                }
            }
            else  // sin fltro
            {
                query = query.Replace(":FECHA_EMIS2", str_defe);
                query = query.Replace(":FECHA_RECE2", str_defe);
                query = query.Replace(":FECHA_DOCU2", str_defe);
            }



            /*Verifica si el Check de folio ERP esta habilitado*/
            if (ChkFoli_clie.Checked)
            {
                /* Si el folio hasta tiene valor, el folio desde no puede ser nulo */
                if ((Foli_erp1.Text == "" || Foli_erp1.Text == "0") && (Foli_erp2.Text != "" && Foli_erp2.Text != "0"))
                    Foli_erp1.Text = "1";

                /* Si el folio desde tiene valor, el folio hasta no puede ser nulo */
                if ((Foli_erp2.Text == "" || Foli_erp2.Text == "0") && (Foli_erp1.Text != "" && Foli_erp1.Text != "0"))
                    Foli_erp2.Text = Foli_erp1.Text;


                /* Filtro de Folio Desde */
                if (Foli_erp1.Text != "" && Foli_erp1.Text != "0")
                {
                    //						if (DbnetGlobal.Base_dato == "SQLSERVER" && this.DbnetContext.TipoMonitor == "DTO")
                    //							query = query.Replace(":FOLIO1","Convert(int,Replace(DTE.FOLI_DOCU,'.0','')) >= " + Foli_docu1.Text + "");
                    //						else
                    query = query.Replace(":FOLIO_ERP1", "DTE.FOLI_CLIE >= '" + Foli_erp1.Text + "'");
                }
                else
                    query = query.Replace(":FOLIO_ERP1", str_defe);


                /* Filtro de Folio Hasta */
                if (Foli_erp2.Text != "" && Foli_erp2.Text != "0")
                {
                    //						if (DbnetGlobal.Base_dato == "SQLSERVER" && this.DbnetContext.TipoMonitor == "DTO")
                    //							query = query.Replace(":FOLIO2","Convert(int,Replace(DTE.FOLI_DOCU,'.0','')) <= " + Foli_docu2.Text + "");
                    //						else
                    query = query.Replace(":FOLIO_ERP2", "DTE.FOLI_CLIE <= '" + Foli_erp2.Text + "'");
                }
                else
                    query = query.Replace(":FOLIO_ERP2", str_defe);

                query = query.Replace(":FOLIO1", str_defe);
                query = query.Replace(":FOLIO2", str_defe);
                query = query.Replace(":FOLIO_ERP", "1=2");

            }
            else
            {

                /* Si el folio hasta tiene valor, el folio desde no puede ser nulo */
                if ((Foli_docu1.Text == "" || Foli_docu1.Text == "0") && (Foli_docu2.Text != "" && Foli_docu2.Text != "0"))
                    Foli_docu1.Text = "1";

                /* Si el folio desde tiene valor, el folio hasta no puede ser nulo */
                if ((Foli_docu2.Text == "" || Foli_docu2.Text == "0") && (Foli_docu1.Text != "" && Foli_docu1.Text != "0"))
                    Foli_docu2.Text = Foli_docu1.Text;

                /* Filtro de Folio Desde */
                if (Foli_docu1.Text != "" && Foli_docu1.Text != "0")
                {
                    if (DbnetGlobal.Base_dato == "SQLSERVER" && this.DbnetContext.TipoMonitor == "DTO")
                        query = query.Replace(":FOLIO1", "Convert(int,Replace(DTE.FOLI_DOCU,'.0','')) >= " + Foli_docu1.Text + "");
                    else
                        query = query.Replace(":FOLIO1", "DTE.FOLI_DOCU >= " + Foli_docu1.Text + "");
                }
                else
                    query = query.Replace(":FOLIO1", str_defe);

                /* Filtro de Folio Hasta */
                if (Foli_docu2.Text != "" && Foli_docu2.Text != "0")
                {
                    if (DbnetGlobal.Base_dato == "SQLSERVER" && this.DbnetContext.TipoMonitor == "DTO")
                        query = query.Replace(":FOLIO2", "Convert(int,Replace(DTE.FOLI_DOCU,'.0','')) <= " + Foli_docu2.Text + "");
                    else
                        query = query.Replace(":FOLIO2", "DTE.FOLI_DOCU <= " + Foli_docu2.Text + "");
                }
                else
                    query = query.Replace(":FOLIO2", str_defe);

                query = query.Replace(":FOLIO_ERP1", str_defe);
                query = query.Replace(":FOLIO_ERP2", str_defe);
                query = query.Replace(":FOLIO_ERP", str_defe);

            }
            /* Filtro de Tipo */
            if (Tipo_docu.Text != "" && Tipo_docu.Text != "0")
                query = query.Replace(":TIPO", "DTE.TIPO_DOCU = '" + Tipo_docu.Text + "'");
            else
                query = query.Replace(":TIPO", str_defe);

            /* Filtro de Persona */
            if (Codi_pers.Text != "" && Codi_pers.Text != "0")
            {
                query = query.Replace(":PERS_EMIS", "DTE.RUTT_EMIS = '" + Codi_pers.Text + "'");
                query = query.Replace(":PERS_RECE", "DTE.RUTT_RECE = '" + Codi_pers.Text + "'");
            }
            else
            {
                query = query.Replace(":PERS_EMIS", str_defe);
                query = query.Replace(":PERS_RECE", str_defe);

            }

            /* Filtro de Sucursal */
            if (Codi_sucu.Text != "" && Codi_sucu.Text != "0" && this.DbnetContext.TipoMonitor == "DTE")
            {
                query = query.Replace(":SUCURSAL", "DTE.NOMB_SUCU = '" + Codi_sucu.Text + "'");
            }
            else if (Codi_sucu.Text != "" && Codi_sucu.Text != "0" && this.DbnetContext.TipoMonitor == "DTO")
            {
                query = query.Replace(":SUCURSAL", "DTE.CODI_SUCU = '" + Codi_sucu.Text + "'");
            }
            else
            {
                query = query.Replace(":SUCURSAL", str_defe);
            }

            /* Filtro de Estados Anormales */
            if (this.EstadoAnormal.Checked == true)
            {
                query = query.Replace(":ESTA_ANOR", "'RED'");
            }
            else
            {
                query = query.Replace(":ESTA_ANOR", "'TODOS'");
            }



            this.DbnetContext.dtSelectInicial = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            this.DbnetContext.dtSelect = this.DbnetContext.dtSelectInicial;

            this.DbnetContext.nroPagina = 0;
            this.DbnetContext.totPagina = 0;
            this.DbnetContext.totRegistro = 0;

            this.DbnetContext.totPagina = (this.DbnetContext.dtSelect.Rows.Count / this.DbnetContext.regpag) + 1;
            this.DbnetContext.totRegistro = this.DbnetContext.dtSelect.Rows.Count;

            buscar(1);

            try
            {
                Session.Add("listador", lis);
            }
            catch
            {
                Session["listador"] = lis;
            }
        }
        catch (Exception e)
        {   /*
            lbMensaje.Text = "";
            lbMensaje.Text += "Query :" + query + "<br>";
            lbMensaje.Text += "Error :" + e.Message + "<br>";
            MsgError(1);
             */
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Reporte DTO Excel", "VarChar", 50, "in",
                                  "pmsaj_erro", e.Message, "VarChar", 150, "in",
                                  "pbin_erro", "WEB", "VarChar", 50, "in",
                                  "p_mensaje", "", "VarChar", 200, "out");

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = sp.return_String("p_mensaje");
        }
    }
    private void buscar(int pagina)
    {
        try
        {
            try { this.DbnetContext.dtGrilla.Dispose(); }
            catch { }

            this.DbnetContext.dtGrilla = new DataTable();
            this.DbnetContext.dtGrilla.Columns.Add("Lin", typeof(int));
            this.DbnetContext.dtGrilla.Columns.Add("Imp");
            this.DbnetContext.dtGrilla.Columns.Add("Con");


            for (int i = 0; i < lis.dtEncabezados.Count; i++)
            {
                DataRowView drEncabezado = lis.dtEncabezados[i];
                this.DbnetContext.dtGrilla.Columns.Add(drEncabezado["Descripcion"].ToString());
            }

            int ini = this.DbnetContext.regpag * (pagina - 1);
            int fin = this.DbnetContext.regpag * (pagina - 1) + this.DbnetContext.regpag;

            if (fin > this.DbnetContext.totRegistro)
                fin = this.DbnetContext.totRegistro;
            for (int r = ini; r < fin; r++)
            {
                DataRow drSelect = this.DbnetContext.dtSelect.Rows[r];
                DataRow drGrilla = this.DbnetContext.dtGrilla.NewRow();
                drGrilla["Lin"] = r + 1;
                drGrilla["Imp"] = "<input class='dbnchk' type='checkbox' id='chk_imp_" + (r + 1) + "' onclick='javascript:chk_imp();'/><input type='hidden' id='hid_corr_" + (r + 1) + "' value='" + drSelect["corr_docu"].ToString() + "'>";
                drGrilla["Con"] = "<input class='dbnchk' type='checkbox' id='chk_con_" + (r + 1) + "' onclick='javascript:chk_con();'/>";

                for (int i = 0; i < lis.dtEncabezados.Count; i++)
                {
                    DataRowView drEncabezado = lis.dtEncabezados[i];
                    if (drEncabezado["Codigo"].ToString().Substring(0, 4) == "FASE")
                    {
                        string color = drSelect["COLOR_FASE"].ToString();

                        if (color.ToUpper() == "RED")
                        { color = "#C82A1D"; }
                        else if (color.ToUpper() == "GREEN")
                        {
                            color = "#40A04A";
                        }
                        else if (color.ToUpper() == "YELLOW")
                        {
                            color = "#FFC54D";
                        }
                        else
                        { }

                        //string fase_ant = "<font style='color: 40A04A; background: 40A04A; border-style=solid; filter: alpha(opacity=10);-moz-opacity:.10;opacity:.10;'>.</font>";
                        string fase_ant = "<font style='color:#40A04A; background:#40A04A; border-style=solid;'>.</font>";
                        string fase_car = "<font style='color: " + color + "; background: " + color + "; border-style=solid;'>.</font>";
                        //string fase_sig = "<font style='color: white; background: Transparent; border-style=solid;'></font>";

                        string fase_sig = "";

                        if (((r + 1) % 2) == 0)
                        {
                            fase_sig = "<font style='color:#E6E6E6; border-style=solid;'>.</font>";
                        }
                        else
                        {
                            fase_sig = "<font style='color: white; border-style=solid;'>.</font>";
                        }

                        string fase = "";
                        int cod_fase = Convert.ToInt16(drSelect[drEncabezado["Codigo"].ToString()].ToString().Split('-')[0]);
                        int max_fase = Convert.ToInt16(drSelect[drEncabezado["Codigo"].ToString()].ToString().Split('-')[1]);

                        int j = 1;
                        if (cod_fase > max_fase)
                        {
                            for (; j <= max_fase; j++)
                                fase += fase_ant;
                        }
                        else
                        {
                            for (; j < cod_fase; j++)
                                fase += fase_ant;
                            for (; j == cod_fase; j++)
                                fase += fase_car;
                            for (; j <= max_fase; j++)
                                fase += fase_sig;
                        }
                        //fase = "\n<font style='DISPLAY: block; TEXT-ALIGN: Center'>" + fase + "</fase>";

                        fase = "\n<font style='DISPLAY: block; TEXT-ALIGN: Center'>" + fase;
                        if (!color.ToUpper().Equals("#40A04A") && cod_fase != j && cod_fase != 0 && cod_fase < j)
                            fase += " <a href=\"javascript:__doPostBack('Grilla$_ctl" + (this.DbnetContext.dtGrilla.Rows.Count + 3) + "$_ctl4','')\"><b>?</b></a>&nbsp;</font>";
                        //  fase +=" <a href=\"javascript:__doPostBack('Grilla$_ctl3$_ctl5','')\"><img src=../librerias/img/help.jpg border=0 alt=Ayuda></a>";
                        else
                            fase += " &nbsp;&nbsp;</font>";

                        drGrilla[drEncabezado["Descripcion"].ToString()] = fase;

                    }
                    else
                    {
                        string campo = "";

                        /* Alineacion */
                        if (drEncabezado["Alineacion"].ToString() != "")
                            campo += "<font style='DISPLAY: block; TEXT-ALIGN: " + drEncabezado["Alineacion"].ToString() + "'>";

                        /* Formato */
                        if (drEncabezado["Formato"].ToString() == "")
                            campo += drSelect[drEncabezado["Codigo"].ToString()].ToString();
                        else
                            campo += dbnFormat.Numero(drSelect[drEncabezado["Codigo"].ToString()].ToString(), drEncabezado["Formato"].ToString());

                        /* Alineacion */
                        if (drEncabezado["Alineacion"].ToString() != "")
                            campo += "</font>";

                        drGrilla[drEncabezado["Descripcion"].ToString()] = campo;
                    }
                }
                this.DbnetContext.dtGrilla.Rows.Add(drGrilla);
            }
            this.DbnetContext.nroPagina = pagina;

            Grilla.DataSource = this.DbnetContext.dtGrilla;
            Grilla.CurrentPageIndex = 0;
            Grilla.DataBind();

            ActualizacionBar();
        }
        catch (Exception e)
        {   /*
            lbMensaje.Text += "Error :<br>" + e.Message + "<br>";
            MsgError(1);
             */
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Reporte DTO Excel", "VarChar", 50, "in",
                                  "pmsaj_erro", e.Message, "VarChar", 150, "in",
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
        this.barBack.Click += new System.Web.UI.ImageClickEventHandler(this.barBack_Click);
        this.barNext.Click += new System.Web.UI.ImageClickEventHandler(this.barNext_Click);
        this.barFirst.Click += new System.Web.UI.ImageClickEventHandler(this.barFirst_Click);
        this.barLast.Click += new System.Web.UI.ImageClickEventHandler(this.barLast_Click);
        this.barExcel.Click += new System.Web.UI.ImageClickEventHandler(this.barExcel_Click);
        this.txtRegPag.SelectedIndexChanged += new System.EventHandler(this.txtRegPag_SelectedIndexChanged);
        this.btBuscar.Click += new System.Web.UI.ImageClickEventHandler(this.btBuscar_Click);
        this.Fecha_desde.TextChanged += new System.EventHandler(this.Fecha_desde_TextChanged);
        this.Tipo_docu.TextChanged += new System.EventHandler(this.Tipo_docu_TextChanged);
        this.lvTipo_docu.SelectedIndexChanged += new System.EventHandler(this.lvTipo_docu_SelectedIndexChanged);
        this.Codi_pers.TextChanged += new System.EventHandler(this.Codi_pers_TextChanged);
        this.lvCodi_pers.SelectedIndexChanged += new System.EventHandler(this.lvCodi_pers_SelectedIndexChanged);
        this.Codi_sucu.TextChanged += new System.EventHandler(this.Codi_sucu_TextChanged);
        this.lvCodi_sucu.SelectedIndexChanged += new System.EventHandler(this.lvCodi_sucu_SelectedIndexChanged);
        this.ChkFoli_clie.CheckedChanged += new System.EventHandler(this.ChkFoli_clie_CheckedChanged);
        this.Grilla.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.GrillaComando);
        this.Grilla.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.GrillaOrden);
        this.Load += new System.EventHandler(this.Page_Load);

    }
    #endregion
    private void barFirst_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        barNavegacion("first");
    }
    private void barBack_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        barNavegacion("back");
    }
    private void barNext_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        barNavegacion("next");
    }
    private void barLast_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        barNavegacion("last");
    }
    private void barSearch_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        barNavegacion("search");
    }
    private void barNew_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        barNavegacion("new");
    }
    private void barNavegacion(string op)
    {
        string src = "";
        lbMensaje.Text = "";
        switch (op)
        {
            case "first":					// Primera Pagina
                buscar(1);
                break;
            case "back":					// Pagina Anterior
                if (this.DbnetContext.nroPagina > 1)
                    buscar(this.DbnetContext.nroPagina - 1);
                else
                    DbnetTool.MsgAlerta("Primera pagina", this.Page);
                break;
            case "next":					// Pagina Siguiente
                if (this.DbnetContext.nroPagina < this.DbnetContext.totPagina)
                    buscar(this.DbnetContext.nroPagina + 1);
                else
                    DbnetTool.MsgAlerta("Ultima pagina", this.Page);
                break;
            case "last":					// Ultima Pagina
                this.DbnetContext.nroPagina = this.DbnetContext.totPagina;
                buscar(this.DbnetContext.nroPagina);
                break;
            case "search":
                if (this.DbnetContext.opBusqueda == 0)
                    this.DbnetContext.opBusqueda = 1;
                else
                    this.DbnetContext.opBusqueda = 0;
                src = "barBuscar(" + this.DbnetContext.opBusqueda + ")";
                break;
            case "ant":
                try
                {
                    if (lis.dtAnterior.ToString() != "")
                    {
                        DataRow drSelect = this.DbnetContext.dtSelect.Rows[0];
                        for (int i = 0; (i < lis.dtParametrosAnterior.Rows.Count); i++)
                        {
                            DataRow dr = lis.dtParametrosAnterior.Rows[i];
                            switch (i)
                            {
                                case 0:
                                    DbnetContext.Key1 = dr[0].ToString();
                                    if (DbnetContext.Key1 != null)
                                        DbnetContext.Val1 = drSelect[DbnetContext.Key1].ToString();
                                    break;
                                case 1:
                                    DbnetContext.Key2 = dr[0].ToString();
                                    if (DbnetContext.Key2 != null)
                                        DbnetContext.Val2 = drSelect[DbnetContext.Key2].ToString();
                                    break;
                                case 2:
                                    DbnetContext.Key3 = dr[0].ToString();
                                    if (DbnetContext.Key3 != null)
                                        DbnetContext.Val3 = drSelect[DbnetContext.Key3].ToString();
                                    break;
                                case 3:
                                    DbnetContext.Key4 = dr[0].ToString();
                                    if (DbnetContext.Key4 != null)
                                        DbnetContext.Val4 = drSelect[DbnetContext.Key4].ToString();
                                    break;
                                case 4:
                                    DbnetContext.Key5 = dr[0].ToString();
                                    if (DbnetContext.Key5 != null)
                                        DbnetContext.Val5 = drSelect[DbnetContext.Key5].ToString();
                                    break;
                            }
                        }
                        src = "window.location.href=\"../dbnWeb/dbnListador.aspx?listado=" + lis.dtAnterior + "\"";
                    }
                    else
                        src = "window.location.href=\"../dbnWeb/dbnHome.aspx\"";
                }
                catch
                {
                    src = "window.location.href=\"../dbnWeb/dbnHome.aspx\"";
                }
                break;
            case "new":
                for (int i = 0; (i < lis.dtParametros.Rows.Count); i++)
                {
                    DataRow dr = lis.dtParametros.Rows[i];
                    if (i == 0) DbnetContext.Key1 = dr[0].ToString();
                    else if (i == 1) DbnetContext.Key2 = dr[0].ToString();
                    else if (i == 2) DbnetContext.Key3 = dr[0].ToString();
                    else if (i == 3) DbnetContext.Key4 = dr[0].ToString();
                    else if (i == 4) DbnetContext.Key5 = dr[0].ToString();
                }
                src = "window.location.href=\"../" + lis.dtMantenedor + "\"";
                break;
        }
        ClientScript.RegisterStartupScript(typeof(Page), "bar", "<script type=\"text/javascript\">" + src + "</script>");
    }
    private void GrillaOrden(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
    {
        string sortGrilla = "";
        for (int i = 0; i < lis.dtEncabezados.Count; i++)
        {
            if (lis.dtEncabezados[i]["Descripcion"].ToString() == e.SortExpression)
                sortGrilla = lis.dtEncabezados[i]["Codigo"].ToString();
        }
        if (sortGrilla != "")
        {
            DataView sortView = new DataView(this.DbnetContext.dtSelectInicial);
            sortView.Sort = sortGrilla;
            this.DbnetContext.dtSelect = DbnetTool.DataView_To_DataTable(sortView);
        }
        else
            this.DbnetContext.dtSelect = this.DbnetContext.dtSelectInicial;

        buscar(1);
        lbMensaje.Text = "";
    }
    private void GrillaComando(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
        string comando = e.CommandName.ToString().ToUpper();
        string sep = "\\";
        string ind_ensa = "E";	/* Indicador de entrada Salida	*/
        string dir_arch = "xml";	/* Directorio ubicacion archivo */
        string archivo = "";
        string origen = "";
        string origenXml = "";
        string cache = DbnetGlobal.Path + "cache\\";
        string pScript = "";
        string[] extension;
        bool flag_xslt = false;

        lbMensaje.Text = "";
        try
        {
            int indGrilla = e.Item.ItemIndex;
            DataView sortView = new DataView(this.DbnetContext.dtGrilla);
            int indSelect = Convert.ToInt32(sortView[indGrilla].Row["Lin"]) - 1;
            DataRow drSelect = this.DbnetContext.dtSelect.Rows[indSelect];

            switch (comando)
            {
                case "XML":
                case "PDF":
                case "PDF-M":
                case "HTML":
                case "ARHE":
                case "TRAZO":
                case "RESP1":
                case "RESP2":
                case "RESP3":
                    string query = "";
                    string cert = "";
                    bool sw = false;

                    if (!FormatoRespuesta.Checked)
                        flag_xslt = true;

                    if (comando == "PDF-M")
                    {
                        ind_ensa = drSelect["ENSA_PDF"].ToString();
                        dir_arch = drSelect["DIRE_PDF"].ToString();
                        archivo = drSelect["ARCH_PDF"].ToString().ToLower().Replace(".pdf", "_me.pdf");
                    }
                    else if (comando == "TRAZO")
                    {
                        ind_ensa = drSelect["ENSA_PDF"].ToString();
                        dir_arch = drSelect["DIRE_PDF"].ToString();
                        archivo = drSelect["ARCH_PDF"].ToString().ToLower().Replace(".pdf", "_me.trazos.pdf");
                    }
                    else if (comando == "RESP3")
                    {
                        ind_ensa = drSelect["ENSA_" + comando].ToString();
                        dir_arch = drSelect["DIRE_" + comando].ToString();
                        archivo = drSelect["ARCH_COMER"].ToString();
                        string[] nombre = archivo.Split('\\');
                        archivo = nombre[nombre.Length - 1];
                    }
                    else
                    {
                        ind_ensa = drSelect["ENSA_" + comando].ToString();
                        dir_arch = drSelect["DIRE_" + comando].ToString();
                        archivo = drSelect["ARCH_" + comando].ToString();
                    }
                    if (archivo.Length == 0)
                    {

                        lbMensaje.Text = "El Archivo no existe";
                        MsgError(1);
                        break;
                    }
                    else
                        archivo = archivo.Substring(0, archivo.Length - 4) + archivo.Substring(archivo.Length - 4, 4).ToLower();

                    string prefijo = "";
                    if (comando == "ARHE" && this.DbnetContext.TipoMonitor == "DTE")
                    {
                        if (archivo.Substring(0, 9) == "RESP_DTE_")
                        {
                            prefijo = archivo.Substring(0, 9);
                        }
                        if (archivo.Substring(0, 8) == "DTEMAIL_")
                        {
                            prefijo = archivo.Substring(0, 8);
                        }
                        if (archivo.Substring(0, 10) == "RESP_ENVI_")
                        {
                            prefijo = archivo.Substring(0, 10);
                        }
                        if (archivo.Substring(0, 6) == "ENVFIN")
                        {
                            prefijo = archivo.Substring(0, 6);
                        }
                    }

                    if (ind_ensa == "S")
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                    else
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";

                    DataTable dtSalidas = new DataTable();
                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
                        {
                            if (flag_xslt && (comando == "RESP1" || comando == "RESP2" || comando == "RESP3"))
                            {
                                archivo = archivo.Replace(".xml", ".html");
                                origen = drSalidas[0].ToString() + sep + dir_arch + sep + archivo;
                            }
                            else
                            {
                                origen = drSalidas[0].ToString() + sep + dir_arch + sep + archivo;
                            }
                        }
                        else if (dir_arch != "" && dir_arch != " ")
                        {
                            origen = dir_arch + sep + archivo;
                        }
                        else
                        {
                            extension = archivo.Split('.');
                            origen = archivo;
                            archivo = "tmp" + System.DateTime.Now.Day.ToString() +
                                              System.DateTime.Now.Hour.ToString() +
                                              System.DateTime.Now.Minute.ToString() +
                                              System.DateTime.Now.Second.ToString() +
                                              System.DateTime.Now.Millisecond.ToString() + "." + extension[1];
                        }

                        try
                        {
                            File.Copy(origen, cache + archivo, true);

                            /* Si es HTML debe copiar timbre, el logo debe estar en el directorio cache*/
                            if (comando == "HTML" && ind_ensa == "S")
                                File.Copy(origen.ToLower().Replace(".html", ".jpg"), cache + archivo.ToLower().Replace(".html", ".jpg"), true);

                            lbMensaje.Text = "";

                            sw = true;
                            pScript = "<script type=\"text/javascript\"> " +
                                      "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                      "</script>";
                            ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                        }
                        catch (Exception ex)
                        {

                            //Obtener parametro para DTE
                            string dtebd = "0";
                            query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLBD'";
                            dtebd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);

                            //Obtener parametro para DTO
                            string dtobd = "0";
                            query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLDTO'";
                            dtobd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);

                            if (this.DbnetContext.TipoMonitor == "DTO" && dtobd == "1")
                            {
                                //lbMensaje.Text += "XMLDTO: "+dtobd+"<br>";
                                string Foli_docu = drSelect["FOLI_DOCU"].ToString();
                                string Tipo_docu = drSelect["TIPO_DOCU"].ToString();
                                string Corr_envi = drSelect["CORR_ENVI"].ToString();
                                string Rutt_emis = drSelect["RUTT_EMIS"].ToString();
                                string corr_envi1 = "";
                                query = "select corr_envi1 from dte_enca_docu where codi_empr = " + DbnetContext.Codi_empr + " and tipo_docu = " + Tipo_docu + " and foli_docu = " + Foli_docu;
                                DataTable salida = new DataTable();
                                DataRow fila;
                                DbnetTool.ctrlSqlInjection(this.Page.Form);
                                salida = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                if (salida.Rows.Count > 0)
                                {
                                    fila = salida.Rows[0];
                                    corr_envi1 = fila[0].ToString();
                                }

                                string comando_bin = "egateDTE";
                                string parametros = "";
                                if (comando == "XML")
                                {
                                    comando_bin = "egateDescarga";
                                    parametros = " -te dto -ts XML -tl 3";
                                    parametros += " -empr " + DbnetContext.Codi_empr;
                                    parametros += " -corr " + Corr_envi.ToString();
                                }
                                else if (comando == "PDF" || comando == "HTML")
                                {
                                    comando_bin = "egateDTE";
                                    parametros = " -te dto -ts html -tl 3";
                                    parametros += " -empr " + DbnetContext.Codi_empr;
                                    parametros += " -tdte " + Tipo_docu.ToString();
                                    parametros += " -fdte " + Foli_docu.ToString();
                                    parametros += " -re " + Rutt_emis.ToString();
                                    parametros += " -corr " + Corr_envi.ToString();
                                }
                                lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);

                                try
                                {
                                    if (File.Exists(origen))
                                    {
                                        File.Copy(origen, cache + archivo, true);
                                        /* Si es HTML debe copiar timbre, el logo debe estar en el directorio cache*/
                                        if (comando == "HTML" && ind_ensa == "S")
                                            File.Copy(origen.ToLower().Replace(".html", ".jpg"), cache + archivo.ToLower().Replace(".html", ".jpg"), true);

                                        lbMensaje.Text = "";
                                        sw = true;
                                        pScript = "<script type=\"text/javascript\"> " +
                                            "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                            "</script>";
                                        ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                                    }
                                    else
                                    {
                                        lbMensaje.Text = "No se genero el archivo XML de DTO." + origen + "<br>";
                                        MsgError(1);
                                    }
                                }
                                catch (Exception eex)
                                {
                                    //lbMensaje.Text += eex.Message + "<br>";
                                    //MsgError(1);
                                    DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Contabilización de DTOs", "VarChar", 50, "in",
                                  "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                  "pbin_erro", "WEB", "VarChar", 50, "in",
                                  "p_mensaje", "", "VarChar", 200, "out");

                                    chkDespliega.Checked = true;
                                    lbMensaje.Enabled = true;
                                    lbMensaje.Visible = true;
                                    lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                    lbEx.Text = sp.return_String("p_mensaje");
                                }

                                MsgError(1);
                            }
                            /***********************************/
                            else
                            {
                                //lbMensaje.Text += ex.Message;
                                //MsgError(1);
                                DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Contabilización de DTOs", "VarChar", 50, "in",
                                  "pmsaj_erro", ex.Message, "VarChar", 150, "in",
                                  "pbin_erro", "WEB", "VarChar", 50, "in",
                                  "p_mensaje", "", "VarChar", 200, "out");

                                chkDespliega.Checked = true;
                                lbMensaje.Enabled = true;
                                lbMensaje.Visible = true;
                                lbEx.Text = sp.return_String("p_mensaje");
                            }
                        }
                    }
                    break;
                case "DETA":
                    //Obtener parametro para DTE
                    string xmlbd = "0";
                    query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLBD'";
                    xmlbd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
                    //else xmlbd = "0";						
                    if (xmlbd == "1")
                    {

                        sw = false;
                        query = "";
                        ind_ensa = drSelect["ENSA_XML"].ToString();
                        dir_arch = "xml";//drSelect["DIRE_" + comando].ToString();
                        archivo = drSelect["ARCH_XML"].ToString();

                        if (ind_ensa == "S")
                            query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                        else
                            query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";

                        dtSalidas = new DataTable();
                        dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                        for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                        {
                            //DbnetTool.MsgAlerta("despues for",this.Page);
                            DataRow drSalidas = dtSalidas.Rows[i];
                            if (dir_arch == "xml")
                            {
                                origen = drSalidas[0].ToString() + sep + dir_arch + sep + archivo;
                            }
                            else if (dir_arch != "" && dir_arch != " ")
                            {
                                origen = dir_arch + sep + archivo;
                            }
                            else
                            {
                                extension = archivo.Split('.');
                                origen = archivo;
                                archivo = "tmp" + System.DateTime.Now.Day.ToString() +
                                    System.DateTime.Now.Hour.ToString() +
                                    System.DateTime.Now.Minute.ToString() +
                                    System.DateTime.Now.Second.ToString() +
                                    System.DateTime.Now.Millisecond.ToString() + "." + extension[1];
                            }

                            string comando_bin = "";
                            string parametros = "";
                            string Foli_docu = drSelect["FOLI_DOCU"].ToString();
                            string Tipo_docu = drSelect["TIPO_DOCU"].ToString();

                            comando_bin = "egateDTE";
                            parametros += " -te bd -ts xml -tl 3 -empr " + DbnetContext.Codi_empr;
                            parametros += " -tdte " + Tipo_docu.ToString();
                            parametros += " -fdte " + Foli_docu.ToString();
                            parametros += " -s " + origen;
                            try
                            {	//Generar xml
                                lbMensaje.Text = DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                            }
                            catch (Exception ex)
                            {
                                //lbMensaje.Text += ex.Message + "<br>";
                                //MsgError(1);
                                DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Contabilización de DTOs", "VarChar", 50, "in",
                                  "pmsaj_erro", ex.Message, "VarChar", 150, "in",
                                  "pbin_erro", "WEB", "VarChar", 50, "in",
                                  "p_mensaje", "", "VarChar", 200, "out");

                                chkDespliega.Checked = true;
                                lbMensaje.Enabled = true;
                                lbMensaje.Visible = true;
                                lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                lbEx.Text = sp.return_String("p_mensaje");
                            }

                            comando_bin = "procesaXslt.bat ";
                            parametros = " %EGATE_HOME%";
                            if (this.DbnetContext.TipoMonitor == "DTE")
                            {
                                parametros += sep + "config" + sep + "par" + sep + "dte.xslt ";
                            }
                            else if (this.DbnetContext.TipoMonitor == "DTO")
                            {
                                parametros += sep + "config" + sep + "par" + sep + "dto.xslt ";
                            }
                            parametros += drSalidas[0].ToString() + sep + dir_arch + sep + archivo + " ";
                            parametros += drSalidas[0].ToString() + sep + dir_arch + sep + archivo.Substring(0, archivo.Length - 4);
                            try
                            {
                                lbMensaje.Text = lbMensaje.Text + DbnetTool.Ejecuta_Proceso_Xslt(DbnetContext, comando_bin, parametros);
                            }
                            catch (Exception ex)
                            {
                                //lbMensaje.Text += ex.Message + "<br>";
                                //MsgError(1);
                                DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Contabilización de DTOs", "VarChar", 50, "in",
                                  "pmsaj_erro", ex.Message, "VarChar", 150, "in",
                                  "pbin_erro", "WEB", "VarChar", 50, "in",
                                  "p_mensaje", "", "VarChar", 200, "out");

                                chkDespliega.Checked = true;
                                lbMensaje.Enabled = true;
                                lbMensaje.Visible = true;
                                lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                lbEx.Text = sp.return_String("p_mensaje");
                            }
                            try
                            {
                                File.Copy(origen.Substring(0, origen.Length - 4) + ".html", cache + archivo.Substring(0, archivo.Length - 4) + ".html", true);

                                lbMensaje.Text = "";
                                sw = true;

                                pScript = "<script type=\"text/javascript\"> " +
                                    "window.open(\"" + "../cache/" + archivo.Substring(0, archivo.Length - 4) + ".html\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                    "</script>";
                                ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                            }
                            catch (Exception ex)
                            {
                                //lbMensaje.Text += ex.Message + " FIN" + "<br>";
                                //MsgError(1);
                                DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Contabilización de DTOs", "VarChar", 50, "in",
                                  "pmsaj_erro", ex.Message, "VarChar", 150, "in",
                                  "pbin_erro", "WEB", "VarChar", 50, "in",
                                  "p_mensaje", "", "VarChar", 200, "out");

                                chkDespliega.Checked = true;
                                lbMensaje.Enabled = true;
                                lbMensaje.Visible = true;
                                lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                lbEx.Text = sp.return_String("p_mensaje");
                            }
                            sw = true;
                        }
                        MsgError(1);
                    }
                    else
                    {
                        for (int i = 0; (i < lis.dtParametros.Rows.Count); i++)
                        {
                            DataRow dr = lis.dtParametros.Rows[i];
                            switch (i)
                            {
                                case 0:
                                    DbnetContext.Key1 = dr[0].ToString();
                                    if (DbnetContext.Key1 != null)
                                        DbnetContext.Val1 = drSelect[DbnetContext.Key1].ToString();
                                    break;
                                case 1:
                                    DbnetContext.Key2 = dr[0].ToString();
                                    if (DbnetContext.Key2 != null)
                                        DbnetContext.Val2 = drSelect[DbnetContext.Key2].ToString();
                                    break;
                                case 2:
                                    DbnetContext.Key3 = dr[0].ToString();
                                    if (DbnetContext.Key3 != null)
                                        DbnetContext.Val3 = drSelect[DbnetContext.Key3].ToString();
                                    break;
                                case 3:
                                    DbnetContext.Key4 = dr[0].ToString();
                                    if (DbnetContext.Key4 != null)
                                        DbnetContext.Val4 = drSelect[DbnetContext.Key4].ToString();
                                    break;
                                case 4:
                                    DbnetContext.Key5 = dr[0].ToString();
                                    if (DbnetContext.Key5 != null)
                                        DbnetContext.Val5 = drSelect[DbnetContext.Key5].ToString();
                                    break;
                            }
                        }

                        pScript = "<script type=\"text/javascript\"> " +
                            "window.open(\"../" + lis.dtMantenedor + "\" , \"_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=no\");" +
                            "</script>";
                        ClientScript.RegisterStartupScript(typeof(Page), "PaginaDetalle", pScript);
                    }
                    break;
                case "HELP":
                    DbnetTool.MsgError(drSelect["FASE_HELP"].ToString(), this.Page);
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            //lbMensaje.Text = ex.Message;
            //MsgError(1);
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                  "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                  "pproc_erro", "Menu: Contabilización de DTOs", "VarChar", 50, "in",
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
    private void btBuscar_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        this.DbnetContext.regpag = Convert.ToInt16(txtRegPag.SelectedValue);
        if (lis.Status)
        {
            sw_buscar = true;
            buscar();
        }
    }
    private void Fecha_desde_TextChanged(object sender, System.EventArgs e)
    {

    }

    private void lvTipo_docu_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Tipo_docu.Text = lvTipo_docu.SelectedValue;
    }

    private void Tipo_docu_TextChanged(object sender, System.EventArgs e)
    {
        try
        {
            lvTipo_docu.Query = qryTipo_docu;
            lvTipo_docu.Rescata(DbnetContext.dbConnection);
            lvTipo_docu.Selecciona(Tipo_docu.Text);
        }
        catch
        {
            DbnetTool.MsgError("Tipo de documentos no existe", this.Page);
            lvTipo_docu.SelectedIndex = 0;
        }
    }

    private void Codi_pers_TextChanged(object sender, System.EventArgs e)
    {
        try
        {
            lvCodi_pers.Query = qryCodi_pers;
            lvCodi_pers.Rescata(DbnetContext.dbConnection);
            lvCodi_pers.Selecciona(Codi_pers.Text);
        }
        catch
        {
            //DbnetTool.MsgError("Persona no existe",this.Page);
            //Codi_pers.Text = "";
            //lvCodi_pers.SelectedIndex = 0;
        }
    }

    private void lvCodi_pers_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Codi_pers.Text = lvCodi_pers.SelectedValue;
    }

    private void Codi_sucu_TextChanged(object sender, System.EventArgs e)
    {
        try
        {
            lvCodi_sucu.Query = qryCodi_sucu;
            lvCodi_sucu.Rescata(DbnetContext.dbConnection);
            lvCodi_sucu.Selecciona(Codi_sucu.Text);
        }
        catch
        {
            //DbnetTool.MsgError("Centro Costo no existe",this.Page);
            //Codi_sucu.Text = "";
            //lvCodi_sucu.SelectedIndex = 0;
        }
    }

    private void lvCodi_sucu_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Codi_sucu.Text = lvCodi_sucu.SelectedValue;
    }

    private void txtRegPag_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        btBuscar_Click(null, null);
    }

    private void barExcel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        // Export the details of specified columns
        try
        {

            DateTime fechaHoy = DateTime.Today;
            string fechaAc = fechaHoy.ToString("d");
            string dd = fechaAc.Substring(0, 2);
            string mm = fechaAc.Substring(3, 2);
            string aaaa = fechaAc.Substring(6, 4);
            

            string ruta = DbnetGlobal.Path + "sql\\";
            string archivoExcel = ruta + "PROV" + aaaa + mm + dd + ".csv";
            string nombre = "PROV" + aaaa + mm + dd + ".csv"; ;
            string archivoEjecutar = ruta + "consultaDoc.sql";

            string codiPers = Codi_pers.Text;
            string tipoDocu = Tipo_docu.Text;
            string foliDocu1 = Foli_docu1.Text;
            string foliDocu2 = Foli_docu2.Text;
            string fechDesd = Fecha_desde.Text;
            string fechHast = Fecha_hasta.Text;

            if (codiPers == "0")
                codiPers = "%";

            if (tipoDocu == "0")
                tipoDocu = "%";

            if (foliDocu1 == "0" || foliDocu1 == "")
                foliDocu1 = "0";

            if (foliDocu2 == "0" || foliDocu2 == "")
                foliDocu2 = "9999999999999";

            if (fechDesd == "0" || fechDesd == "")
                fechDesd = "0000-00-00";

            if (fechHast == "0" || fechHast == "")
                fechHast = "9999-99-99";
            
            string comandos = "";
            comandos = "sqlplus ";
            
            string parametros = "@" + archivoEjecutar + " " + archivoExcel + " " + DbnetContext.Codi_empr + " " + codiPers + " " + tipoDocu+ " ERA " + foliDocu1 + " " + foliDocu2 + " " + fechDesd + " " + fechHast;
            lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comandos, parametros);

            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + nombre);
            Response.TransmitFile(archivoExcel);
            Response.End();

        }
        catch (Exception Ex)
        {
            lbMensaje.Text += Ex.Message;
            MsgError(1);
        }
    }


    private void ChkFoli_clie_CheckedChanged(object sender, System.EventArgs e)
    {

        if (ChkFoli_clie.Checked)
        {
            Foli_erp1.Visible = true;
            Foli_erp2.Visible = true;
            Foli_erp1.Text = "1";
            Foli_erp2.Text = "9999999999";
            Foli_docu1.Visible = false;
            Foli_docu2.Visible = false;
        }
        else
        {
            Foli_erp1.Visible = false;
            Foli_erp2.Visible = false;
            Foli_docu1.Text = "1";
            Foli_docu2.Text = "9999999999";
            Foli_docu1.Visible = true;
            Foli_docu2.Visible = true;
        }
    }
    protected void lvCodi_persLoad_Click(object sender, ImageClickEventArgs e)
    {
        lvCodi_pers.Query = qryCodi_pers;
        lvCodi_pers.Rescata(DbnetContext.dbConnection);
    }

    protected void Grilla_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    //protected void btnImprimirSel_Click(object sender, EventArgs e)
    //{
    //    string query;
    //    string a = txtPasaCuenta.Text.Replace("\t", "").Replace(" ", "").Replace(".0", "");
    //    string b = txtPasaImp.Text.Replace("\t", "").Replace(" ", "").Replace(".0", "");
    //    //cuenta
    //    string[] cadena = a.Split(new string[] { "#$#" }, StringSplitOptions.None);


    //    for (int x = 0; x < cadena.Length - 1; x++)
    //    {
    //        if (cadena[x].Length != 0)
    //        {
    //            string[] item = cadena[x].Split('|');
    //            string Rut;
    //            Rut = item[2].Substring(0, item[2].Length - (item[2].Substring(item[2].Length - 2, 2).Length));
    //            if (Rut.Length < 9)
    //            {
    //                int w = 9 - Rut.Length;
    //                for (int y = 0; y < w; y++)
    //                {
    //                    Rut = "0" + Rut;
    //                }
    //            }
    //            string TipoDocu;
    //            TipoDocu = item[0];
    //            if (TipoDocu.Length < 3)
    //            {
    //                int w = 3 - TipoDocu.Length;
    //                for (int y = 0; y < w; y++)
    //                {
    //                    TipoDocu = "0" + TipoDocu;
    //                }
    //            }

    //            string Foli_docu;
    //            Foli_docu = item[1];
    //            if (Foli_docu.Length < 10)
    //            {
    //                int w = 10 - Foli_docu.Length;
    //                for (int y = 0; y < w; y++)
    //                {
    //                    Foli_docu = "0" + Foli_docu;
    //                }
    //            }
                  
    //            query = "update dto_enca_docu_p" +
    //                          " set indi_conta='N' where " +
    //                          " corr_docu=" + item[3];

    //            DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

    //            query = "select valo_paem from para_empr where codi_paem = 'CXML' and codi_empr = " + DbnetContext.Codi_empr + "";
    //            string pathDestino = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
               
    //            if (pathDestino != "")
    //            {

    //                query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";
    //                string pathArch = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
    //                string ArchivoR = "R" + Rut + "T" + TipoDocu + "F" + Foli_docu + ".xml";
    //                string ArchivoE = "E" + Rut + "T" + TipoDocu + "F" + Foli_docu + ".xml";
    //                string Archivo;
    //                if (File.Exists(pathArch + "\\xml_con\\" + ArchivoR))
    //                {
    //                    Archivo = ArchivoR;
    //                }
    //                else
    //                {
    //                    Archivo = ArchivoE;
    //                }
                   
                    
    //                if (pathArch != "" && File.Exists(pathArch +"\\xml_con\\" + Archivo))
    //                {
    //                    File.Copy(pathArch +"\\xml_con\\" + Archivo,pathDestino +"\\"+ Archivo, true);
    //                }                    

    //            }
    //        }

    //    }


    //        cadena = b.Split(new string[] { "#$#" }, StringSplitOptions.None);
    //        for (int x = 0; x < cadena.Length - 1; x++)
    //        {
    //            if (cadena[x].Length != 0)
    //            {
    //                string[] item = cadena[x].Split('|');

    //                string proceso = "egateDTE";
    //                string parametros = " -empr " + DbnetContext.Codi_empr;
    //                parametros += " -h " + DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name='EGATE_HOME'");
    //                parametros += " -tl 3 ";
    //                parametros += " -tdte " + item[0];
    //                parametros += " -fdte " + item[1];
    //                parametros += " -te dto -ts html -p ";
    //                parametros += " -re " + item[2].Substring(0, item[2].IndexOf('-'));
    //                parametros += " -impr " + DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name='EGATE_IMPR_DTOWEB'");


    //                //lbMensaje.Text = DbnetTool.Ejecuta_Proceso_Espera(DbnetContext,proceso,parametros);
    //                //MsgError(2);
                   

    //                if (DbnetGlobal.Base_dato.ToLower().CompareTo("sqlserver") == 0)
    //                {

    //                    qryins = "Insert into se_pipe(pipe_stat, pipe_cmd, pipe_codi_usua)" +
    //                            "values ('P' ," +
    //                            "'" + proceso + " " + parametros + "'," +
    //                            "'" + DbnetContext.Codi_usua + "')";
    //                }
    //                else
    //                {
    //                    qryins = "Insert into se_pipe(pipe_id, pipe_stat, pipe_cmd, pipe_codi_usua)" +
    //                            "values (seq_se_pipe.nextval ,'P' ," +
    //                            "'" + proceso + " " + parametros + "'," +
    //                            "'" + DbnetContext.Codi_usua + "')";
    //                }
    //                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, qryins);

    //            }
    //        }

    //    }
    }