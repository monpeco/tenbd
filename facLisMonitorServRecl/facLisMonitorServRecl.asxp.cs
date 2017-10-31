using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Util;
using System.Xml;
using System.Xml.Xsl;
using DbnetWebLibrary;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Script.Services;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Data.SqlClient;//

public partial class facLisMonitorServicio : DbnetPage
{
    private DbnetListadorXML lis;
    private string query = "";
    private string archivoXslt;
    private string basedir;
    private string destinoHTML;
    private static string lib = "librerias/xml-conf/";
    private static string qryCodi_pers = "";
    private static string qryTipo_docu = "";
    private static string qryCodi_sucu = "";
    private static string tipoDocumento = "";
    public static string codi_emex_auto = "";
    public static string DTO_COME = "";
    public static int codi_empr_auto;
    public bool par_trazos = false;	//Lapiz Digital
    public bool par_folio_erp = false; //Folio ERP
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
        mostrar(opAyuda, opError);
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
        if (Grilla.Items.Count != 0)
        {
            positiongrid1.Visible = true;
            positiongrid2.Visible = true;
        }
        else
        {
            positiongrid1.Visible = false;
            positiongrid2.Visible = false;
        }

        if (Request.Params.AllKeys.Contains("par2"))
        {
            if (Request.Params["par2"].ToUpper() == "DTO_COME")
                DTO_COME = "1";
            else
                DTO_COME = "0";
        }
        else
            DTO_COME = "0";
        if (Request.Params["par1"].ToUpper().Equals("DTE"))
        {
            listresp.Visible = true;
            divRespuesta.Visible = true;
            rdEmision.Visible = false;
            rdRecepcion.Visible = false;
            DbnetWebLabel20.Visible = false;
            divREME.Style.Add("display", "none");
            divDllAccion.Style.Add("display", "none");
            divEstaReme.Style.Add("display", "none");

            // OT 9376978 - 10-05-2017|am
            List<EstadosComer> oListaEventos = new List<EstadosComer>();
            oListaEventos.Add(new EstadosComer { valor = "CED", texto = "Consulta de evento del DTE" });
            oListaEventos.Add(new EstadosComer { valor = "CCD", texto = "Consulta de Cedible" });

            ddlAccionReclamo.DataSource = oListaEventos;
            ddlAccionReclamo.DataValueField = "valor";
            ddlAccionReclamo.DataTextField = "texto";
            ddlAccionReclamo.DataBind();
			
			dllRespuestaReclamo.Visible = false;
	
        }
        else if (Request.Params["par1"].ToUpper().Equals("DTO"))
        {
            RespDTE.Style.Add("display", "none");
            divEstadoCliente.Style.Add("display", "none");
            listresp.Visible = false;
            divRespuesta.Visible = true;
            rdEmision.Visible = true;
            rdRecepcion.Visible = true;
            DbnetWebLabel20.Visible = true;
            lbServicio.Visible = false;
            lv_Servicio.Visible = false;
            btnServicio.Visible = false;
            FormatoRespuesta.Checked = true;
            ddlEstaEnvi.Visible = false;
            lbEstaEnvi.Visible = false;
            lblEstadoSii.InnerText = "Estado Documento";
            divEstasii.Style.Add("height", "21px");
            positiongrid1.Style.Add("width", "262px");
            positiongrid2.Style.Add("width", "648px");
            List<EstadosComer> oLista = new List<EstadosComer>();
            oLista.Add(new EstadosComer { valor = "RME", texto = "Generar Recibo de Mercadería según Ley 19.983" });
            oLista.Add(new EstadosComer { valor = "APR", texto = "Aprobar Comercialmente" });
            oLista.Add(new EstadosComer { valor = "ARE", texto = "Aprobar con Reparos comercialmente" });
            oLista.Add(new EstadosComer { valor = "REC", texto = "Rechazar comercialmente" });
			
			// OT 9376978 - 27-04-2017|am
            List<EstadosComer> oListaEventos = new List<EstadosComer>();
            oListaEventos.Add(new EstadosComer { valor = "---", texto = "Seleccione Evento SII" });
            oListaEventos.Add(new EstadosComer { valor = "ACD", texto = "Acepta de Contenido del Documento" });
            oListaEventos.Add(new EstadosComer { valor = "ERM", texto = "Otorga Recibo de Mercadeías o Servicios" });
            oListaEventos.Add(new EstadosComer { valor = "RCD", texto = "Reclama Contenido del Documento" });
            oListaEventos.Add(new EstadosComer { valor = "RFP", texto = "Reclama Falta Parcial de Mercaderías" });
            oListaEventos.Add(new EstadosComer { valor = "RFT", texto = "Reclama Falta Total de Mercaderías" });
            oListaEventos.Add(new EstadosComer { valor = "FRS", texto = "Solicitar Fecha de Recepción en SII" });
			// OT 9376978 - 27-04-2017|am
			
			dllRespuestaReclamoDTE.Visible = false;
			
            if (DTO_COME == "1")
            {
                oLista = oLista.Where(x => !x.valor.Equals("RME")).ToList();
                ddlAccion.DataSource = oLista;
                ddlAccion.DataValueField = "valor";
                ddlAccion.DataTextField = "texto";
                ddlAccion.DataBind();
                divEstadoSii.Visible = false;
                divEstaReme.Visible = false;
				
				// OT 9376978 - 27-04-2017|am
                oListaEventos = oListaEventos.Where(x => !x.valor.Equals("ACD")).ToList();
                ddlAccionReclamo.DataSource = oListaEventos;
                ddlAccionReclamo.DataValueField = "valor";
                ddlAccionReclamo.DataTextField = "texto";
                ddlAccionReclamo.DataBind();
                divEstadoSii.Visible = false;
                divEstaReme.Visible = false;
				// OT 9376978 - 27-04-2017|am

            }
            else
            {
                ddlAccion.DataSource = oLista;
                ddlAccion.DataValueField = "valor";
                ddlAccion.DataTextField = "texto";
                ddlAccion.DataBind();
				
				// OT 9376978 - 27-04-2017|am
                ddlAccionReclamo.DataSource = oListaEventos;
                ddlAccionReclamo.DataValueField = "valor";
                ddlAccionReclamo.DataTextField = "texto";
                ddlAccionReclamo.DataBind();
				// OT 9376978 - 27-04-2017|am
            }
        }

       

        basedir = System.Web.HttpRuntime.AppDomainAppPath;

        // Carga el contexto de la session
        DbnetContext = (DbnetSesion)Session["contexto"];
        Session["coneccion"] = DbnetContext.dbConnection;
        codi_emex_auto = DbnetContext.Codi_emex;
        codi_empr_auto = DbnetContext.Codi_empr;
        Session["tipomonitor"] = DbnetContext.TipoMonitor.ToString();

        lis = (DbnetListadorXML)Session["monitor"];
        // Introducir aquí el código de usuario para inicializar la página
        if (!IsPostBack)
        {
            this.DbnetContext.nroPagina = 1;
            this.DbnetContext.regpag = 0;
            this.DbnetContext.totRegistro = 0;
            this.DbnetContext.totPagina = 1;
            Fecha_desde.Focus();
            string p_listado = "";
            tipoDocumento = Request.Params["par1"];
            Session.Remove("oListaMonitor");
            Session.Remove("strCambio");
            if (tipoDocumento.ToString().Equals("DTO"))
            {
                lv_Servicio.Visible = false;
                btnServicio.Visible = false;
                lbServicio.Visible = false;
            }
            cmbUsuarios.Enabled = DbnetTool.SelectInto(DbnetContext.dbConnection, "select count(codi_usua) from usua_sist where codi_usua='" + DbnetContext.Codi_usua.ToString() + "' and usua_filt='N'") == "1" ? true : false;
            if (cmbUsuarios.Enabled)
            {
                cmbUsuarios.Query = "select codi_usua , nomb_usua from usua_sist where codi_emex='" + DbnetContext.Codi_emex + "' union select 'T','- Todos -' from dual order by 2";
                cmbUsuarios.Rescata(DbnetContext.dbConnection);
                cmbUsuarios.SelectedIndex = 0;
            }
            else
            {
                cmbUsuarios.Query = "Select codi_usua,nomb_usua from usua_sist where codi_usua='" + DbnetContext.Codi_usua + "' and codi_emex = '" + DbnetContext.Codi_emex + "' order by nomb_usua ";
                cmbUsuarios.Rescata(DbnetContext.dbConnection);
                cmbUsuarios.Selecciona(DbnetContext.Codi_usua.ToString());
            }

            Habilita(Request.Params["par1"]);
            switch (Request.Params["par1"])
            {
                case "DTE":
                    this.DbnetContext.TipoMonitor = "DTE";
                    p_listado = "fac-monDTE50-reclamo";
                    lbCodi_pers.Text = "Receptor";
                    break;
                case "DTE_ESTA":
                    this.DbnetContext.TipoMonitor = "DTE";
                    p_listado = "fac-monDTEEstados";
                    lbCodi_pers.Text = "Receptor";
                    break;
                case "DTE_TOTAL":
                    this.DbnetContext.TipoMonitor = "DTE";
                    p_listado = "fac-monDTETotales";
                    lbCodi_pers.Text = "Receptor";
                    break;
                case "DTO":
                    this.DbnetContext.TipoMonitor = "DTO";
                    p_listado = "fac-monDTO50-reclamo";
                    lbCodi_pers.Text = "Emisor";
                    break;
                case "DTO_ESTA":
                    this.DbnetContext.TipoMonitor = "DTO";
                    p_listado = "fac-monDTOEstados";
                    lbCodi_pers.Text = "Receptor";
                    break;
                case "DTO_TOTAL":
                    this.DbnetContext.TipoMonitor = "DTO";
                    p_listado = "fac-monDTOTotales";
                    lbCodi_pers.Text = "Receptor";
                    break;
            }

            if (DTO_COME == "1")
                p_listado = "fac-monDTO50_COME";
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
                lbTitulo.Text = lis.dtTitulo;
                Grilla.Width = lis.dtAncho;
                Grilla.PageSize = this.DbnetContext.regpag;
                ChkFoli_clie.Visible = false;
                if (this.DbnetContext.TipoMonitor == "DTE")
                {
                    if (DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name = 'EGATE_FOLI_CLIE'") == "1")
                        ChkFoli_clie.Visible = true;
                }
                else
                {
                    if (DTO_COME != "1")
                        Grilla.Columns[10].Visible = true;
                }

                this.DbnetContext.dtGrilla = new DataTable();

                qryCodi_pers = "Select codi_pers, " + DbnetContext.Auxdbo + "initcap(nomb_pers) from personas union select '0',' Todos' from dual order by 2";
                if (cmbUsuarios.Enabled)
                { qryTipo_docu = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec='S' union select 0,' Todos' from dual order by 2"; }
                else
                { qryTipo_docu = "Select a.tipo_docu, " + DbnetContext.Auxdbo + "initcap(b.desc_tido) from dte_usua_docu a , dte_tipo_docu b where a.codi_usua='" + DbnetContext.Codi_usua + "' and b.tipo_docu=a.tipo_docu and b.docu_elec='S' union select 0,'[Seleccione]' from dual order by 2"; }
                qryCodi_sucu = "Select codi_ofic, " + DbnetContext.Auxdbo + "initcap(nomb_ofic) from oficina where codi_empr='" + DbnetContext.Codi_empr + "' union select '0',' Todos' from dual order by 1";

                lvCodi_sucu.Query = qryCodi_sucu;
                lvCodi_sucu.Rescata(DbnetContext.dbConnection);

                lvTipo_docu.Query = qryTipo_docu;
                lvTipo_docu.Rescata(DbnetContext.dbConnection);

                Tipo_docu.Text = "0";
                lvTipo_docu.Selecciona(Tipo_docu.Text);
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
                { fecha_desde = DateTime.Today.AddDays(-7); }
                DateTime fecha_hasta = DateTime.Today;
                Fecha_desde.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                Fecha_hasta.Text = DateTime.Now.ToString("yyyy-MM-dd");

                try
                { Session.Add("monitor", lis); }
                catch
                { Session["monitor"] = lis; }
            }
            else
            { DbnetTool.MsgError("Definicion de Listado no existe", this.Page); }
            cmbEstados_SelectedIndexChanged(null, null);
        }
        ActualizacionBar();
    }
    private void Habilita(string pTipo)
    {
        //Definicion de parametros que habilitan funcionalidad 
        if (DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name = 'EGATE_FOLI_CLIE'") == "1")
            par_folio_erp = true;
        else
            par_folio_erp = false;
        if (DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name = 'EGATE_ANOTO_DIAS'") != "")
            par_trazos = true;
        else
            par_trazos = false;

        // Habilitacion o Deshabilitacion de Columnas 
        if (pTipo == "DTE")
        {
            Grilla2.Columns[12].Visible = true;// Serv
            Grilla2.Columns[11].Visible = true;	// Boton XML
            Grilla2.Columns[10].Visible = true;	// Boton PDF
            Grilla2.Columns[9].Visible = true;// Boton PDF-Merito
            Grilla2.Columns[8].Visible = DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name = 'EGATE_HABI_ADJU'").Equals("1") ? true : false;// Boton Adjunto
            Grilla2.Columns[7].Visible = false;// Boton HTML
            Grilla2.Columns[6].Visible = true;// Boton Archivo
            Grilla2.Columns[5].Visible = true;// Boton Ayuda
            Grilla2.Columns[4].Visible = true;// Boton Detalle
            Grilla2.Columns[3].Visible = false;  // Mail
            Grilla2.Columns[2].Visible = true;// Lin
            Grilla2.Columns[1].Visible = true;// Tipo
            Grilla2.Columns[0].Visible = true;  // Folio
            if (par_folio_erp == true)			// Folio ERP
            {
                ChkFoli_clie.Visible = true;
                Grilla.Columns[0].Visible = true;
            }
            else
            {
                ChkFoli_clie.Visible = false;
                Grilla.Columns[0].Visible = false;
            }
            Grilla.Columns[1].Visible = true;  // Rut
            Grilla.Columns[2].Visible = true;  // Tipo Contribuyente
            Grilla.Columns[3].Visible = true;  // Nombre
            Grilla.Columns[4].Visible = true;  // Total
            Grilla.Columns[5].Visible = true;  // Emision
            Grilla.Columns[6].Visible = true;  // Carga
            Grilla.Columns[7].Visible = true;  // Envio SII
            Grilla.Columns[8].Visible = true;  // Respuesta SII
            Grilla.Columns[9].Visible = true;  // Envio Contribuyente
            Grilla.Columns[10].Visible = false; // Recepcion DTE
            Grilla.Columns[11].Visible = true;  // Resp.Tecnica
            Grilla.Columns[12].Visible = true;  // Boton Resp.Tecnica
            if (par_trazos == true)				// Boton PDF-Trazo
                Grilla.Columns[13].Visible = false;
            Grilla.Columns[14].Visible = true;  // Resp.Comercial
            Grilla.Columns[15].Visible = true;  // Boton Resp.Comercial
            Grilla.Columns[16].Visible = false;  // Fecha Recibo de Mercadería
            Grilla.Columns[17].Visible = true;  // Recibo de Mercadería
            Grilla.Columns[18].Visible = true;  // botón Recibo de Mercadería
            Grilla.Columns[19].Visible = true;  // Estado DTE
            Grilla.Columns[20].Visible = true;  // Estado Envio
            Grilla.Columns[21].Visible = true;  // Corr. Envio
            Grilla.Columns[22].Visible = true;  // Track-ID
            Grilla.Columns[23].Visible = true;  // Usuario
            Grilla.Columns[27].Visible = false;  // Evento Reclamo OT 9376978 10-05-2017|AM
            Grilla.Columns[28].Visible = false;  // Usuario Reclamo
            Grilla.Columns[29].Visible = false;  // Fecha Reclamo
            Grilla.Columns[30].Visible = false;  // Código respuesta SII
            Grilla.Columns[31].Visible = false;  // Fecha recepción SII
            Grilla.Columns[32].Visible = false;  // Fecha recepción Suite
            Grilla.Columns[33].Visible = true;  // Evento Reclamo (DTE)
            Grilla.Columns[34].Visible = true;  // Es Cedible (DTE)
            cmbUsuarios.Visible = true;
            lblUsuarios.Visible = true;
        }
        else if (pTipo == "DTO")
        {
            Grilla2.Columns[12].Visible = true; // Servicio
            Grilla2.Columns[11].Visible = true;	// Boton XML
            Grilla2.Columns[10].Visible = true;	// Boton PDF
            Grilla2.Columns[9].Visible = false;	// Boton PDF-Merito
            Grilla2.Columns[8].Visible = false; // Boton Adjunto
            Grilla2.Columns[7].Visible = false; // Boton HTML
            Grilla2.Columns[6].Visible = true;  // Boton Archivo
            Grilla2.Columns[5].Visible = true;  // Boton Ayuda
            Grilla2.Columns[4].Visible = true;  // Boton Detalle
            Grilla2.Columns[3].Visible = (DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name = 'EGATE_RECE_MAIL'") == "S") ? true : false;  //Boton Mail
            Grilla2.Columns[2].Visible = true;  // Lin
            Grilla2.Columns[1].Visible = true;  // Tipo
            Grilla2.Columns[0].Visible = true;  // Folio

            Grilla.Columns[0].Visible = false;  // Folio ERP
            Grilla.Columns[1].Visible = true;// Rut
            Grilla.Columns[2].Visible = true;// Tipo Contyribyente
            Grilla.Columns[3].Visible = false;  // Nombre
            Grilla.Columns[4].Visible = true;// Total
            Grilla.Columns[5].Visible = true;// Emision
            Grilla.Columns[6].Visible = false;  // Carga
            Grilla.Columns[7].Visible = false;  // Envio SII
            Grilla.Columns[8].Visible = false;  // Respuesta SII
            Grilla.Columns[9].Visible = false;  // Envio Contribuyente
            Grilla.Columns[12].Visible = false; // Boton Resp.Tecnica
            Grilla.Columns[13].Visible = false; // Boton PDF-Trazo
            Grilla.Columns[15].Visible = false; // Boton Resp.Comercial
            Grilla.Columns[19].Visible = true;  // Estado DTE
            Grilla.Columns[20].Visible = false; // Estado Envio
            Grilla.Columns[21].Visible = false; // Corr. Envio
            Grilla.Columns[22].Visible = false; // Track-ID
            Grilla.Columns[23].Visible = false; // Usuario
            Grilla.Columns[27].Visible = true;  // Evento Reclamo OT 9376978 10-05-2017|AM
            Grilla.Columns[28].Visible = true;  // Usuario Reclamo 
            Grilla.Columns[29].Visible = true;  // Fecha Reclamo
            Grilla.Columns[30].Visible = true;  // Código respuesta SII
            Grilla.Columns[31].Visible = true;  // Fecha recepción SII
            Grilla.Columns[32].Visible = true;  // Fecha recepción Suite
            Grilla.Columns[33].Visible = false;  // Evento Reclamo (DTE)
            Grilla.Columns[34].Visible = false;  // Es Cedible (DTE)
            cmbUsuarios.Visible = false;
            lblUsuarios.Visible = false;
            if (DTO_COME == "1")
            {
                Grilla.Columns[10].Visible = false; // Recepcion DTE
                Grilla.Columns[11].Visible = false; // Resp.Tecnica
                Grilla.Columns[14].Visible = false; // Resp.Comercial
                Grilla.Columns[17].Visible = false; // Recibo Mercadería
                Grilla.Columns[18].Visible = false; // botón Recibo de Mercadería
                Grilla.Columns[23].Visible = true;  // Usuario
            }
            else
            {
                Grilla.Columns[10].Visible = true;  // Recepcion DTE
                Grilla.Columns[11].Visible = true;  // Resp.Tecnica
                Grilla.Columns[14].Visible = true;  // Resp.Comercial
                Grilla.Columns[16].Visible = true;  // Fecha Recibo Mercadería
                Grilla.Columns[17].Visible = true;  // Recibo Mercadería
                Grilla.Columns[18].Visible = true;  // botón Recibo de Mercadería
                Grilla.Columns[23].Visible = false; // Usuario
            }


            if (Request.Params.AllKeys.Contains("par2"))
            {
                if (Request.Params["par2"].ToUpper() == "DTO2")
                {
                    lbTitulo.Text = "Consulta DTE's de Recepción";
                    Grilla2.Columns[12].Visible = false;
                    chkServicio.Visible = false;
                    lbServicio.Visible = false;
                    lv_Servicio.Visible = false;
                    btnServicio.Visible = false;
                    divDllAccion.Visible = false;
                }
            }

        }
    }
    private void ActualizacionBar()
    {
        this.DbnetContext.regpag = txtRegPag.SelectedValue == "" ? 10 : Convert.ToInt16(txtRegPag.SelectedValue);
        Grilla.PageSize = this.DbnetContext.regpag;
        lbPagina.Text = this.DbnetContext.nroPagina.ToString() + "/" + this.DbnetContext.totPagina.ToString();
        lbRegistro.Text = this.DbnetContext.totRegistro.ToString();
    }
    private void buscar()
    {
        Session.Remove("oListaMonitor");
        lbMensaje.Text = "";
        if (Request.Params["par1"] == "DTO" && DTO_COME != "1")
        { Grilla.Columns[11].Visible = true; }
        string str_defe = "1=1";
        int tpodocu = 0;
        if (cmbUsuarios.Enabled)
            tpodocu = 0;
        else
            tpodocu = 1;
        try
        {
            query = lis.dtQuery;
            query = query.Replace(":P_CODI_EMPR", DbnetContext.Codi_empr.ToString());
            query = query.Replace(":P_CODI_PERS", DbnetContext.Codi_pers.ToString());
            query = query.Replace(":P_CODI_CECO", DbnetContext.Codi_ceco.ToString());
            query = query.Replace(":P_CODI_USUA", DbnetContext.Codi_usua.ToString());
            query = query.Replace(":P_CODI_MODU", DbnetContext.Codi_modu.ToString());

            // Filtro de Fecha Desde  
            if (!string.IsNullOrEmpty(Fecha_desde.Text))
            {
                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {
                    if (Request.Params["par1"] == "DTO")
                    {
                        if (rdEmision.Checked == true)
                        {
                            query = query.Replace(":FECHA_RECE1", "DTE.FECH_EMIS >= '" + Fecha_desde.Text + "' ");
                            query = query.Replace(":FECHA_DOCU1", str_defe);
                        }
                        if (rdRecepcion.Checked == true)
                        {
                            query = query.Replace(":FECHA_RECE1", "DTE.FECH_RECE >= '" + Fecha_desde.Text + "' ");
                            query = query.Replace(":FECHA_DOCU1", str_defe);
                        }
                        query = query.Replace(":FECHA_EMIS1", str_defe);
                    }
                    else  // DTE
                    {
                        query = query.Replace(":FECHA_EMIS1", "Convert(DateTime,DTE.FECH_EMIS,120) >= Convert(DateTime,'" + Fecha_desde.Text + "',120)");
                        query = query.Replace(":FECHA_RECE1", "Convert(DateTime,DTE.FECH_RECE,120) >= Convert(DateTime,'" + Fecha_desde.Text + "',120)");
                        query = query.Replace(":FECHA_DOCU1", "Convert(DateTime,DTE.FECH_DOCU,120) >= Convert(DateTime,'" + Fecha_desde.Text + "',120)");
                    }
                }
                else  // Oracle
                {
                    if (Request.Params["par1"] == "DTO")
                    {
                        if (rdEmision.Checked == true)
                        {
                            query = query.Replace(":FECHA_RECE1", "DTE.FECH_EMIS >= '" + Fecha_desde.Text + "' ");
                            query = query.Replace(":FECHA_DOCU1", str_defe);
                        }
                        if (rdRecepcion.Checked == true)
                        {
                            query = query.Replace(":FECHA_RECE1", "DTE.FECH_RECE >= '" + Fecha_desde.Text + "' ");
                            query = query.Replace(":FECHA_DOCU1", str_defe);
                        }
                    }
                    else // DTE
                    {
                        query = query.Replace(":FECHA_EMIS1", "to_date(DTE.FECH_EMIS,'YYYY-MM-DD') >= to_date('" + Fecha_desde.Text + "','yyyy-mm-dd')");
                        query = query.Replace(":FECHA_RECE1", "to_date(DTE.FECH_RECE,'YYYY-MM-DD') >= to_date('" + Fecha_desde.Text + "','yyyy-mm-dd')");
                        query = query.Replace(":FECHA_DOCU1", "to_date(DTE.FECH_DOCU,'YYYY-MM-DD') >= to_date('" + Fecha_desde.Text + "','yyyy-mm-dd')");
                    }
                }
            }
            else // sin filtro de fecha desde
            {
                query = query.Replace(":FECHA_EMIS1", str_defe);
                query = query.Replace(":FECHA_RECE1", str_defe);
                query = query.Replace(":FECHA_DOCU1", str_defe);
            }


            // Filtro de Fecha Hasta  
            if (Fecha_hasta.Text != "")
            {
                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {
                    if (Request.Params["par1"] == "DTO")
                    {
                        if (rdEmision.Checked == true)
                        {
                            query = query.Replace(":FECHA_RECE2", "DTE.FECH_EMIS <= '" + Fecha_hasta.Text + "' ");
                            query = query.Replace(":FECHA_RECE2", str_defe);
                            query = query.Replace(":FECHA_DOCU2", str_defe);
                        }
                        if (rdRecepcion.Checked == true)
                        {
                            query = query.Replace(":FECHA_RECE2", "DTE.FECH_RECE <= '" + Fecha_hasta.Text + "' ");
                            query = query.Replace(":FECHA_EMIS2", str_defe);
                            query = query.Replace(":FECHA_DOCU2", str_defe);
                        }
                    }
                    else  //DTE
                    {
                        query = query.Replace(":FECHA_EMIS2", "Convert(DateTime,DTE.FECH_EMIS,120) <= Convert(DateTime,'" + Fecha_hasta.Text + "',120)");
                        query = query.Replace(":FECHA_RECE2", "Convert(DateTime,DTE.FECH_RECE,120) <= Convert(DateTime,'" + Fecha_hasta.Text + "',120)");
                        query = query.Replace(":FECHA_DOCU2", "Convert(DateTime,DTE.FECH_DOCU,120) <= Convert(DateTime,'" + Fecha_hasta.Text + "',120)");
                    }
                }
                else // Oracle
                {
                    if (Request.Params["par1"] == "DTO")
                    {
                        if (rdEmision.Checked == true)
                        {
                            query = query.Replace(":FECHA_RECE2", "DTE.FECH_EMIS <= '" + Fecha_hasta.Text + "'");
                            query = query.Replace(":FECHA_RECE2", str_defe);
                            query = query.Replace(":FECHA_DOCU2", str_defe);
                        }
                        if (rdRecepcion.Checked == true)
                        {
                            query = query.Replace(":FECHA_RECE2", "DTE.FECH_RECE <= '" + Fecha_hasta.Text + "' ");
                            query = query.Replace(":FECHA_EMIS2", str_defe);
                            query = query.Replace(":FECHA_DOCU2", str_defe);
                        }
                    }
                    else  // DTE
                    {
                        query = query.Replace(":FECHA_EMIS2", "to_date(DTE.FECH_EMIS,'YYYY-MM-DD') <= to_date('" + Fecha_hasta.Text + "','yyyy-mm-dd')");
                        query = query.Replace(":FECHA_RECE2", "to_date(DTE.FECH_RECE,'YYYY-MM-DD') <= to_date('" + Fecha_hasta.Text + "','yyyy-mm-dd')");
                        query = query.Replace(":FECHA_DOCU2", "to_date(DTE.FECH_DOCU,'YYYY-MM-DD') <= to_date('" + Fecha_hasta.Text + "','yyyy-mm-dd')");
                    }
                }
            }
            else  // sin fltro
            {
                query = query.Replace(":FECHA_EMIS2", str_defe);
                query = query.Replace(":FECHA_RECE2", str_defe);
                query = query.Replace(":FECHA_DOCU2", str_defe);
            }

            //Verifica si el Check de folio ERP esta habilitado 
            if (ChkFoli_clie.Checked)
            {
                // Si el folio hasta tiene valor, el folio desde no puede ser nulo  
                if ((string.IsNullOrEmpty(Foli_erp1.Text) || Foli_erp1.Text == "0") && (!string.IsNullOrEmpty(Foli_erp2.Text) && Foli_erp2.Text != "0"))
                    Foli_erp1.Text = "1";

                // Si el folio desde tiene valor, el folio hasta no puede ser nulo  
                if ((string.IsNullOrEmpty(Foli_erp2.Text) || Foli_erp2.Text == "0") && (!string.IsNullOrEmpty(Foli_erp1.Text) && Foli_erp1.Text != "0"))
                    Foli_erp2.Text = Foli_erp1.Text;


                // Filtro de Folio Desde  
                if (!string.IsNullOrEmpty(Foli_erp1.Text) && Foli_erp1.Text != "0")
                {
                    query = query.Replace(":FOLIO_ERP1", "DTE.FOLI_CLIE >= '" + Foli_erp1.Text + "'");
                }
                else
                    query = query.Replace(":FOLIO_ERP1", str_defe);


                // Filtro de Folio Hasta  
                if (!string.IsNullOrEmpty(Foli_erp2.Text) && Foli_erp2.Text != "0")
                {
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
                // Si el folio hasta tiene valor, el folio desde no puede ser nulo  
                if ((string.IsNullOrEmpty(Foli_docu1.Text) || Foli_docu1.Text == "0") && (!string.IsNullOrEmpty(Foli_docu2.Text) && Foli_docu2.Text != "0"))
                    Foli_docu1.Text = "1";

                // Si el folio desde tiene valor, el folio hasta no puede ser nulo  
                if ((string.IsNullOrEmpty(Foli_docu2.Text) || Foli_docu2.Text == "0") && (!string.IsNullOrEmpty(Foli_docu1.Text) && Foli_docu1.Text != "0"))
                    Foli_docu2.Text = Foli_docu1.Text;

                // Filtro de Folio Desde  
                if (!string.IsNullOrEmpty(Foli_docu1.Text) && Foli_docu1.Text != "0")
                {
                    if (this.DbnetContext.TipoMonitor == "DTO")
                        query = query.Replace(":FOLIO1", "DTE.FOLI_NUME >= " + Foli_docu1.Text + "");
                    else
                        query = query.Replace(":FOLIO1", "DTE.FOLI_DOCU >= " + Foli_docu1.Text + "");
                } 
                else
                    query = query.Replace(":FOLIO1", str_defe);

                // Filtro de Folio Hasta  
                if (!string.IsNullOrEmpty(Foli_docu2.Text) && Foli_docu2.Text != "0")
                {
                    if (this.DbnetContext.TipoMonitor == "DTO")
                        query = query.Replace(":FOLIO2", "DTE.FOLI_NUME <= " + Foli_docu2.Text + "");
                    else
                        query = query.Replace(":FOLIO2", "DTE.FOLI_DOCU <= " + Foli_docu2.Text + "");
                }
                else
                    query = query.Replace(":FOLIO2", str_defe);

                query = query.Replace(":FOLIO_ERP1", str_defe);
                query = query.Replace(":FOLIO_ERP2", str_defe);
                query = query.Replace(":FOLIO_ERP", str_defe);

            }
            // Filtro de Tipo  
            if (!string.IsNullOrEmpty(Tipo_docu.Text) && Tipo_docu.Text != "0")
                query = query.Replace(":TIPO", "DTE.TIPO_DOCU = '" + Tipo_docu.Text + "'");
            else
            {
                if (tpodocu == 1)
                {
                    query = query.Replace(":TIPO", "DTE.TIPO_DOCU in (select tipo_docu from dte_usua_docu where codi_usua='" + DbnetContext.Codi_usua + "')");
                }
                else
                {
                    query = query.Replace(":TIPO", str_defe);
                }
            }

            // Filtro de Persona  
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

            // Filtro de Sucursal  
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

            //************************************************************************************************* 
            //***********************************FILTRO RESPUESTA********************************************** 
            //************************************************************************************************* 
            switch (this.DbnetContext.TipoMonitor)
            {
                case "DTE":
                    string queryresp = "";
                    string valorlistresp = "";
                    if (CheckTec.Checked == true)
                    {
                        valorlistresp = listresp.SelectedValue.ToString();
                        if (valorlistresp.Equals("ERR"))
                        {
                            queryresp = "DTE.ESTA_DOCU1 in ('FRM','ERR','ER1','ER2','ER3','ER4','ER5')";
                        }
                        else if (valorlistresp.Equals("SIN"))
                        {
                            queryresp = "DTE.ESTA_DOCU1 = 'INI' " +
                                        " AND EXISTS (SELECT ENC.CORR_ENVI FROM DTE_ENVI_DOCU ENC" +
                                        " WHERE ENC.CORR_ENVI = DTE.CORR_ENVI1 AND ENC.ESTA_ENVI != 'CER')";
                        }
                        else
                        {
                            queryresp = "DTE.ESTA_DOCU1 = 'DOK'" +
                                        " AND EXISTS (SELECT ENC.CORR_ENVI FROM DTE_ENVI_DOCU ENC" +
                                        " WHERE ENC.CORR_ENVI = DTE.CORR_ENVI1 AND ENC.ESTA_ENVI = 'CER')";
                        }

                        query = query.Replace(":RESPUESTA", queryresp);
                        query = query.Replace(":STTRCAMBIO", "1=1");

                    }
                    else if (CheckCom.Checked == true)
                    {
                        valorlistresp = listresp.SelectedValue.ToString();
                        string query1 = "";
                        if (valorlistresp.Equals("ACD"))
                        {
                            queryresp = "DTE.ESTA_DOCU1 = 'ACD'";
                        }
                        else if (valorlistresp.Equals("ACC"))
                        {
                            queryresp = "DTE.ESTA_DOCU1 = 'ACC'";
                        }
                        else if (valorlistresp.Equals("ROC"))
                        {
                            queryresp = "DTE.ESTA_DOCU1 = 'ROC'";
                        }
                        else if (valorlistresp.Equals("RME"))
                        {
                            query1 = " DTE.ESTA_REME IS NOT NULL ";
                            query = query.Replace(":STTRCAMBIO", query1);
                        }
                        else
                        {
                            queryresp = "DTE.ESTA_DOCU1 = 'DOK'" +
                                        " AND EXISTS (SELECT ENC.CORR_ENVI FROM DTE_ENVI_DOCU ENC" +
                                        " WHERE ENC.CORR_ENVI = DTE.CORR_ENVI1 AND ENC.ESTA_ENVI = 'CER')";
                        }
                        if (query1.Length == 0)
                            query = query.Replace(":STTRCAMBIO", "1=1");
                        if (queryresp.Length == 0)
                            query = query.Replace(":RESPUESTA", " 1=1");
                        else { query = query.Replace(":RESPUESTA", queryresp); }
                    }
                    else { query = query.Replace(":RESPUESTA", " 1=1"); query = query.Replace(":STTRCAMBIO", "1=1"); }

                    if (ddlEstaDocu.SelectedValue == "TODOS")
                        query = query.Replace(":ESTA_DOCU", " AND 1=1");
                    else
                        query = query.Replace(":ESTA_DOCU", " AND DTE.esta_docu = '" + ddlEstaDocu.SelectedValue + "'");

                    if (ddlEstaEnvi.SelectedValue == "TODOS")
                        query = query.Replace(":ESTA_ENVI", " AND 1=1");
                    else
                        query = query.Replace(":ESTA_ENVI", " AND ENV.ESTA_ENVI = '" + ddlEstaEnvi.SelectedValue + "'");
					
					//OT 9376978 12-05-2017|AM
					//Filtro Evento Reclamo
					if (dllEventoReclamo.SelectedValue == "TODOS")
						query = query.Replace(":EVEN_RECL", "");
					else if (dllEventoReclamo.SelectedValue == "TOA")
						query = query.Replace(":EVEN_RECL", " AND DTE.EVEN_RECL IN ('ACD', 'ERM') ");
					else if (dllEventoReclamo.SelectedValue == "TOR")
						query = query.Replace(":EVEN_RECL", " AND DTE.EVEN_RECL IN ('RCD', 'RFP', 'RFT') ");
					else
						query = query.Replace(":EVEN_RECL", " AND DTE.EVEN_RECL = '" + dllEventoReclamo.SelectedValue + "' ");

					//Filtro Respuesta Reclamo
					if (dllRespuestaReclamoDTE.SelectedValue == "TODOS")
						query = query.Replace(":DOCU_ESCD", "");
					else
                        query = query.Replace(":DOCU_ESCD", "AND DTE.DOCU_ESCD = '" + dllRespuestaReclamoDTE.SelectedValue + "' ");

					//OT 9376978 12-05-2017|AM
					
                    break;
                case "DTO":

                    //OT 9376978 05-05-2017|AM
                    //Filtro Evento Reclamo
                    if (dllEventoReclamo.SelectedValue == "TODOS")
                        query = query.Replace(":EVEN_RECL", "");
                    else if (dllEventoReclamo.SelectedValue == "TOA")
                        query = query.Replace(":EVEN_RECL", " AND DTE.EVEN_RECL IN ('ACD', 'ERM') ");
                    else if (dllEventoReclamo.SelectedValue == "TOR")
                        query = query.Replace(":EVEN_RECL", " AND DTE.EVEN_RECL IN ('RCD', 'RFP', 'RFT') ");
                    else
                        query = query.Replace(":EVEN_RECL", " AND DTE.EVEN_RECL = '" + dllEventoReclamo.SelectedValue + "' ");

                    //Filtro Respuesta Reclamo
                    if (dllRespuestaReclamo.SelectedValue == "TODOS")
                        query = query.Replace(":CSII_RECL", "");
                    else
                        query = query.Replace(":CSII_RECL", "AND DTE.CSII_RECL = '" + dllRespuestaReclamo.SelectedValue + "' ");

                    //OT 9376978 05-05-2017|AM
                    switch (DbnetGlobal.Base_dato.ToUpper())
                    {
                        case "SQLSERVER":
                            if (ddlEstaDocu.SelectedValue == "TODOS")
                                query = query.Replace(":ESTA_DOCU", " AND 1=1");
                            else
                                query = query.Replace(":ESTA_DOCU", " AND isnull(DTE.esta_docu,'TODOS') = '" + ddlEstaDocu.SelectedValue + "'");

                            ////OT 9376978 05-05-2017|AM
                            ////Filtro Evento Reclamo
                            //if (dllEventoReclamo.SelectedValue == "TODOS")
                            //    query = query.Replace(":EVEN_RECL", "");
                            //else if (dllEventoReclamo.SelectedValue == "TOA")
                            //    query = query.Replace(":EVEN_RECL", " AND DTE.EVEN_RECL IN ('ACD', 'ERM') ");
                            //else if (dllEventoReclamo.SelectedValue == "TOR")
                            //    query = query.Replace(":EVEN_RECL", " AND DTE.EVEN_RECL IN ('RCD', 'RFP', 'RFT') ");
                            //else
                            //    query = query.Replace(":EVEN_RECL", " AND DTE.EVEN_RECL = '" + dllEventoReclamo.SelectedValue + "' ");

                            ////OT 9376978 05-05-2017|AM
                            break;
                        case "ORACLE":
                            if (ddlEstaDocu.SelectedValue == "TODOS")
                                query = query.Replace(":ESTA_DOCU", "");
                            else
                                query = query.Replace(":ESTA_DOCU", " AND nvl(DTE.esta_docu,'TODOS') = '" + ddlEstaDocu.SelectedValue + "'");
                            break;
                    }
                    break;
            }

            query = query.Replace(":ESTA_ANOR", "'" + cmbEstados.SelectedValue + "'");
            if (cmbUsuarios.Visible == false)
            {
                query = query.Replace(":FILTRO", "1=1");
            }
            else
            {
                if (cmbUsuarios.SelectedItem.ToString() == "- Todos -")
                    query = query.Replace(":FILTRO", "1=1");
                else
                    query = query.Replace(":FILTRO", "DTE.CODI_USUA='" + cmbUsuarios.SelectedValue.ToString() + "'");
            }
            if (envElec.Checked || respComerTradi.Checked)
            {
                if (envElec.Checked)
                {
                    query = query.Replace(":STRCAMBIO", ddlTipo.SelectedValue);
                }
                if (respComerTradi.Checked)
                {
                    query = query.Replace(":STRCAMBIO", ddlTipo.SelectedValue);
                }
            }
            else
            { query = query.Replace(":STRCAMBIO", "1=1"); }
            //ToDo: agregar validación de parametro general que la cantidad no sea mayor a 50.000

            int iEgateValiMDTE;
            int.TryParse(DbnetTool.SelectInto(DbnetContext.dbConnection, "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_VALI_MDTE'"), out iEgateValiMDTE);
            string sQueryCount = query.Substring(query.IndexOf("FROM")).Replace("\t", " ");
            if (sQueryCount.Contains("ORDER BY"))
                sQueryCount = string.Format("SELECT COUNT(*) {0}", sQueryCount.Substring(0, sQueryCount.IndexOf("ORDER BY")));
            if (DbnetContext.TipoMonitor.Equals("DTE"))
                if (iEgateValiMDTE > 1)
                {
                    int iDx;
                    int.TryParse(DbnetTool.SelectInto(DbnetContext.dbConnection, sQueryCount), out iDx);
                    if (iDx <= iEgateValiMDTE)
                    {
                        buscar(query);
                    }
                    else
                    {
                        DbnetTool.MsgAlerta(string.Format("No se puede realizar una búsqueda mayor a {0} registros, favor realice un nuevo filtro.", iEgateValiMDTE), this.Page);
                    }
                }
                else
                {
                    buscar(query);
                }
            else
                buscar(query);
        }
        catch (Exception e)
        {
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                   "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                   "pproc_erro", "Menu: Listador de DTEs", "VarChar", 50, "in",
                                   "pmsaj_erro", e.Message, "VarChar", 150, "in",
                                   "pbin_erro", "WEB", "VarChar", 50, "in",
                                   "p_mensaje", "", "VarChar", 200, "out");

            chkDespliega.Checked = lbMensaje.Enabled = lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = sp.return_String("p_mensaje");
        }
        imgCargando.Style.Add("display", "none");
    }
    private void buscar(string sQuery)
    {
        this.DbnetContext.dtSelectInicial = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, sQuery);
        this.DbnetContext.dtSelect = this.DbnetContext.dtSelectInicial;

        this.DbnetContext.nroPagina = 0;
        this.DbnetContext.totPagina = 0;
        this.DbnetContext.totRegistro = 0;

        this.DbnetContext.totPagina = (this.DbnetContext.dtSelect.Rows.Count / this.DbnetContext.regpag) + 1;
        this.DbnetContext.totRegistro = this.DbnetContext.dtSelect.Rows.Count;

        if (DbnetContext.dtSelectInicial.Rows.Count != 0)
        {
            positiongrid1.Visible = true;
            positiongrid2.Visible = true;
        }
        else
        {
            positiongrid1.Visible = false;
            positiongrid2.Visible = false;
        }
        buscar(1);

        try
        { Session.Add("listador", lis); }
        catch
        { Session["listador"] = lis; }
    }
    private void buscar(int pagina)
    {
        try
        {
            try { this.DbnetContext.dtGrilla.Dispose(); }
            catch { }

            this.DbnetContext.dtGrilla = new DataTable();
            this.DbnetContext.dtGrilla.Columns.Add("Lin", typeof(int));
            this.DbnetContext.dtGrilla.Columns.Add("Serv");
            this.DbnetContext.dtGrilla.Columns.Add("Mail");
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
                drGrilla["Serv"] = "<input class='dbnchk' type='checkbox' id='chk_serv_" + (r + 1) + "' onclick='javascript:chk_serv();'/>";
                for (int i = 0; i < lis.dtEncabezados.Count; i++)
                {
                    DataRowView drEncabezado = lis.dtEncabezados[i];
                    if (drEncabezado["Codigo"].ToString().Substring(0, 4) == "FASE")
                    {
                        string color = drSelect["COLOR_FASE"].ToString();
                        string fase_ant = "<font style='color:#40A04A; background:#40A04A; border-style=solid; font-size:16px;'>.</font>";
                        string fase_car = "";
                        if (color.ToUpper() == "RED")
                        {
                            color = "#C82A1D";
                            fase_car = "<font style='color: " + color + "; background: " + color + "; border-style=solid; font-size:16px;'>...</font>";
                        }
                        else if (color.ToUpper() == "GREEN")
                        {
                            color = "#40A04A";
                            fase_car = "<font style='color: " + color + "; background: " + color + "; border-style=solid; font-size:16px;'>.</font>";
                        }
                        else if (color.ToUpper() == "YELLOW")
                        {
                            color = "#FFC54D";
                            fase_car = "<font style='color: " + color + "; background: " + color + "; border-style=solid; font-size:16px;'>...</font>";
                        }
                        else
                        { }
                        string fase_sig = "";

                        if (((r + 1) % 2) == 0)
                            fase_sig = "<font style='color:#E6E6E6; border-style=solid; font-size:16px;'>.</font>";
                        else
                            fase_sig = "<font style='color: white; border-style=solid; font-size:16px;'>.</font>";
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
                        fase = "\n<font style='DISPLAY: block; TEXT-ALIGN: Center'>" + fase;
                        if (!color.ToUpper().Equals("#40A04A") && cod_fase != j && cod_fase != 0 && cod_fase < j)
                            fase += " <a href=\"javascript:__doPostBack('Grilla$_ctl" + (this.DbnetContext.dtGrilla.Rows.Count + 3) + "$_ctl5','')\"><b>?</b></a></font>";
                        else
                            fase += " &nbsp;</font>";

                        drGrilla[drEncabezado["Descripcion"].ToString()] = fase;
                    }
                    else
                    {
                        string campo = "";

                        // Alineacion  
                        if (drEncabezado["Alineacion"].ToString() != "")
                            campo += "<font style='DISPLAY: block; TEXT-ALIGN: " + drEncabezado["Alineacion"].ToString() + "'>";

                        // Formato  
                        if (drEncabezado["Formato"].ToString() == "")
                            campo += drSelect[drEncabezado["Codigo"].ToString()].ToString();
                        else
                            campo += dbnFormat.Numero(drSelect[drEncabezado["Codigo"].ToString()].ToString(), drEncabezado["Formato"].ToString());

                        // Alineacion  
                        if (drEncabezado["Alineacion"].ToString() != "")
                            campo += "</font>";

                        drGrilla[drEncabezado["Descripcion"].ToString()] = campo;
                    }
                }
                this.DbnetContext.dtGrilla.Rows.Add(drGrilla);
            }
            this.DbnetContext.nroPagina = pagina;

            Grilla.DataSource = this.DbnetContext.dtGrilla;
            Grilla2.DataSource = this.DbnetContext.dtGrilla;
            Grilla2.DataBind();
            Grilla.CurrentPageIndex = 0;
            Grilla.DataBind();

            ActualizacionBar();
        }
        catch (Exception e)
        {
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                   "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                   "pproc_erro", "Monitor", "VarChar", 50, "in",
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
        this.btBuscar.Click += new System.Web.UI.ImageClickEventHandler(this.btBuscar_Click);
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
    protected void GrillaOrden(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
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
    protected void GrillaComando(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
        string comando = e.CommandName.ToString().ToUpper();
        string sep = "\\";
        string ind_ensa = "E";	// Indicador de entrada Salida	 
        string dir_arch = "xml";	// Directorio ubicacion archivo  
        string archivo = "";
        string origen = "";
        string origenXml = "";
        string cache = DbnetGlobal.Path + "cache\\";
        string pScript = "";
        string[] extension;
        bool flag_xslt = false;
        string query = "";
        string cert = "";
        bool sw = false;
        string prefijo = "";
        string tablaResp = "";
        string tipo_arch = "";
        string Corr_envi2 = "";
        string p_corr_envi1 = "";

        if (!FormatoRespuesta.Checked)
        { flag_xslt = true; }
        else
        { flag_xslt = false; }

        lbMensaje.Text = "";
        try
        {
            int indGrilla = e.Item.ItemIndex;
            DataView sortView = new DataView(this.DbnetContext.dtGrilla);
            int indSelect = Convert.ToInt32(sortView[indGrilla].Row["Lin"]) - 1;
            DataRow drSelect = this.DbnetContext.dtSelect.Rows[indSelect];

            if (this.DbnetContext.TipoMonitor == "DTE")
            {
                Corr_envi2 = drSelect["CORR_ENVI"].ToString();
                p_corr_envi1 = drSelect["CORR_ENVI1"].ToString();
            }
            switch (comando)
            {
                //************************************************************************************************************************************************************ 
                case "BITA":
                    string Foli_docu1 = drSelect["FOLI_DOCU"].ToString();
                    string Tipo_docu1 = drSelect["TIPO_DOCU"].ToString();

                    DbnetContext.Key1 = "TIPO";
                    DbnetContext.Val1 = Request.Params["par1"];
                    DbnetContext.Key4 = "CORR_ENVI";
                    DbnetContext.Val4 = drSelect["CORR_ENVI"].ToString();

                    if (this.DbnetContext.TipoMonitor == "DTO")
                    {
                        DbnetContext.Key5 = "CORR_DOCU";
                        DbnetContext.Val5 = drSelect["CORR_DOCU"].ToString();
                    }
                    else
                    {
                        DbnetContext.Key5 = "CORR_DOCU";
                        DbnetContext.Val5 = drSelect["DESC_FASE"].ToString();
                    }

                    DbnetContext.Val3 = Foli_docu1;
                    DbnetContext.Key3 = "FOLI_DOCU";
                    DbnetContext.Val2 = Tipo_docu1;
                    DbnetContext.Key2 = "TIPO_DOCU";

                    string archivo02 = "facCnsDocumento50.aspx";
                    pScript = "<script type=\"text/javascript\"> " +
                              "window.open(\"" + "../factura/" + archivo02 + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                              "</script>";
                    ClientScript.RegisterStartupScript(typeof(Page), "PaginaBitacora", pScript);
                    break;
                case "MAIL":
                    string pathTemp = WebConfigurationManager.AppSettings["dirTemp"];

                    string Foli_docu_EML = drSelect["FOLI_DOCU"].ToString();
                    string Tipo_docu_EML = drSelect["TIPO_DOCU"].ToString();
                    string Corr_docu_EML = drSelect["CORR_DOCU"].ToString();
                    string Rutt_emis_EML = drSelect["RUTT_EMIS"].ToString();

                    string fileName = @"\" + "E" + Rutt_emis_EML + "_" + Tipo_docu_EML + "_" + Foli_docu_EML + ".eml";
                    string FullFile = pathTemp + fileName;

                    if (!Directory.Exists(pathTemp))
                    {
                        DbnetTool.MsgError("No existe directorio para descargar correos [" + pathTemp + "].", this.Page);
                        return;
                    }

                    // Cambiar a acceso a DBQ_ARCH con Grupo, tipo =  EML, luego a DBQ_ARCH_CLOB con Codi_arch 
                    //int codi_arch = 1159;
                    //string querylob = "select clob_arch from dbq_arch_clob where codi_arch = " + codi_arch;

                    string querylob = "select clob_docu from dto_docu_lob where corr_docu = " + Corr_docu_EML;

                    DataTable dtSalidasLob = new DataTable();
                    dtSalidasLob = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, querylob);
                    if (dtSalidasLob.Rows.Count == 0)
                    {
                        DbnetTool.MsgError("Correo Origen no registrado para documento.", this.Page);
                        return;
                    }

                    DataRow drSalidasLob = dtSalidasLob.Rows[0];
                    string base64 = drSalidasLob[0].ToString();

                    if (EsBase64(base64))
                    {
                        byte[] toDecodeAsBytes;

                        toDecodeAsBytes = System.Convert.FromBase64String(base64);
                        archivo = System.Text.ASCIIEncoding.ASCII.GetString(toDecodeAsBytes);
                        base64 = System.Text.Encoding.UTF8.GetString(toDecodeAsBytes);
                    }

                    using (StreamWriter FileMail = File.CreateText(FullFile))
                    {
                        FileMail.WriteLine(base64);
                    }

                    IPHostEntry host;
                    string localIP = "";
                    host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (IPAddress ip in host.AddressList)
                    {
                        if (ip.AddressFamily.ToString() == "InterNetwork")
                        {
                            localIP = ip.ToString();
                        }
                    }
                    string archivo1 = EncodeToken(FullFile);
                    Response.Write("<script>window.open('http://localhost:53262/dbnVisualizadorMensajes/dbnMensajeCorreo.aspx?ID=" + archivo1 + "','MSJ','resizable=yes,titlebar=no,toolbar=no,location=no,status=no,scrollbars=yes');</script>");

                    string URL = "'http://" + localIP + "/dbnVisualizadorMensajes/dbnMensajeCorreo.aspx?ID=" + archivo1 + "','MSJ','resizable=yes,titlebar=no,toolbar=no,status=no,scrollbars=yes,width=1100,height=600,menubar=yes'";
                    pScript = "<script>" + "window.open(" + URL + ");</script>";
                    ClientScript.RegisterStartupScript(typeof(Page), "PaginaBitacora", pScript);

                    break;
                case "XML":
                    ind_ensa = drSelect["ENSA_" + comando].ToString();
                    dir_arch = drSelect["DIRE_" + comando].ToString();
                    archivo = drSelect["ARCH_" + comando].ToString();

                    if (ind_ensa == "S")
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                    else
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";

                    DataTable dtSalidas = new DataTable();
                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                    if (archivo.Length > 0)
                    {
                        archivo = archivo.Substring(0, archivo.Length - 4) + archivo.Substring(archivo.Length - 4, 4).ToLower();
                    }
                    else
                    {
                        lbMensaje.Text = "El Archivo no esta registrado en la base de datos";
                        MsgError(1);
                        break;
                    }

                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
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

                        try
                        {
                            if (System.IO.File.Exists(origen))
                            {
                                File.Copy(origen, cache + archivo, true);
                                lbMensaje.Text = "";
                                sw = true;
                                pScript = "<script type=\"text/javascript\"> " +
                                          "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                          "</script>";
                                ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                            }
                            else
                            {
                                //Obtener parametro para DTE
                                string Foli_docu = drSelect["FOLI_DOCU"].ToString();
                                string Tipo_docu = drSelect["TIPO_DOCU"].ToString();
                                string Corr_envi = drSelect["CORR_ENVI"].ToString();
                                string Rutt_emis = drSelect["RUTT_EMIS"].ToString();
                                string Corr_Docu = "0";
                                string DTEXMLDB = "0";
                                string DTOXMLDB = "0";

                                if (this.DbnetContext.TipoMonitor == "DTE")
                                {
                                    query = "SELECT COUNT(*) FROM DTE_DOCU_LOB WHERE codi_empr = " + DbnetContext.Codi_empr + " and tipo_docu = " + Tipo_docu + " and foli_docu = " + Foli_docu + " and tipo_arch='" + comando + "'";
                                    DTEXMLDB = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
                                }

                                if (this.DbnetContext.TipoMonitor == "DTO")
                                {
                                    Corr_Docu = drSelect["CORR_DOCU"].ToString();
                                    query = "SELECT COUNT(*) FROM DTO_DOCU_LOB WHERE codi_empr = " + DbnetContext.Codi_empr + "  and corr_docu = " + Corr_Docu + " and tipo_arch= '" + comando + "'";
                                    DTOXMLDB = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
                                }

                                //DTE CAMBIADO 
                                if (this.DbnetContext.TipoMonitor == "DTE")
                                //11-09-2012 
                                {
                                    tablaResp = "dte_docu_lob";
                                    tipo_arch = "XML";
                                    string pScriptXML = "";
                                    pScriptXML = "<script type=\"text/javascript\"> " +
                                                          "window.open(\"" + "../factura/facShowFiles.aspx?tabla=" + tablaResp + "&tipo_arch=" + tipo_arch + "&foli_docu=" + Foli_docu + "&tipo_docu=" + Tipo_docu + "&codi_empr=" + DbnetContext.Codi_empr + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                                          "</script>";
                                    ClientScript.RegisterStartupScript(typeof(Page), "XML-DTE", pScriptXML);
                                }

                                else if (this.DbnetContext.TipoMonitor == "DTO")
                                {

                                    tablaResp = "dto_docu_lob";
                                    tipo_arch = "XML";
                                    string pScriptXML = "";
                                    pScriptXML = "<script type=\"text/javascript\"> " +
                                                          "window.open(\"" + "../factura/facShowFiles.aspx?tabla=" + tablaResp + "&tipo_arch=" + tipo_arch + "&corr_docu=" + Corr_Docu + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                                          "</script>";
                                    ClientScript.RegisterStartupScript(typeof(Page), "XML-DTO", pScriptXML);
                                }
                                else
                                {
                                    lbMensaje.Text += "Respuesta no cargada.<br>";
                                    MsgError(1);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                           "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                           "pproc_erro", "Monitor", "VarChar", 50, "in",
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
                    break;
                case "PDF":
                    ind_ensa = drSelect["ENSA_" + comando].ToString();
                    dir_arch = drSelect["DIRE_" + comando].ToString();
                    archivo = drSelect["ARCH_" + comando].ToString();

                    if (ind_ensa == "S")
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                    else
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";

                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                    if (archivo.Length > 0)
                    {
                        archivo = archivo.Substring(0, archivo.Length - 4) + archivo.Substring(archivo.Length - 4, 4).ToLower();
                    }
                    else
                    {
                        lbMensaje.Text = "El Archivo no esta registrado en la base de datos";
                        MsgError(1);
                        break;
                    }

                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
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
                        try
                        {
                            if (System.IO.File.Exists(origen))
                            {
                                if (!File.Exists(cache + archivo))
                                {
                                    File.Copy(origen, cache + archivo, true);
                                    Thread.Sleep(2000);
                                }
                                lbMensaje.Text = "";
                                sw = true;
                                DespliegaPDF(archivo);
                            }
                            else
                            {
                                //Obtener parametro para DTE                                 
                                string Foli_docu = drSelect["FOLI_DOCU"].ToString();
                                string Tipo_docu = drSelect["TIPO_DOCU"].ToString();
                                string Rutt_emis = drSelect["RUTT_EMIS"].ToString();
                                string Corr_envi = drSelect["CORR_ENVI"].ToString();

                                string comando_bin = "egateDTE";
                                string parametros = string.Empty;
                                if (this.DbnetContext.TipoMonitor.Equals("DTE"))
                                {
                                    parametros = string.Format(" -te bd -ts html -tl 3 -empr {0} -tdte {1} -fdte {2}", 
                                                    DbnetContext.Codi_empr, Tipo_docu.Trim().ToString(),Foli_docu.Trim().ToString());
                                }
                                else if (DbnetContext.TipoMonitor.Equals("DTO"))
                                {
                                    parametros = string.Format(" -te dto -ts html -tl 3 -empr {0} -tdte {1} -fdte {2} -re {3} -corr {4}",
                                                    DbnetContext.Codi_empr,Tipo_docu.Trim().ToString(),Foli_docu.Trim().ToString(),
                                                    Rutt_emis.ToString(), Corr_envi.ToString());
                                }
                                if (!string.IsNullOrEmpty(parametros))
                                {
                                    lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                    lbMensaje.Text = "";
                                }

                                try
                                {
                                    if (System.IO.File.Exists(origen))
                                    {
                                        File.Copy(origen, cache + archivo, true);
                                        Thread.Sleep(2000);
                                    }
                                    if (!File.Exists(cache + archivo))
                                    {
                                        File.Copy(origen, cache + archivo, true);
                                        Thread.Sleep(2000);
                                    }
                                    if (File.Exists(cache + archivo))
                                    {
                                        lbMensaje.Text = "";
                                        sw = true;
                                        DespliegaPDF(archivo);
                                    }
                                    else
                                    {
                                        lbMensaje.Text = "Respuesta no cargada. <br>";
                                        DbnetTool.MsgAlerta("Respuesta no cargada", this.Page);
                                        MsgError(1);
                                    }
                                }
                                catch (Exception eex)
                                {
                                    DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                       "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                       "pproc_erro", "Monitor", "VarChar", 50, "in",
                                       "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                       "pbin_erro", "WEB", "VarChar", 50, "in",
                                       "p_mensaje", "", "VarChar", 200, "out");
                                    chkDespliega.Checked = true;
                                    lbMensaje.Enabled = true;
                                    lbMensaje.Visible = true;
                                    lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                    lbEx.Text = sp.return_String("p_mensaje");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                           "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                           "pproc_erro", "Monitor-PDF.", "VarChar", 50, "in",
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
                    break;
                case "PDF-M":
                    ind_ensa = drSelect["ENSA_PDF"].ToString();
                    dir_arch = drSelect["DIRE_PDF"].ToString();
                    archivo = drSelect["ARCH_PDF"].ToString().Replace(".pdf", "_me.pdf");
                    archivo = drSelect["ARCH_PDF"].ToString().Replace(".PDF", "_me.pdf");
                    if (ind_ensa == "S")
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                    else
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";


                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                    if (archivo.Length > 0)
                    {
                        archivo = archivo.Substring(0, archivo.Length - 4) + archivo.Substring(archivo.Length - 4, 4).ToLower();
                    }
                    else
                    {
                        lbMensaje.Text = "El Archivo no esta registrado en la base de datos";
                        MsgError(1);
                        break;
                    }

                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
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
                        try
                        {
                            if (System.IO.File.Exists(origen))
                            {
                                File.Copy(origen, cache + archivo, true);
                                lbMensaje.Text = "";
                                sw = true;
                                DespliegaPDF(archivo);
                            }
                            else
                            {
                                //Obtener parametro para DTE                                   
                                string Foli_docu = drSelect["FOLI_DOCU"].ToString();
                                string Tipo_docu = drSelect["TIPO_DOCU"].ToString();
                                string Rutt_emis = drSelect["RUTT_EMIS"].ToString();
                                string Corr_envi = drSelect["CORR_ENVI"].ToString();

                                if (this.DbnetContext.TipoMonitor == "DTE")
                                {
                                    string comando_bin = "egateDTE";
                                    string parametros = "";

                                    parametros += " -te bd -ts html -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                    parametros += " -merit ";
                                    if (!string.IsNullOrEmpty(parametros))
                                    {
                                        for (int iCount = 0; iCount < 5; iCount++)
                                        {
                                            lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                            lbMensaje.Text = "";
                                            if (System.IO.File.Exists(origen))
                                                break;
                                        }
                                    }

                                    try
                                    {
                                        if (System.IO.File.Exists(origen))
                                        {
                                            File.Copy(origen, cache + archivo, true);
                                            Thread.Sleep(2000);
                                        }
                                        if (!File.Exists(cache + archivo))
                                        {
                                            File.Copy(origen, cache + archivo, true);
                                            Thread.Sleep(2000);
                                        }
                                        if (File.Exists(cache + archivo))
                                        {
                                            lbMensaje.Text = "";
                                            sw = true;
                                            DespliegaPDF(archivo);
                                        }
                                        else
                                        {
                                            lbMensaje.Text = "Respuesta no cargada. <br>";
                                            DbnetTool.MsgAlerta("Respuesta no cargada", this.Page);
                                            MsgError(1);
                                        }
                                    }
                                    catch (Exception eex)
                                    {
                                        DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                           "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                           "pproc_erro", "Monitor", "VarChar", 50, "in",
                                           "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                           "pbin_erro", "WEB", "VarChar", 50, "in",
                                           "p_mensaje", "", "VarChar", 200, "out");
                                        chkDespliega.Checked = true;
                                        lbMensaje.Enabled = true;
                                        lbMensaje.Visible = true;
                                        lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                        lbEx.Text = sp.return_String("p_mensaje");
                                    }
                                }
                                else
                                {
                                    lbMensaje.Text += "Respuesta no cargada. <br>";
                                    MsgError(1);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                          "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                          "pproc_erro", "Monitor.PDF-M()", "VarChar", 50, "in",
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
                    break;
                case "HTML":
                    ind_ensa = drSelect["ENSA_" + comando].ToString();
                    dir_arch = drSelect["DIRE_" + comando].ToString();
                    archivo = drSelect["ARCH_" + comando].ToString();
                    if (ind_ensa == "S")
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                    else
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";

                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                    if (archivo.Length > 0)
                        archivo = archivo.Substring(0, archivo.Length - 4) + archivo.Substring(archivo.Length - 4, 4).ToLower();
                    else
                    {
                        lbMensaje.Text = "El Archivo no esta registrado en la base de datos";
                        MsgError(1);
                        break;
                    }

                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
                        {
                            origen = drSalidas[0].ToString() + sep + dir_arch + sep + archivo;
                        }
                        else if (!string.IsNullOrEmpty(dir_arch))
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
                            if (System.IO.File.Exists(origen))
                            {
                                File.Copy(origen, cache + archivo, true);
                                lbMensaje.Text = "";
                                sw = true;
                                DespliegaPDF(archivo);
                            }
                            else
                            {
                                //Obtener parametro para DTE                                   
                                string Foli_docu = drSelect["FOLI_DOCU"].ToString();
                                string Tipo_docu = drSelect["TIPO_DOCU"].ToString();
                                string Rutt_emis = drSelect["RUTT_EMIS"].ToString();
                                string Corr_envi = drSelect["CORR_ENVI"].ToString();

                                if (this.DbnetContext.TipoMonitor == "DTE")
                                {
                                    string comando_bin = "egateDTE";
                                    string parametros = "";

                                    parametros = string.Format(" -te bd -ts html -tl 3 -empr {0} -tdte {1} -fdte {2}",
                                                    DbnetContext.Codi_empr,Tipo_docu.Trim().ToString(),Foli_docu.Trim().ToString());

                                    if (parametros != "")
                                    {
                                        lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                        lbMensaje.Text = "";
                                    }

                                    try
                                    {
                                        if (System.IO.File.Exists(origen))
                                        {
                                            File.Copy(origen, cache + archivo, true);
                                            lbMensaje.Text = "";
                                            sw = true;
                                            DespliegaPDF(archivo);
                                        }
                                        else
                                        {
                                            lbMensaje.Text = "Respuesta no cargada. <br>";
                                            DbnetTool.MsgAlerta("Respuesta no cargada", this.Page);
                                            MsgError(1);
                                        }
                                    }
                                    catch (Exception eex)
                                    {
                                        DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                           "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                           "pproc_erro", "Monitor", "VarChar", 50, "in",
                                           "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                           "pbin_erro", "WEB", "VarChar", 50, "in",
                                           "p_mensaje", "", "VarChar", 200, "out");
                                        chkDespliega.Checked = true;
                                        lbMensaje.Enabled = true;
                                        lbMensaje.Visible = true;
                                        lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                        lbEx.Text = sp.return_String("p_mensaje");
                                    }

                                }
                                else if (this.DbnetContext.TipoMonitor == "DTO")
                                {
                                    //Obtener parametro para DTO
                                    string comando_bin = "egateDTE";
                                    string parametros = " -te dto -ts html -tl 3";
                                    parametros += " -empr " + DbnetContext.Codi_empr;
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                    parametros += " -re " + Rutt_emis.ToString();
                                    parametros += " -corr " + Corr_envi.ToString();
                                    lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                    lbMensaje.Text = "";

                                    try
                                    {
                                        if (File.Exists(origen))
                                        {
                                            File.Copy(origen, cache + archivo, true);
                                            lbMensaje.Text = "";
                                            sw = true;
                                            DespliegaPDF(archivo);
                                        }
                                        else
                                        {
                                            lbMensaje.Text = "Respuesta no cargada. <br>";
                                            DbnetTool.MsgAlerta("Respuesta no cargada", this.Page);
                                            MsgError(1);
                                        }
                                    }
                                    catch (Exception eex)
                                    {
                                        DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                           "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                           "pproc_erro", "Monitor", "VarChar", 50, "in",
                                           "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                           "pbin_erro", "WEB", "VarChar", 50, "in",
                                           "p_mensaje", "", "VarChar", 200, "out");

                                        chkDespliega.Checked = true;
                                        lbMensaje.Enabled = true;
                                        lbMensaje.Visible = true;
                                        lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                        lbEx.Text = sp.return_String("p_mensaje");
                                    }
                                }
                                else
                                {
                                    lbMensaje.Text += "Respuesta no cargada <br>";
                                    DbnetTool.MsgAlerta("Respuesta no cargada", this.Page);
                                    MsgError(1);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                           "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                           "pproc_erro", "Monitor.HTML()", "VarChar", 50, "in",
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
                    break;
                case "ARHE":
                    if (!FormatoRespuesta.Checked)
                    {
                        flag_xslt = true;
                    }
                    else
                    {
                        flag_xslt = false;
                    }

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

                    prefijo = "";
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

                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
                        {
                            if (flag_xslt && (comando == "RESP1" || comando == "RESP2" || comando == "RESP3"))
                            {
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
                            if (flag_xslt && (comando == "RESP1" || comando == "RESP2" || comando == "RESP3" || comando == "XML"))
                                File.Copy("nada", cache + archivo, true);
                            else
                                File.Copy(origen, cache + archivo, true);

                            // Si es HTML debe copiar timbre, el logo debe estar en el directorio cache 
                            if (comando == "HTML" && ind_ensa == "S")
                                File.Copy(origen.ToLower().Replace(".html", ".jpg"), cache + archivo.ToLower().Replace(".html", ".jpg"), true);

                            lbMensaje.Text = "";

                            sw = true;
                            pScript = "<script type=\"text/javascript\"> " +
                                      "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                      "</script>";
                            ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                        }
                        catch (Exception)
                        {
                            //Obtener parametro para DTE
                            string dtebd = "0";
                            query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLBD'";
                            dtebd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);

                            //Obtener parametro para DTO
                            string dtobd = "0";
                            query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLDTO'";
                            dtobd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);

                            if (this.DbnetContext.TipoMonitor == "DTE")
                            {
                                // Obtener Certificado para firmar  
                                if (DbnetGlobal.Base_dato == "SQLSERVER")
                                    query = "SELECT ARCH_CERT FROM DTE_CERT_PERS C,DTE_AUTO_PERS A WHERE A.CODI_EMPR = " + DbnetContext.Codi_empr + " AND A.TIPO_AUTO = 'ENV' AND A.DEFE_AUTO = 1 AND A.CODI_PERS = C.CODI_PERS AND GETDATE() BETWEEN C.FEIN_CERT AND C.FETE_CERT";
                                else
                                    query = "SELECT ARCH_CERT FROM DTE_CERT_PERS C,DTE_AUTO_PERS A WHERE A.CODI_EMPR = " + DbnetContext.Codi_empr + " AND A.TIPO_AUTO = 'ENV' AND A.DEFE_AUTO = 1 AND A.CODI_PERS = C.CODI_PERS AND SYSDATE BETWEEN C.FEIN_CERT AND C.FETE_CERT";

                                DataRow fila;
                                DataTable salida = new DataTable();
                                salida = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                if (salida.Rows.Count > 0)
                                {
                                    fila = salida.Rows[0];
                                    cert = fila[0].ToString();
                                }
                                else
                                {
                                    lbMensaje.Text += "No se encontro Certificado Vigente." + "<br>";
                                    cert = "";
                                }

                                string Foli_docu = drSelect["FOLI_DOCU"].ToString();
                                string Tipo_docu = drSelect["TIPO_DOCU"].ToString();
                                string Corr_envi = drSelect["CORR_ENVI"].ToString();
                                string Rutt_emis = drSelect["RUTT_EMIS"].ToString();
                                string corr_envi1 = "";
                                query = "select corr_envi1 from dte_enca_docu where codi_empr = " + DbnetContext.Codi_empr + " and tipo_docu = " + Tipo_docu + " and foli_docu = " + Foli_docu;
                                salida = new DataTable();
                                DbnetTool.ctrlSqlInjection(this.Page.Form);
                                salida = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                if (salida.Rows.Count > 0)
                                {
                                    fila = salida.Rows[0];
                                    corr_envi1 = fila[0].ToString();
                                }

                                int tam = origen.Length;
                                origen = origen.Substring(0, tam - 4) + origen.Substring(tam - 4, 4).ToLower();

                                string comando_bin = "egateDTE";
                                string parametros = "";
                                if (comando == "XML")
                                {
                                    origen = origen.Replace("_sign.xml", ".xml");
                                    parametros += " -te bd -ts xml -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                    parametros += " -s " + origen;
                                    if (cert != "")
                                    {
                                        parametros += " -f ";
                                        parametros += " -acert " + cert.ToString();
                                        origen = origen.Replace(".xml", "_sign.xml");
                                    }
                                }
                                if (comando == "HTML" || comando == "PDF" || comando == "PDF-M")
                                {
                                    parametros += " -te bd -ts html -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                }
                                if (comando == "PDF-M")
                                {
                                    parametros += " -merit ";
                                }
                                if (prefijo == "ENVFIN")
                                {
                                    comando_bin = "egateEnvio";
                                    parametros += " -te bd -ts xml -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -corr " + Corr_envi.ToString();
                                    parametros += " -v -s " + origen;
                                    parametros += " -f -acert " + cert;
                                }
                                if (prefijo == "DTEMAIL_" || comando == "RESP1")
                                {
                                    comando_bin = "egateDescarga";
                                    parametros += " -te envio -ts DTEMAIL -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -corr " + Corr_envi.ToString();
                                }
                                if (prefijo == "RESP_ENVI_" || comando == "RESP2")
                                {
                                    comando_bin = "egateDescarga";
                                    parametros += " -te envio -ts RESP_ENVI -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -corr " + corr_envi1.ToString();
                                }
                                if (prefijo == "RESP_DTE_" || comando == "RESP3")
                                {
                                    comando_bin = "egateDescarga";
                                    parametros += " -te dte -ts RESP_DTE -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                }

                                if (parametros != "")
                                {
                                    lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                    lbMensaje.Text = "";
                                }

                                try
                                {
                                    if (flag_xslt && (comando == "RESP1" || comando == "RESP2" || comando == "RESP3"))
                                    {
                                        if (comando == "RESP1")
                                        {
                                            archivoXslt = Path.Combine(basedir, "librerias/xslt/dtemail.xslt");
                                            origenXml = drSalidas[0].ToString() + sep + dir_arch + sep + "DTEMAIL_" + Corr_envi + ".xml";
                                            destinoHTML = drSalidas[0].ToString() + sep + dir_arch + sep + "DTEMAIL_" + Corr_envi + ".html";
                                        }
                                        else if (comando == "RESP2")
                                        {
                                            archivoXslt = Path.Combine(basedir, "librerias/xslt/resp_envi.xslt");
                                            origenXml = drSalidas[0].ToString() + sep + dir_arch + sep + "RESP_ENVI_" + corr_envi1 + "_sign.xml";
                                            destinoHTML = drSalidas[0].ToString() + sep + dir_arch + sep + "RESP_ENVI_" + corr_envi1 + "_sign.html";
                                        }
                                        else if (comando == "RESP3")
                                        {
                                            string arch_resp;
                                            query = "SELECT ARCH_RESP FROM DTE_ENCA_DOCU WHERE foli_docu = " + Foli_docu;
                                            salida = new DataTable();
                                            DbnetTool.ctrlSqlInjection(this.Page.Form);
                                            salida = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                            if (salida.Rows.Count > 0)
                                            {
                                                fila = salida.Rows[0];
                                                arch_resp = fila[0].ToString();
                                                archivoXslt = Path.Combine(basedir, "librerias/xslt/resp_dte.xslt");
                                                origenXml = arch_resp;
                                                destinoHTML = arch_resp.Substring(0, arch_resp.Length - 4) + ".html";
                                            }
                                            else
                                            {
                                                lbMensaje.Text += "<br>" + "No existe el archivo de respuesta";
                                                MsgError(1);
                                            }
                                        }
                                        if (File.Exists(origenXml))
                                        {
                                            lbMensaje.Text += DbnetTool.ProcesaXslt(origenXml, archivoXslt, destinoHTML);
                                            lbMensaje.Text = "";
                                        }
                                        else
                                        {
                                            lbMensaje.Text = "Imposible Recuperar Archivo " + origenXml;
                                            MsgError(1);
                                        }
                                    }//fin if flagxslt	

                                    File.Copy(origen, cache + archivo, true);
                                    // Si es HTML debe copiar timbre, el logo debe estar en el directorio cache 
                                    if (comando == "HTML" && ind_ensa == "S")
                                        File.Copy(origen.ToLower().Replace(".html", ".jpg"), cache + archivo.ToLower().Replace(".html", ".jpg"), true);

                                    lbMensaje.Text = "";
                                    sw = true;
                                    pScript = "<script type=\"text/javascript\"> " +
                                        "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                        "</script>";
                                    ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                                }

                                catch (Exception eex)
                                {
                                    DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                          "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                          "pproc_erro", "Monitor", "VarChar", 50, "in",
                                          "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                          "pbin_erro", "WEB", "VarChar", 50, "in",
                                          "p_mensaje", "", "VarChar", 200, "out");
                                    chkDespliega.Checked = true;
                                    lbMensaje.Enabled = true;
                                    lbMensaje.Visible = true;
                                    lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                    lbEx.Text = sp.return_String("p_mensaje");
                                }
                            }// FIN IF FILTRO DTE  
                            else if (this.DbnetContext.TipoMonitor == "DTO" && dtobd == "1")
                            {
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
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                    parametros += " -re " + Rutt_emis.ToString();
                                    parametros += " -corr " + Corr_envi.ToString();
                                }
                                lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                lbMensaje.Text = "";

                                try
                                {
                                    if (File.Exists(origen))
                                    {
                                        File.Copy(origen, cache + archivo, true);
                                        // Si es HTML debe copiar timbre, el logo debe estar en el directorio cache 
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
                                    DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                           "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                           "pproc_erro", "Monitor", "VarChar", 50, "in",
                                           "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                           "pbin_erro", "WEB", "VarChar", 50, "in",
                                           "p_mensaje", "", "VarChar", 200, "out");


                                    chkDespliega.Checked = true;
                                    lbMensaje.Enabled = true;
                                    lbMensaje.Visible = true;
                                    lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                    lbEx.Text = sp.return_String("p_mensaje");
                                }
                            }
                            else
                            { }
                        }
                    }
                    break;
                case "TRAZO":
                    if (!FormatoRespuesta.Checked)
                        flag_xslt = true;
                    else
                        flag_xslt = false;

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

                    prefijo = "";
                    if (comando == "ARHE" && this.DbnetContext.TipoMonitor == "DTE")
                    {
                        if (archivo.Substring(0, 9) == "RESP_DTE_")
                            prefijo = archivo.Substring(0, 9);
                        if (archivo.Substring(0, 8) == "DTEMAIL_")
                            prefijo = archivo.Substring(0, 8);
                        if (archivo.Substring(0, 10) == "RESP_ENVI_")
                            prefijo = archivo.Substring(0, 10);
                        if (archivo.Substring(0, 6) == "ENVFIN")
                            prefijo = archivo.Substring(0, 6);
                    }

                    if (ind_ensa == "S")
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                    else
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";

                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
                        {
                            if (flag_xslt && (comando == "RESP1" || comando == "RESP2" || comando == "RESP3"))
                                origen = drSalidas[0].ToString() + sep + dir_arch + sep + archivo;
                            else
                                origen = drSalidas[0].ToString() + sep + dir_arch + sep + archivo;
                        }
                        else if (dir_arch != "" && dir_arch != " ")
                            origen = dir_arch + sep + archivo;
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
                            if (flag_xslt && (comando == "RESP1" || comando == "RESP2" || comando == "RESP3" || comando == "XML"))
                                File.Copy("nada", cache + archivo, true);
                            else
                                File.Copy(origen, cache + archivo, true);

                            // Si es HTML debe copiar timbre, el logo debe estar en el directorio cache 
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

                            if (this.DbnetContext.TipoMonitor == "DTE")
                            {
                                // Obtener Certificado para firmar  
                                if (DbnetGlobal.Base_dato == "SQLSERVER")
                                    query = "SELECT ARCH_CERT FROM DTE_CERT_PERS C,DTE_AUTO_PERS A WHERE A.CODI_EMPR = " + DbnetContext.Codi_empr + " AND A.TIPO_AUTO = 'ENV' AND A.DEFE_AUTO = 1 AND A.CODI_PERS = C.CODI_PERS AND GETDATE() BETWEEN C.FEIN_CERT AND C.FETE_CERT";
                                else
                                    query = "SELECT ARCH_CERT FROM DTE_CERT_PERS C,DTE_AUTO_PERS A WHERE A.CODI_EMPR = " + DbnetContext.Codi_empr + " AND A.TIPO_AUTO = 'ENV' AND A.DEFE_AUTO = 1 AND A.CODI_PERS = C.CODI_PERS AND SYSDATE BETWEEN C.FEIN_CERT AND C.FETE_CERT";

                                DataRow fila;
                                DataTable salida = new DataTable();
                                salida = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                if (salida.Rows.Count > 0)
                                {
                                    fila = salida.Rows[0];
                                    cert = fila[0].ToString();
                                }
                                else
                                {
                                    lbMensaje.Text += "No se encontro Certificado Vigente." + "<br>";
                                    cert = "";
                                }

                                string Foli_docu = drSelect["FOLI_DOCU"].ToString();
                                string Tipo_docu = drSelect["TIPO_DOCU"].ToString();
                                string Corr_envi = drSelect["CORR_ENVI"].ToString();
                                string Rutt_emis = drSelect["RUTT_EMIS"].ToString();
                                string corr_envi1 = "";
                                query = "select corr_envi1 from dte_enca_docu where codi_empr = " + DbnetContext.Codi_empr + " and tipo_docu = " + Tipo_docu + " and foli_docu = " + Foli_docu;
                                salida = new DataTable();
                                DbnetTool.ctrlSqlInjection(this.Page.Form);
                                salida = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                if (salida.Rows.Count > 0)
                                {
                                    fila = salida.Rows[0];
                                    corr_envi1 = fila[0].ToString();
                                }

                                int tam = origen.Length;
                                origen = origen.Substring(0, tam - 4) + origen.Substring(tam - 4, 4).ToLower();

                                string comando_bin = "egateDTE";
                                string parametros = "";
                                if (comando == "XML")
                                {
                                    origen = origen.Replace("_sign.xml", ".xml");
                                    parametros += " -te bd -ts xml -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                    parametros += " -s " + origen;
                                    if (cert != "")
                                    {
                                        parametros += " -f ";
                                        parametros += " -acert " + cert.ToString();
                                        origen = origen.Replace(".xml", "_sign.xml");
                                    }
                                }
                                if (comando == "HTML" || comando == "PDF" || comando == "PDF-M")
                                {
                                    parametros += " -te bd -ts html -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                }
                                if (comando == "PDF-M")
                                {
                                    parametros += " -merit ";
                                }
                                if (prefijo == "ENVFIN")
                                {
                                    comando_bin = "egateEnvio";
                                    parametros += " -te bd -ts xml -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -corr " + Corr_envi.ToString();
                                    parametros += " -v -s " + origen;
                                    parametros += " -f -acert " + cert;
                                }
                                if (prefijo == "DTEMAIL_" || comando == "RESP1")
                                {
                                    comando_bin = "egateDescarga";
                                    parametros += " -te envio -ts DTEMAIL -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -corr " + Corr_envi.ToString();
                                }
                                if (prefijo == "RESP_ENVI_" || comando == "RESP2")
                                {
                                    comando_bin = "egateDescarga";
                                    parametros += " -te envio -ts RESP_ENVI -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -corr " + corr_envi1.ToString();
                                }
                                if (prefijo == "RESP_DTE_" || comando == "RESP3")
                                {
                                    comando_bin = "egateDescarga";
                                    parametros += " -te dte -ts RESP_DTE -tl 3 -empr " + DbnetContext.Codi_empr;
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                }

                                if (parametros != "")
                                    DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);

                                try
                                {
                                    if (flag_xslt && (comando == "RESP1" || comando == "RESP2" || comando == "RESP3"))
                                    {
                                        comando_bin = "procesaXslt.bat ";
                                        parametros = DbnetTool.SelectInto(DbnetContext.dbConnection, "select '%'+param_value+'%' from sys_param where  param_name='EGATE_HOME'");
                                        if (comando == "RESP1")
                                        {
                                            archivoXslt = Path.Combine(basedir, "librerias/xslt/dtemail.xslt");
                                            origenXml = drSalidas[0].ToString() + sep + dir_arch + sep + "DTEMAIL_" + Corr_envi + ".xml";
                                            destinoHTML = drSalidas[0].ToString() + sep + dir_arch + sep + "DTEMAIL_" + Corr_envi + ".html";
                                        }
                                        else if (comando == "RESP2")
                                        {
                                            archivoXslt = Path.Combine(basedir, "librerias/xslt/resp_envi.xslt");
                                            origenXml = drSalidas[0].ToString() + sep + dir_arch + sep + "RESP_ENVI_" + corr_envi1 + "_sign.xml";
                                            destinoHTML = drSalidas[0].ToString() + sep + dir_arch + sep + "RESP_ENVI_" + corr_envi1 + "_sign.html";
                                        }
                                        else if (comando == "RESP3")
                                        {
                                            string arch_resp;
                                            query = "SELECT ARCH_RESP FROM DTE_ENCA_DOCU WHERE foli_docu = " + Foli_docu;
                                            salida = new DataTable();
                                            DbnetTool.ctrlSqlInjection(this.Page.Form);
                                            salida = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                            if (salida.Rows.Count > 0)
                                            {
                                                fila = salida.Rows[0];
                                                arch_resp = fila[0].ToString();

                                                archivoXslt = Path.Combine(basedir, "librerias/xslt/resp_dte.xslt");
                                                origenXml = arch_resp;
                                                destinoHTML = arch_resp.Substring(0, arch_resp.Length - 4) + ".html";
                                            }
                                            else
                                            {
                                                lbMensaje.Text += "<br>" + "No existe el archivo de respuesta";
                                                MsgError(1);
                                            }
                                        }
                                        if (File.Exists(origenXml))
                                            DbnetTool.ProcesaXslt(origenXml, archivoXslt, destinoHTML);
                                        else
                                        {
                                            lbMensaje.Text = "Imposible Recuperar Archivo " + origenXml;
                                            MsgError(1);
                                        }
                                    }//fin if flagxslt	

                                    File.Copy(origen, cache + archivo, true);
                                    // Si es HTML debe copiar timbre, el logo debe estar en el directorio cache 
                                    if (comando == "HTML" && ind_ensa == "S")
                                        File.Copy(origen.ToLower().Replace(".html", ".jpg"), cache + archivo.ToLower().Replace(".html", ".jpg"), true);

                                    lbMensaje.Text = "";
                                    sw = true;
                                    pScript = "<script type=\"text/javascript\"> " +
                                        "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                        "</script>";
                                    ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                                }
                                catch (Exception eex)
                                {
                                    DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                          "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                          "pproc_erro", "Monitor.TRAZO()", "VarChar", 50, "in",
                                          "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                          "pbin_erro", "WEB", "VarChar", 50, "in",
                                          "p_mensaje", "", "VarChar", 200, "out");


                                    chkDespliega.Checked = true;
                                    lbMensaje.Enabled = true;
                                    lbMensaje.Visible = true;
                                    lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                    lbEx.Text = sp.return_String("p_mensaje");
                                }
                            }// FIN IF FILTRO DTE  
                            else if (this.DbnetContext.TipoMonitor == "DTO" && dtobd == "1")
                            {
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
                                    parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                    parametros += " -fdte " + Foli_docu.Trim().ToString();
                                    parametros += " -re " + Rutt_emis.ToString();
                                    parametros += " -corr " + Corr_envi.ToString();
                                }
                                lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                lbMensaje.Text = "";
                                try
                                {
                                    if (File.Exists(origen))
                                    {
                                        File.Copy(origen, cache + archivo, true);
                                        // Si es HTML debe copiar timbre, el logo debe estar en el directorio cache 
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
                                        lbMensaje.Text = "Respuesta no cargada <br>";
                                        DbnetTool.MsgAlerta("Respuesta no cargada", this.Page);
                                        MsgError(1);
                                    }
                                }
                                catch (Exception eex)
                                {
                                    DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                          "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                          "pproc_erro", "Monitor", "VarChar", 50, "in",
                                          "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                          "pbin_erro", "WEB", "VarChar", 50, "in",
                                          "p_mensaje", "", "VarChar", 200, "out");

                                    chkDespliega.Checked = true;
                                    lbMensaje.Enabled = true;
                                    lbMensaje.Visible = true;
                                    lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                    lbEx.Text = sp.return_String("p_mensaje");
                                }
                            }
                            else
                            {
                                DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                                       "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                                       "pproc_erro", "Monitor", "VarChar", 50, "in",
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
                    }
                    break;
                //respuesta del SII tipo_arch=DTEMAIL 
                case "RESP1":

                    ind_ensa = drSelect["ENSA_" + comando].ToString();
                    dir_arch = drSelect["DIRE_" + comando].ToString();
                    archivo = drSelect["ARCH_" + comando].ToString();
                    if (archivo.Length == 0)
                    {
                        lbMensaje.Text = "El Archivo no esta registrado en la base de datos";
                        MsgError(1);
                        break;
                    }
                    else
                        archivo = archivo.Substring(0, archivo.Length - 4) + archivo.Substring(archivo.Length - 4, 4).ToLower();
                    if (ind_ensa == "S")
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                    else
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";

                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
                        {
                            if (flag_xslt)
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
                            if (File.Exists(origen) && flag_xslt == false)
                            {
                                File.Copy(origen, cache + archivo, true);
                                lbMensaje.Text = "";
                                sw = true;
                                pScript = "<script type=\"text/javascript\"> " +
                                          "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                          "</script>";
                                ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                            }
                            else
                            {
                                //Obtener parametro para DTE
                                string dtebd = "0";
                                query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLBD'";
                                dtebd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);

                                //Obtener parametro para DTO
                                string dtobd = "0";
                                query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLDTO'";
                                dtobd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);

                                //Modifica DTE 
                                if (this.DbnetContext.TipoMonitor == "DTE")
                                //11-09-2012 
                                {
                                    tablaResp = "dte_envi_lob";
                                    tipo_arch = !FormatoRespuesta.Checked ? "DTEMAIL" : "xDTEMAIL";

                                    string pScriptResp1 = "";
                                    pScriptResp1 = "<script type=\"text/javascript\"> " +
                                                   "window.open(\"" + "../factura/facShowFiles.aspx?tabla=" + tablaResp + "&tipo_arch=" + tipo_arch + "&corr_envi=" + Corr_envi2 + "&codi_empr=" + DbnetContext.Codi_empr + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                                   "</script>";
                                    ClientScript.RegisterStartupScript(typeof(Page), "PaginaBitacora", pScriptResp1);
                                }// FIN IF FILTRO DTE  
                                //DTO  
                                else if (this.DbnetContext.TipoMonitor == "DTO" && dtobd == "1")
                                {
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
                                        parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                        parametros += " -fdte " + Foli_docu.Trim().ToString();
                                        parametros += " -re " + Rutt_emis.ToString();
                                        parametros += " -corr " + Corr_envi.ToString();
                                    }
                                    lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                    lbMensaje.Text = "";
                                    try
                                    {
                                        if (File.Exists(origen))
                                        {
                                            File.Copy(origen, cache + archivo, true);
                                            //Si es HTML debe copiar timbre, el logo debe estar en el directorio cache
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
                                        DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                               "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                               "pproc_erro", "Monitor", "VarChar", 50, "in",
                                               "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                               "pbin_erro", "WEB", "VarChar", 50, "in",
                                               "p_mensaje", "", "VarChar", 200, "out");

                                        chkDespliega.Checked = true;
                                        lbMensaje.Enabled = true;
                                        lbMensaje.Visible = true;
                                        lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                        lbEx.Text = sp.return_String("p_mensaje");
                                    }
                                }
                                else
                                {
                                    lbMensaje.Text += "El Archivo aún no se encuentra disponible <br>";
                                    MsgError(1);
                                }
                            }
                        }
                        catch (Exception)
                        { }
                    }
                    break;
                //respuesta tecnica 
                case "RESP2":
                    ind_ensa = drSelect["ENSA_" + comando].ToString();
                    dir_arch = drSelect["DIRE_" + comando].ToString();
                    archivo = drSelect["ARCH_" + comando].ToString();
                    if (archivo.Length == 0)
                    {
                        lbMensaje.Text = "El Archivo no esta registrado en la base de datos";
                        MsgError(1);
                        break;
                    }
                    else
                        archivo = archivo.Substring(0, archivo.Length - 4) + archivo.Substring(archivo.Length - 4, 4).ToLower();
                    if (ind_ensa == "S")
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                    else
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";

                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
                        {
                            if (flag_xslt)
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
                            if (File.Exists(origen) && flag_xslt == false)
                            {
                                File.Copy(origen, cache + archivo, true);
                                lbMensaje.Text = "";
                                sw = true;
                                pScript = "<script type=\"text/javascript\"> " +
                                          "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                          "</script>";
                                ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                            }
                            else
                            {
                                //Obtener parametro para DTE
                                string dtebd = "0";
                                query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLBD'";
                                dtebd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
                                //Obtener parametro para DTO
                                string dtobd = "0";
                                query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLDTO'";
                                dtobd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
                                //******  DTE  ******** 
                                if (this.DbnetContext.TipoMonitor == "DTE")
                                {
                                    string Foli_docu2 = drSelect["FOLI_DOCU"].ToString();
                                    string Tipo_docu2 = drSelect["TIPO_DOCU"].ToString();

                                    query = "select corr_envi1 from dte_enca_docu where codi_empr = " + DbnetContext.Codi_empr + " and tipo_docu = " + Tipo_docu2 + " and foli_docu = " + Foli_docu2;
                                    DataRow fila;
                                    DataTable salida = new DataTable();
                                    salida = new DataTable();
                                    salida = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                    if (salida.Rows.Count > 0)
                                    {
                                        fila = salida.Rows[0];
                                        p_corr_envi1 = fila[0].ToString();
                                    }
                                    tablaResp = "dte_envi_lob";
                                    tipo_arch = !FormatoRespuesta.Checked ? "RESP_TECN" : "xRESP_TECN";
                                    string pScriptResp1 = "";
                                    pScriptResp1 = "<script type=\"text/javascript\"> " +
                                                   "window.open(\"" + "../factura/facShowFiles.aspx?tabla=" + tablaResp + "&tipo_arch=" + tipo_arch + "&corr_envi1=" + p_corr_envi1 + "&codi_empr=" + DbnetContext.Codi_empr + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                                   "</script>";
                                    ClientScript.RegisterStartupScript(typeof(Page), "PaginaBitacora", pScriptResp1);
                                }// FIN IF FILTRO DTE  
                                //DTO          
                                else if (this.DbnetContext.TipoMonitor == "DTO" && dtobd == "1")
                                {
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
                                        parametros += " -tdte " + Tipo_docu.Trim().ToString();
                                        parametros += " -fdte " + Foli_docu.Trim().ToString();
                                        parametros += " -re " + Rutt_emis.ToString();
                                        parametros += " -corr " + Corr_envi.ToString();
                                    }
                                    lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                    lbMensaje.Text = "";
                                    try
                                    {
                                        if (File.Exists(origen))
                                        {
                                            File.Copy(origen, cache + archivo, true);
                                            // Si es HTML debe copiar timbre, el logo debe estar en el directorio cache 
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
                                        DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                               "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                               "pproc_erro", "Monitor", "VarChar", 50, "in",
                                               "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                               "pbin_erro", "WEB", "VarChar", 50, "in",
                                               "p_mensaje", "", "VarChar", 200, "out");
                                        chkDespliega.Checked = true;
                                        lbMensaje.Enabled = true;
                                        lbMensaje.Visible = true;
                                        lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                        lbEx.Text = sp.return_String("p_mensaje");
                                    }
                                }
                                else
                                {
                                    lbMensaje.Text += "El Archivo " + origen + " aún no se encuentra disponible <br>";
                                    MsgError(1);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;
                // respuesta comercial  
                case "RESP3":
                    ind_ensa = drSelect["ENSA_" + comando].ToString();
                    dir_arch = drSelect["DIRE_" + comando].ToString();
                    if (ind_ensa == "S")
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_SALI' ORDER BY PARAM_NAME";
                    else
                        query = "select param_value from sys_param where param_name = 'EGATE_DIRE_ENTR' ORDER BY PARAM_NAME";

                    dtSalidas = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                    for (int i = 0; (i < dtSalidas.Rows.Count) && (sw == false); i++)
                    {
                        DataRow drSalidas = dtSalidas.Rows[i];
                        if (dir_arch == "xml_con" || dir_arch == "xml_sii" || dir_arch == "xml_res_sii" || dir_arch == "xml" || dir_arch == "pdf" || dir_arch == "html" || dir_arch == "flat")
                        {
                            if (flag_xslt)
                            {
                                archivo = archivo.Replace(".xml", ".html");
                                origen = drSalidas[0].ToString() + sep + dir_arch + sep + archivo;
                            }
                            else
                                origen = drSalidas[0].ToString() + sep + dir_arch + sep + archivo;
                        }
                        else if (dir_arch != "" && dir_arch != " ")
                            origen = dir_arch + sep + archivo;
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
                            if (File.Exists(origen) && flag_xslt == false)
                            {
                                File.Copy(origen, cache + archivo, true);
                                lbMensaje.Text = "";
                                sw = true;
                                pScript = "<script type=\"text/javascript\"> " +
                                          "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                          "</script>";
                                ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
                            }
                            else
                            {
                                //Obtener parametro para DTE
                                string dtebd = "0";
                                query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLBD'";
                                dtebd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);

                                //Obtener parametro para DTO
                                string dtobd = "0";
                                query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_XMLDTO'";
                                dtobd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);

                                //CAMBIO DTE 
                                if (this.DbnetContext.TipoMonitor == "DTE")
                                //11-09-2012 
                                {
                                    tablaResp = "dte_docu_lob";
                                    tipo_arch = !FormatoRespuesta.Checked ? "RESP_COME" : "xRESP_COME";

                                    string Foli_docu2 = drSelect["FOLI_DOCU"].ToString().Trim();
                                    string Tipo_docu2 = drSelect["TIPO_DOCU"].ToString().Trim();


                                    string pScriptResp1 = "";
                                    pScriptResp1 = "<script type=\"text/javascript\"> " +
                                                   "window.open(\"" + "../factura/facShowFiles.aspx?tabla=" + tablaResp + "&tipo_arch=" + tipo_arch + "&tipo_docu=" + Tipo_docu2 + "&foli_docu=" + Foli_docu2 + "&codi_empr=" + DbnetContext.Codi_empr + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                                   "</script>";
                                    ClientScript.RegisterStartupScript(typeof(Page), "PaginaBitacora", pScriptResp1);

                                }// FIN IF FILTRO DTE  
                                //DTO FILTRO   
                                else if (this.DbnetContext.TipoMonitor == "DTO" && dtobd == "1")
                                {
                                    string Foli_docu = drSelect["FOLI_DOCU"].ToString().Trim();
                                    string Tipo_docu = drSelect["TIPO_DOCU"].ToString().Trim();
                                    string Corr_envi = drSelect["CORR_ENVI"].ToString().Trim();
                                    string Rutt_emis = drSelect["RUTT_EMIS"].ToString().Trim();
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
                                        parametros += " -corr " + Corr_envi.ToString().Trim();
                                    }
                                    else if (comando == "PDF" || comando == "HTML")
                                    {
                                        comando_bin = "egateDTE";
                                        parametros = " -te dto -ts html -tl 3";
                                        parametros += " -empr " + DbnetContext.Codi_empr;
                                        parametros += " -tdte " + Tipo_docu.Trim().ToString().Trim();
                                        parametros += " -fdte " + Foli_docu.Trim().ToString().Trim();
                                        parametros += " -re " + Rutt_emis.ToString();
                                        parametros += " -corr " + Corr_envi.ToString();
                                    }
                                    if (!string.IsNullOrEmpty(parametros))
                                    {
                                        DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, comando_bin, parametros);
                                    }
                                    try
                                    {
                                        if (File.Exists(origen))
                                        {
                                            File.Copy(origen, cache + archivo, true);
                                            // Si es HTML debe copiar timbre, el logo debe estar en el directorio cache 
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
                                        DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                                "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                                "pproc_erro", "Monitor", "VarChar", 50, "in",
                                                "pmsaj_erro", eex.Message, "VarChar", 150, "in",
                                                "pbin_erro", "WEB", "VarChar", 50, "in",
                                                "p_mensaje", "", "VarChar", 200, "out");
                                        chkDespliega.Checked = true;
                                        lbMensaje.Enabled = true;
                                        lbMensaje.Visible = true;
                                        lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                                        lbEx.Text = sp.return_String("p_mensaje");
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        { }
                    }
                    break;
                case "DETA":
                    //Obtener parametro para DTE
                    string xmlbd = "0";
                    query = "SELECT PARAM_VALUE FROM SYS_PARAM WHERE PARAM_NAME = 'EGATE_NORBD'";
                    xmlbd = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
                    if (xmlbd == "1")
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
                            "window.open(\"../" + lis.dtMantenedor + "\" , \"_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                            "</script>";
                        ClientScript.RegisterStartupScript(typeof(Page), "PaginaDetalle", pScript);
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
                        DbnetContext.Val5 = "S";
                        pScript = "<script type=\"text/javascript\"> " +
                            "window.open(\"../" + lis.dtMantenedor + "\" , \"_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=no\");" +
                            "</script>";
                        ClientScript.RegisterStartupScript(typeof(Page), "PaginaDetalle", pScript);
                    }
                    break;
                case "HELP":
                    if (drSelect["FASE_HELP"].ToString().Trim().Length == 0)
                    {
                        DbnetTool.MsgError("Respuesta no cargada", this.Page);
                        lbMensaje.Text = "Respuesta no cargada <br>";
                        MsgError(1);
                    }
                    else
                        DbnetTool.MsgError(drSelect["FASE_HELP"].ToString(), this.Page);
                    break;
                case "RESP_RECI":
                    string sCorrDocu = string.Empty;
                    string pScriptReci = string.Empty;
                    string sParametros = string.Empty;
                    tipo_arch = !FormatoRespuesta.Checked ? "RESP_RECI" : "xRESP_RECI";
                    if (this.DbnetContext.TipoMonitor.Equals("DTE"))
                    {
                        string Foli_docu = drSelect["FOLI_DOCU"].ToString().Trim();
                        string Tipo_docu = drSelect["TIPO_DOCU"].ToString().Trim();
                        sParametros = string.Format("tipo_arch={0}&tipo_moni={1}&tipo_docu={2}&foli_docu={3}&tabla=dte_docu_lob", tipo_arch, this.DbnetContext.TipoMonitor, Tipo_docu, Foli_docu);
                        
                    }
                    else
                    {
                        sCorrDocu = drSelect["CORR_DOCU"].ToString();
                        sParametros = string.Format("tipo_arch={0}&tipo_moni={1}&corr_docu={2}&tabla=dto_docu_lob", tipo_arch, this.DbnetContext.TipoMonitor, sCorrDocu);                        
                    }
                    pScriptReci = string.Format("<script type=\"text/javascript\"> window.open(\"" + "../factura/facShowFiles.aspx?{0}\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\"); </script>",
                                                 sParametros);
                    ClientScript.RegisterStartupScript(typeof(Page), "RECIBO-DTO", pScriptReci);
                    break;
                case "ADJ":
                    tablaResp = "dte_arch_adju";
                    tipo_arch = "ADJU_PDF";
                    string sTipoDocu = drSelect["TIPO_DOCU"].ToString().Trim();
                    string sFoliDocu = drSelect["FOLI_DOCU"].ToString().Trim();

                    string sQuery = string.Format("select {0} {1}(NMBF_ADJU,' ') FROM DTE_ARCH_ADJU WHERE CODI_EMPR={2} AND TIPO_DOCU={3} AND FOLI_DOCU={4} {5}",
                        DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "TOP 1" : "", DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "ISNULL" : "NVL", DbnetContext.Codi_empr.ToString(), sTipoDocu, sFoliDocu, DbnetGlobal.Base_dato.Equals("ORACLE")?"AND ROWNUM <2":"");
                    string sRuta = DbnetTool.SelectInto(DbnetContext.dbConnection, sQuery);
                    if (!string.IsNullOrEmpty(sRuta))
                    {
                        string sNombre = System.IO.Path.GetFileName(sRuta);
                        File.Delete(cache + sNombre);
                        if (File.Exists(sRuta))
                        { File.Copy(sRuta, cache + sNombre); Thread.Sleep(1500); }
                        if (File.Exists(cache + sNombre))
                            DespliegaPDF(sNombre);
                    }
                    else
                        DbnetTool.MsgAlerta("No existen adjuntos asociados al folio seleccionado.", this.Page);
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                            "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                            "pproc_erro", "Monitor. GrillaComando()", "VarChar", 50, "in",
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
    private void DespliegaPDF(string archivo)
    {
        string pScript = "<script type=\"text/javascript\"> " +
                                             "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                             "</script>";
        ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
    }
    protected void btBuscar_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        this.DbnetContext.regpag = Convert.ToInt16(txtRegPag.SelectedValue);
        if (lis.Status)
        {
            buscar();
        }
    }
    protected void lvTipo_docu_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Tipo_docu.Text = lvTipo_docu.SelectedValue;
    }
    protected void Tipo_docu_TextChanged(object sender, System.EventArgs e)
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
    protected void Codi_sucu_TextChanged(object sender, System.EventArgs e)
    {
        try
        {
            lvCodi_sucu.Query = qryCodi_sucu;
            lvCodi_sucu.Rescata(DbnetContext.dbConnection);
            lvCodi_sucu.Selecciona(Codi_sucu.Text);
        }
        catch
        { }
    }
    protected void lvCodi_sucu_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Codi_sucu.Text = lvCodi_sucu.SelectedValue;
    }
    protected void txtRegPag_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        btBuscar_Click(null, null);
    }
    protected void barExcel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        // Export the details of specified columns
        CultureInfo ci;
        string lang = Request.UserLanguages[0];
        ci = CultureInfo.CreateSpecificCulture(lang);
        Thread.CurrentThread.CurrentCulture = ci;
        string punto_decimal;
        punto_decimal = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
        try
        {
            if (DbnetContext.dtSelect.Rows.Count == 0)
                buscar();
            DataTable dtExcel = new DataTable();
            dtExcel = DbnetContext.dtSelect.Copy();
            int iLargoEnc = lis.dtEncabezados.Count;
            if (dtExcel.Rows.Count > 0)
            {
                for (int i = 0; i < iLargoEnc; i++)
                {
                    DataRowView drEncabezado = lis.dtEncabezados[i];
                    int iColumns = dtExcel.Columns.Count;
                    for (int x = 0; x < iColumns; x++)
                    {
                        if (dtExcel.Columns[x].ColumnName.ToUpper().Equals(drEncabezado["Codigo"].ToString().ToUpper()))
                        {
                            dtExcel.Columns[x].ColumnName = drEncabezado["Descripcion"].ToString().Replace(" ", ".");
                            break;
                        }
                    }
                    string sFormato = drEncabezado["Formato"].ToString();
                    string sDescripcion = drEncabezado["Descripcion"].ToString().Replace(" ", ".");
                    if (!string.IsNullOrEmpty(sFormato))
                    {
                        int iLargo = dtExcel.Rows.Count;
                        for (int x = 0; x < iLargo; x++)
                        {
                            string sTotal = dbnFormat.Numero(dtExcel.Rows[x][sDescripcion].ToString(), sFormato);
                            decimal dTotal;
                            decimal.TryParse(sTotal, out dTotal);
                            dtExcel.Rows[x][sDescripcion] = dTotal;
                        }
                    }
                }

                if (this.DbnetContext.TipoMonitor.ToUpper().Equals("DTE"))
                {
                    if (dtExcel.Rows.Count > 0)
                    {
                        dtExcel.Columns.RemoveAt(0);
                        dtExcel.Columns.Remove("DESC_TIDO");
                        dtExcel.Columns.Remove("ESTA_DOCU");
                        dtExcel.Columns.Remove("Correlativo");
                        dtExcel.Columns.Remove("RUTT_EMIS");
                        dtExcel.Columns.Remove("NOMB_EMIS");
                        dtExcel.Columns.Remove("RUTT_RECE");
                        if (DbnetTool.SelectInto(DbnetContext.dbConnection, "select param_value from sys_param where param_name = 'EGATE_FOLI_CLIE'") == "0")
                            dtExcel.Columns.Remove("FOLIO-ERP");
                        dtExcel.Columns.Remove("CORR_ENVI1");
                        dtExcel.Columns.Remove("Estado.Env");
                        dtExcel.Columns.Remove("Envio.Sii");
                        dtExcel.Columns.Remove("Envio.Contribuyente");
                        dtExcel.Columns.Remove("Resp.Tecnica");
                        dtExcel.Columns.Remove("Resp.Comercial");
                        dtExcel.Columns.Remove("COLOR_FASE");
                        dtExcel.Columns.Remove("Estado.Doc");
                        dtExcel.Columns.Remove("RTDG_EMIS");
                        dtExcel.Columns.Remove("ENSA_ARHE");
                        dtExcel.Columns.Remove("DIRE_ARHE");
                        dtExcel.Columns.Remove("ARCH_ARHE");
                        dtExcel.Columns.Remove("FASE_HELP");
                        dtExcel.Columns.Remove("ENSA_PDF");
                        dtExcel.Columns.Remove("DIRE_PDF");
                        dtExcel.Columns.Remove("ARCH_PDF");
                        dtExcel.Columns.Remove("ENSA_HTML");
                        dtExcel.Columns.Remove("ARCH_HTML");
                        dtExcel.Columns.Remove("ENSA_XML");
                        dtExcel.Columns.Remove("DIRE_XML");
                        dtExcel.Columns.Remove("ARCH_XML");
                        dtExcel.Columns.Remove("ENSA.RESP1");
                        dtExcel.Columns.Remove("DIRE_RESP1");
                        dtExcel.Columns.Remove("ENSARESP2");
                        dtExcel.Columns.Remove("DIRE_RESP2");
                        dtExcel.Columns.Remove("ARCH_RESP2");
                        dtExcel.Columns.Remove("ENSA.RESP3");
                        dtExcel.Columns.Remove("DIRE_RESP3");
                        dtExcel.Columns.Remove("ARCH_RESP3");
                        dtExcel.Columns.Remove("ARCH_COMER");
                        dtExcel.Columns.Remove("Fecha_Recibo");
                        dtExcel.Columns.Remove("Carga");
                        dtExcel.Columns.Remove("DIRE_HTML");
                        dtExcel.Columns.Remove("ARCH_RESP1");
                        Export objExport = new Export("Web");
                        objExport.ExportDetails(dtExcel, Export.ExportFormat.Excel, "Listado.xls");
                    }
                    else
                    { DbnetTool.MsgAlerta("No existen datos a exportar.", this.Page); }
                }
                else
                {
                    if (dtExcel.Rows.Count > 0)
                    {
                        dtExcel.Columns.Remove("TIPO");
                        dtExcel.Columns.Remove("desc_tido");
                        dtExcel.Columns.Remove("ESTA_DOCU");
                        dtExcel.Columns.Remove("rutt_emis");
                        dtExcel.Columns.Remove("Emision");
                        dtExcel.Columns.Remove("Total");
                        dtExcel.Columns.Remove("corr_envi");
                        dtExcel.Columns.Remove("Carga");
                        dtExcel.Columns.Remove("Envio.SII");
                        dtExcel.Columns.Remove("Envio.Contribuyente");
                        dtExcel.Columns.Remove("COLOR_FASE");
                        dtExcel.Columns.Remove("ENSA_ARHE");
                        dtExcel.Columns.Remove("FASE_HELP");
                        dtExcel.Columns.Remove("ENSA_XML");
                        dtExcel.Columns.Remove("DIRE_XML");
                        dtExcel.Columns.Remove("ARCH_XML");
                        dtExcel.Columns.Remove("ENSA_HTML");
                        dtExcel.Columns.Remove("DIRE_HTML");
                        dtExcel.Columns.Remove("ARCH_HTML");
                        dtExcel.Columns.Remove("ENSA_PDF");
                        dtExcel.Columns.Remove("DIRE_PDF");
                        dtExcel.Columns.Remove("ARCH_PDF");
                        dtExcel.Columns.Remove("ENSA_RESP2");
                        dtExcel.Columns.Remove("DIRE_RESP2");
                        dtExcel.Columns.Remove("ARCH_RESP2");
                        dtExcel.Columns.Remove("ENSA_RESP3");
                        dtExcel.Columns.Remove("DIRE_RESP3");
                        dtExcel.Columns.Remove("ARCH_RESP3");
                        dtExcel.Columns.Remove("CORR_DOCU");
                        dtExcel.Columns.Remove("CODI_ESAP");
                        dtExcel.Columns.Remove("ARCH_ARHE");
                        dtExcel.Columns.Remove("DIRE_ARHE");
                        dtExcel.Columns.Remove("USUARIO");
                        Export objExport = new Export("Web");
                        objExport.ExportDetails(dtExcel, Export.ExportFormat.Excel, "Listado.xls");
                    }
                    else
                    { DbnetTool.MsgAlerta("No existen datos a exportar.", this.Page); }
                }
            }
        }
        catch (Exception Ex)
        {
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                   "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                   "pproc_erro", "Monitor: export excel.", "VarChar", 50, "in",
                                   "pmsaj_erro", Ex.Message, "VarChar", 150, "in",
                                   "pbin_erro", "WEB", "VarChar", 50, "in",
                                   "p_mensaje", "", "VarChar", 200, "out");
            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = sp.return_String("p_mensaje");
        }
    }
    private bool isNumeric(string sTexto)
    {
        bool bFlag = true;
        int iValor = 0;
        try
        {
            iValor = Convert.ToInt32(sTexto);
        }
        catch (Exception)
        {
            bFlag = false;
        }
        return bFlag;
    }
    protected void ChkFoli_clie_CheckedChanged(object sender, System.EventArgs e)
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
    protected void todo_chec_CheckedChanged(object sender, EventArgs e)
    {
        int iDx = 0, iOkList = 0;
        string evenRecl = "0";	/* OT 9376978 03-05-2017|AM */
        string fechRSII = "0";
        string docuEscd = "0";	/* OT 9376978 11-05-2017|AM */

        List<FacListMonitorServicioDTE> _oListaMonitorServicio;
        if (HttpContext.Current.Session["oListaMonitor"] == null)
            _oListaMonitorServicio = new List<FacListMonitorServicioDTE>();
        else
            _oListaMonitorServicio = (List<FacListMonitorServicioDTE>)HttpContext.Current.Session["oListaMonitor"];

        foreach (DataGridItem com in Grilla2.Items)
        {
            CheckBox gril_chec = (CheckBox)com.FindControl("selection");
            if (_oListaMonitorServicio != null)
            {
                var dbnetContext = (DbnetSesion)HttpContext.Current.Session["contexto"];
                var oDataDt = dbnetContext.dtGrilla;
                var oDataSelect = dbnetContext.dtSelect;
                var oResultado = oDataDt.Rows[iDx];
                string foliDocu = string.Empty;
                string foliDocuDto = string.Empty;
                if (tipoDocumento.ToUpper().Equals("DTO"))
                {
                    foliDocuDto = oResultado["Folio-ERP"].ToString();
                    foliDocuDto = foliDocuDto.Substring(foliDocuDto.IndexOf("'>") + 2);
                    foliDocuDto = foliDocuDto.Substring(0, foliDocuDto.IndexOf("</"));
                }

                foliDocu = oResultado["Folio"].ToString();
                try
                {
                    foliDocu = foliDocu.Substring(foliDocu.IndexOf("'>") + 2);
                    foliDocu = foliDocu.Substring(0, foliDocu.IndexOf("</"));
                }
                catch
                { foliDocu = oResultado[3].ToString(); }
                string tipoDocu = oResultado["Tipo"].ToString();
                string corrDocu = string.Empty;
                string codiEsap = string.Empty;
                string codiReme = string.Empty;
                string estaDocu = string.Empty;
                string ruttEmis = "0";
                if (Request.Params["par1"].ToString().ToUpper().Equals("DTO"))
                {
                    string sRut = oResultado["Rut"].ToString();
                    sRut = sRut.Substring(sRut.IndexOf("'>") + 2);
                    sRut = sRut.Substring(0, sRut.IndexOf("-"));
                    foreach (DataRow item in oDataSelect.Rows.Cast<DataRow>().Where(item => item["RUTT_EMIS"].ToString() == sRut && item["FOLI_DOCU"].ToString() == foliDocuDto && item["TIPO_DOCU"].ToString() == tipoDocu))
                    { corrDocu = item["CORR_DOCU"].ToString(); codiEsap = item["CODI_ESAP"].ToString(); codiReme = item["CODI_REME"].ToString(); estaDocu = item["ESTA_DOCU"].ToString(); ruttEmis = item["RUTT_EMIS"].ToString(); evenRecl = item["EVEN_RECL"].ToString();  }
                }
                if (Request.Params["par1"].ToString().ToUpper().Equals("DTE"))
                {
                    string sQuery = string.Format("select esta_docu, even_recl, docu_escd from dte_enca_docu where codi_empr = {0} and foli_docu = {1} and tipo_docu = {2} ", dbnetContext.Codi_empr, foliDocu, tipoDocu);	/* OT 9376978 03-05-2017|AM */
                    oDataSelect = DbnetTool.Ejecuta_Select(dbnetContext.dbConnection, sQuery);
                    estaDocu = oDataSelect.Rows[0]["ESTA_DOCU"].ToString();
                    evenRecl = oDataSelect.Rows[0]["EVEN_RECL"].ToString();
                    docuEscd = oDataSelect.Rows[0]["DOCU_ESCD"].ToString();
                    
                    //{ corrDocu = item["CORR_DOCU"].ToString(); codiEsap = item["CODI_ESAP"].ToString(); codiReme = item["CODI_REME"].ToString(); estaDocu = item["ESTA_DOCU"].ToString(); ruttEmis = item["RUTT_EMIS"].ToString(); }
                }
                if ((chkServicio.Checked) || (chkServicioReclamo.Checked)) // OT 9376978 05-05-2017|AM
                {
                    var oData = _oListaMonitorServicio.Count(
                                        x => x.CodiEmpr.Equals(dbnetContext.Codi_empr) &&
                                         x.FoliDocu.Equals(foliDocu) &&
                                         x.TipoDocu.Equals(tipoDocu) &&
                                         x.RuttEmis.Equals(ruttEmis));
                    if (oData == 0)
                    {
                        var oData2 = new FacListMonitorServicioDTE
                        {
                            CodiEmpr = dbnetContext.Codi_empr,
                            FoliDocu = foliDocu,
                            TipoDocu = tipoDocu,
                            CorrDocu = corrDocu,
                            CodiEsap = codiEsap,
                            CodiReme = codiReme,
                            EstaDocu = estaDocu,
                            RuttEmis = ruttEmis,
                            EvenRecl = evenRecl,
                            FechRSII = fechRSII, /* Agregar asignacion de campo EVEN_RECL y FECH_RECE_SII - OT 9376978 03-05-2017|AM */
                            DocuEscd = docuEscd  /* OT 9376978 03-05-2017|AM */
                        };
                        _oListaMonitorServicio.Add(oData2);
                        gril_chec.Checked = true;
                    }
                    else if (oData == 1)
                        iOkList++;
                }
                else
                {
                    var oData = _oListaMonitorServicio.FirstOrDefault(
                                        x => x.CodiEmpr.Equals(dbnetContext.Codi_empr) &&
                                             x.FoliDocu.Equals(foliDocu) &&
                                             x.TipoDocu.Equals(tipoDocu) &&
                                             x.RuttEmis.Equals(ruttEmis));
                    _oListaMonitorServicio.Remove(oData);
                    gril_chec.Checked = false;
                }
            }
            iDx++;
        }
        HttpContext.Current.Session["oListaMonitor"] = _oListaMonitorServicio;
    }
	
    protected void btnImprimirSel_Click(object sender, EventArgs e)
    {
        string foliDocu = string.Empty;
        string corr_docu = string.Empty;
        string query = string.Empty;
        string query2 = string.Empty;
        string queryInsert = string.Empty;
        try
        {
            if (lv_Servicio.SelectedIndex == 0)
            { lbMensaje.Text = "Debe ingresar Servicio"; }
            else
            {
                var _oListaMonitorServicio = (List<FacListMonitorServicioDTE>)Session["oListaMonitor"];
                if (_oListaMonitorServicio != null)
                {
                    if (_oListaMonitorServicio.Count > 0)
                    {
                        foreach (var item in _oListaMonitorServicio)
                        {
                            foliDocu = item.FoliDocu;
                            string tipo_docu = item.TipoDocu;
                            corr_docu = item.CorrDocu;

                            if (tipoDocumento.Equals("DTE"))
                            {
                                if (lv_Servicio.SelectedValue.ToLower().Contains("dte_envi_clie"))
                                {
                                    string Foli_docu2 = item.FoliDocu;
                                    string Tipo_docu2 = item.TipoDocu;
                                    string p_esta_docu = "";
                                    query = string.Format("select {3}(esta_docu,'ING'), {3}(corr_envi1,0) from dte_enca_docu where codi_empr = {0} and tipo_docu = {1} and foli_docu = {2}", DbnetContext.Codi_empr, Tipo_docu2, Foli_docu2, (DbnetGlobal.Base_dato.ToUpper().Equals("SQLSERVER")) ? "isnull" : "nvl");
                                    string nCorrEnvi = string.Empty;
                                    DataTable salida = new DataTable();
                                    salida = new DataTable();
                                    salida = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                    if (salida.Rows.Count > 0)
                                    {
                                        p_esta_docu = salida.Rows[0][0].ToString();
                                        nCorrEnvi = salida.Rows[0][1].ToString();
                                    }
                                    int iDx = (lv_Servicio.SelectedValue.ToLower().Contains("0")) ? 0 : (lv_Servicio.SelectedValue.ToLower().Contains("1")) ? 1 : 2;
                                    switch (p_esta_docu.ToUpper().Trim())
                                    {
                                        case "DOK":
                                        case "RLV":
                                        case "EPR":
                                        case "RPR":
                                            queryInsert = "INSERT INTO qse_docu_serv " +
                                                        "(CODI_SERV ,CODI_EMEX ,CODI_EMPR ,TIPO_DOCU ,FOLI_DOCU " +
                                                        ",ESTA_PROC ,TIME_PROC ,CANT_PROC ,CODI_DOCU " +
                                                        ",TABL_ORIG ,VALO_PK1  ,VALO_PK2  ,VALO_PK3 " +
                                                        ",VALO_PK4  ,VALO_PK5  ,TABL_ESTA , FLAG_DOCU) " +
                                                        "VALUES " +
                                                        "('dte_envi_clie', '{0}','{1}','{2}', '{3}' " +
                                                        ",'ING', {6}, 1, '{4}' " +
                                                        ",'DTE_ENCA_DOCU', '{0}', '{1}','{2}'" +
                                                        ",'{3}',{5}, 'OFF', '{7}')";
                                            if (DbnetGlobal.Base_dato == "SQLSERVER")
                                                queryInsert = string.Format(queryInsert, DbnetContext.Codi_emex, DbnetContext.Codi_empr, tipo_docu.Trim(),
                                                    foliDocu.Trim(), Request.Params["par1"], "substring(convert(char,getdate(),120),0,8)", "getdate()", iDx.ToString());
                                            else
                                                queryInsert = string.Format(queryInsert, DbnetContext.Codi_emex, DbnetContext.Codi_empr, tipo_docu.Trim(),
                                                    foliDocu.Trim(), Request.Params["par1"], "to_char(sysdate,'YYYY-MM')", "sysdate", iDx.ToString());
                                            DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, queryInsert);
                                            lbMensaje.Text = "Documento(s) enviado(s) al Servicio " +  (lv_Servicio.SelectedValue.Contains('-')? lv_Servicio.SelectedValue.Substring(0,lv_Servicio.SelectedValue.IndexOf('-')): lv_Servicio.SelectedValue).ToString();
                                            break;
                                        default:
                                            lbMensaje.Text = "Documento(s) no enviado. Estado documento(s) distinto de 'DOK', 'RLV, 'EPR', 'RPR'";
                                            break;
                                    }
                                }
                                else
                                {
                                    query = "INSERT INTO qse_docu_serv " +
                                            "(CODI_SERV ,CODI_EMEX ,CODI_EMPR ,TIPO_DOCU ,FOLI_DOCU " +
                                            ",ESTA_PROC ,TIME_PROC ,CANT_PROC ,CODI_DOCU " +
                                            ",TABL_ORIG ,VALO_PK1  ,VALO_PK2  ,VALO_PK3 " +
                                            ",VALO_PK4  ,VALO_PK5  ,TABL_ESTA) " +
                                            "VALUES " +
                                            "('dte_gen_pdf', '{0}','{1}','{2}', '{3}' " +
                                            ",'ING', {10}, 0, '{4}' " +
                                            ",'DTE_ENCA_DOCU', '{5}', '{6}','{7}'" +
                                            ",'{8}',{11},'{9}')";
                                    if (DbnetGlobal.Base_dato.Equals("SQLSERVER"))
                                    {
                                        string sQuery = "select top 1 isnull(nomb_cana,'GNPDF') as nomb_cana from dbq_cana where modo_cana = 'GNPDF'";
                                        var sNombCana = DbnetTool.SelectInto(DbnetContext.dbConnection, sQuery);

                                        query = string.Format(query, DbnetContext.Codi_emex, DbnetContext.Codi_empr, tipo_docu.Trim(),
                                                                foliDocu.Trim(), Request.Params["par1"], DbnetContext.Codi_emex, DbnetContext.Codi_empr,
                                                                tipo_docu.Trim(), foliDocu.Trim(), sNombCana, "getdate()", "substring(convert(char,getdate(),120),0,8)");
                                    }
                                    else
                                    {
                                        string sQuery = "select nvl(nomb_cana,'GNPDF') as nomb_cana from dbq_cana where modo_cana = 'GNPDF' and rownum < 2";
                                        var sNombCana = DbnetTool.SelectInto(DbnetContext.dbConnection, sQuery);
                                        query = string.Format(query, DbnetContext.Codi_emex, DbnetContext.Codi_empr, tipo_docu.Trim(),
                                                                foliDocu.Trim(), Request.Params["par1"], DbnetContext.Codi_emex, DbnetContext.Codi_empr,
                                                                tipo_docu.Trim(), foliDocu.Trim(), sNombCana, "sysdate", "to_char(sysdate,'YYYY-MM')");
                                    }
                                    DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                    lbMensaje.Text = "Documento(s) enviado(s) al Servicio " + lv_Servicio.SelectedValue;
                                }
                            }
                            else //DTO
                            {
                                query = "update dto_enca_docu_p set codi_serv ='{0}' " +
                                         "where codi_empr= '{1}' and foli_docu='{2}' and tipo_docu='{3}' and corr_docu='{4}'";
                                query = string.Format(query, lv_Servicio.SelectedValue.ToString(), DbnetContext.Codi_empr, foliDocu.Trim(),
                                                        tipo_docu.Trim(), corr_docu.Trim());
                                DbnetTool.ctrlSqlInjection(this.Page.Form);
                                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                                lbMensaje.Text = "Documento(s) enviado(s) al Servicio " + lv_Servicio.SelectedValue;
                            }
                        }
                        Session.Remove("oListaMonitor");
                    }
                    else { lbMensaje.Text = "Debe Seleccionar algun Documento"; }
                }
            } //fin del if
            if (!string.IsNullOrEmpty(lbMensaje.Text))
            {
                lbMensaje.Enabled = true;
                lbMensaje.Visible = true;
            }
        }
        catch (Exception ex)
        {
            string s1 = queryInsert;
            Session.Remove("oListaMonitor");
            DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                                 "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                                 "pproc_erro", "Menu: Monitor Servicios", "VarChar", 50, "in",
                                 "pmsaj_erro", ex.Message, "VarChar", 150, "in",
                                 "pbin_erro", "WEB", "VarChar", 50, "in",
                                 "p_mensaje", "", "VarChar", 200, "out");
            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = sp.return_String("p_mensaje") + "-Folio " + foliDocu.ToString() + s1;
        }
    }
    protected void Codi_pers_TextChanged(object sender, System.EventArgs e)
    {
        string queryAutoComplete2 = string.Empty;
        if (!string.IsNullOrEmpty(Codi_pers.Text))
        {
            DataTable table2 = new DataTable();
            if (DbnetContext.TipoMonitor.ToString() == "DTE")
            {
                queryAutoComplete2 = "select nomb_rece AS nomb_empr from dte_enca_docu where codi_empr= " +
                     DbnetContext.Codi_empr + " and codi_emex='" + DbnetContext.Codi_emex +
                     "' AND rutt_rece = '" + Codi_pers.Text +
                     "' GROUP BY nomb_rece ";
            }
            else
            {
                if (DbnetContext.TipoMonitor.ToString() == "DTO")
                {
                    queryAutoComplete2 = "select nomb_emis AS nomb_empr from dto_enca_docu_p where codi_empr= " +
                     DbnetContext.Codi_empr + " and codi_emex='" + DbnetContext.Codi_emex +
                     "' AND rutt_emis = '" + Codi_pers.Text +
                     "' GROUP BY nomb_emis ";
                }
            }
            DbnetTool.ctrlSqlInjection(this.Form);
            table2 = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, queryAutoComplete2);

            if (table2.Rows.Count != 0)
            {
                Filtro_Empresa.Text = table2.Rows[0][0].ToString();
            }
            else
            {
                Filtro_Empresa.Text = "";
            }
        }
        else
        {
            Filtro_Empresa.Text = "";
        }

    }
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string[] GetCompletionList(string prefixText, int count, string contextKey)
    {
        System.Data.OleDb.OleDbConnection ole = (System.Data.OleDb.OleDbConnection)HttpContext.Current.Session["coneccion"];
        string dteodto = (string)HttpContext.Current.Session["tipomonitor"];

        string[] resultados = new string[100];

        if (dteodto.ToString() == "DTE")
        {
            DataTable table = GetTable(ole, prefixText);
            DataTable dt = table;
            resultados = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                resultados[i] = dt.Rows[i][0].ToString();
            }
            return resultados;
        }
        else
        {
            if (dteodto.ToString() == "DTO")
            {
                DataTable table = GetTableDTO(ole, prefixText);
                DataTable dt = table;
                resultados = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    resultados[i] = dt.Rows[i][0].ToString();
                }
                return resultados;
            }
        }

        return resultados;
    }
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string[] GetCompletionList2(string prefixText, int count, string contextKey)
    {
        System.Data.OleDb.OleDbConnection ole = (System.Data.OleDb.OleDbConnection)HttpContext.Current.Session["coneccion"];
        string dteodto = (string)HttpContext.Current.Session["tipomonitor"];
        string[] resultados = new string[100];
        if (dteodto.ToString() == "DTE")
        {
            DataTable table = GetTable2(ole, prefixText);
            DataTable dt = table;
            resultados = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                resultados[i] = dt.Rows[i][0].ToString();
            }
            return resultados;
        }
        else
        {
            if (dteodto.ToString() == "DTO")
            {
                DataTable table = GetTable2DTO(ole, prefixText);
                DataTable dt = table;
                resultados = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    resultados[i] = dt.Rows[i][0].ToString();
                }
                return resultados;
            }
        }
        return resultados;
    }

    static DataTable GetTable(System.Data.OleDb.OleDbConnection ole, string prefijo)
    {
        string queryAutoComplete = string.Empty;
        System.Data.OleDb.OleDbConnection coneccion = ole;
        DataTable table = new DataTable();
        queryAutoComplete = "select nomb_rece AS nomb_empr from dte_enca_docu where codi_empr= " +
            codi_empr_auto + " and codi_emex='" + codi_emex_auto +
            "' AND nomb_rece LIKE ('%" + prefijo + "%')" +
            " GROUP BY nomb_rece ";
        table = DbnetTool.Ejecuta_Select(ole, queryAutoComplete);
        return table;
    }
    static DataTable GetTableDTO(System.Data.OleDb.OleDbConnection ole, string prefijo)
    {
        string queryAutoComplete = string.Empty;
        System.Data.OleDb.OleDbConnection coneccion = ole;
        DataTable table = new DataTable();
        queryAutoComplete = "select nomb_emis AS nomb_empr from dto_enca_docu_p where codi_empr= " +
            codi_empr_auto + " and codi_emex='" + codi_emex_auto +
            "' AND nomb_emis LIKE ('%" + prefijo + "%')" +
            " GROUP BY nomb_emis ";
        table = DbnetTool.Ejecuta_Select(ole, queryAutoComplete);
        return table;
    }
    static DataTable GetTable2(System.Data.OleDb.OleDbConnection ole, string prefijo)
    {
        string queryAutoComplete = string.Empty;
        System.Data.OleDb.OleDbConnection coneccion = ole;
        DataTable table = new DataTable();
        queryAutoComplete = "select rutt_rece AS rut_empr from dte_enca_docu where codi_empr= " +
             codi_empr_auto + " and codi_emex='" + codi_emex_auto +
             "' AND rutt_rece LIKE ('%" + prefijo + "%')" +
             " GROUP BY rutt_rece ";
        table = DbnetTool.Ejecuta_Select(ole, queryAutoComplete);
        return table;
    }
    static DataTable GetTable2DTO(System.Data.OleDb.OleDbConnection ole, string prefijo)
    {
        string queryAutoComplete = string.Empty;
        System.Data.OleDb.OleDbConnection coneccion = ole;
        DataTable table = new DataTable();
        queryAutoComplete = "select rutt_emis AS rut_empr from dto_enca_docu_p where codi_empr= " +
             codi_empr_auto + " and codi_emex='" + codi_emex_auto +
             "' AND rutt_emis LIKE ('%" + prefijo + "%')" +
             " GROUP BY rutt_emis ";
        table = DbnetTool.Ejecuta_Select(ole, queryAutoComplete);
        return table;
    }
    protected void Filtro_Empresa_TextChanged(object sender, EventArgs e)
    {
        string queryAutoComplete1 = string.Empty;
        if (Filtro_Empresa.Text != "")
        {
            DataTable table1 = new DataTable();
            if (DbnetContext.TipoMonitor.ToString() == "DTE")
            {
                queryAutoComplete1 = "select rutt_rece AS rut_empr from dte_enca_docu where codi_empr= " +
                    DbnetContext.Codi_empr + " and codi_emex='" + DbnetContext.Codi_emex +
                    "' AND nomb_rece = '" + Filtro_Empresa.Text.Replace("'", "''") +
                    "' GROUP BY rutt_rece ";
            }
            else
            {
                if (DbnetContext.TipoMonitor.ToString() == "DTO")
                {
                    queryAutoComplete1 = "select rutt_emis AS rut_empr from dto_enca_docu_p where codi_empr= " +
                    DbnetContext.Codi_empr + " and codi_emex='" + DbnetContext.Codi_emex +
                    "' AND nomb_emis = '" + Filtro_Empresa.Text.Replace("'", "''") +
                    "' GROUP BY rutt_emis ";
                }
            }
            table1 = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, queryAutoComplete1);
            if (table1.Rows.Count != 0)
            {
                Codi_pers.Text = table1.Rows[0][0].ToString();
            }
            else
            {
                Codi_pers.Text = "";
            }
        }
        else
        {
            Codi_pers.Text = "";
        }
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = false)]
    public static void CkbGrilla2(string sId, bool sChecked, string sTipo)/***Aqui***/
    {
        List<FacListMonitorServicioDTE> _oListaMonitorServicio;
        if (HttpContext.Current.Session["oListaMonitor"] == null)
            _oListaMonitorServicio = new List<FacListMonitorServicioDTE>();
        else
            _oListaMonitorServicio = (List<FacListMonitorServicioDTE>)HttpContext.Current.Session["oListaMonitor"];
        if (_oListaMonitorServicio != null)
        {
            var sIdGrilla = string.Empty;
            int iIdx = 0;
            if (sId.Contains("ctl"))
            {
                sIdGrilla = sId.Substring(Convert.ToInt32(sId.IndexOf("ctl")) + 3, 5);
                int.TryParse(sIdGrilla.Substring(0, sIdGrilla.IndexOf('_')), out iIdx);
                iIdx = iIdx - 2;
            }
            if (sId.Contains("_selection_"))
            {
                sIdGrilla = sId.Substring(Convert.ToInt32(sId.IndexOf("_selection_") + 11));
                int.TryParse(sIdGrilla, out iIdx);
            }
            var dbnetContext = (DbnetSesion)HttpContext.Current.Session["contexto"];
            var oDataDt = dbnetContext.dtGrilla;
            var oResultado = oDataDt.Rows[iIdx];
            tipoDocumento = dbnetContext.TipoMonitor;

            string foliDocu = string.Empty;
            string foliDocuDto = string.Empty;
            if (tipoDocumento.ToUpper().Equals("DTO"))
            {
                foliDocuDto = oResultado["Folio-ERP"].ToString();
                foliDocuDto = foliDocuDto.Substring(foliDocuDto.IndexOf("'>") + 2);
                foliDocuDto = foliDocuDto.Substring(0, foliDocuDto.IndexOf("</"));
            }

            foliDocu = oResultado["Folio"].ToString();
            try
            {
                foliDocu = foliDocu.Substring(foliDocu.IndexOf("'>") + 2);
                foliDocu = foliDocu.Substring(0, foliDocu.IndexOf("</"));
            }
            catch
            { foliDocu = oResultado[3].ToString(); }
            string tipoDocu = oResultado["Tipo"].ToString().Trim();
            string corrDocu = string.Empty;
            string codiEsap = string.Empty;
            string codiReme = string.Empty;
            string estaDocu = string.Empty;
            string ruttEmis = "0";
            string evenRecl = "0";	/* OT 9376978 03-05-2017|AM */
            string fechRSII = "0";
            string docuEscd = "0";	/* OT 9376978 11-05-2017|AM */
            if (tipoDocumento.ToUpper().Equals("DTO"))
            {
                string sRut = oResultado["Rut"].ToString();
                sRut = sRut.Substring(sRut.IndexOf("'>") + 2);
                sRut = sRut.Substring(0, sRut.IndexOf("-"));
                corrDocu = oResultado["CORR_DOCU"].ToString();
                string sQuery = null;
                if (DbnetGlobal.Base_dato == "SQLSERVER"){
                    sQuery = string.Format("select codi_esap, codi_reme, esta_docu, rutt_emis, even_recl, FECH_RECE_SII from dto_enca_docu_p where codi_empr = {0} and corr_docu = {1} ", dbnetContext.Codi_empr, corrDocu);	/* OT 9376978 03-05-2017|AM */
                }
                else{ //Oracle
                    sQuery = string.Format("select codi_esap, codi_reme, esta_docu, rutt_emis, even_recl, to_char(FECH_RECE_SII,'YYYY-MM-dd') as FECH_RECE_SII from dto_enca_docu_p where codi_empr = {0} and corr_docu = {1} ", dbnetContext.Codi_empr, corrDocu);	/* OT 9376978 13-07-2017|AM */
                }
                var oDataSelect = DbnetTool.Ejecuta_Select(dbnetContext.dbConnection, sQuery);
                if (oDataSelect.Rows.Count > 0)
                {
                    codiEsap = oDataSelect.Rows[0]["CODI_ESAP"].ToString();
                    codiReme = oDataSelect.Rows[0]["CODI_REME"].ToString();
                    estaDocu = oDataSelect.Rows[0]["ESTA_DOCU"].ToString();
                    ruttEmis = oDataSelect.Rows[0]["RUTT_EMIS"].ToString();
                    evenRecl = oDataSelect.Rows[0]["EVEN_RECL"].ToString();/*Aca se necesita campo reclamo EVEN_RECL - OT 9376978 03-05-2017|AM*/
                    fechRSII = oDataSelect.Rows[0]["FECH_RECE_SII"].ToString();/*Aca se necesita campo reclamo FECH_RECE_SII*/
                }
            }
            else if (tipoDocumento.ToUpper().Equals("DTE")) /* OT 9376978 11-05-2017|AM */
            {
                //select esta_docu from dte_enca_docu where codi_empr = 1 and foli_docu = 68273 and tipo_docu = 33
                string sQuery = string.Format("select esta_docu, even_recl, docu_escd from dte_enca_docu where codi_empr = {0} and foli_docu = {1} and tipo_docu = {2} ", dbnetContext.Codi_empr, foliDocu, tipoDocu);	/* OT 9376978 03-05-2017|AM */
                var oDataSelect = DbnetTool.Ejecuta_Select(dbnetContext.dbConnection, sQuery);
                estaDocu = oDataSelect.Rows[0]["ESTA_DOCU"].ToString();
                evenRecl = oDataSelect.Rows[0]["EVEN_RECL"].ToString();
				docuEscd = oDataSelect.Rows[0]["DOCU_ESCD"].ToString();
            }
            if (sChecked)
            {
                var oData =
                    _oListaMonitorServicio.Count(
                        x => x.CodiEmpr.Equals(dbnetContext.Codi_empr) &&
                             x.FoliDocu.Equals(foliDocu) &&
                             x.TipoDocu.Equals(tipoDocu) &&
                             x.EstaDocu.Equals(estaDocu) &&
                             x.RuttEmis.Equals(ruttEmis));
                if (oData == 0)
                {
                    var oData2 = new FacListMonitorServicioDTE
                    {
                        CodiEmpr = dbnetContext.Codi_empr,
                        FoliDocu = foliDocu,
                        TipoDocu = tipoDocu,
                        CorrDocu = corrDocu,
                        CodiEsap = codiEsap,
                        CodiReme = codiReme,
                        EstaDocu = estaDocu,
                        RuttEmis = ruttEmis,
                        EvenRecl = evenRecl,
                        FechRSII = fechRSII, /* Agregar asignacion de campo EVEN_RECL y FECH_RECE_SII - OT 9376978 03-05-2017|AM */
                        DocuEscd = docuEscd  /* OT 9376978 03-05-2017|AM */
                    };

                    _oListaMonitorServicio.Add(oData2);
                }
            }
            else
            {
                var oData =
                    _oListaMonitorServicio.FirstOrDefault(
                        x => x.CodiEmpr.Equals(dbnetContext.Codi_empr) &&
                             x.FoliDocu.Equals(foliDocu) &&
                             x.TipoDocu.Equals(tipoDocu) &&
                             x.EstaDocu.Equals(estaDocu) &&
                             x.RuttEmis.Equals(ruttEmis));
                _oListaMonitorServicio.Remove(oData);
            }

            HttpContext.Current.Session["oListaMonitor"] = _oListaMonitorServicio;
        }
    }
	// OT 9376978 - 28-04-2017|am 
    [WebMethod]
    [ScriptMethod(UseHttpGet = false)]
    public static object btnProcesarReclamo(ReclamoDTO oReclamoDTO)/***Aqui***/
    {
        string sEGRE = "";
        string sEGRC = "";
        string sMesgRech = "";
        var oContexto = (DbnetSesion)HttpContext.Current.Session["contexto"];
        var oListaGrilla = (List<FacListMonitorServicioDTE>)HttpContext.Current.Session["oListaMonitor"];

        if ((oListaGrilla != null) && (oListaGrilla.Count != 0))
        {
            if (new string[] { "CED", "CCD" }.Contains(oReclamoDTO.sAccion))				//CASO EMISION
            {
                //1 Validar cuales son los x que tengan "Rechazados tecnicamente"
                //var oListaGrillaRechTecn = oListaGrilla.Where(x => (x.EstaDocu.ToUpper() != "DOK" && x.EstaDocu.ToUpper() != "RLV" && x.EstaDocu.ToUpper() != "RPR"));
                string[] aRechTecn = new string[] { "DOK", "RLV", "RPR" };
                var oListaGrillaRechTecn = oListaGrilla.Where(x => !(aRechTecn.Contains(x.EstaDocu.ToUpper())));	
                int iEvenRechTecn = oListaGrillaRechTecn.Count();

                if (iEvenRechTecn > 0)
                {
                    string tmpMesgRechTecn = "";
                    foreach (var item in oListaGrillaRechTecn)
                        tmpMesgRechTecn += string.Format("{0}-{1}, ", item.TipoDocu, item.FoliDocu);
                    sMesgRech += string.Format(" Los documentos {0} están rechazados técnicamente.", tmpMesgRechTecn);
                }

                //2 Validar cuales son los x que tengan "Tipo de documento != 33, 34 o 43"
                string[] aRechTipoDoc = new string[] { "33", "34", "43" };
                var oListaGrillaRechTipoDoc = oListaGrilla.Where(x => !(aRechTipoDoc.Contains(x.TipoDocu.Trim().ToUpper())));
                //var oListaGrillaRechTipoDoc = oListaGrilla.Where(x => (x.TipoDocu != "33" && x.TipoDocu != "34" && x.TipoDocu != "43"));
                int iEvenRechTipoDoc = oListaGrillaRechTipoDoc.Count();

                if (iEvenRechTipoDoc > 0)
                {
                    string tmpMesgRechTipoDoc = "";
                    foreach (var item in oListaGrillaRechTipoDoc)
                        tmpMesgRechTipoDoc += string.Format("{0}-{1}, ", item.TipoDocu, item.FoliDocu);
                    sMesgRech += string.Format(" Los documentos {0} no son del tipo de documento 33, 34 o 43.", tmpMesgRechTipoDoc);
                }


                if (oReclamoDTO.sAccion == "CED")
                {
                    //3.1 Validar cuales son los x que NO tengan "Evento de Reclamo"
                    string[] aEvenRecl = new string[] { "ACD", "ERM", "RCD", "RFP", "RFT", "8" };
                    var oListaGrillaRechEvenRecl = oListaGrilla.Where( x => aEvenRecl.Contains(x.EvenRecl) );
                    int iEvenRechEvenRecl = oListaGrillaRechEvenRecl.Count();

                    if (iEvenRechEvenRecl > 0)
                    {
                        string tmpMesgRechEvenRecl = "";
                        foreach (var item in oListaGrillaRechEvenRecl)
                            tmpMesgRechEvenRecl += string.Format("{0}-{1}, ", item.TipoDocu, item.FoliDocu);
                        sMesgRech += string.Format(" Los documentos {0} ya tienen evento de reclamo.", tmpMesgRechEvenRecl);
                    }

                    //3.2 Validar cuales son los x que no estén en estado 10 (no emitido desde el 14-01-2017 en adelante)
                    var oListaGrillaRechEsta10 = oListaGrilla.Where(x => x.EvenRecl == "10");
                    int iEvenRechEsta10 = oListaGrillaRechEsta10.Count();

                    if (iEvenRechEsta10 > 0)
                    {
                        string tmpMesgRechEsta10 = "";
                        foreach (var item in oListaGrillaRechEsta10)
                            tmpMesgRechEsta10 += string.Format("{0}-{1}, ", item.TipoDocu, item.FoliDocu);
                        sMesgRech += string.Format(" Los documentos {0} no fueron emitidos desde el 14 de Enero de 2017 en adelante.", tmpMesgRechEsta10);
                    }
                }
                else if (oReclamoDTO.sAccion == "CCD")
                {
                    //4 Validar cuales son los x que NO 
                    string[] aDocuEscd = new string[] { "10", "21", "24", "25" };
                    var oListaGrillaDocuEscd = oListaGrilla.Where(x => aDocuEscd.Contains(x.DocuEscd));
                    int iEvenDocuEscd = oListaGrillaDocuEscd.Count();

                    if (iEvenDocuEscd > 0)
                    {
                        string tmpMesgDocuEscd = "";
                        foreach (var item in oListaGrillaDocuEscd)
                            tmpMesgDocuEscd += string.Format("{0}-{1}, ", item.TipoDocu, item.FoliDocu);
                        sMesgRech += string.Format(" Los documentos {0} son 'No Cedible'.", tmpMesgDocuEscd);
                    }
                }
                //como los dos caso van a llamar de igual forma al sp dbo.prc_recl
                oReclamoDTO = EjecutaQueryReclamo(oReclamoDTO, sEGRE, sEGRC, oListaGrilla, oContexto);

				oReclamoDTO.iCodigo = 4;
                oReclamoDTO.sMensaje = sMesgRech;
                oReclamoDTO.iDocumentosSeleccionados = oListaGrilla.Count();
            }
			else if(new[] {"ACD","ERM","RCD","RFP","RFT","FRS"}.Contains(oReclamoDTO.sAccion)) //CASO RECEPCION
            {
                //1 Validar cuales son los x que tengan "Recahzados tecnicamente"
                var oListaGrillaRechTecn = oListaGrilla.Where(x => (x.EstaDocu.ToUpper() != "INI" && x.EstaDocu.ToUpper() != "ERA"));
                int iEvenRechTecn = oListaGrillaRechTecn.Count();

                if (iEvenRechTecn > 0)
                {
                    string tmpMesgRechTecn = "";
                    foreach (var item in oListaGrillaRechTecn)
                        tmpMesgRechTecn += string.Format("{0}-{1}, ", item.TipoDocu, item.FoliDocu);
                    sMesgRech += string.Format(" Los documentos {0} están rechazados técnicamente.", tmpMesgRechTecn);
                }

				//2 Validar cuales son los x que tengan "Tipo de documento != 33, 34 o 43"
				var oListaGrillaRechTipoDoc = oListaGrilla.Where(x => (x.TipoDocu != "33" && x.TipoDocu != "34" && x.TipoDocu != "43"));
				int iEvenRechTipoDoc = oListaGrillaRechTipoDoc.Count();

				if (iEvenRechTipoDoc > 0)
				{
					string tmpMesgRechTipoDoc = "";
					foreach (var item in oListaGrillaRechTipoDoc)
						tmpMesgRechTipoDoc += string.Format("{0}-{1}, ", item.TipoDocu, item.FoliDocu);
					sMesgRech += string.Format(" Los documentos {0} no son del tipo de documento 33, 34 o 43.", tmpMesgRechTipoDoc);
				}

                if (oReclamoDTO.sAccion != "FRS")
                {
                    //3 Validar cuales tienen "Evento Reclamo"
                    string[] aEvenRecl = new string[] { "ACD", "ERM", "RCD", "RFP", "RFT" };
                    var oListaGrillaRechEvenRecl = oListaGrilla.Where(x => (aEvenRecl.Contains(x.EvenRecl)));
                    //var oListaGrillaRechEvenRecl = oListaGrilla.Where(x => (!string.IsNullOrEmpty(x.EvenRecl)));
                    int iEvenRechEvenRecl = oListaGrillaRechEvenRecl.Count();

                    if (iEvenRechEvenRecl > 0)
                    {
                        string tmpMesgRechEvenRecl = "";
                        foreach (var item in oListaGrillaRechEvenRecl)
                            tmpMesgRechEvenRecl += string.Format("{0}-{1}, ", item.TipoDocu, item.FoliDocu);
                        sMesgRech += string.Format(" Los documentos {0} ya tienen evento reclamo.", tmpMesgRechEvenRecl);
                    }
                }
                else
                {
                    //4 Validar cuales tienen "Fecha recepcion SII"
                    var oListaGrillaRechFechSII = oListaGrilla.Where(x => (!string.IsNullOrEmpty(x.FechRSII)));
                    int iEvenRechFechSII = oListaGrillaRechFechSII.Count();

                    if (iEvenRechFechSII > 0)
                    {
                        string tmpMesgRechFechSII = "";
                        foreach (var item in oListaGrillaRechFechSII)
                            tmpMesgRechFechSII += string.Format("{0}-{1}, ", item.TipoDocu, item.FoliDocu);
                        sMesgRech += string.Format(" Los documentos {0} ya tienen fecha de receción ante SII.", tmpMesgRechFechSII);
                    }
                }

				oReclamoDTO = EjecutaQueryReclamo(oReclamoDTO, sEGRE, sEGRC, oListaGrilla, oContexto);
                oReclamoDTO.sMensaje = sMesgRech;
				oReclamoDTO.iCodigo = 4;
				oReclamoDTO.iDocumentosSeleccionados = oListaGrilla.Count();

            }
            else if(oReclamoDTO.sAccion == "---")
            {
                oReclamoDTO.iCodigo = 5;
                oReclamoDTO.sMensaje = "Debe seleccionar la acción de Aceptación o Reclamo.";
            }

        }
        else
        {
            oReclamoDTO.iCodigo = 5;
            oReclamoDTO.sMensaje = "No ha seleccionado ningun " + oContexto.TipoMonitor.ToString() + ".";
        }
        HttpContext.Current.Session.Remove("oListaMonitor");
        return oReclamoDTO;
	}
	
	// OT 9376978 - 27-04-2017|am modelo
    [WebMethod]
    [ScriptMethod(UseHttpGet = false)]
    public static object btnProcesarEnvio(RecepcionDTO oRecepcionDTO)
    {
        string sEGRE = "";
        string sEGRC = "";
        var oContexto = (DbnetSesion)HttpContext.Current.Session["contexto"];
        var oListaGrilla = (List<FacListMonitorServicioDTE>)HttpContext.Current.Session["oListaMonitor"];
        if (oListaGrilla != null)
        {
            #region Validacion de Parametros por Empresa
            switch (oRecepcionDTO.sAccion)
            {
                case "ARE":
                    //Validacion Parametro EGAC Aprobación Comercial
                    string query = string.Format("select valo_paem from para_empr where codi_paem = '{0}' and codi_empr = {1} and codi_emex = '{2}'",
                                                 "EGAC", oContexto.Codi_empr, oContexto.Codi_emex);
                    var oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, query);
                    for (int i = 0; i < oData.Rows.Count; i++)
                    { sEGRE = oData.Rows[i][0].ToString(); }
                    if (string.IsNullOrEmpty(sEGRE))
                    {
                        if (string.IsNullOrEmpty(oRecepcionDTO.sDescripcion))
                        {
                            oRecepcionDTO.iCodigo = 1;
                            oRecepcionDTO.sMensaje = "Falta el parámetro para la aprobación comercial de forma másiva.";
                            return oRecepcionDTO;
                        }
                        else
                        {
                            if (oRecepcionDTO.iCodigo == 1)
                            {
                                sEGRE = oRecepcionDTO.sDescripcion;
                                query = string.Format("insert into para_empr (codi_empr, codi_paem, tipo_como, desc_paem, valo_paem, obli_paem, codi_emex)" +
                                                      "values({0},'{1}','ALL', 'Aprobación Masiva','{2}', 'N', '{3}')"
                                                      , oContexto.Codi_empr, "EGAC", sEGRE, oContexto.Codi_emex);
                                DbnetTool.Ejecuta_Select(oContexto.dbConnection, query);
                            }
                        }
                    }
                    break;
                case "RME":
                    //Validacion Parametro EGRE Recibo Comercial
                    query = string.Format("select valo_paem from para_empr where codi_paem = '{0}' and codi_empr = {1} and codi_emex = '{2}'",
                                                 "EGRE", oContexto.Codi_empr, oContexto.Codi_emex);
                    oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, query);
                    for (int i = 0; i < oData.Rows.Count; i++)
                    { sEGRE = oData.Rows[i][0].ToString(); }
                    if (string.IsNullOrEmpty(sEGRE))
                    {
                        if (string.IsNullOrEmpty(oRecepcionDTO.sDescripcion))
                        {
                            oRecepcionDTO.iCodigo = 1;
                            oRecepcionDTO.sMensaje = "Falta el parámetro para el Recibo de Mercadería de forma másiva.";
                            return oRecepcionDTO;
                        }
                        else
                        {
                            if (oRecepcionDTO.iCodigo == 1)
                            {
                                sEGRE = oRecepcionDTO.sDescripcion;
                                query = string.Format("insert into para_empr (codi_empr, codi_paem, tipo_como, desc_paem, valo_paem, obli_paem, codi_emex)" +
                                                      "values({0},'{1}','ALL', 'Recibo mercaderia masivo','{2}', 'N', '{3}')"
                                                      , oContexto.Codi_empr, "EGRE", sEGRE, oContexto.Codi_emex);
                                DbnetTool.Ejecuta_Select(oContexto.dbConnection, query);
                            }
                        }
                    }
                    break;
                case "REC":

                    //Validacion Parametro EGRC para Rechazo Comercial
                    query = string.Format("select valo_paem from para_empr where codi_paem = '{0}' and codi_empr = {1} and codi_emex = '{2}'",
                                          "EGRC", oContexto.Codi_empr, oContexto.Codi_emex);
                    oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, query);
                    for (int i = 0; i < oData.Rows.Count; i++)
                    { sEGRC = oData.Rows[i][0].ToString(); }
                    if (string.IsNullOrEmpty(sEGRC))
                    {
                        if (string.IsNullOrEmpty(oRecepcionDTO.sDescripcion))
                        {
                            oRecepcionDTO.iCodigo = 2;
                            oRecepcionDTO.sMensaje = "Falta el parámetro de reparo o rechazo comercial.";
                            return oRecepcionDTO;
                        }
                        else
                        {
                            if (oRecepcionDTO.iCodigo == 2)
                            {
                                sEGRC = oRecepcionDTO.sDescripcion;
                                query = string.Format("insert into para_empr (codi_empr, codi_paem, tipo_como, desc_paem, valo_paem, obli_paem, codi_emex)" +
                                                      "values({0},'{1}','ALL', 'Observacion Rechazo Comercial','{2}', 'N', '{3}')"
                                                        , oContexto.Codi_empr, "EGRC", sEGRC, oContexto.Codi_emex);
                                DbnetTool.Ejecuta_Select(oContexto.dbConnection, query);
                            }
                        }
                    }
                    break;
            }
            // Fin validaciones de parametros por empresa
            #endregion

            //Listado no aplican
            //Con Respuesta Comercial
            int iCodiEsap = oListaGrilla.Count(x => !string.IsNullOrEmpty(x.CodiEsap));
            //Rechazado Tecnicamente
            int iEstaDocu = oListaGrilla.Count(x => x.EstaDocu.ToUpper() != "INI" && x.EstaDocu != "ERA");

            int iEstaDocuER1 = oListaGrilla.Count(x => x.EstaDocu.ToUpper().Equals("ER1"));
            //Con Recibo de Mercaderia
            int iCodiReme = oListaGrilla.Count(x => x.CodiReme.ToUpper().Equals("SÍ"));
            int iNoApli = (iCodiReme + iEstaDocu + iCodiEsap);
            oRecepcionDTO.iRechTec = iEstaDocu; oRecepcionDTO.iRecimerc = iCodiReme; oRecepcionDTO.iRespCome = iCodiEsap; oRecepcionDTO.iRegistroNoAplica = iNoApli; oRecepcionDTO.iDocumentosSeleccionados = oListaGrilla.Count();

            if (oRecepcionDTO.bAceptacion.Value)
            {
                oRecepcionDTO = EjecutaQuery(oRecepcionDTO, sEGRE, sEGRC, oListaGrilla, oContexto);
                oRecepcionDTO.iCodigo = 4;
            }
            else
            {
                if (DTO_COME == "1")
                {
                    oRecepcionDTO = EjecutaQuery(oRecepcionDTO, sEGRE, sEGRC, oListaGrilla, oContexto);
                    oRecepcionDTO.iCodigo = 4;
                }
                else if (iEstaDocuER1 > 0)
                {
                    oRecepcionDTO.iCodigo = 6;
                    oRecepcionDTO.sMensaje = "Existe Documento con Error de Firma";
                    return oRecepcionDTO;
                }
                else if (iCodiEsap > 0 || iEstaDocu > 0 || iCodiReme > 0)
                {
                    oRecepcionDTO.iCodigo = 3;
                    oRecepcionDTO.sMensaje = "Los documentos seleccionados no cumplen con condiciones para realizar la acción";
                    return oRecepcionDTO;
                }
                else
                {
                    oRecepcionDTO = EjecutaQuery(oRecepcionDTO, sEGRE, sEGRC, oListaGrilla, oContexto);
                    oRecepcionDTO.iCodigo = 4;
                }
            }
        }
        else
        {
            oRecepcionDTO.iCodigo = 5;
            oRecepcionDTO.sMensaje = "No ha seleccionado ningun DTO.";
            return oRecepcionDTO;
        }
        return oRecepcionDTO;
    }
    public static RecepcionDTO EjecutaQuery(RecepcionDTO oDTO, string sEGRE, string sEGRC, List<FacListMonitorServicioDTE> oLista, DbnetSesion oContexto)
    {
        int iContador = 0;
        int iRespTecOk = 0;
        int iRechTecOk = 0;
        int iCodiRemeOk = 0;
        int iReciNoApli = 0;
        string sRazoEsap = string.Empty, sReciRece = string.Empty, sEstaDocu = string.Empty, sCodiReme = string.Empty, sCodiEsap = string.Empty;
        string sQuerySelect = "";
        switch (oDTO.sAccion.ToUpper())
        {
            case "RME":
                foreach (var item in oLista)
                {
                    try
                    {
                        sQuerySelect = string.Format("select razo_esap, reci_rece, esta_docu, codi_reme, codi_esap from dto_enca_docu_p where corr_docu = {0} and codi_empr = {1} and codi_emex = '{2}'", item.CorrDocu, item.CodiEmpr, oContexto.Codi_emex);
                        var oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQuerySelect);
                        sRazoEsap = string.Empty; sReciRece = string.Empty; sEstaDocu = string.Empty; sCodiReme = string.Empty; sCodiEsap = string.Empty;
                        foreach (DataRow dr in oData.Rows)
                        {
                            sRazoEsap = dr["RAZO_ESAP"].ToString();
                            sReciRece = dr["RECI_RECE"].ToString();
                            sEstaDocu = dr["ESTA_DOCU"].ToString();
                            sCodiReme = dr["CODI_REME"].ToString();
                            sCodiEsap = dr["CODI_ESAP"].ToString();
                        }
                        //Para poder dar recepción de mercadería, el codi_reme debe ser nulo y no debe tener recepción Comercial
                        if (string.IsNullOrEmpty(sCodiReme))
                        {
                            string sQueryUpdate = string.Format("update dto_enca_docu_p set reci_rece = '{0}', fech_reme = {1}, codi_reme = '{2}', usua_reme = '{3}' where codi_empr = {4} and corr_docu = {5} and codi_emex = '{6}' ",
                                sEGRE, DbnetGlobal.Base_dato.Equals("SQLSERVER")? "getdate()":"sysdate", "RME", oContexto.Codi_usua, item.CodiEmpr, item.CorrDocu, oContexto.Codi_emex);
                            DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQueryUpdate);
                            iContador++;
                        }
                        else if (!string.IsNullOrEmpty(sCodiReme)) { iCodiRemeOk++; }
                        else if (sCodiEsap == "REC") { iRechTecOk++; }
                        else if (sCodiEsap == "APR" || sCodiEsap == "ARE") { iRespTecOk++; }
                    }
                    catch (Exception) { iReciNoApli++; }
                }
                break;
            case "APR":
                foreach (var item in oLista)
                {
                    try
                    {
                        sQuerySelect = string.Format("select razo_esap, reci_rece, esta_docu, codi_reme, codi_esap from dto_enca_docu_p where corr_docu = {0} and codi_empr = {1} and codi_emex = '{2}'", item.CorrDocu, item.CodiEmpr, oContexto.Codi_emex);
                        var oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQuerySelect);
                        sRazoEsap = string.Empty; sReciRece = string.Empty; sEstaDocu = string.Empty; sCodiReme = string.Empty; sCodiEsap = string.Empty;
                        foreach (DataRow dr in oData.Rows)
                        {
                            sRazoEsap = dr["RAZO_ESAP"].ToString();
                            sReciRece = dr["RECI_RECE"].ToString();
                            sEstaDocu = dr["ESTA_DOCU"].ToString();
                            sCodiReme = dr["CODI_REME"].ToString();
                            sCodiEsap = dr["CODI_ESAP"].ToString();
                        }

                        if (DTO_COME == "1")
                        {
                            string sQueryUpdate = string.Empty;
                            sQueryUpdate = string.Format("update dto_enca_docu_p set codi_esap = '{0}', reci_rece = '{1}', fech_esap = {2}, usua_esap = '{6}', razo_esap='Aprobar Comercialmente'  where codi_empr = {3} and corr_docu = {4} and codi_emex = '{5}'",
                                    "APR", sEGRE, DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "getdate()" : "sysdate", item.CodiEmpr, item.CorrDocu, oContexto.Codi_emex, oContexto.Codi_usua);
                            DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQueryUpdate);
                            iContador++;
                        }
                        else if (!string.IsNullOrEmpty(sEstaDocu) && string.IsNullOrEmpty(sCodiEsap))
                        {
                            if (sEstaDocu == "INI" || sEstaDocu == "ERA")
                            {
                                string sQueryUpdate = string.Format("update dto_enca_docu_p set codi_esap = '{0}', reci_rece = '{1}', fech_esap = {2}, usua_esap = '{6}'  where codi_empr = {3} and corr_docu = {4} and codi_emex = '{5}'",
                                                "APR", sEGRE, DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "getdate()" : "sysdate", item.CodiEmpr, item.CorrDocu, oContexto.Codi_emex, oContexto.Codi_usua);
                                        DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQueryUpdate);
                                        iContador++;
                            }
                        }
                        else if (sCodiEsap == "REC") { iRechTecOk++; }
                        else if (sCodiEsap == "APR" || sCodiEsap == "ARE") { iRespTecOk++; }
                    }
                    catch (Exception) { iReciNoApli = iReciNoApli++; }
                }
                break;
            case "ARE":
                foreach (var item in oLista)
                {
                    try
                    {
                        sQuerySelect = string.Format("select razo_esap, reci_rece, esta_docu, codi_reme, codi_esap from dto_enca_docu_p where corr_docu = {0} and codi_empr = {1} and codi_emex = '{2}'", item.CorrDocu, item.CodiEmpr, oContexto.Codi_emex);
                        var oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQuerySelect);
                        sRazoEsap = string.Empty; sReciRece = string.Empty; sEstaDocu = string.Empty; sCodiReme = string.Empty; sCodiEsap = string.Empty;
                        foreach (DataRow dr in oData.Rows)
                        {
                            sRazoEsap = dr["RAZO_ESAP"].ToString();
                            sReciRece = dr["RECI_RECE"].ToString();
                            sEstaDocu = dr["ESTA_DOCU"].ToString();
                            sCodiReme = dr["CODI_REME"].ToString();
                            sCodiEsap = dr["CODI_ESAP"].ToString();
                        }
                        if (DTO_COME == "1")
                        {
                            string sQueryUpdate = string.Format("update dto_enca_docu_p set codi_esap = '{0}', razo_esap = '{1}', reci_rece = '{2}', fech_esap = {3}, usua_esap = '{7}' where codi_empr = {4} and corr_docu = {5} and codi_emex = '{6}'",
                                        "ARE", sEGRC, sEGRE, DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "getdate()" : "sysdate", item.CodiEmpr, item.CorrDocu, oContexto.Codi_emex, oContexto.Codi_usua);
                                    DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQueryUpdate);
                                    iContador++;
                        }
                        else if (!string.IsNullOrEmpty(sEstaDocu) && string.IsNullOrEmpty(sCodiEsap))
                        {
                            if (sEstaDocu == "INI" || sEstaDocu == "ERA")
                            {
                                string sQueryUpdate = string.Format("update dto_enca_docu_p set codi_esap = '{0}', razo_esap = '{1}', reci_rece = '{2}', fech_esap = {3}, usua_esap = '{7}' where codi_empr = {4} and corr_docu = {5} and codi_emex = '{6}'",
                                            "ARE", sEGRC, sEGRE, DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "getdate()" : "sysdate", item.CodiEmpr, item.CorrDocu, oContexto.Codi_emex, oContexto.Codi_usua);
                                        DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQueryUpdate);
                                        iContador++;
                            }
                        }
                        else if (sCodiEsap == "REC") { iRechTecOk++; }
                        else if (sCodiEsap == "APR" || sCodiEsap == "ARE") { iRespTecOk++; }
                    }
                    catch (Exception) { iReciNoApli = iReciNoApli++; }
                }
                break;
            case "REC":
                foreach (var item in oLista)
                {
                    try
                    {
                        sQuerySelect = string.Format("select razo_esap, reci_rece, esta_docu, codi_reme, codi_esap from dto_enca_docu_p where corr_docu = {0} and codi_empr = {1} and codi_emex = '{2}'", item.CorrDocu, item.CodiEmpr, oContexto.Codi_emex);
                        var oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQuerySelect);
                        sRazoEsap = string.Empty; sReciRece = string.Empty; sEstaDocu = string.Empty; sCodiReme = string.Empty; sCodiEsap = string.Empty;
                        foreach (DataRow dr in oData.Rows)
                        {
                            sRazoEsap = dr["RAZO_ESAP"].ToString();
                            sReciRece = dr["RECI_RECE"].ToString();
                            sEstaDocu = dr["ESTA_DOCU"].ToString();
                            sCodiReme = dr["CODI_REME"].ToString();
                            sCodiEsap = dr["CODI_ESAP"].ToString();
                        }
                        if (DTO_COME == "1")
                        {
                            string sQueryUpdate = string.Empty;
                                    sQueryUpdate = string.Format("update dto_enca_docu_p set codi_esap = '{0}', razo_esap = '{1}', reci_rece = '{2}', fech_esap = {3}, usua_esap = '{7}' where codi_empr = {4} and corr_docu = {5} and codi_emex = '{6}'",
                                    "REC", sEGRC, sEGRE, DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "getdate()" : "sysdate", item.CodiEmpr, item.CorrDocu, oContexto.Codi_emex, oContexto.Codi_usua);
                                    DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQueryUpdate);
                                    iContador++;
                        }
                        else if (!string.IsNullOrEmpty(sEstaDocu) && string.IsNullOrEmpty(sCodiEsap))
                        {
                            if (sEstaDocu == "INI" || sEstaDocu == "ERA")
                            {
                                if (string.IsNullOrEmpty(sCodiReme))
                                {
                                    string sQueryUpdate = string.Format("update dto_enca_docu_p set codi_esap = '{0}', razo_esap = '{1}', reci_rece = '{2}', fech_esap = {3}, usua_esap = '{7}' where codi_empr = {4} and corr_docu = {5} and codi_emex = '{6}'",
                                            "REC", sEGRC, sEGRE, DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "getdate()" : "sysdate", item.CodiEmpr, item.CorrDocu, oContexto.Codi_emex, oContexto.Codi_usua);
                                            DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQueryUpdate);
                                            iContador++;
                                }
                                else
                                {
                                    string sQueryUpdate = string.Format("update dto_enca_docu_p set codi_esap = '{0}', razo_esap = '{1}', reci_rece = '{2}', fech_esap = {3}, usua_esap = '{7}' where codi_empr = {4} and corr_docu = {5} and codi_emex = '{6}'",
                                            "REC", sEGRC + ". En referencia a la Ley número 19.983", sEGRE, DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "getdate()" : "sysdate", item.CodiEmpr, item.CorrDocu, oContexto.Codi_emex, oContexto.Codi_usua);
                                            DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQueryUpdate);
                                            iContador++;
                                }
                            }
                        }
                        else if (sCodiEsap == "REC") { iRechTecOk++; }
                        else if (sCodiEsap == "APR" || sCodiEsap == "ARE") { iRespTecOk++; }
                    }
                    catch (Exception) { iReciNoApli = iReciNoApli++; }
                }
                break;
        }
        oDTO.iRechTec = iRechTecOk;
        oDTO.iRespCome = iRespTecOk;
        oDTO.iRecimerc = iCodiRemeOk;
        oDTO.iRegistroSatisfactorio = iContador;
        oDTO.iReciNoApli = iReciNoApli;
        return oDTO;
    }
    public static ReclamoDTO EjecutaQueryReclamo(ReclamoDTO oDTO, string sEGRE, string sEGRC, List<FacListMonitorServicioDTE> oLista, DbnetSesion oContexto)
    {
        int iContador = 0;
		int iReciNoApli = 0;
		string sQuerySelect = string.Empty; string sMensajeRechazo = string.Empty; string sFechRSII = string.Empty; string sEvenRecl = string.Empty; string sTipoDocu = string.Empty; string sFoliDocu = string.Empty; string sRuttRece = string.Empty; string sDigiRece = string.Empty; string sRuttEmis = string.Empty; string sDigiEmis = string.Empty;  
        string sEvento = oDTO.sAccion.ToUpper();
        string sCodiEmex = oContexto.Codi_emex;
        string sMensajeError = "";

        /*
         if (oDTO.sAccion.ToUpper() == "FRS")
             selecciono fecha del la tabla
             si es nula llamo a putEvento
             sino concateno folio-tipo en mensaje
         else if (oDTO.sAccion.ToUpper() IN "ACD", "ERM", "RCD", "RFP", "RFT", "FRS")
              selecciono datos de reclamo even_recl, eusa_recl, fech_recl
              si EVEN_RECL es nulo 
                    update de even_recl, eusa_recl, fech_recl
                    llamo a putevento
              sino concateno folio-tipo en mensaje
         else if(oDTO.sAccion.ToUpper() IN "CED", "CDC")
           validaciones
           si es nula llamo a prc_put_message (a traves de prc_recl)
         */
        if (sEvento == "FRS")
        {
            foreach (var item in oLista)
            {
                try
                {
                    sQuerySelect = string.Format("select fech_rece_sii, even_recl, tipo_docu, round(foli_docu, 0) as foli_docu, rutt_rece, digi_rece, rutt_emis, digi_emis from dto_enca_docu_p where corr_docu = {0} and codi_empr = {1} and codi_emex = '{2}' and (esta_docu = 'INI' or esta_docu = 'ERA') and (tipo_docu = 33 or tipo_docu = 34 or tipo_docu = 43) /*and (fech_rece_sii is null)*/ ", item.CorrDocu, item.CodiEmpr, sCodiEmex);
                    
                    if (DbnetGlobal.Base_dato.ToUpper() == "ORACLE")  //AM|17-07-2017
                        sQuerySelect = sQuerySelect.Replace("fech_rece_sii", "to_char(fech_rece_sii,'dd-mon-yy hh24:mi:ss') as fech_rece_sii");

                    var oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQuerySelect);
                    sFechRSII = string.Empty;
                    
                    foreach (DataRow dr in oData.Rows)
                    {
                        sFechRSII = dr["FECH_RECE_SII"].ToString();
                        sEvenRecl = dr["EVEN_RECL"].ToString();
                        sTipoDocu = dr["TIPO_DOCU"].ToString();
                        sFoliDocu = dr["FOLI_DOCU"].ToString();
                        sRuttRece = dr["RUTT_RECE"].ToString();
                        sDigiRece = dr["DIGI_RECE"].ToString();
                        sRuttEmis = dr["RUTT_EMIS"].ToString();
                        sDigiEmis = dr["DIGI_EMIS"].ToString();
                    }
                    //valida que el registro no tenga ya un "Fecha de Recepcion"
                    if (string.IsNullOrEmpty(sFechRSII))
                    {
                        string codi_empr_ = item.CodiEmpr.ToString();
                        string corr_docu_ = item.CorrDocu.ToString();
                        string pi_codi_erro_ = "";
                        string pi_mens_erro_ = "";
                        string pi_corr_qmsg_ = "";

                        DbnetProcedure sp = new DbnetProcedure(oContexto.dbConnection, "prc_recl",
                                   "codi_emex", sCodiEmex, "VarChar", 40, "in",
                                   "rutt_empr", sRuttRece, "Int", 10, "in",
                                   "digi_empr", sDigiRece, "VarChar", 1, "in",
                                   "rutt_emis", sRuttEmis, "Int", 10, "in",
                                   "digi_emis", sDigiEmis, "VarChar", 1, "in",
                                   "tipo_docu", sTipoDocu, "Int", 10, "in",
                                   "foli_docu", sFoliDocu, "Int", 10, "in",
                                   "even_sii", sEvento, "VarChar", 4, "in",
                                   "codi_empr", codi_empr_, "Int", 9, "in",
                                   "corr_docu", corr_docu_, "Int", 18, "in",
                                   "pi_codi_erro", "", "Int", 5, "out",
                                   "pi_mens_erro", "", "VarChar", 80, "out",
                                   "pi_corr_qmsg", "", "Int", 22, "out"
                                   );
                        pi_codi_erro_ = sp.return_String("pi_codi_erro");
                        pi_mens_erro_ = sp.return_String("pi_mens_erro");
                        pi_corr_qmsg_ = sp.return_String("pi_corr_qmsg");
                        iContador++;

                    }
                    else //Si ya tiene fecha, se reporta
                    {
                        sMensajeRechazo += string.Format("{0}-{1}, ", sTipoDocu, sFoliDocu);
                        iReciNoApli++;
                    }
                }
                catch (Exception e) {
                    sMensajeError += string.Format("Database error (folio:{0}-{1})", sTipoDocu, sFoliDocu); 
                    iReciNoApli++; 
                }
            }
            if (!string.IsNullOrEmpty(sMensajeRechazo))
                oDTO.sMensaje = string.Format("Los documentos {0} ya tienen Fecha de Recepción.", sMensajeRechazo);

            if (!string.IsNullOrEmpty(sMensajeError))
                oDTO.sMensaje += sMensajeError;

        }
        else if (new string[] { "ACD", "ERM", "RCD", "RFP", "RFT" }.Contains(sEvento))//los otros casos de Recepcion
        {
            foreach (var item in oLista)
            {
                try
                {
                    sQuerySelect = string.Format("select even_recl, tipo_docu, round(foli_docu, 0) as foli_docu, rutt_rece, digi_rece, rutt_emis, digi_emis from dto_enca_docu_p where corr_docu = {0} and codi_empr = {1} and codi_emex = '{2}' and (esta_docu = 'INI' or esta_docu = 'ERA') and (tipo_docu = 33 or tipo_docu = 34 or tipo_docu = 43) ", item.CorrDocu, item.CodiEmpr, sCodiEmex);
                    var oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQuerySelect);
                    sEvenRecl = string.Empty; 
                    foreach (DataRow dr in oData.Rows)
                    {
                        sEvenRecl = dr["EVEN_RECL"].ToString();
                        sTipoDocu = dr["TIPO_DOCU"].ToString();
                        sFoliDocu = dr["FOLI_DOCU"].ToString();
                        sRuttRece = dr["RUTT_RECE"].ToString();
                        sDigiRece = dr["DIGI_RECE"].ToString();
                        sRuttEmis = dr["RUTT_EMIS"].ToString();
                        sDigiEmis = dr["DIGI_EMIS"].ToString();
                    }
                    //valida que el registro no tenga ya un "Evento Reclamo"
                    if (string.IsNullOrEmpty(sEvenRecl))
                    {
                        string sQueryUpdate = string.Format("update dto_enca_docu_p set usua_recl = '{0}', even_recl = '{1}', fech_recl = {2} where codi_empr = {3} and corr_docu = {4} and codi_emex = '{5}' and (esta_docu = 'INI' or esta_docu = 'ERA') and (tipo_docu = 33 or tipo_docu = 34 or tipo_docu = 43) ",
                             oContexto.Codi_usua, oDTO.sAccion.ToUpper(), DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "getdate()" : "sysdate", item.CodiEmpr, item.CorrDocu, sCodiEmex);
                        DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQueryUpdate);

                        string codi_empr_ = item.CodiEmpr.ToString();
                        string corr_docu_ = item.CorrDocu.ToString();

                        string pi_codi_erro_ = "";
                        string pi_mens_erro_ = "";
                        string pi_corr_qmsg_ = "";

                        DbnetProcedure sp = new DbnetProcedure(oContexto.dbConnection, "prc_recl",
                                   "codi_emex", sCodiEmex, "VarChar", 40, "in",
                                   "rutt_empr", sRuttRece,  "Int", 10, "in",
                                   "digi_empr", sDigiRece,  "VarChar", 1, "in",
                                   "rutt_emis", sRuttEmis,  "Int", 10, "in",
                                   "digi_emis", sDigiEmis,  "VarChar", 1, "in",
                                   "tipo_docu", sTipoDocu,  "Int", 10, "in",
                                   "foli_docu", sFoliDocu,  "Int", 10, "in",
                                   "even_sii",  sEvento,    "VarChar", 4, "in",
                                   "codi_empr", codi_empr_, "Int", 9, "in",
                                   "corr_docu", corr_docu_, "Int", 18, "in",
                                   "pi_codi_erro", "", "Int", 5, "out",
                                   "pi_mens_erro", "", "VarChar", 80, "out",
                                   "pi_corr_qmsg", "", "Int", 22, "out"
                                   );
                        pi_codi_erro_ = sp.return_String("pi_codi_erro");
                        pi_mens_erro_ = sp.return_String("pi_mens_erro");
                        pi_corr_qmsg_ = sp.return_String("pi_corr_qmsg");


                        iContador++;
                    }
                    else //Si ya tiene "Evento Reclamo", se reporta
                    {
                        sMensajeRechazo += string.Format("{0}-{1}, ", sTipoDocu, sFoliDocu);
                        iReciNoApli++;
                    }
                }
                catch (Exception e)
                {
                    sMensajeError += string.Format("Database error (folio:{0}-{1})", sTipoDocu, sFoliDocu);
                    iReciNoApli++;
                }
            }
            if (!string.IsNullOrEmpty(sMensajeRechazo))
                oDTO.sMensaje = string.Format("Los documentos {0} ya tienen un evento. No se les puede aplicar este evento.", sMensajeRechazo);

            if (!string.IsNullOrEmpty(sMensajeError))
                oDTO.sMensaje += sMensajeError;
        }
        else if (new string[] { "CED", "CCD" }.Contains(sEvento))//los casos de Emision
        {
            string sMsgRechEvenRecl = "";
            string sMsgRechDocuEscd = "";
            int nCountDTE = 0;
			foreach (var item in oLista)
            {
				try
                {
                    string sDocuEscd = "";
                    string scorrEnvi = "";
                    sFoliDocu = "";
                    sQuerySelect = string.Format(" select codi_empr, codi_emex, even_recl, tipo_docu, foli_docu, rutt_rece, digi_rece, rutt_emis, digi_emis, docu_escd, corr_envi from dte_enca_docu where codi_empr = {0} and tipo_docu = {1} and foli_docu = {2} and codi_emex = '{3}' and (esta_docu = 'DOK' or esta_docu = 'RLV' or esta_docu = 'RPR') and (tipo_docu = 33 or tipo_docu = 34 or tipo_docu = 43) ", item.CodiEmpr, item.TipoDocu, item.FoliDocu, sCodiEmex);
                    var oData = DbnetTool.Ejecuta_Select(oContexto.dbConnection, sQuerySelect);
                    foreach (DataRow dr in oData.Rows)
                    {
                        sEvenRecl = dr["EVEN_RECL"].ToString();
                        sTipoDocu = dr["TIPO_DOCU"].ToString();
                        sFoliDocu = dr["FOLI_DOCU"].ToString();
                        sRuttRece = dr["RUTT_RECE"].ToString();
                        sDigiRece = dr["DIGI_RECE"].ToString();
                        sRuttEmis = dr["RUTT_EMIS"].ToString();
                        sDigiEmis = dr["DIGI_EMIS"].ToString();
                        sDocuEscd = dr["DOCU_ESCD"].ToString();
                        scorrEnvi = dr["CORR_ENVI"].ToString();
                        nCountDTE++;
                    }

                    if ((sEvento == "CED") && (new string[] { "ACD", "ERM", "RCD", "RFP", "RFT", "10", "8" }.Contains(sEvenRecl)))
                    {
                        sMsgRechEvenRecl += string.Format("{0}-{1}, ", sTipoDocu, sFoliDocu);
                        iReciNoApli++;
                        continue;
                    }
                    else if ( (sEvento == "CCD") && (!string.IsNullOrEmpty(sDocuEscd)) )
                    {
                        sMsgRechDocuEscd += string.Format("{0}-{1}, ", sTipoDocu, sFoliDocu);
                        iReciNoApli++;
                        continue;
                    }

                    string codi_empr_ = item.CodiEmpr.ToString();
                    string pi_codi_erro_ = "";
                    string pi_mens_erro_ = "";
                    string pi_corr_qmsg_ = "";

                    if (!string.IsNullOrEmpty(sFoliDocu))
                    {
                        DbnetProcedure sp = new DbnetProcedure(oContexto.dbConnection, "prc_recl",
                                   "codi_emex", sCodiEmex, "VarChar", 40, "in",
                                   "rutt_empr", sRuttRece, "Int", 10, "in",
                                   "digi_empr", sDigiRece, "VarChar", 1, "in",
                                   "rutt_emis", sRuttEmis, "Int", 10, "in",
                                   "digi_emis", sDigiEmis, "VarChar", 1, "in",
                                   "tipo_docu", sTipoDocu, "Int", 10, "in",
                                   "foli_docu", sFoliDocu, "Int", 10, "in",
                                   "even_sii", sEvento, "VarChar", 4, "in",
                                   "codi_empr", codi_empr_, "Int", 9, "in",
                                   "corr_docu", "0", "Int", 18, "in",
                                   "pi_codi_erro", "", "Int", 5, "out",
                                   "pi_mens_erro", "", "VarChar", 80, "out",
                                   "pi_corr_qmsg", "", "Int", 22, "out"
                                   );
                        pi_codi_erro_ = sp.return_String("pi_codi_erro");
                        pi_mens_erro_ = sp.return_String("pi_mens_erro");
                        pi_corr_qmsg_ = sp.return_String("pi_corr_qmsg");
                        iContador++;
                    }
                    else
                    {
                        iReciNoApli++;
                    }

                }
                catch (Exception e)
                {
                    sMensajeError += string.Format("Database error (folio:{0}-{1})", sTipoDocu, sFoliDocu);
                    iReciNoApli++;
                }
			}
        }

        oDTO.iRegistroSatisfactorio = iContador;
        oDTO.iReciNoApli = iReciNoApli;
        return oDTO;
    }
	/* OT 9376978 03-05-2017|AM */
    
    [WebMethod]
    [ScriptMethod(UseHttpGet = false)]
    public static object ValorCbb(string idCheck)
    {
        string sHtml = "";
        List<EnvioDTO> _oLista = new List<EnvioDTO>();
        switch (idCheck)
        {
            case "envElec":
                _oLista.Add(new EnvioDTO { sValue = "1", sDescripcion = "Documentos con Recibo Electrónico según ley 19.983", sData1 = "dte.codi_reme", sData2 = "NOT NULL" });
                _oLista.Add(new EnvioDTO { sValue = "2", sDescripcion = "Documentos sin Recibo según Ley 19.983", sData1 = "dte.codi_reme", sData2 = "NULL" });
                break;
            case "respComerTradi":
                _oLista.Add(new EnvioDTO { sValue = "3", sDescripcion = "Documentos Aprobados", sData1 = "dte.codi_esap", sData2 = "APR" });
                _oLista.Add(new EnvioDTO { sValue = "4", sDescripcion = "Documentos Aprobados con Reparos", sData1 = "dte.codi_esap", sData2 = "ARE" });
                _oLista.Add(new EnvioDTO { sValue = "5", sDescripcion = "Documentos Rechazados", sData1 = "dte.codi_esap", sData2 = "REC" });
                _oLista.Add(new EnvioDTO { sValue = "6", sDescripcion = "Documentos Sin Respuesta Comercial", sData1 = "dte.codi_esap", sData2 = "NULL" });
                break;
        }

        HttpContext.Current.Session["oListaComer"] = _oLista;

        foreach (EnvioDTO item in _oLista)
        { sHtml += @"<option value='" + item.sValue + "' data-s1='" + item.sData1 + "' data-s2='" + item.sData2 + "'>" + item.sDescripcion + "</option>"; }

        return sHtml;
    }
    [WebMethod]
    [ScriptMethod(UseHttpGet = false)]
    public static void CambioCbb(string idSelect, string valueSelect)
    {
        string sCambio = "";
        var s1 = valueSelect.Split(';');
        string sChec = string.Empty;
        if (s1[1].ToLower() != "null" && s1[1].ToLower() != "not null")
            sCambio = " " + s1[0].ToUpper() + " = '" + s1[1] + "' ";
        else if (s1[1].ToLower() == "null" || s1[1].ToLower() == "not null")
            sCambio = " " + s1[0].ToUpper() + " is " + s1[1] + " ";
        HttpContext.Current.Session["strCambio"] = sCambio;
        if (s1[2].Equals("1") || s1[2].Equals("2"))
            sChec = "envElec;" + s1[2];
        if (int.Parse(s1[2]) >= 3)
            sChec = "respComerTradi;" + s1[2];
        HttpContext.Current.Session["strSelecciona"] = sChec;
    }
    protected void CheckTec_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckTec.Checked)
        {
            listresp.Items.Clear();
            listresp.Items.Add(new ListItem("Aprobado", "APR"));
            listresp.Items.Add(new ListItem("Rechazado", "ERR"));
            listresp.Items.Add(new ListItem("Sin Respuesta", "SIN"));
            CheckCom.Checked = false;
        }
        else
        { CheckTec.Checked = false; }
    }
    protected void CheckCom_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckCom.Checked)
        {
            listresp.Items.Clear();
            listresp.Items.Add(new ListItem("Con Recibo de Mercadería según Ley 19.983", "RME"));
            listresp.Items.Add(new ListItem("Aprobado", "ACC"));
            listresp.Items.Add(new ListItem("Aprobado con Reparos", "ACD"));
            listresp.Items.Add(new ListItem("Rechazado", "ROC"));
            listresp.Items.Add(new ListItem("Sin Respuesta", "SIN"));
            CheckTec.Checked = false;
        }
        else
        {
            CheckCom.Checked = false;
        }
    }
    protected void cmbEstados_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (this.DbnetContext.TipoMonitor)
        {
            case "DTE":
                ddlEstaDocu.Items.Clear();
                List<Estados> oListaDTE = new List<Estados>();
                oListaDTE.Add(new Estados { Descripcion = "Todos los documentos", EstaDocu = "TODOS", sColor = "TODOS" });
                oListaDTE.Add(new Estados { Descripcion = "Todos los documentos", EstaDocu = "TODOS", sColor = "GREEN" });
                oListaDTE.Add(new Estados { Descripcion = "DTE OK", EstaDocu = "DOK", sColor = "GREEN" });
                oListaDTE.Add(new Estados { Descripcion = "DTE con errores aceptables", EstaDocu = "ERA", sColor = "GREEN" });
                oListaDTE.Add(new Estados { Descripcion = "DTE firmado", EstaDocu = "FIR", sColor = "GREEN" });
                oListaDTE.Add(new Estados { Descripcion = "DTE Ingresado interno", EstaDocu = "ING", sColor = "GREEN" });
                oListaDTE.Add(new Estados { Descripcion = "DTE pendiente", EstaDocu = "PEN", sColor = "GREEN" });
                oListaDTE.Add(new Estados { Descripcion = "DTE Aceptado con Reparos Leves", EstaDocu = "RLV", sColor = "GREEN" });
                oListaDTE.Add(new Estados { Descripcion = "Aprobado Con Reparos por el SII", EstaDocu = "RPR", sColor = "GREEN" });
                oListaDTE.Add(new Estados { Descripcion = "Generado XML", EstaDocu = "XML", sColor = "GREEN" });
                oListaDTE.Add(new Estados { Descripcion = "Todos los documentos", EstaDocu = "TODOS", sColor = "RED" });
                oListaDTE.Add(new Estados { Descripcion = "DTE con error de firma", EstaDocu = "EFI", sColor = "RED" });
                oListaDTE.Add(new Estados { Descripcion = "DTE no recibido - Error de Firma", EstaDocu = "ER1", sColor = "RED" });
                oListaDTE.Add(new Estados { Descripcion = "DTE no recibido - Error en RUT de Emisor", EstaDocu = "ER2", sColor = "RED" });
                oListaDTE.Add(new Estados { Descripcion = "DTE con errores internos", EstaDocu = "ERR", sColor = "RED" });
                oListaDTE.Add(new Estados { Descripcion = "Rechazado por el SII", EstaDocu = "RCH", sColor = "RED" });
                oListaDTE.Add(new Estados { Descripcion = "Dte con error de schema", EstaDocu = "SCM", sColor = "RED" });

                switch (cmbEstados.SelectedValue.ToUpper())
                {
                    case "TODOS":
                        ddlEstaDocu.DataSource = AddCmbEstado(oListaDTE.GroupBy(x => x.EstaDocu).Select(x => x.First()).ToList());
                        break;
                    case "GREEN":
                        ddlEstaDocu.DataSource = AddCmbEstado(oListaDTE.Where(x => x.sColor.ToUpper() == "GREEN").ToList());
                        break;
                    case "RED":
                        ddlEstaDocu.DataSource = AddCmbEstado(oListaDTE.Where(x => x.sColor == "RED").ToList());
                        break;
                }
                break;
            case "DTO":
                ddlEstaDocu.Items.Clear();
                List<Estados> oListaDTO = new List<Estados>();
                oListaDTO.Add(new Estados { Descripcion = "Todos los documentos", EstaDocu = "TODOS", sColor = "TODOS" });
                oListaDTO.Add(new Estados { Descripcion = "Todos los documentos", EstaDocu = "TODOS", sColor = "YELLOW" });
                oListaDTO.Add(new Estados { Descripcion = "DTE Inicializado", EstaDocu = "INI", sColor = "YELLOW" });
                oListaDTO.Add(new Estados { Descripcion = "DTE con errores aceptables", EstaDocu = "ERA", sColor = "YELLOW" });
                oListaDTO.Add(new Estados { Descripcion = "Todos los documentos", EstaDocu = "TODOS", sColor = "RED" });
                oListaDTO.Add(new Estados { Descripcion = "DTE no recibido - Error en RUT de Emisor", EstaDocu = "ER2", sColor = "RED" });
                oListaDTO.Add(new Estados { Descripcion = "DTE no recibido - Error en RUT de Receptor", EstaDocu = "ER3", sColor = "RED" });
                oListaDTO.Add(new Estados { Descripcion = "DTE no recibido - DTE Repetido", EstaDocu = "ER4", sColor = "RED" });
                oListaDTO.Add(new Estados { Descripcion = "Error de Consulta", EstaDocu = "ERQ", sColor = "RED" });
                oListaDTO.Add(new Estados { Descripcion = "DTE con errores internos", EstaDocu = "ERR", sColor = "RED" });
                oListaDTO.Add(new Estados { Descripcion = "Rechazado Por Firma Incorrecta", EstaDocu = "FRM", sColor = "RED" });

                switch (cmbEstados.SelectedValue.ToUpper())
                {
                    case "TODOS":
                        ddlEstaDocu.DataSource = AddCmbEstado(oListaDTO.GroupBy(x => x.EstaDocu).Select(x => x.First()).ToList());
                        break;
                    case "GREEN":
                        ddlEstaDocu.DataSource = AddCmbEstado(oListaDTO.Where(x => x.sColor.ToUpper() == "YELLOW").ToList());
                        break;
                    case "RED":
                        ddlEstaDocu.DataSource = AddCmbEstado(oListaDTO.Where(x => x.sColor.ToUpper() == "RED").ToList());
                        break;
                }
                break;
        }
        ddlEstaDocu.DataTextField = "TEXT";
        ddlEstaDocu.DataValueField = "VALUE";
        ddlEstaDocu.DataBind();
    }
    private DataTable AddCmbEstado(List<Estados> oListado)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("TEXT");
        dt.Columns.Add("VALUE");
        foreach (var item in oListado)
        {
            DataRow dr = dt.NewRow();
            dr["TEXT"] = item.Descripcion;
            dr["VALUE"] = item.EstaDocu;
            dt.Rows.Add(dr);
        }
        return dt;
    }
    private DataTable AddCmbEnvio(List<EnvioDTO> oListado)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("TEXT");
        dt.Columns.Add("VALUE");
        foreach (var item in oListado)
        {
            DataRow dr = dt.NewRow();
            dr["TEXT"] = item.sDescripcion;
            dr["VALUE"] = item.sData2.ToUpper().Contains("NULL") ? item.sData1 + " is " + item.sData2 : item.sData1 + " = '" + item.sData2 + "'";
            dt.Rows.Add(dr);
        }
        return dt;
    }
    protected void envElec_CheckedChanged(object sender, EventArgs e)
    {
        if (envElec.Checked)
        {
            respComerTradi.Checked = false;
            List<EnvioDTO> _oLista = new List<EnvioDTO>();
            _oLista.Add(new EnvioDTO { sValue = "1", sDescripcion = "Documentos con Recibo Electrónico según ley 19.983", sData1 = "dte.codi_reme", sData2 = "NOT NULL" });
            _oLista.Add(new EnvioDTO { sValue = "2", sDescripcion = "Documentos sin Recibo según Ley 19.983", sData1 = "dte.codi_reme", sData2 = "NULL" });
            ddlTipo.DataSource = AddCmbEnvio(_oLista);
            ddlTipo.DataTextField = "TEXT";
            ddlTipo.DataValueField = "VALUE";
            ddlTipo.DataBind();
            ddlTipo.Enabled = true;
        }
        else
        {
            ddlTipo.Enabled = false;
        }
    }
    protected void respComerTradi_CheckedChanged(object sender, EventArgs e)
    {
        if (respComerTradi.Checked)
        {
            envElec.Checked = false;
            List<EnvioDTO> _oLista = new List<EnvioDTO>();
            _oLista.Add(new EnvioDTO { sValue = "3", sDescripcion = "Documentos Aprobados", sData1 = "dte.codi_esap", sData2 = "APR" });
            _oLista.Add(new EnvioDTO { sValue = "4", sDescripcion = "Documentos Aprobados con Reparos", sData1 = "dte.codi_esap", sData2 = "ARE" });
            _oLista.Add(new EnvioDTO { sValue = "5", sDescripcion = "Documentos Rechazados", sData1 = "dte.codi_esap", sData2 = "REC" });
            _oLista.Add(new EnvioDTO { sValue = "6", sDescripcion = "Documentos Sin Respuesta Comercial", sData1 = "dte.codi_esap", sData2 = "NULL" });
            ddlTipo.DataSource = AddCmbEnvio(_oLista);
            ddlTipo.DataTextField = "TEXT";
            ddlTipo.DataValueField = "VALUE";
            ddlTipo.DataBind();
            ddlTipo.Enabled = true;
        }
        else
        {
            ddlTipo.Enabled = false;
        }
    }
    public static bool EsBase64(string str)
    {
        return Regex.IsMatch(str, @"[A-Za-z0-9+/=]");
    }
    public static string EncodeToken(string toEncode)
    {
        string returnValue = toEncode;
        string clave = "";
        clave = EncodeTo64(System.DateTime.Now.ToString("fffmmyyyyhhfff")).Substring(0, 14);
        returnValue = clave + returnValue;
        returnValue = EncodeTo64(returnValue);
        clave = EncodeTo64(System.DateTime.Now.ToString("yyyyfffmmfffhh")).Substring(0, 14);
        returnValue = clave + returnValue;
        returnValue = EncodeTo64(returnValue);
        return returnValue;
    }
    static public string EncodeTo64(string toEncode)
    {
        byte[] toEncodeAsBytes
              = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
        string returnValue
              = System.Convert.ToBase64String(toEncodeAsBytes);
        return returnValue;
    }
}

public class EstadosComer
{
    public string valor { get; set; }
    public string texto { get; set; }
}
public class Estados
{
    public string EstaDocu { get; set; }
    public string Descripcion { get; set; }
    public string sColor { get; set; }
}