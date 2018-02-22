using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DbnetWebLibrary;
using System.Web.Script.Services;
using System.Web.Services;
using System.Linq;

public partial class facPrcIngDTEPropio : DbnetPage
{
    private string query = "";
    private string querymodo = "";
    private string querylvgiro = "";
    private string giro_rece_lvgiro = "";
    private string tipo_ingre;
    private int opAyuda = 0;
    private int opError = 0;
    private int asig_folio = 0;
    private string modo_foli = "0";
    private int modo_caf_ingr = 0;
    private static bool isValid = true;      // Si se produce un error de validación de schema
    private static String msgSchema = "";
    private String msgValidar = "";

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
    protected void barAyuda_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        if (opAyuda == 0)
            MsgAyuda(1);
        else
            MsgAyuda(0);

        mostrar(opAyuda, opError);

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Tipo_docu.Focus();
            DbnetContext = (DbnetSesion)Session["contexto"];
            string javascript = "<script type=\"text/javascript\">\n" +
                                 " var Arr = new Array;\n " +
                                 " function xDato(codigo,nombre,tipo){\n " +
                                 " this.codigo=codigo; \n " +
                                 " this.nombre=nombre; \n " +
                                 " this.tipo=tipo; \n " +
                                 " this.Codigo=function(){ return this.codigo; } \n" +
                                 " this.Nombre=function(){ return this.nombre; } \n" +
                                 " this.Tipo=function(){ return this.tipo; } \n" +
                                 " } \n" +
                                 " Arr = new Array; \n";
            string querysg = "select prod_desc from mant_prod order by prod_desc asc";
            string estadoDocu = "";
            DataTable dtsg = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, querysg);
            javascript += " var productos = [ ";
            for (int i = 0; i < dtsg.Rows.Count; i++)
            {
                DataRow dr = dtsg.Rows[i];
                javascript += " '" + dr["prod_desc"].ToString().Trim() + "' , ";
            }
            javascript += " '' ]; ";

            string query = "select code, code_desc from  sys_code Where domain_code=86 order by code";
            DataTable dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            javascript += " Arr['tipocodigo']= new Array; \n";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                javascript += string.Format(" Arr['tipocodigo'][" + i + "] = new xDato('{0}','{1}',0);\n", dr["code"].ToString().Trim(), dr["code"].ToString().Trim());
            }

            query = "select codi_impu," + DbnetContext.Auxdbo + "initcap(nomb_impu) nomb_impu , tipo_impu tipo_impu from  dte_tipo_impu order by nomb_impu";
            dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            javascript += " Arr['impuesto']= new Array; \n";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                javascript += string.Format(" Arr['impuesto'][" + i + "] = new xDato('{0}','{1}','{2}');\n", dr["codi_impu"].ToString().Trim(), dr["nomb_impu"].ToString().Trim(), dr["tipo_impu"].ToString().Trim());
            }

            query = "select code, code_desc, code_dele from sys_code where domain_code=92 ";
            dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            javascript += " Arr['documento']= new Array; \n";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                javascript += string.Format(" Arr['documento'][" + i + "] = new xDato('{0}','{1}','{2}');\n", dr["code"].ToString().Trim(), dr["code_desc"].ToString().Trim(), dr["code_dele"].ToString().Trim());
            }

            query = "select '0' code,'[Seleccione]' code_desc2,'-' from dual union select code, '[' " + DbnetContext.Auxcon + " code " + DbnetContext.Auxcon + " '] - ' " + DbnetContext.Auxcon + " code_desc code_desc2, code_desc from sys_code where domain_code=957 order by 3";
            dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            javascript += " Arr['medida']= new Array; \n";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                javascript += string.Format(" Arr['medida'][" + i + "] = new xDato('{0}','{1}',0);\n", dr["code"].ToString(), dr["code_desc2"].ToString());
            }

            query = "select code, code_desc from sys_code where  domain_code = '67' ";
            dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            javascript += " Arr['tpobultos']= new Array; \n";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                javascript += string.Format(" Arr['tpobultos'][" + i + "] = new xDato('{0}','{1}',0);\n", dr["code"].ToString(), dr["code_desc"].ToString());
            }

            query = string.Format("select '0', '[Seleccione]' from dual union select codi_ofic, nomb_ofic from oficina where codi_empr={0} order by 1 asc",DbnetContext.Codi_empr.ToString());
            dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            javascript += " Arr['oficina']= new Array; \n";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                javascript += string.Format(" Arr['oficina'][" + i + "] = new xDato('{0}','{1}',0);\n",dr[0].ToString(),dr[1].ToString());
            }
            //selecciona caf_ingr
            modo_caf_ingr = 0;

            querymodo = "select param_value from sys_param where param_name = 'EGATE_CAF_INGR'";
            string sValue = DbnetTool.SelectInto(DbnetContext.dbConnection, querymodo);

            modo_caf_ingr = (sValue.Equals("1")) ? 1 : 0;
            
            //selecciona si es folio automatico o manual
            modo_foli = "0";
            querymodo = "select param_value from sys_param where param_name = 'EGATE_FOLI_ABI'";
            dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, querymodo);

            if (dt.Rows.Count == 0)
                modo_foli = "0";
            else
            {
                DataRow dr = dt.Rows[0];
                modo_foli = (dr["param_value"].ToString() == "0")? "0":"1";
            }
            javascript += "</script>\n";
            Session.Add("javascript", javascript);
            Session["user_tipo"] = Request.Params["TIPO"].ToString();
            if (!IsPostBack)
            {
                divSucursal.Attributes.Add("style","display:none;");
                lb_tipo.Text = Session["user_tipo"].ToString();
                string qcombo = "";
                qcombo = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=:PAR order by 3";
                if ((Session["user_tipo"].ToString().Contains("E")) || (Session["user_tipo"].ToString().Contains("M")))
                {
                    string var54 = Session["user_tipo"].ToString();
                    if (Session["user_tipo"].ToString().Contains("E"))
                    {
                        btEnvSii.Enabled = false;
                        btEnvSii.Visible = true;
                        if (modo_foli == "1" && Session["user_modo"].ToString().Contains("I"))
                            Foli_docu.Enabled = true;
                        else
                            Foli_docu.Enabled = false;
                        tipo_ingre = "E";
                        btPreVisualizar.Enabled = true;
                    }

                    if (Session["user_tipo"].ToString().Contains("M"))
                    {
                        btEnvSii.Enabled = false;
                        btEnvSii.Visible = false;
                        Foli_docu.Enabled = true;
                        tipo_ingre = "M";
                    }
                    Session["tipo_ingre"] = tipo_ingre;
                }
                else
                {
                    if (Session["user_modo"].ToString().Contains("M"))
                    {
                        if (DbnetTool.SelectInto(DbnetContext.dbConnection, "select docu_elec from dte_tipo_docu where tipo_docu=" + DbnetContext.Val1) == "S")
                        {
                            btEnvSii.Enabled = false;
                            btEnvSii.Visible = true;
                            if (modo_foli == "1")
                            { Foli_docu.Enabled = true; }
                            else
                            { Foli_docu.Enabled = false; }
                            tipo_ingre = "E";
                        }
                        else
                        {
                            btEnvSii.Enabled = false;
                            btEnvSii.Visible = false;
                            Foli_docu.Enabled = true;
                            tipo_ingre = "M";
                        }
                    }
                    Session["tipo_ingre"] = tipo_ingre;
                }
                lvlval1.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, "select code_desc from sys_code where domain_code=959 and code='1'");
                lvlval2.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, "select code_desc from sys_code where domain_code=959 and code='2'");
                lvlval3.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, "select code_desc from sys_code where domain_code=959 and code='3'");
                lvlval4.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, "select code_desc from sys_code where domain_code=959 and code='4'");
                lvlval5.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, "select code_desc from sys_code where domain_code=959 and code='5'");
                lvlval6.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, "select code_desc from sys_code where domain_code=959 and code='6'");
                lvlval7.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, "select code_desc from sys_code where domain_code=959 and code='7'");
                lvlval8.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, "select code_desc from sys_code where domain_code=959 and code='8'");
                lvlval9.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, "select code_desc from sys_code where domain_code=959 and code='9'");

                cmbClausula.Query = qcombo.Replace(":PAR", "952");
                cmbClausula.Rescata(DbnetContext.dbConnection);
                cmbClausula.SelectedIndex = 0;

                cmbFormaPago.Query = qcombo.Replace(":PAR", "950");
                cmbFormaPago.Rescata(DbnetContext.dbConnection);
                cmbFormaPago.SelectedIndex = 0;

                cmbModalidad.Query = qcombo.Replace(":PAR", "951");
                cmbModalidad.Rescata(DbnetContext.dbConnection);
                cmbModalidad.SelectedIndex = 0;

                cmbMoneda.Query = qcombo.Replace(":PAR", "956");
                cmbMoneda.Rescata(DbnetContext.dbConnection);
                cmbMoneda.SelectedIndex = 0;
                


                cmbImpresora.Query = qcombo.Replace(":PAR", "960");
                cmbImpresora.Rescata(DbnetContext.dbConnection);
                cmbImpresora.SelectedIndex = 0;


                cmbPesoNeto.Query = qcombo.Replace(":PAR", "958");
                cmbPesoNeto.Rescata(DbnetContext.dbConnection);
                cmbPesoNeto.SelectedIndex = 0;

                cmbPesoBruto.Query = qcombo.Replace(":PAR", "958");
                cmbPesoBruto.Rescata(DbnetContext.dbConnection);
                cmbPesoBruto.SelectedIndex = 0;

                cmbPais.Query = qcombo.Replace(":PAR", "955");
                cmbPais.Rescata(DbnetContext.dbConnection);
                cmbPais.SelectedIndex = 0;

                cmbPuerto.Query = qcombo.Replace(":PAR", "954");
                cmbPuerto.Rescata(DbnetContext.dbConnection);
                cmbPuerto.SelectedIndex = 0;

                cmbPuerto2.Query = qcombo.Replace(":PAR", "954");
                cmbPuerto2.Rescata(DbnetContext.dbConnection);
                cmbPuerto2.SelectedIndex = 0;

                cmbTransporte.Query = qcombo.Replace(":PAR", "953");
                cmbTransporte.Rescata(DbnetContext.dbConnection);
                cmbTransporte.SelectedIndex = 0;

                lvIndi_vegd.Query = qcombo.Replace(":PAR", "96");
                lvIndi_vegd.Rescata(DbnetContext.dbConnection);
                lvIndi_vegd.SelectedIndex = 0;

                lvModa_Pago.Query = qcombo.Replace(":PAR", "87");
                lvModa_Pago.Rescata(DbnetContext.dbConnection);
                lvModa_Pago.SelectedIndex = 0;

                lvForm_Pago.Query = qcombo.Replace(":PAR", "94");
                lvForm_Pago.Rescata(DbnetContext.dbConnection);
                lvForm_Pago.SelectedIndex = 2;


                cmbIndServicio.Query = qcombo.Replace(":PAR", "963");
                cmbIndServicio.Rescata(DbnetContext.dbConnection);
                cmbIndServicio.SelectedIndex = 0;

                cmbTranVenta.Query = qcombo.Replace(":PAR", "964");
                cmbTranVenta.Rescata(DbnetContext.dbConnection);
                cmbTranVenta.SelectedIndex = 0;

                cmbTranCompra.Query = qcombo.Replace(":PAR", "965");
                cmbTranCompra.Rescata(DbnetContext.dbConnection);
                cmbTranCompra.SelectedIndex = 0;

                if (Convert.ToInt32(DbnetTool.SelectInto(DbnetContext.dbConnection, "select count(param_value) from sys_param where param_name = 'EGATE_USUA_DOCU' and param_value='N'")) > 0)
                {
                    if (tipo_ingre == "E")
                        lvTipo_Docu.Query = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec='S' and indi_digi='S' union select 0,'[Seleccione]' from dual order by 2";
                    else if (tipo_ingre == "M")
                        lvTipo_Docu.Query = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec!='S' and indi_digi='S' union select 0,'[Seleccione]' from dual order by 2";
                }
                else if (Convert.ToInt32(DbnetTool.SelectInto(DbnetContext.dbConnection, "select count(tipo_docu) from dte_usua_docu where codi_usua='" + DbnetContext.Codi_usua + "'")) > 0)
                {
                    if (tipo_ingre == "E")
                        lvTipo_Docu.Query = "Select a.tipo_docu, " + DbnetContext.Auxdbo + "initcap(b.desc_tido) from dte_usua_docu a , dte_tipo_docu b where a.codi_usua='" + DbnetContext.Codi_usua.ToString() + "' and b.tipo_docu=a.tipo_docu and b.docu_elec='S' union select 0,'[Seleccione]' from dual order by 2";
                    else if (tipo_ingre == "M")
                        lvTipo_Docu.Query = "Select a.tipo_docu, " + DbnetContext.Auxdbo + "initcap(b.desc_tido) from dte_usua_docu a , dte_tipo_docu b where a.codi_usua='" + DbnetContext.Codi_usua.ToString() + "' and b.tipo_docu=a.tipo_docu and b.docu_elec!='S' union select 0,'[Seleccione]' from dual order by 2";
                }
                else
                {
                    if (tipo_ingre == "E")
                        lvTipo_Docu.Query = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec='S' and indi_digi='S' union select 0,'[Seleccione]' from dual order by 2";
                    else if (tipo_ingre == "M")
                        lvTipo_Docu.Query = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec!='S' and indi_digi='S' union select 0,'[Seleccione]' from dual order by 2";
                }


                /*
                ot->711294    Funcionalidad para RRWINE
                * Copia de documento
                */
                if (Request.Params["copia"] == "S")
                {
                    DbnetContext.Val1 = DbnetContext.Val2;
                    DbnetContext.Val2 = DbnetContext.Val3;
                    lvTipo_Docu.Query = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec='S' and indi_digi='S' union select 0,'[Seleccione]' from dual order by 2";
                }


                lvTipo_Docu.Rescata(DbnetContext.dbConnection);
                lvTipo_Docu.SelectedIndex = 0;
                Tipo_docu.Text = lvTipo_Docu.SelectedValue;

                lvGiros.Query = "select codi_ramo," + DbnetContext.Auxdbo + "initcap(nomb_ramo) from ramo group by codi_ramo,nomb_ramo order by nomb_ramo";
                lvGiros.Rescata(DbnetContext.dbConnection);
                lvGiros.SelectedIndex = 0;
                Giro_rece.Text = lvGiros.SelectedValue;

                LvEstado.Query = "select esta_docu," + DbnetContext.Auxdbo + "initcap(desc_esdo) from DTE_ESTA_DOCU where esta_docu='PEN' order by desc_esdo";
                LvEstado.Rescata(DbnetContext.dbConnection);
                LvEstado.SelectedIndex = 0;
                Esta_docu.Text = LvEstado.SelectedValue;

                if (tipo_ingre == "E")
                    lbTitulo.Text = "Ingreso Factura Manual Electr&#243;nica";
                else
                    lbTitulo.Text = "Ingreso Factura Manual ";

                DateTime Fecha_venc = DateTime.Today.AddMonths(1);
                Fech_Emis.Text = DateTime.Now.ToString("yyyy-MM-dd");
                fech_venc.Text = DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd");

                query = "select * from empr where codi_empr=" + DbnetContext.Codi_empr;
                dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                DataRow dr = dt.Rows[0];

                string p_modo = Request.Params["modo"];
                switch (p_modo)
                {
                    case "I":
                        Session["user_modo"] = "I";
                        break;
                    case "E":
                        Session["user_modo"] = "I";
                        break;
                    default:
                        Session["user_modo"] = "M";
                        break;
                }
                lb_modo.Text = Session["user_modo"].ToString();
                
                switch (Session["user_modo"].ToString())
                {
                    case "M":
                        btPreVisualizar.Enabled = true;
                        query = string.Format("select d.tipo_docu tipo_docu," +
                                    " d.foli_docu foli_docu," +
                                    " d.foli_clie foli_clie," +
                                    " d.rutt_rece rutt_rece," +
                                    " d.digi_rece digi_rece," +
                                    " d.dire_rece dire_rece," +
                                    " d.comu_rece comu_rece," +
                                    " d.ciud_rece ciud_rece," +
                                    " d.cont_rece cont_rece," +
                                    " d.giro_rece giro_rece," +
                                    " d.fech_emis fech_emis," +
                                    " d.fech_venc fech_venc," +
                                    " d.esta_docu esta_docu," +
                                    " d.moda_pago moda_pago," +
                                    " d.form_pago form_pago," +
                                    " d.mont_exen mont_exen," +
                                    " d.CRED_ES65 CRED_ES65," +
                                    " d.mont_neto mont_neto," +
                                    " d.impu_vaag impu_vaag," +
                                    " d.mont_tota mont_tota," +
                                    " d.indi_vegd indi_vegd," +
                                    " d.rutt_tran rutt_tran," +
                                    " d.digi_tran digi_tran," +
                                    " d.dire_dest dire_dest," +
                                    " d.comu_dest comu_dest," +
                                    " d.val1 Obse," +
                                    " d.ciud_dest ciud_dest, " +
                                    " d.nomb_rece nomb_rece, " +
                                    " d.from_paex from_paex," +
                                    " d.moda_vent moda_vent," +
                                    " d.clau_expo clau_expo," +
                                    " d.tota_clex tota_clex," +
                                    " d.viaa_tran viaa_tran," +
                                    " d.nomb_tran nomb_tran," +
                                    " d.codi_puem codi_puem," +
                                    " d.codi_pude codi_pude," +
                                    " d.codi_puem codi_puem," +
                                    " d.tota_bult tota_bult," +
                                    " d.mont_flet mont_flet," +
                                    " d.pais_rece pais_rece," +
                                    " d.tipo_mone tipo_mone," +
                                    " {3}(d.mont_baco,0) mont_baco," +
                                    " {3}(d.mont_segu,0) mont_segu," +
                                    " d.val1,d.val2,d.val3,d.val4,d.val5,d.val6,d.val7,d.val8,d.val9," +
                                    " d.Unid_neto," +
                                    " d.Tota_neto," +
                                    " d.Unid_brut," +
                                    " d.Tota_brut," +
                                    " d.info_tran," +
                                    " d.codi_oper," +
                                    " d.nume_book, " +
                                    " d.vent_serv, " +
                                    " d.tran_vent, " +
                                    " d.tran_comp, " +
                                    " d.tipo_moom, " +
                                    " d.tipo_camb, " +
                                    " d.valo_paga " +
                                    " from  DTE_ENCA_DOCU d " +
                                    "where d.codi_empr = {0} " +
                                    "and   d.tipo_docu = {1} " +
                                    "and   d.foli_docu = {2}",
                                    DbnetContext.Codi_empr,DbnetContext.Val1,DbnetContext.Val2, (DbnetGlobal.Base_dato.Equals("SQLSERVER"))? "isnull":"nvl" ) ;

                        DbnetTool.ctrlSqlInjection(this.Page.Form);
                        dt = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dr = dt.Rows[i];
                            Foli_docu.Text = dr["foli_docu"].ToString();
                            Tipo_docu.Text = dr["tipo_docu"].ToString();
                            Foli_docu_rp.Text = dr["foli_clie"].ToString();
                            lvTipo_Docu.SelectedValue = Tipo_docu.Text;
                            Rutt_rece.Text = dr["rutt_rece"].ToString();
                            estadoDocu = dr["esta_docu"].ToString();
                            if(string.IsNullOrEmpty(dr["nomb_rece"].ToString()))
                            {
                                string qryNomb_rece = "SELECT nomb_pers FROM personas where codi_pers='" + Rutt_rece.Text + "'";
                                txtNomb_rece.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, qryNomb_rece);
                            }
                            else
                                txtNomb_rece.Text = dr["nomb_rece"].ToString();

                            Digi_rece.Text = dr["digi_rece"].ToString();

                            if (Rutt_rece.Text.Equals("55555555") || Rutt_rece.Text.Equals("99999999"))
                                txtNomb_rece.Text = dr["nomb_rece"].ToString();

                            Dire_rece.Text = dr["dire_rece"].ToString();
                            Comu_rece.Text = dr["comu_rece"].ToString();
                            Ciud_rece.Text = dr["ciud_rece"].ToString();
                            Fono.Text = dr["cont_rece"].ToString();
                            Fech_Emis.Text = dr["fech_emis"].ToString();
                            fech_venc.Text = dr["fech_venc"].ToString();
                            lvModa_Pago.Selecciona(dr["moda_pago"].ToString());
                            lvForm_Pago.Selecciona(dr["form_pago"].ToString());
                            lvIndi_vegd.Selecciona(dr["indi_vegd"].ToString());
                            Rutt_tran.Text = dr["rutt_tran"].ToString();
                            Digi_tran.Text = dr["digi_tran"].ToString();
                            Dire_dest.Text = dr["dire_dest"].ToString();
                            Comu_dest.Text = dr["comu_dest"].ToString();
                            Ciud_dest.Text = dr["ciud_dest"].ToString();
                            
                            //cambio a giro 19-04-2011
                            giro_rece_lvgiro = dr["giro_rece"].ToString();
                            string IngConcat = string.Empty;
                            IngConcat = (DbnetGlobal.Base_dato == "SQLSERVER") ? "ing" : string.Empty;
                            querylvgiro = "select codi_ramo, nomb_ramo from ramo where substr" + IngConcat + "(nomb_ramo, 0, 41) LIKE '" + giro_rece_lvgiro + "'";
                            DbnetTool.ctrlSqlInjection(this.Page.Form);
                            DataTable dt2 = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, querylvgiro);

                            if (dt2.Rows.Count == 0)
                            {
                                lvGiros.Selecciona2(dr["giro_rece"].ToString());
                                Giro_rece.Text = lvGiros.Text;
                            }
                            else
                            {
                                DataRow dr2 = dt2.Rows[0];
                                lvGiros.Selecciona2(dr2["nomb_ramo"].ToString());
                                Giro_rece.Text = lvGiros.SelectedValue;
                            }

                            cmbFormaPago.Selecciona(dr["from_paex"].ToString());
                            cmbModalidad.Selecciona(dr["moda_vent"].ToString());
                            cmbClausula.Selecciona(dr["clau_expo"].ToString());
                            txtTotalClausula.Text = dr["tota_clex"].ToString();
                            cmbTransporte.Selecciona(dr["viaa_tran"].ToString());
                            cmbPuerto.Selecciona( dr["codi_puem"].ToString());
                            cmbPuerto2.Selecciona(dr["codi_pude"].ToString());
                            txtTotalBultos.Text = dr["tota_bult"].ToString();
                            txtCredec.Text = dr["CRED_ES65"].ToString();
                            txtMntMargenCom.Text = dr["MONT_BACO"].ToString();
                            txtMntSeguro.Text = (dr["MONT_SEGU"].ToString().Equals("0,0000")) ? "0":dr["MONT_SEGU"].ToString();

                            if (!string.IsNullOrEmpty(dr["mont_flet"].ToString()))
                                txtFlete.Text = Convert.ToDouble(dr["mont_flet"]).ToString();
                            else
                                txtFlete.Text = "0";

                            cmbPais.Selecciona(dr["pais_rece"].ToString());
                            cmbMoneda.Selecciona2(dr["tipo_mone"].ToString());
                            tipo_camb.Text = dr["tipo_camb"].ToString().Replace(',', '.');
                            
                            if (Rutt_rece.Text.Equals(DbnetTool.SelectInto(DbnetContext.dbConnection, "select rutt_empr from empr where codi_empr=" + DbnetContext.Codi_empr)))
                            {
                                divSucursal.Attributes.Remove("style");
                                hdSucuDest.Text = dr["val5"].ToString();
                                hdSucuOrig.Text = dr["val6"].ToString();
                            }
                            else
                                divSucursal.Attributes.Add("style", "display:none;");

                            txtVal1.Text = dr["val1"].ToString();
                            txtVal2.Text = dr["val2"].ToString();
                            txtVal3.Text = dr["val3"].ToString();
                            txtVal4.Text = dr["val4"].ToString();
                           // if (!string.IsNullOrEmpty(hdSucuOrig.Text) && !string.IsNullOrEmpty(hdSucuDest.Text))
                            //{
                                txtVal5.Text = dr["val5"].ToString();
                                txtVal6.Text = dr["val6"].ToString();
                            //}
                            txtVal7.Text = dr["val7"].ToString();
                            txtVal8.Text = dr["val8"].ToString();
                            txtVal9.Text = dr["val9"].ToString();

                            
                            cmbPesoNeto.Selecciona(dr["Unid_neto"].ToString());
                            txtPesoNeto.Text = dr["Tota_neto"].ToString();
                            cmbPesoBruto.Selecciona(dr["Unid_brut"].ToString());
                            txtPesoBruto.Text = dr["Tota_brut"].ToString();

                            txtBoking.Text = dr["nume_book"].ToString();
                            txtOperador.Text = dr["codi_oper"].ToString();
                            txtNombreTransp.Text = dr["nomb_tran"].ToString();
                            Patente_tran.Text = dr["info_tran"].ToString();
                            Foli_docu.Enabled = false;
                            cmbIndServicio.Selecciona(dr["vent_serv"].ToString());
                            cmbTranVenta.Selecciona(dr["tran_vent"].ToString());
                            cmbTranCompra.Selecciona(dr["tran_comp"].ToString());
                            valo_paga.Text = dr["valo_paga"].ToString(); 
                        }//fin for

                        if (cmbMoneda.SelectedValue.ToString() != "PESO CL")
                        {
                            tipo_camb.Enabled = true;
                        }

                        string Nullable = string.Empty;
                        Nullable = (DbnetGlobal.Base_dato == "SQLSERVER") ? "isnull" : "nvl";
                        query = string.Format("select {3}((select dte_deta_codi.tipo_codi from dte_deta_codi where dte_deta_codi.tipo_docu =dte_deta_prse.tipo_docu and dte_deta_codi.foli_docu =dte_deta_prse.foli_docu and dte_deta_codi.codi_empr =dte_deta_prse.codi_empr and dte_deta_codi.nume_line=dte_deta_prse.nume_line and dte_deta_codi.corr_codi='1'),'') tipo_codi, " +
                                       "{3}((select dte_deta_codi.codi_item from dte_deta_codi where dte_deta_codi.tipo_docu =dte_deta_prse.tipo_docu and dte_deta_codi.foli_docu =dte_deta_prse.foli_docu and dte_deta_codi.codi_empr =dte_deta_prse.codi_empr and dte_deta_codi.nume_line=dte_deta_prse.nume_line and dte_deta_codi.corr_codi='1'),'') codi_item, " +
                                       "dte_deta_prse.tipo_codi, dte_deta_prse.codi_item, " +
                                       "dte_deta_prse.nomb_item, dte_deta_prse.cant_item, dte_deta_prse.prec_item,dte_deta_prse.desc_item, " +
                                       "dte_deta_prse.neto_item, dte_deta_prse.indi_exen, dte_deta_prse.codi_impu,dte_deta_prse.unid_medi, " +
                                       "dte_deta_prse.dcto_item, dte_deta_prse.desc_porc, " +
                                       "{3}((select dte_deta_codi.tipo_codi from dte_deta_codi where dte_deta_codi.tipo_docu =dte_deta_prse.tipo_docu and dte_deta_codi.foli_docu =dte_deta_prse.foli_docu and dte_deta_codi.codi_empr =dte_deta_prse.codi_empr and dte_deta_codi.nume_line=dte_deta_prse.nume_line and dte_deta_codi.corr_codi='2'),'') tipo_codi, " +
                                       "{3}((select dte_deta_codi.codi_item from dte_deta_codi where dte_deta_codi.tipo_docu =dte_deta_prse.tipo_docu and dte_deta_codi.foli_docu =dte_deta_prse.foli_docu and dte_deta_codi.codi_empr =dte_deta_prse.codi_empr and dte_deta_codi.nume_line=dte_deta_prse.nume_line and dte_deta_codi.corr_codi='2'),'') codi_item, " +
                                       "tipo_codi as tipo_codi_liq,"+
                                       "{3}(cant_refe,0) cant_refe," +
                                       "{3}(unid_refe,0) unid_refe," +
                                       "{3}(prec_refe,0) prec_refe," +
                                       "{3}(prec_mono,0) prec_mono," +
                                       "{3}(codi_mone,' ') codi_mone," +
                                       "{3}(fact_conv,0) fact_conv," +
                                       "{3}(reca_item,0) reca_item," +
                                       "{3}(reca_porc,0) reca_porc," +
                                       "{3}(fech_elab,' ') fech_elab," +
                                       "{3}(fech_vepr,' ') fech_vepr," +
                                       "{3}(desc_mone,0) desc_mone," +
                                       "{3}(reca_mone,0) reca_mone," +
                                       "{3}(valo_mone,0) valo_mone" +
                                       " FROM dte_deta_prse " +
                                       " where dte_deta_prse.codi_empr = {0}" +
                                       " and dte_deta_prse.tipo_docu = {1}" +
                                       " and dte_deta_prse.foli_docu = {2}",
                                       DbnetContext.Codi_empr,DbnetContext.Val1,DbnetContext.Val2, Nullable);

                        txtAuxDetalle.Text = "";
                        DbnetTool.ctrlSqlInjection(this.Page.Form);
                        DataTable deta = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                        for (int i = 0; i < deta.Rows.Count; i++)
                        {
                            string cant_item, neto_item,precio_item;
                            dr = deta.Rows[i];
                            string desc_porc = dr["desc_porc"].ToString();
                            if (string.IsNullOrEmpty(desc_porc)) desc_porc = "0";
                            string desc_cant = dr["dcto_item"].ToString();
                            if (string.IsNullOrEmpty(desc_cant)) desc_cant = "0";

                            cant_item = Math.Pow(Convert.ToDouble(dr["cant_item"]),1).ToString().ToString();
                            precio_item = Math.Pow(Convert.ToDouble(dr["prec_item"]),1).ToString();
                            if (Tipo_docu.Text == "110" || Tipo_docu.Text == "111" || Tipo_docu.Text == "112" || Tipo_docu.Text == "43")
                            {
                                neto_item = Math.Pow(Convert.ToDouble(dr["neto_item"]), 1).ToString();
                            }
                            else
                            {   decimal tto3 = Convert.ToDecimal(Convert.ToDecimal(cant_item) * Convert.ToDecimal(precio_item));
                                neto_item = tto3.ToString();
                            }


                            if (Tipo_docu.Text == "56" || Tipo_docu.Text == "110"  || Tipo_docu.Text == "111" || Tipo_docu.Text == "112")
                            {
                                cmbTranCompra.Enabled = false;
                                cmbTranVenta.Enabled = false;
                            }
                            else if (Tipo_docu.Text == "61")
                            {
                                cmbTranVenta.Enabled = true;
                                cmbTranCompra.Enabled = false;
                            }
                            else
                            {
                                cmbTranCompra.Enabled = true;
                                cmbTranVenta.Enabled = true;
                            }


                            txtAuxDetalle.Text += dr["tipo_codi"].ToString() + "|" +
                                                  dr["codi_item"].ToString() + "|" +
                                                  dr["nomb_item"].ToString() + "|" +
                                               dr["desc_item"].ToString() + "|" +
                                               cant_item.Replace(',', '.') + "|" +
                                               dr["unid_medi"].ToString() + "|" +
                                               precio_item.Replace(',', '.') + "|" +
                                               neto_item.Replace(',','.') + "|" +
                                               (desc_porc == "0" ? (desc_cant != "0" ? "$" : "%") : "%") + "|" +
                                               (desc_porc == "0" ? (desc_cant == "0" ? "0" : desc_cant) : desc_porc) + "|" +
                                               dr["codi_impu"].ToString() + "|" +
                                               dr["indi_exen"].ToString() + "|" +
                                               dr["tipo_codi2"].ToString() + "|" +
                                               dr["codi_item2"].ToString() + "|"+
                                               dr["tipo_codi_liq"].ToString() + "|#$#";
                            txtAgility.Text += dr["cant_refe"].ToString() + "|" +
                                                dr["unid_refe"].ToString() + "|" +
                                                dr["prec_refe"].ToString() + "|" +
                                                dr["prec_mono"].ToString() + "|" +
                                                dr["codi_mone"].ToString() + "|" +
                                                dr["fact_conv"].ToString() + "|" +
                                                dr["reca_item"].ToString() + "|" +
                                                dr["reca_porc"].ToString() + "|" +
                                                dr["fech_elab"].ToString() + "|" +
                                                dr["fech_vepr"].ToString() + "|" +
                                                dr["desc_mone"].ToString() + "|" +
                                                dr["reca_mone"].ToString() + "|" +
                                                dr["valo_mone"].ToString() + "|#$#";
                        }

                        query = string.Format("select tipo_dere, glos_dere, tipo_valo, valo_dere, indi_exen " +
                                "FROM DTE_DESC_RECA " +
                                "where codi_empr = {0} " +
                                "and   tipo_docu = {1} " +
                                "and   foli_docu = {2}",
                                DbnetContext.Codi_empr,DbnetContext.Val1,DbnetContext.Val2 );

                        txtAuxDescuento.Text = "";
                        DbnetTool.ctrlSqlInjection(this.Page.Form);
                        DataTable desc = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                        for (int i = 0; i < desc.Rows.Count; i++)
                        {
                            dr = desc.Rows[i];
                            string tipo = dr["tipo_valo"].ToString();
                            string valor = dr["valo_dere"].ToString();

                            txtAuxDescuento.Text += dr["tipo_dere"].ToString() + "|" +
                                                   dr["glos_dere"].ToString() + "|" +
                                                   dr["tipo_valo"].ToString() + "|" +
                                                   valor + "|" +
                                                   valor + "|" +
                                                   (dr["indi_exen"].ToString() == "1" ? "S" : "N") + "|#$#";
                        }
                        query = string.Format("select si.codi_impu, si.mnsg_erro, si.mont_impu, si.tasa_impu, ti.tipo_impu FROM dte_suma_impu si, dte_tipo_impu ti " +
                                                "where si.codi_empr = {0} " +
                                                "and   si.tipo_docu = {1} " +
                                                "and   si.foli_docu = {2} " +
                                                "and   si.codi_impu=ti.codi_impu ",
                                    DbnetContext.Codi_empr,DbnetContext.Val1,DbnetContext.Val2 ) ;;

                        txtAuxImpuesto.Text = "";
                        DbnetTool.ctrlSqlInjection(this.Page.Form);
                        DataTable impu = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                        for (int i = 0; i < impu.Rows.Count; i++)
                        {
                            dr = impu.Rows[i];
                            txtAuxImpuesto.Text += dr["tipo_impu"].ToString() + "|" +
                                                   dr["codi_impu"].ToString() + "|" +
                                                   Convert.ToDecimal(dr["tasa_impu"].ToString()) + "|" + dr["mont_impu"].ToString() + "|#$#";
                        }

                        query = string.Format("select tipo_refe, foli_refe, " +
                                " fech_refe, codi_refe, razo_refe, indi_regl " +
                                " FROM DTE_DOCU_REFE " +
                                "where codi_empr = {0} " +
                                "and   tipo_docu = {1} " +
                                "and   foli_docu = {2}",
                                    DbnetContext.Codi_empr,DbnetContext.Val1,DbnetContext.Val2 );

                        txtAuxReferencia.Text = "";
                        DbnetTool.ctrlSqlInjection(this.Page.Form);
                        DataTable refe = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                        for (int i = 0; i < refe.Rows.Count; i++)
                        {
                            dr = refe.Rows[i];

                            txtAuxReferencia.Text += dr["tipo_refe"].ToString() + "|" +
                                                     dr["foli_refe"].ToString() + "|" +
                                                     dr["fech_refe"].ToString() + "|" +
                                                     dr["codi_refe"].ToString() + "|" +
                                                     dr["razo_refe"].ToString() + "|" +
                                                     (dr["indi_regl"].ToString() == "1" ? "S" : "N") + "|#$#";
                        }

                        query = string.Format("select codi_tibu, cant_bult, " +
                                " iden_marc, iden_cont, sello_cont, nomb_emis " +
                                " FROM dte_tipo_bult " +
                                "where codi_empr = {0} " +
                                "and   tipo_docu = {1} " +
                                "and   foli_docu = {2} ",
                                    DbnetContext.Codi_empr,DbnetContext.Val1,DbnetContext.Val2 );

                        txtAuxTpoBulto.Text = "";
                        DbnetTool.ctrlSqlInjection(this.Page.Form);
                        DataTable bulto = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                        for (int i = 0; i < bulto.Rows.Count; i++)
                        {
                            dr = bulto.Rows[i];
                            txtAuxTpoBulto.Text += dr["codi_tibu"].ToString() + "|" +
                                                     dr["cant_bult"].ToString() + "|" +
                                                     dr["iden_marc"].ToString() + "|" +
                                                     dr["iden_cont"].ToString() + "|" +
                                                     dr["sello_cont"].ToString() + "|" +
                                                     dr["nomb_emis"].ToString() + "|#$#";
                        }
                        /*si es edicion busco si documento es RCH y existe en dte_docu_lob si es asi lo debe eliminar*/
                        //estadoDocu
                        string xmlbd = "0";
                        string qryEstado = string.Format("select count(*) as total from dte_docu_lob where codi_empr = {0} and tipo_docu = {1} and foli_docu = {2} ",
                                    DbnetContext.Codi_empr,DbnetContext.Val1,DbnetContext.Val2 );
                        DbnetTool.ctrlSqlInjection(this.Page.Form);
                        DataTable xml = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, qryEstado);
                        for (int i = 0; i < xml.Rows.Count; i++)
                        {
                            dr = xml.Rows[i];
                            xmlbd = dr["total"].ToString();
                        }

                        if (estadoDocu == "RCH" && xmlbd != "0")
                        {
                            string queryDelLob = string.Format("delete dte_docu_lob where codi_empr = {0} and tipo_docu = {1} and foli_docu = {2}",
                                    DbnetContext.Codi_empr,DbnetContext.Val1,DbnetContext.Val2 );
                            DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, queryDelLob);

                            query = string.Format("update dte_enca_docu set corr_envi =null, esta_docu1=null, corr_envi1 =null" +
                                           ", feho_firm=null , feho_ted=null, corr_cert_firm=null " +
                                           "where codi_empr={0} and tipo_docu={1} and foli_docu={2} ",
                                    DbnetContext.Codi_empr,DbnetContext.Val1,DbnetContext.Val2 );
                            DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                        }

                        //Comision o cargos
                        string sQuery = string.Format("select tipo_coca, glos_coca, tasa_coca, valo_neto_coca, valo_exen_coca, valo_iva_coca from dte_comi_carg where codi_empr = {0} and tipo_docu = {1} and foli_docu = {2}",
                                                DbnetContext.Codi_empr, DbnetContext.Val1, DbnetContext.Val2);
                        DataTable dtData = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, sQuery);
                        foreach (DataRow item in dtData.Rows)
                        {
                            txtAuxComision.Text += item["tipo_coca"].ToString() + "|" +
                                                item["glos_coca"].ToString() + "|" +
                                                item["tasa_coca"].ToString() + "|" +
                                                item["valo_neto_coca"].ToString() + "|" +
                                                item["valo_exen_coca"].ToString() + "|" +
                                                item["valo_iva_coca"].ToString() + "|#$#";
                        }

                        sQuery = "select comi_neto, comi_exen, comi_ivaa from dte_enca_docu where codi_empr = {0} and tipo_docu = {1} and foli_docu ={2}";
                        sQuery = string.Format(sQuery, DbnetContext.Codi_empr, DbnetContext.Val1, DbnetContext.Val2);
                        dtData = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, sQuery);
                        foreach (DataRow item in dtData.Rows)
                        {
                            txtAuxComiNEI.Text += item["comi_neto"].ToString() + "|" +
                                                    item["comi_exen"].ToString() + "|" +
                                                    item["comi_ivaa"].ToString() + "|#$#";
                        }
                        break;
                    case "I":
                        if (Foli_docu.Text.Equals("0") || string.IsNullOrEmpty(Foli_docu.Text))
                            Session["user_modo"] = "I";
                        else
                            Session["user_modo"] = "M";
                        break;
                    case "D":
                        break;
                }

                // limete de detalle 
                //se define el maximo de lineas de detalle
                lbDetalleMax.Text = "60";

                try
                {
                    query = "SELECT param_value FROM sys_param WHERE param_name = 'EGATE_MAX_DETA' ";
                    DataTable param_detamax = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
                    for (int i = 0; i < param_detamax.Rows.Count; i++)
                    {
                        dr = param_detamax.Rows[i];
                        lbDetalleMax.Text = dr["param_value"].ToString();
                    }
                }
                catch { }
                /*
                 ot->711294    Funcionalidad para RRWINE
                 * Copia de documento
                 */
                if (Request.Params["copia"] == "S")
                {
                    Foli_docu.Text = "";
                    Session["user_modo"] = "I";
                }
                DbnetContext.Key1 = "";DbnetContext.Key2 = "";
                DbnetContext.Key3 = "";DbnetContext.Key4 = "";
                DbnetContext.Key5 = "";DbnetContext.Val1 = "";
                DbnetContext.Val2 = "";DbnetContext.Val3 = "";
                DbnetContext.Val4 = "";DbnetContext.Val5 = "";
            }
        }
        catch (Exception ex)
        {
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. PageLoad()", "");

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
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
    private void InitializeComponent()
    {
        this.barRun.Click += new System.Web.UI.ImageClickEventHandler(this.barRun_Click);
    }
    #endregion
    private bool ValidaFormulario()
    {
        lbMensaje.Text = "";
        bool sw = true;
        return sw;
    }
    private string ValidaFormulario2()
    {
        //validación de datos de cabecera
        msgValidar = "";
        if (lvTipo_Docu.SelectedValue == "0")
            msgValidar += "Ingresar Tipo de Documento" + " <br>";
        if (string.IsNullOrEmpty(Fech_Emis.Text))
            msgValidar += "Ingresar Fecha de Emision" + " <br>";
        if (string.IsNullOrEmpty(fech_venc.Text))
            msgValidar += "Ingresar Fecha de Vencimiento" + " <br>";

        //obtiene los datos ingresados en el detalle
        string[] datos;
        datos = txtAuxDetalle.Text.Split(new string[] { "#$#" }, StringSplitOptions.None);
        int l = 1;

        for (int i = 0; i < datos.Length - 1; i++)
        {
            string[] campos = datos[i].Split('|');

            if (string.IsNullOrEmpty(campos[0]))
                msgValidar += " Ingresar tipo en el detalle item " + l;
            if (string.IsNullOrEmpty(campos[1]))
                msgValidar += " | Ingresar codigo en el detalle item " + l;
            if (string.IsNullOrEmpty(campos[2]))
                msgValidar += " | Ingresar nombre de producto en el detalle item " + l;
            if (string.IsNullOrEmpty(campos[4]))
                msgValidar += " | Ingresar cantidad de productos en el detalle item " + l;
            if (string.IsNullOrEmpty(campos[6]))
                msgValidar += " | Ingresar precio de producto en el detalle item " + l + " <br>";

            l++; // indicador para el numero de item
        }
        // valida cuando no se ha ingresado un detalle 
        if (datos.Length == 1)
            msgValidar += " Debe ingresar detalle" + " <br>";

        //valida referencias
        bool validaDatosRefe = false;
        bool global = false;
        if (Tipo_docu.Text == "61") validaDatosRefe = true;
        if (Tipo_docu.Text == "56") validaDatosRefe = true;
        if (Tipo_docu.Text == "111") validaDatosRefe = true;
        if (Tipo_docu.Text == "112") validaDatosRefe = true;


        l = 0;
        datos = txtAuxReferencia.Text.Split(new string[] { "#$#" }, StringSplitOptions.None);
        if (validaDatosRefe)
        {
            if (datos.Length == 1)
                msgValidar += " Debe ingresar referencia" + " <br>";
        }
        for (int i = 0; i < datos.Length - 1; i++)
        {
            string[] camposrefe = datos[i].Split('|');
            global = camposrefe[5] == "S";
            if (Tipo_docu.Text == "61") validaDatosRefe = true;
            if (Tipo_docu.Text == "56") validaDatosRefe = true;
            if (Tipo_docu.Text == "111") validaDatosRefe = true;
            if (Tipo_docu.Text == "112") validaDatosRefe = true;
            if (validaDatosRefe)
            {
                if (camposrefe[1] == "" && !global)
                    msgValidar += "Ingresar folio de referencia " + l;
                if (camposrefe[2] == "" && !global)
                    msgValidar += "Ingresar fecha de referencia " + l;
                if (camposrefe[3] == "")
                    msgValidar += "Ingresar codigo de referencia " + l;
            }
        }

        //validar documento 52
        if (Tipo_docu.Text == "52")
        {
            if (txtTotalBultos.Text == "0")
                txtTotalBultos.Text = "";

            if (txtTotalBultos.Text != "")
                if (lvIndi_vegd.SelectedValue != "8" && lvIndi_vegd.SelectedValue != "9")
                    msgValidar += "Seleccionar indicador de traslado 'Traslado para exportación' o 'Venta para exportación' para Guia de despacho " + " <br>";
        }

        string[] totales = txtTotales.Text.Split('|');
        if ( (new string[] { "110", "111", "112" }.Contains(Tipo_docu.Text)) && (cmbMoneda.SelectedValue.ToString() != "PESO CL") && (Convert.ToDouble(totales[3]) > 0) )
        {
            if (string.IsNullOrEmpty(tipo_camb.Text))
            {
                msgValidar += "Ingresar Monto Tipo de Cambio ";
            }
        }

        return msgValidar;
    }
    private void ValidaPostFormulario()
    {
        lbMensaje.Text = "";
        lb_msgschema.Text = "";
        msgSchema = "";
        isValid = true;
        //definir rutas
        string dirEsuite = DbnetDir.direESuite(DbnetContext.dbConnection);
        string dirted = dirEsuite + "in\\ted\\";
        string Certificado = dirEsuite + "config\\cer\\test.pem ";
        string dirTemp = dirEsuite + "temp\\";
        string NombreArchivo = dirTemp + "xml_temp" + ".xml";
        string NombreArchivoFirmado = dirTemp + "xml_temp_sign" + ".xml";
        string schema = dirEsuite + "config\\schema\\DTE_v10.xsd ";
        string archRpta = dirTemp + "salida_valid" + ".txt";
        string schema2 = dirTemp + "DTE_v10.xsd ";
        string NombreArchivoFirmado2 = dirTemp + "ejemplo_valid_ok.xml";

        //Eliminar el ted si existe
        try
        {
            string nomted = DbnetTool.Nombre_Estandar(DbnetContext, DbnetContext.Codi_empr.ToString(), Tipo_docu.Text, Foli_docu.Text);
            nomted += ".ted";
            File.Delete(dirted + nomted);
        }
        catch
        {

        }
        //proceso de generar xml desde BDpdf
        string proceso = "egateDTE";
        string parametros = " -te bd -ts xml -prev ";
        parametros += " -empr " + DbnetContext.Codi_empr;
        parametros += " -tdte " + Tipo_docu.Text;
        parametros += " -fdte " + Foli_docu.Text;
        parametros += " -s " + NombreArchivo;
        parametros += " -f ";
        parametros += " -acert " + Certificado;
        lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, proceso, parametros);

        //agregar atributos al xml firmado
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(NombreArchivoFirmado);

        XmlAttribute atributo1, atributo2, atributo3, atributo4;

        xmldoc.DocumentElement.Attributes.RemoveAll();
        atributo1 = xmldoc.CreateAttribute("xmlns");
        atributo1.Value = "http://www.sii.cl/SiiDte";
        atributo2 = xmldoc.CreateAttribute("xmlns:xsi");
        atributo2.Value = "http://www.w3.org/2001/XMLSchema-instance";
        atributo3 = xmldoc.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
        atributo3.Value = "http://www.sii.cl/SiiDte " + schema;
        atributo4 = xmldoc.CreateAttribute("version");
        atributo4.Value = "1.0";

        xmldoc.DocumentElement.Attributes.Append(atributo1);
        xmldoc.DocumentElement.Attributes.Append(atributo2);
        xmldoc.DocumentElement.Attributes.Append(atributo3);
        xmldoc.DocumentElement.Attributes.Append(atributo4);
        xmldoc.Save(NombreArchivoFirmado);

        /// Validar schema desde aplicacion
        XmlTextReader r = new XmlTextReader(NombreArchivoFirmado);

        XmlValidatingReader v = new XmlValidatingReader(r);
        v.ValidationType = ValidationType.Schema;
        v.ValidationEventHandler += new ValidationEventHandler(MyValidationEventHandler);
        // leer el xml para que se active el evento de validacion
        while (v.Read())
        {}
        v.Close();

        // Comprobar si el documento es válido o no.
        if (isValid)
        {
            lblschema.Text = "Documento Valido";
            lb_msgschema.Text = "";
        }
        else
        {
            lblschema.Text = "Documento NO Valido";
            lb_msgschema.Text = msgSchema;
        }

    }
    public static void MyValidationEventHandler(object sender, ValidationEventArgs args)
    {
        isValid = false;
        msgSchema += args.Message + '\n';
    }
    private string comp_cmb(string texto, string campo, bool accion)
    {
        Exception myException;
        if (texto == "[Seleccione]")
        {
            if (accion)
            {
                myException = new Exception("Debe seleccionar un valor para el campo " + campo + "!");
                throw myException;
            }
            else
                return "";
        }
        else
            return texto;
    }
    private void grabarDTE()
    {
        string sLugar = "grabarDTE()";
        try
        {
            string v_Tasa_vaag;
            if (Tipo_docu.Text == "34" || Tipo_docu.Text == "110")
                v_Tasa_vaag = "";
            else
                v_Tasa_vaag = DbnetTool.SelectInto(DbnetContext.dbConnection, "select PORC_IMPU from dte_tipo_impu where codi_impu = 14");

            string sQuery = string.Format("select count(*) from dte_enca_docu where esta_docu not in ('PEN','RCH') and codi_empr = {0} and tipo_docu = {1} and foli_docu = {2}", 
                                    DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text);

            string oData = DbnetTool.SelectInto(DbnetContext.dbConnection, sQuery);
            if (oData == "0")
            {
                string p_existe = string.Empty;
                string v_Vers_enca = string.Empty;
                sLugar = "antes de llamar a pra_get_val";
                DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "para_get_val",
                    "paem_codi_paem", "EGATE_VERS_ENCA", "VarChar", 30, "in",
                    "paem_valo_paem", "", "VarChar", 100, "out",
                    "p_existe", "", "VarChar", 1, "out",
                    "p_mensaje", "", "VarChar", 200, "out");

                p_existe = sp.return_String("p_existe");
                if (p_existe == "N")
                {
                    lbMensaje.Text = sp.return_String("p_mensaje");
                    return;
                }
                else
                    v_Vers_enca = sp.return_String("paem_valo_paem");

                string v_Rutt_emis = string.Empty;
                string v_Digi_emis = string.Empty;
                string v_Nomb_emis = string.Empty;
                string v_Dire_emis = string.Empty;
                string v_Giro_emis = string.Empty;
                string v_Comu_emis = string.Empty;
                string v_Ciud_emis = string.Empty;
                string queryRamo = string.Empty;
                queryRamo = "SELECT codi_ramo FROM empr where codi_empr="+ DbnetContext.Codi_empr.ToString() + " ";
                string codiRamo = DbnetTool.SelectInto(DbnetContext.dbConnection, queryRamo);

                if (codiRamo.Length == 0)
                {
                    queryRamo = "SELECT nomb_ramo FROM ramo where codi_ramo = '" + codiRamo + "'";
                    string codiRamoR = DbnetTool.SelectInto(DbnetContext.dbConnection, queryRamo);
                    if (codiRamoR.Length > 0)
                    {
                        if (DbnetTool.SelectInto(DbnetContext.dbConnection, "SELECT codi_ramo FROM ramo where nomb_ramo='" + codiRamoR + "' AND codi_empr = "+ DbnetContext.Codi_empr.ToString() + " ").Length == 0)
                        {
                            queryRamo = "Insert into ramo(codi_ramo,nomb_ramo,codi_empr)" +
                                "values (" +
                                "'" + codiRamo + "'," +
                                "'" + codiRamoR + "'," +
                                "" + DbnetContext.Codi_empr.ToString() + ")";

                            DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, queryRamo);
                        }
                    }
                }

                sLugar = "antes de llamar a dte_empr_dato";
                DbnetProcedure sp1 = new DbnetProcedure(DbnetContext.dbConnection, "dte_empr_dato",
                "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                "prutt_empr", "", "VarChar", 10, "out",
                "pdigi_empr", "", "VarChar", 1, "out",
                "pnomb_empr", "", "VarChar", 100, "out",
                "pdire_empr", "", "VarChar", 50, "out",
                "pdesc_giro", "", "VarChar", 40, "out",
                "pcodi_sii_empr", "", "VarChar", 6, "out",
                "pnomb_coem", "", "VarChar", 20, "out",
                "pnomb_ciem", "", "VarChar", 20, "out",
                "p_existe", "", "VarChar", 1, "out",
                "p_mensaje", "", "VarChar", 200, "out");

                p_existe = sp1.return_String("p_existe");
                if (p_existe == "N")
                {
                    lbMensaje.Text = sp1.return_String("p_mensaje");
                    return;
                }
                else
                {
                    v_Rutt_emis = sp1.return_String("prutt_empr");
                    v_Digi_emis = sp1.return_String("pdigi_empr");
                    v_Nomb_emis = sp1.return_String("pnomb_empr");
                    v_Dire_emis = sp1.return_String("pdire_empr");
                    v_Giro_emis = sp1.return_String("pdesc_giro");
                    v_Comu_emis = sp1.return_String("pnomb_coem");
                    v_Ciud_emis = sp1.return_String("pnomb_ciem");
                }
                query = "";
                int largo;
                largo = (lvGiros.Descripcion.Length > 40) ? 40 : lvGiros.Descripcion.Length;
                asObjectException obj = new asObjectException();

                string[] totales = txtTotales.Text.Split('|');

                sLugar = "antes de obj.Set" + ", txtTotales.Text:[" + txtTotales.Text + "]";
                obj.Set("codi_empr", DbnetContext.Codi_empr.ToString(), 1);
                obj.Set("tipo_docu", Tipo_docu.Text, 1);
                //TODO-AM obj.Set("foli_docu", Foli_docu.Text, 1, "Error al asignarse un Folio");
                string temp = "200";
                obj.Set("foli_docu", temp, 1);

                obj.Set("esta_docu", Esta_docu.Text, 0);
                obj.Set("fech_emis", Convert.ToDateTime(Fech_Emis.Text).ToString("yyyy-MM-dd"), 0);

                obj.Set("form_pago", lvForm_Pago.SelectedValue == "0" ? "nulo" : lvForm_Pago.SelectedValue, 0);
                obj.Set("moda_pago", lvModa_Pago.SelectedValue == "0" ? "" : lvModa_Pago.SelectedValue, 0);
                obj.Set("impr_dest", cmbImpresora.SelectedIndex == 0 ? "" : cmbImpresora.Descripcion.ToString(), 0);
                obj.Set("fech_venc", Convert.ToDateTime(fech_venc.Text).ToString("yyyy-MM-dd"), 0);
                obj.Set("rutt_emis", v_Rutt_emis.ToString(), 1, "Rut de emisor, no se logro determinar y este");
                obj.Set("digi_emis", v_Digi_emis, 0, "Rut de emisor, no se logro determinar y este");
                obj.Set("nomb_emis", v_Nomb_emis.Replace("'", "''"), 0, "Nombre de emisor, no se logro determinar y este ");
                obj.Set("giro_emis", v_Giro_emis.Replace("'", "''"), 0, "Giro de Emisor, no se logro determinar y este");
                obj.Set("dire_orig", v_Dire_emis.Replace("'", "''"), 0, "Direccion de emisor, no se logro determinar y este");
                obj.Set("comu_orig", v_Comu_emis.Replace("'", "''"), 0, "Comuna de emisor, no se logro determinar y este");
                obj.Set("ciud_orig", v_Ciud_emis.Replace("'", "''"), 0, "Ciudad de Emisor, no se logro determinar y este");
                //si es 110, 111 o 112 cambio el rut por el 5555555-5
                if (Tipo_docu.Text.Equals("110") || Tipo_docu.Text.Equals("111") || Tipo_docu.Text.Equals("112"))
                {
                    obj.Set("rutt_rece", "55555555", 0, "Rut de Receptor");
                    obj.Set("digi_rece", "5", 0, "Digito Verificador del Receptor");
                }
                else
                {
                    obj.Set("rutt_rece", Rutt_rece.Text, 0, "Rut de Receptor");
                    obj.Set("digi_rece", Digi_rece.Text.ToUpper(), 0, "Digito Verificador del Receptor");
                }

                obj.Set("nomb_rece", txtNomb_rece.Text, 0, lb_Fete_auto.Text);

                obj.Set("codi_rece", Giro_rece.Text.Replace("'", "''"), 0);
                obj.Set("giro_rece", lvGiros.Descripcion.ToString().Substring(0, largo), 0);
                obj.Set("dire_rece", Dire_rece.Text.Replace("'","''"), 0);
                obj.Set("comu_rece", Comu_rece.Text.Replace("'", "''"), 0);
                obj.Set("ciud_rece", Ciud_rece.Text.Replace("'", "''"), 0);
                obj.Set("indi_vegd", lvIndi_vegd.SelectedValue, 1);
                obj.Set("rutt_tran", Rutt_tran.Text, 1);
                obj.Set("digi_tran", Digi_tran.Text, 0);
                obj.Set("info_tran", Patente_tran.Text.Replace("'", "''"), 0);
                obj.Set("dire_dest", Dire_dest.Text.Replace("'", "''"), 0);
                obj.Set("comu_dest", Comu_dest.Text.Replace("'", "''"), 0);
                obj.Set("cont_rece", Fono.Text, 0);
                obj.Set("ciud_dest", Ciud_dest.Text.Replace("'", "''"), 0);
                obj.Set("vers_enca", v_Vers_enca, 0);
                obj.Set("tasa_vaag", (string.IsNullOrEmpty(v_Tasa_vaag) ? "0" : v_Tasa_vaag.Replace(",", ".")), 1);
                if (Tipo_docu.Text == "34" || Tipo_docu.Text == "110" || Tipo_docu.Text == "111" || Tipo_docu.Text == "112")
                {
                    obj.Set("impu_vaag", "0", 1);
                    obj.Set("mont_neto", "0", 1);
                }
                else
                {
                    obj.Set("impu_vaag", (string.IsNullOrEmpty(totales[2]) ? "0" : totales[2].Replace(",", ".")), 1);
                    obj.Set("mont_neto", dbnFormat.Numero(totales[0], "###0"), 1);
                }
                switch (Tipo_docu.Text)
                {
                    case "33":
                    case "56":
                    case "61":
                        obj.Set("CRED_ES65", txtCredec.Text.Trim(), 1);
                        break;
                    case "52":
                    case "110":
                    case "111":
                    case "112":
                        if (!string.IsNullOrEmpty(txtMntSeguro.Text) && !txtMntSeguro.Text.Equals("0"))
                        {
                            decimal dMontSegu;
                            decimal.TryParse(txtMntSeguro.Text, out dMontSegu);
                            string valiMont = dMontSegu.ToString();
                            if (valiMont.Contains(","))
                            {
                                valiMont = valiMont.Substring(0, valiMont.IndexOf(","));
                                if (valiMont.Length > 14)
                                {
                                    DbnetTool.MsgAlerta("No se puede ingresar más de 14 decimales", Page);
                                    return;
                                }
                            }
                            else if(valiMont.Length>14)
                            {
                                DbnetTool.MsgAlerta("No se puede ingresar más de 14 decimales", Page);
                                return;
                            }

                            if(dMontSegu >0)
                                obj.Set("MONT_SEGU", dMontSegu.ToString().Replace(",","."), 1);
                        }
                        break;
                }

                if (Tipo_docu.Text == "110" || Tipo_docu.Text == "111" || Tipo_docu.Text=="112")
                {


                }

                if (!string.IsNullOrEmpty(txtMntMargenCom.Text) && !txtMntMargenCom.Text.Equals("0"))
                {
                    double dMontBaco;
                    double.TryParse(txtMntMargenCom.Text.Replace(",","."), out dMontBaco);
                    if(dMontBaco >0)
                        obj.Set("MONT_BACO", dMontBaco.ToString(), 1);
                }

                obj.Set("mont_exen", (string.IsNullOrEmpty(totales[1].Replace(",", ".")) ? "0" : totales[1].Replace(",", ".")), 1);
                obj.Set("mont_nofa", (string.IsNullOrEmpty(totales[6].Replace(",", ".")) ? "0" : totales[6].Replace(",", ".")), 1);
                obj.Set("mont_tota", (string.IsNullOrEmpty(totales[3].Replace(",", ".")) ? "0" : totales[3].Replace(",", ".")), 1);
                obj.Set("foli_clie", Foli_docu_rp.Text, 0);

                obj.Set("from_paex", comp_cmb(cmbFormaPago.SelectedValue, lblFormaPago.Text, true), 0);
                obj.Set("moda_vent", comp_cmb(cmbModalidad.SelectedValue, lblModa.Text, true), 0);
                obj.Set("clau_expo", comp_cmb(cmbClausula.SelectedValue, lblClausula.Text, true), 0);
                obj.Set("tota_clex", string.IsNullOrEmpty(txtTotalClausula.Text) ? "0" : txtTotalClausula.Text, 1, "Total Clausula");
                obj.Set("viaa_tran", comp_cmb(cmbTransporte.SelectedValue, lblViaTransporte.Text, true), 0);
                obj.Set("codi_puem", comp_cmb(cmbPuerto.SelectedValue, lblPuerto.Text, true), 0);
                obj.Set("codi_pude", comp_cmb(cmbPuerto2.SelectedValue, lblPuerto2.Text, true), 0);
                obj.Set("tota_bult", string.IsNullOrEmpty(txtTotalBultos.Text) ? "0" : txtTotalBultos.Text, 1, "Total de Bultos");
                obj.Set("mont_flet", string.IsNullOrEmpty(txtFlete.Text) ? "0" : txtFlete.Text, 1, "Monto de Flete");
                obj.Set("pais_rece", cmbPais.SelectedValue == "0" ? "" : cmbPais.SelectedValue, 0);
                obj.Set("tipo_mone", cmbMoneda.SelectedValue == "0" ? "" : cmbMoneda.SelectedItem.Text, 0);
                //obj.Set("tipo_moom", cmbMoneda.SelectedValue == "0" ? "" : cmbMoneda.SelectedItem.Text, 0);
                //obj.Set("tipo_camb", tipo_camb.Text, 1);

                if (Rutt_rece.Text.Equals(v_Rutt_emis) && Tipo_docu.Text.Equals("52"))
                {
                    obj.Set("val5", hdSucuDest.Text, 0);
                    obj.Set("val6", hdSucuOrig.Text, 0);
                }
                else
                {
                    obj.Set("val5", txtVal5.Text, 0);
                    obj.Set("val6", txtVal6.Text, 0);
                }
                obj.Set("val1", txtVal1.Text, 0);
                obj.Set("val2", txtVal2.Text, 0);
                obj.Set("val3", txtVal3.Text, 0);
                obj.Set("val4", txtVal4.Text, 0);
               // obj.Set("val5", txtVal5.Text, 0);
               // obj.Set("val6", txtVal6.Text, 0);
                obj.Set("val7", txtVal7.Text, 0);
                obj.Set("val8", txtVal8.Text, 0);
                obj.Set("val9", txtVal9.Text, 0);
                obj.Set("Unid_neto", comp_cmb(cmbPesoNeto.SelectedValue, "Unidad del Peso Neto", true), 1);
                obj.Set("Unid_brut", comp_cmb(cmbPesoNeto.SelectedValue, "Unidad del Peso Bruto", true), 1);
                obj.Set("Tota_neto", txtPesoNeto.Text, 1);
                obj.Set("Tota_brut", txtPesoBruto.Text, 1);
                obj.Set("codi_usua", DbnetContext.Codi_usua.ToString(), 0);
                obj.Set("VENT_SERV", cmbIndServicio.SelectedValue,0);
                obj.Set("tran_vent", cmbTranVenta.SelectedValue, 0);
                obj.Set("tran_comp", cmbTranCompra.SelectedValue, 0);
                //obj.Set("noaf_otmo", (string.IsNullOrEmpty(totales[1].Replace(",", ".")) ? "0" : totales[4].Replace(",", ".")), 1);
                //obj.Set("mont_otmo", (string.IsNullOrEmpty(totales[1].Replace(",", ".")) ? "0" : totales[5].Replace(",", ".")), 1);
                // Caso Documentos de Exportación con Tipo Moneda distinto a "PESO CL" y monto mayor a 0
                if ((new string[] { "110", "111", "112" }.Contains(Tipo_docu.Text)) && (cmbMoneda.SelectedValue.ToString() != "PESO CL") && (Convert.ToDouble(totales[3]) > 0) )
                {
                    obj.Set("tipo_moom", "PESO CL", 0);
                    obj.Set("tipo_camb", tipo_camb.Text, 1);
                    obj.Set("noaf_otmo", (string.IsNullOrEmpty(totales[1].Replace(",", ".")) ? "0" : totales[4].Replace(",", ".")), 1);
                    obj.Set("mont_otmo", (string.IsNullOrEmpty(totales[1].Replace(",", ".")) ? "0" : totales[5].Replace(",", ".")), 1);
                }

                if (string.IsNullOrEmpty(valo_paga.Text))
                { valo_paga.Text = "0"; }

                obj.Set("valo_paga", valo_paga.Text, 1);
                if (Tipo_docu.Text == "43")
                {
                    string[] oData1 = txtAuxComiNEI.Text.Split('|');
                    obj.Set("comi_neto", oData1[0], 0);
                    obj.Set("comi_exen", oData1[1], 0);
                    obj.Set("comi_ivaa", oData1[2], 0);
                }

                obj.Set("nume_book", txtBoking.Text, 0);
                obj.Set("codi_oper", txtOperador.Text, 0);
                obj.Set("nomb_tran", txtNombreTransp.Text, 0);

                sLugar = "antes de sQueryCount";
                string where = string.Empty;
                string accion = "I";
                string sQueryCount = string.Format("select count(*) from dte_enca_docu where codi_empr = {0} and tipo_docu = {1} and foli_docu = {2} and rutt_emis = {3} ", 
                                            DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text, v_Rutt_emis);
                string s1 = DbnetTool.SelectInto(DbnetContext.dbConnection, sQueryCount);
                if (s1 == "1")
                {
					where = string.Format(" codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2} and rutt_emis = {3} ",
                                            DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text, v_Rutt_emis);
                    accion = "U";
                }

                sLugar = "antes de Insertar/Actualizar Registro";
                query = DbnetTool.SQL(accion, "DTE_ENCA_DOCU", obj, where);
                DbnetTool.ctrlSqlInjection(this.Page.Form);
                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                obj.Clear();
                sLugar = "antes de actualizar a estado PEN";
				query = string.Format("UPDATE dte_enca_docu set esta_docu = 'PEN' where codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2} and rutt_emis = {3} ",
                                            DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text, v_Rutt_emis);
                DbnetTool.ctrlSqlInjection(this.Page.Form);
                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

                //if (Session["user_modo"].ToString().Contains("I"))
                graba_acec();

                //DETALLE dte_deta_prse
                graba_detalle();

                //DESCUENTO dte_desc_reca
                graba_descuento();

                //IMPUESTO dte_suma_impu
                graba_impuesto();

                //REFERENCIA dte_docu_refe
                graba_referencia();

                //REFERENCIA dte_tipo_bult
                graba_tpoBultos();
                //graba_exportacion();

                //Comisiones o Cargos
                Graba_Comisiones();

                DbnetTool.MsgAlerta("Cambios Ejecutado, Folio Nº: " + Foli_docu.Text, this.Page);
                Session["user_modo"] = "M";
                Foli_docu.Enabled = false;

                
                    btEnvSii.Enabled = lb_msgschema.Text == "";
                

            }
            else
            {
                lbMensaje.Text = "El folio del documento ya existe";
                DbnetTool.MsgAlerta("El folio del documento ya existe y/o ha sido utilizado", this.Page);
            }
        }
        catch (Exception ex)
        {
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. Grabar DTE", sLugar);
            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
        }
    }
    public string restacero(string valor_string)
    {
        if (valor_string.IndexOf('.') > 0)
        {
            int s = 0;
            for (int x = 1; x <= valor_string.Length; x++)
            {
                string var = valor_string.Substring(valor_string.Length - x, 1);
                if (var != "0" || var == ".")
                {
                    s = x - 1;
                    if (var == ".")
                    {
                        s = s + 1;
                    }
                    break;
                }
            }
            valor_string = valor_string.Substring(0, valor_string.Length - s);
        }
        return valor_string;
    }
    private void graba_acec()
    {
        string sLugar = "graba_acec()";
        try
        {
            query = string.Format("DELETE DTE_DETA_ACEC WHERE codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2} AND corr_acec = '0'", 
                            DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text);
            DbnetTool.ctrlSqlInjection(this.Page.Form);
            DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
            string acec = "";
            acec = DbnetTool.SelectInto(DbnetContext.dbConnection, "select codi_acec from dte_acec_empr where codi_empr =" + DbnetContext.Codi_empr);

            query = "insert into DTE_DETA_ACEC (" +
                    " codi_empr, tipo_docu, foli_docu,corr_acec ,codi_acec)  values  (" +
                    "'" + DbnetContext.Codi_empr + "'," +
                    "'" + Tipo_docu.Text + "'," +
                    "'" + Foli_docu.Text + "'," +
                    "'0'," +
                    "'" + acec + "')";
            DbnetTool.ctrlSqlInjection(this.Page.Form);
            DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
        }
        catch (Exception ex)
        {
            //sLugar
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. graba_acec()", sLugar);

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
        }
    }
    private void graba_detalle()
    {
        string sLugar = "graba_detalle()";
        query = string.Format("DELETE DTE_DETA_CODI WHERE codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2}", 
                        DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text);
        DbnetTool.ctrlSqlInjection(this.Page.Form);
        DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

        query = string.Format("DELETE DTE_DETA_PRSE WHERE codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2}", 
                        DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text);
        DbnetTool.ctrlSqlInjection(this.Page.Form);
        DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

        try
        {
            string[] datos = txtAuxDetalle.Text.Split(new string[] { "#$#" }, StringSplitOptions.None);
            string[] sAdicionales = txtAgility.Text.Split(new string[] { "#$#" }, StringSplitOptions.None);
            asObjectException obj = new asObjectException();
            for (int i = 0; i < datos.Length - 1; i++)
            {
                string[] campos = datos[i].Split('|'); sLugar = "datos[i]" + datos[i];
                string[] sAdicionales1;
                try
                {
                    if (sAdicionales.Length > 1)
                        sAdicionales1 = sAdicionales[i].Split('|');
                    else
                        sAdicionales1 = new string[] { };
                }
                catch (Exception)
                { sAdicionales1 = new string[] { }; }
                query = "";
                obj.Set("codi_empr", DbnetContext.Codi_empr.ToString(), 1);
                obj.Set("tipo_docu", Tipo_docu.Text, 1);
                obj.Set("foli_docu", Foli_docu.Text, 1);
                obj.Set("nume_line", Convert.ToString(i + 1), 1);
                obj.Set("nomb_item", campos[2].Replace("'", "''"), 0);
                //cuando aparesca comilla simple pasar a '' para q insert
                obj.Set("desc_item", campos[3].Replace("'", "''"), 0);
                string cant_item = restacero(campos[4].ToString().Replace(",", "."));
                obj.Set("cant_item", cant_item, 1);
                obj.Set("unid_medi", campos[5] == "0" ? "" : campos[5], 0);
                string prec_item = restacero(campos[6].ToString().Replace(",", "."));
                obj.Set("prec_item", prec_item, 1);
                string total;
                if (Tipo_docu.Text == "110" || Tipo_docu.Text == "111" || Tipo_docu.Text == "112")
                    total = campos[7].ToString();
                else
                {
                    decimal tto = Math.Round(Convert.ToDecimal(campos[7].Replace(".",",")));
                    total = tto.ToString();
                }

                string neto_item = restacero(total.ToString().Replace(".", ","));
                
                //se comenta ya que no estaba grabando el dcto_item para los porcentajes
                if (campos[8] == "$")
                {
                    string dcto_item = restacero(campos[9].ToString().Replace(",", "."));
                    obj.Set("dcto_item", dcto_item, 1);

                    Decimal netoItem = Convert.ToDecimal(neto_item);
                    obj.Set("neto_item", netoItem.ToString(), 1);
                }
                else
                {
                    string desc_porc = restacero(campos[9].ToString().Replace(",", "."));
                    obj.Set("desc_porc", desc_porc, 1);

                    Decimal precItem = Convert.ToDecimal(prec_item);
                    Decimal descProc = Convert.ToDecimal(desc_porc);

                    string dcto_item = Convert.ToString(Math.Round((precItem * descProc) / 100, 0).ToString()).Replace(",", ".");
                    obj.Set("dcto_item", dcto_item, 1);

                    Decimal netoItem = Convert.ToDecimal(neto_item);
                    obj.Set("neto_item", netoItem.ToString(), 1);
                }
                obj.Set("codi_impu", campos[10].Trim(), 0);
                obj.Set("indi_exen", campos[11], 1); sLugar = sLugar + "135";
                if (sAdicionales1.Length > 1)
                {
                    obj.Set("cant_refe", sAdicionales1[0], 1);
                    obj.Set("unid_refe", sAdicionales1[1], 0);
                    obj.Set("prec_refe", sAdicionales1[2], 1);
                    obj.Set("prec_mono", sAdicionales1[3], 1);
                    obj.Set("codi_mone", sAdicionales1[4], 0);
                    obj.Set("fact_conv", sAdicionales1[5], 1);
                    obj.Set("reca_item", sAdicionales1[6], 1);
                    obj.Set("reca_porc", sAdicionales1[7], 1);
                    obj.Set("fech_elab", sAdicionales1[8], 0);
                    obj.Set("fech_vepr", sAdicionales1[9], 0);
                    obj.Set("desc_mone", sAdicionales1[10].Replace("'", "''"), 1);
                    obj.Set("reca_mone", sAdicionales1[11], 1);
                    obj.Set("valo_mone", sAdicionales1[12], 1);
                }

                if (Tipo_docu.Text == "43")
                {
                    obj.Set("tipo_codi", campos[14].Trim(), 0);
                }
                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, DbnetTool.SQL("I", "DTE_DETA_PRSE", obj, ""));
                obj.Clear();
                //primero tengo que obtener la linea para los dos la misma
                // y solo cambia el corr del codigo
                sLugar = "antes insert dte_deta_codi";
                int corr_codi = 1;
                if (!string.IsNullOrEmpty(campos[0].Trim()))
                {
                    obj.Set("codi_empr", DbnetContext.Codi_empr.ToString(), 1);
                    obj.Set("tipo_docu", Tipo_docu.Text, 1);
                    obj.Set("foli_docu", Foli_docu.Text, 1);
                    obj.Set("nume_line", Convert.ToString(i + 1), 1);
                    obj.Set("corr_codi", Convert.ToString(corr_codi), 1);
                    obj.Set("tipo_codi", campos[0], 0); /*tipo_codi1*/
                    obj.Set("codi_item", campos[1], 0); /*codi_item1*/
                    DbnetTool.ctrlSqlInjection(this.Page.Form);
                    DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, DbnetTool.SQL("I", "DTE_DETA_CODI", obj, ""));
                    obj.Clear();
                }
                if (!string.IsNullOrEmpty(campos[12].Trim()))
                {
                    if (campos[0] != "") { corr_codi = corr_codi + 1; }
                    obj.Set("codi_empr", DbnetContext.Codi_empr.ToString(), 1);
                    obj.Set("tipo_docu", Tipo_docu.Text, 1);
                    obj.Set("foli_docu", Foli_docu.Text, 1);
                    obj.Set("nume_line", Convert.ToString(i + 1), 1);
                    obj.Set("corr_codi", Convert.ToString(corr_codi), 1);
                    obj.Set("tipo_codi", campos[12], 0); /*tipo_codi2*/
                    obj.Set("codi_item", campos[13], 0); /*codi_item2*/
                    DbnetTool.ctrlSqlInjection(this.Page.Form);
                    DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, DbnetTool.SQL("I", "DTE_DETA_CODI", obj, ""));
                    obj.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            //sLugar
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. Detalle", sLugar);

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
        }
    }
    private void graba_descuento()
    {
        string sLugar = "graba_descuento()";
        query = string.Format("DELETE DTE_DESC_RECA WHERE codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2}",
                        DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text);
        DbnetTool.ctrlSqlInjection(this.Page.Form);
        DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);

        try
        {
            DbnetTool.ctrlSqlInjection(this.Page.Form);
            string[] datos = txtAuxDescuento.Text.Split(new string[] { "#$#" }, StringSplitOptions.None);
            asObjectException obj = new asObjectException();

            for (int i = 0; i < datos.Length - 1; i++)
            {

                string[] campos = datos[i].Split('|');
                query = "";

                obj.Set("codi_empr", DbnetContext.Codi_empr.ToString(), 1);
                obj.Set("tipo_docu", Tipo_docu.Text, 1);
                obj.Set("foli_docu", Foli_docu.Text, 1);
                obj.Set("nume_dere", Convert.ToString(i + 1), 1);
                obj.Set("tipo_dere", campos[0], 0);
                obj.Set("glos_dere", campos[1].Replace("'", "''"), 0);
                obj.Set("tipo_valo", campos[2], 0);
                decimal tto = Math.Round(Convert.ToDecimal(campos[2] == "%" ? Convert.ToString(campos[3].Replace(".", ",")) : Convert.ToString(campos[4].Replace(".", ","))));
                obj.Set("valo_dere", tto.ToString(), 1);
                obj.Set("indi_exen", (campos[5] != "S" ? "0" : "1"), 1);

                DbnetTool.ctrlSqlInjection(this.Page.Form);
                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, DbnetTool.SQL("I", "DTE_DESC_RECA", obj, ""));
                obj.Clear();
            }
        }
        catch (Exception ex)
        {
            //sLugar
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. Descuento", sLugar);

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
        }
    }
    private void graba_impuesto()
    {
        string sLugar = "graba_impuesto()";
        query = string.Format("DELETE DTE_SUMA_IMPU WHERE codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2}",
                        DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text);
        
        DbnetTool.ctrlSqlInjection(this.Page.Form);
        DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
        asObjectException obj = new asObjectException();
        try
        {
            DbnetTool.ctrlSqlInjection(this.Page.Form);
            string[] datos = txtAuxImpuesto.Text.Split(new string[] { "#$#" }, StringSplitOptions.None);
            for (int i = 0; i < datos.Length - 1; i++)
            {
                string[] campos = datos[i].Split('|');
                query = "";

                obj.Set("codi_empr", DbnetContext.Codi_empr.ToString(), 1);
                obj.Set("tipo_docu", Tipo_docu.Text, 1);
                obj.Set("foli_docu", Foli_docu.Text, 1);
                obj.Set("codi_impu", campos[1], 0);
                obj.Set("mnsg_erro", campos[0].Replace("'", "''"), 0);
                decimal tto = Math.Round(Convert.ToDecimal(campos[3].Replace(".", ",")));
                obj.Set("mont_impu", tto.ToString(), 1);
                obj.Set("tasa_impu", Convert.ToDecimal(campos[2].Replace("%", "")).ToString(), 1);
                DbnetTool.ctrlSqlInjection(this.Page.Form);
                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, DbnetTool.SQL("I", "DTE_SUMA_IMPU", obj, ""));
                obj.Clear();
            }
        }
        catch (Exception ex)
        {
            //sLugar
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. Impuesto", sLugar);

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
        }
    }
    private void graba_referencia()
    {
        string sLugar = "graba_referencia()";
        query = string.Format("DELETE DTE_DOCU_REFE WHERE codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2}",
                        DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text);
        DbnetTool.ctrlSqlInjection(this.Page.Form);
        DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
        asObjectException obj = new asObjectException();
        try
        {
            DbnetTool.ctrlSqlInjection(this.Page.Form);
            string[] datos = txtAuxReferencia.Text.Split(new string[] { "#$#" }, StringSplitOptions.None);
            for (int i = 0; i < datos.Length - 1; i++)
            {
                string[] campos = datos[i].Split('|');
                query = "";
                obj.Set("codi_empr", DbnetContext.Codi_empr.ToString(), 1);
                obj.Set("tipo_docu", Tipo_docu.Text, 0);
                obj.Set("foli_docu", Foli_docu.Text, 0);
                obj.Set("nume_refe", Convert.ToInt16(i + 1).ToString(), 1);
                obj.Set("tipo_refe", campos[0], 0);
                obj.Set("foli_refe", campos[1], 0);
                obj.Set("fech_refe", Convert.ToDateTime(campos[2]).ToString("yyyy-MM-dd"), 0);
                obj.Set("codi_refe", campos[3], 0);
                obj.Set("razo_refe", campos[4].Replace("'", "''"), 0);
                obj.Set("indi_regl", campos[5] != "S" ? "0" : "1", 1);
                DbnetTool.ctrlSqlInjection(this.Page.Form);
                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, DbnetTool.SQL("I", "DTE_DOCU_REFE", obj, ""));
                obj.Clear();
            }
        }
        catch (Exception ex)
        {
            //sLugar
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. Referencias", sLugar);

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
        }
    }
    private void graba_tpoBultos()
    {
        string sLugar = "graba_impuesto()";
        query = string.Format("DELETE DTE_TIPO_BULT WHERE codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2}",
                        DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text);
        DbnetTool.ctrlSqlInjection(this.Page.Form);
        DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
        asObjectException obj = new asObjectException();
        try
        {
            DbnetTool.ctrlSqlInjection(this.Page.Form);
            string[] datos = txtAuxTpoBulto.Text.Split(new string[] { "#$#" }, StringSplitOptions.None);

            for (int i = 0; i < datos.Length - 1; i++)
            {
                string[] campos = datos[i].Split('|');

                obj.Set("codi_empr", DbnetContext.Codi_empr.ToString(), 1);
                obj.Set("tipo_docu", Tipo_docu.Text, 1);
                obj.Set("foli_docu", Foli_docu.Text, 1);
                obj.Set("codi_tibu", campos[0], 1);
                obj.Set("cant_bult", campos[1], 1);
                obj.Set("iden_marc", campos[2].Replace("'", "''"), 0);
                obj.Set("iden_cont", campos[3].Replace("'", "''"), 0);
                obj.Set("sello_cont", campos[4].Replace("'", "''"), 0);
                obj.Set("nomb_emis", campos[5].Replace("'", "''"), 0);
                DbnetTool.ctrlSqlInjection(this.Page.Form);
                DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, DbnetTool.SQL("I", "dte_tipo_bult", obj, ""));
                obj.Clear();
            }
        }
        catch (Exception ex)
        {
            //sLugar
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. TipoBulto", sLugar);

            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
        }
    }
    private void Graba_Comisiones()
    {
        string sLugar = "graba_impuesto()";
        query = string.Format("DELETE DTE_COMI_CARG WHERE codi_empr = {0} AND tipo_docu = {1} AND foli_docu = {2} ",
                        DbnetContext.Codi_empr, Tipo_docu.Text, Foli_docu.Text);
        DbnetTool.ctrlSqlInjection(this.Page.Form);
        DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
        asObjectException obj = new asObjectException();
        int iNumeCoca = 1;
        try
        {
            DbnetTool.ctrlSqlInjection(this.Page.Form);
            string[] datos = txtAuxComision.Text.Split(new string[] { "#$#" }, StringSplitOptions.None);
            foreach (var item in datos)
            {
                string[] sCampos = item.Split('|');
                if (sCampos.Length > 1)
                {
                    obj.Set("codi_empr", DbnetContext.Codi_empr.ToString(), 1);
                    obj.Set("tipo_docu", Tipo_docu.Text.Trim(), 1);
                    obj.Set("foli_docu", Foli_docu.Text.Trim(), 1);
                    obj.Set("nume_coca", iNumeCoca.ToString(), 1);
                    obj.Set("tipo_coca", sCampos[0], 0);
                    obj.Set("glos_coca", sCampos[1].Replace("'", "''"), 0);
                    obj.Set("tasa_coca", sCampos[2], 1);
                    obj.Set("valo_neto_coca", sCampos[3], 1);
                    obj.Set("valo_exen_coca", sCampos[4], 1);
                    obj.Set("valo_iva_coca", sCampos[5], 1);
                    DbnetTool.ctrlSqlInjection(this.Page.Form);
                    DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, DbnetTool.SQL("I", "dte_comi_carg", obj, ""));
                    obj.Clear();
                    iNumeCoca++;
                }
            }
        }
        catch (Exception ex)
        {
            //sLugar
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. GrabaComisiones", sLugar);
            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
        }
    }
    protected void Grabar()
    {
        string sLugar = "Grabar()";
        try
        {
            querymodo = "select param_value from sys_param where param_name = 'EGATE_CAF_INGR'";
            string sValue = DbnetTool.SelectInto(DbnetContext.dbConnection, querymodo);

            modo_caf_ingr = (sValue.Equals("1")) ? 1 : 0;
            string cod = DbnetContext.Codi_ceco.ToString();
            string p_existe = string.Empty;
            string folio = string.Empty;
            string mensaje = string.Empty;
            if (Session["user_modo"].ToString().Contains("I") && tipo_ingre == "E" && asig_folio == 0 && modo_foli == "0")
            {
                asig_folio = 1;
                if (modo_caf_ingr == 1)
                {
                    sLugar = "antes de llamar a PrcAsigFolioCafIngr";
                    DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcAsigFolioCafIngr",
                        "p_codi_empr", DbnetContext.Codi_empr.ToString(), "DECIMAL", 3, "in",
                        "p_tipo_docu", Tipo_docu.Text, "VarChar", 3, "in",
                        "p_codi_ceco", cod, "VarChar", 16, "in",
                        "p_foli_docu", "", "VarChar", 10, "out",
                        "p_existe", "", "VarChar", 1, "out",
                        "p_mensaje", "", "VarChar", 200, "out");

                    p_existe = sp.return_String("p_existe");
                    folio = sp.return_String("p_foli_docu");
                    mensaje = sp.return_String("p_mensaje");
                    asig_folio = 1;
                }
                else
                {
                    sLugar = "antes de llamar a PrcAsigFolio";
                    DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcAsigFolio",
                        "p_codi_empr", DbnetContext.Codi_empr.ToString(), "DECIMAL", 3, "in",
                        "p_tipo_docu", Tipo_docu.Text, "VarChar", 3, "in",
                        "p_codi_ceco", cod, "VarChar", 16, "in",
                        "p_foli_docu", "", "VarChar", 10, "out",
                        "p_existe", "", "VarChar", 1, "out",
                        "p_mensaje", "", "VarChar", 200, "out");

                    p_existe = sp.return_String("p_existe");
                    folio = sp.return_String("p_foli_docu");
                    mensaje = sp.return_String("p_mensaje");
                    asig_folio = 1;
                }
                asig_folio = 0;
            }
            else
            {
                p_existe = "S";
                folio = Foli_docu.Text;
            }

            if (p_existe == "S")
            {
                Foli_docu.Text = folio;
                grabarDTE();
                asig_folio = 0;
                DbnetTool.MsgAlerta("Cambios Ejecutado", this.Page);
            }
            else
            {
                lbMensaje.Text += mensaje;
                DbnetTool.MsgError(lbMensaje.Text, this.Page);
                MsgError(1);
            }
        }
        catch (Exception ex)
        {
            //sLugar
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. Grabar()", sLugar);
            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            DbnetTool.MsgError("Verifique que el tipo de documento posee folios asignados y que hay folios disponibles para asignar.", this.Page);
        }
    }
    protected void barRun_Click(object sender, ImageClickEventArgs e)
    {
        lbMensaje.Text = "";
        try
        {
            if (tipo_ingre == null)
                tipo_ingre = Session["tipo_ingre"].ToString();
            btEnvSii.Enabled = false;
            lb_msgschema.Text = "";
            msgSchema = "";
            lb_msgschema.Text += ValidaFormulario2();

            if (string.IsNullOrEmpty(lb_msgschema.Text))
            {
                //flag para capturar el folio asignado y no dar uno mas
                Grabar();

                if ((tipo_ingre == "E") && (string.IsNullOrEmpty(lbMensaje.Text)))
                {
                    ValidaPostFormulario();                    
                }

                if (!string.IsNullOrEmpty(lb_msgschema.Text))
                {
                    btEnvSii.Enabled = false;
                }

            }
            else
                lblschema.Text = "Faltan datos";
        }
        catch (Exception ex)
        {
            String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. barRun()", "");
            chkDespliega.Checked = true;
            lbMensaje.Enabled = true;
            lbMensaje.Visible = true;
            lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
            lbEx.Text = erroMensaje;
        }
    }
    protected void barExit_Click(object sender, ImageClickEventArgs e)
    {
        salir();
    }
    private void salir()
    {
        switch (Session["user_modo"].ToString())
        {
            case "M":
                string pagina = "../dbnWeb/dbnListador.aspx?listado=fac-lisDTEPropio&modo=M";
                ClientScript.RegisterStartupScript(typeof(Page), "Pagina", "<script type=\"text/javascript\"> window.location.href=\"" + pagina + "\" </script>");
                break;
            case "I":
                /*
                    ot->711294    Funcionalidad para RRWINE
                    Copia de documento
                */
                if (Request.Params["copia"] == "S")
                {
                    pagina = "../dbnWeb/dbnListador.aspx?listado=fac-lisConsultaDTE&modo=CE&sincarga=S";
                    ClientScript.RegisterStartupScript(typeof(Page), "Pagina", "<script type=\"text/javascript\"> window.location.href=\"" + pagina + "\" </script>");

                }
                else
                {
                    pagina = "../dbnWeb/dbnListador.aspx?listado=fac-lisDTEPropio&modo=M";
                    ClientScript.RegisterStartupScript(typeof(Page), "Pagina", "<script type=\"text/javascript\"> window.location.href=\"" + pagina + "\" </script>");
                }
                break;
        }
    }
    protected void lvTipo_Docu_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        string queryIndServicio = "";
        Tipo_docu.Text = lvTipo_Docu.SelectedValue;

        if (Tipo_docu.Text.ToString() == "52")
        {
            cmbIndServicio.Enabled = false;
            queryIndServicio = "select '0','[Seleccione]' ,'-' from dual ";
            cmbIndServicio.Query = queryIndServicio;
            cmbIndServicio.Rescata(DbnetContext.dbConnection);
            cmbIndServicio.SelectedIndex = 0;
        }
        else if (Tipo_docu.Text.ToString() == "56" || Tipo_docu.Text.ToString() == "110" || Tipo_docu.Text.ToString() == "111" || Tipo_docu.Text.ToString() == "112")
        {
            cmbIndServicio.Enabled = true;
            queryIndServicio = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=963 order by 1";
            cmbIndServicio.Query = queryIndServicio;
            cmbIndServicio.Rescata(DbnetContext.dbConnection);
            cmbIndServicio.SelectedIndex = 0;

            cmbTranVenta.Enabled=false;
            cmbTranCompra.Enabled = false;
        }
        else if (Tipo_docu.Text.ToString() == "61" )
        {
            cmbIndServicio.Enabled = true;
            queryIndServicio = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=963 order by 1";
            cmbIndServicio.Query = queryIndServicio;
            cmbIndServicio.Rescata(DbnetContext.dbConnection);
            cmbIndServicio.SelectedIndex = 0;
            cmbTranVenta.Enabled = true;
            cmbTranCompra.Enabled = false;
        }
        else
        {
            cmbIndServicio.Enabled = true;
            queryIndServicio = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=963 and code in (1,2,3) order by 1";
            cmbIndServicio.Query = queryIndServicio;
            cmbIndServicio.Rescata(DbnetContext.dbConnection);
            cmbIndServicio.SelectedIndex = 0;
            cmbTranVenta.Enabled = true;
            cmbTranCompra.Enabled = true;
        }

    }
    private void Busca_Receptor()
    {
        string p_existe;
        /*procedimiento que rescata los datos del receptor*/
        DbnetProcedure sp2 = new DbnetProcedure(DbnetContext.dbConnection, "dte_pers_dato",
            "empresa", DbnetContext.Codi_empr.ToString(), "Decimal", 3, "in",
            "pcodi_pers", Rutt_rece.Text, "VarChar", 16, "in",
            "prutt_rece", "", "Decimal", 10, "out",
            "pdgto_rece", "", "VarChar", 1, "out",
            "pnomb_rece", "", "VarChar", 80, "out",
            "pcodi_ramo", "", "VarChar", 12, "out",
            "pcodi_comu", "", "VarChar", 8, "out",
            "pdire_rece", "", "VarChar", 80, "out",
            "pgiro_rece", "", "VarChar", 50, "out",
            "pnomb_comu", "", "VarChar", 20, "out",
            "pcodi_ciud", "", "VarChar", 8, "out",
            "pnomb_ciud", "", "VarChar", 20, "out",
            "p_existe", "", "VarChar", 1, "out",
            "p_mensaje", "", "VarChar", 200, "out");

        p_existe = sp2.return_String("p_existe");
        if (p_existe == "S")
        {
            Giro_rece.Text = sp2.return_String("pcodi_ramo");
            Dire_rece.Text = sp2.return_String("pdire_rece");
            Comu_rece.Text = sp2.return_String("pnomb_comu");
            Ciud_rece.Text = sp2.return_String("pnomb_ciud");
            Digi_rece.Text = sp2.return_String("pdgto_rece");
            Fono.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, string.Format("SELECT fono_pers FROM PERSONAS WHERE codi_pers={0}", Rutt_rece.Text));
            Fech_Emis.Focus();
        }
        else
        {
            Giro_rece.Text = Dire_rece.Text = Comu_rece.Text = Ciud_rece.Text = Digi_rece.Text = Fono.Text = "";
            DbnetTool.MsgError("El Cliente no existe, debe crearlo primero!", this.Page);
            Rutt_rece.Focus();
        }
    }
    protected void Rutt_rece_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string queryRamo;
            if (!string.IsNullOrEmpty(Rutt_rece.Text))
            {
                queryRamo = "SELECT codi_ramo FROM personas where codi_pers='" + Rutt_rece.Text + "'";
                string codiRamo = DbnetTool.SelectInto(DbnetContext.dbConnection, queryRamo);
                if (codiRamo.Length > 0)
                {
                    queryRamo = "SELECT nomb_ramo FROM ramo where codi_ramo = '" + codiRamo + "'";
                    string codiRamoR = DbnetTool.SelectInto(DbnetContext.dbConnection, queryRamo);
                    if (codiRamoR.Length > 0)
                    {
                        if (DbnetTool.SelectInto(DbnetContext.dbConnection, "SELECT codi_ramo FROM ramo where codi_ramo='" + codiRamo + "' AND codi_empr = " + DbnetContext.Codi_empr.ToString() + "").Length == 0)
                        {
                            queryRamo = "Insert into ramo(codi_ramo,nomb_ramo,codi_empr)" +
                                "values (" +
                                "'" + codiRamo + "'," +
                                "'" + codiRamoR + "'," +
                                "" + DbnetContext.Codi_empr.ToString() + ")";
                            DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, queryRamo);
                        }
                    }
                }
                query = "SELECT nomb_pers FROM personas where codi_pers='" + Rutt_rece.Text + "'"; 
                txtNomb_rece.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
                Busca_Receptor();
                int iRut = 0;
                int.TryParse(Rutt_rece.Text, out iRut);
                if ((iRut < 100000) & (Tipo_docu.Text != "110" & Tipo_docu.Text != "111" & Tipo_docu.Text != "112"))
                {
                    Rutt_rece.Text = "55555555";
                    Digi_rece.Text = "5";
                    query = "SELECT nomb_pers FROM personas where codi_pers='" + Rutt_rece.Text + "'";
                    txtNomb_rece.Text = DbnetTool.SelectInto(DbnetContext.dbConnection, query);
                    Busca_Receptor();
                }
                if (Rutt_rece.Text.Equals(DbnetTool.SelectInto(DbnetContext.dbConnection, "select rutt_empr from empr where codi_empr=" + DbnetContext.Codi_empr)))
                    divSucursal.Attributes.Remove("style");
                else
                    divSucursal.Attributes.Add("style", "display:none;");
            }
            else
                Giro_rece.Text = Dire_rece.Text = Comu_rece.Text = Ciud_rece.Text = Digi_rece.Text = Fono.Text= "";
            lvGiros.Query = "select codi_ramo," + DbnetContext.Auxdbo + "initcap(nomb_ramo) from ramo group by codi_ramo,nomb_ramo order by nomb_ramo";
            lvGiros.Rescata(DbnetContext.dbConnection);
            lvGiros.SelectedValue = Giro_rece.Text;
        }
        catch
        {
            DbnetTool.MsgError("Receptor no existe", this.Page);
            Rutt_rece.Text = "";
        }
    }
    protected void lvGiros_SelectedIndexChanged(object sender, EventArgs e)
    {
        Giro_rece.Text = lvGiros.SelectedValue;
    }
    protected void Giro_rece_TextChanged(object sender, EventArgs e)
    {
        try
        {
            lvGiros.Query = "select codi_ramo," + DbnetContext.Auxdbo + "initcap(nomb_ramo) from ramo where codi_empr = " + DbnetContext.Codi_empr.ToString() + " order by nomb_ramo";
            lvGiros.Rescata(DbnetContext.dbConnection);
            lvGiros.SelectedValue = Giro_rece.Text;
        }
        catch
        {
            DbnetTool.MsgError("El Giro no existe", this.Page);
            Giro_rece.Text = "";
            lvGiros.SelectedIndex = -1;
        }
    }
    protected void LvEstado_SelectedIndexChanged(object sender, EventArgs e)
    {
        Esta_docu.Text = LvEstado.SelectedValue;
    }
    protected void Esta_docu_TextChanged(object sender, EventArgs e)
    {
        try
        {
            LvEstado.Query = "select esta_docu," + DbnetContext.Auxdbo + "initcap(desc_esdo) from DTE_ESTA_DOCU where esta_docu='PEN' order by desc_esdo";
            LvEstado.Rescata(DbnetContext.dbConnection);
            LvEstado.SelectedValue = Esta_docu.Text;
        }
        catch
        {
            DbnetTool.MsgError("El Estado no Existe", this.Page);
            LvEstado.Text = "";
            LvEstado.SelectedIndex = -1;
        }
    }
    protected void btEnvSii_Click(object sender, EventArgs e)
    {
        string sLugar = string.Empty;
        DbnetContext = (DbnetSesion)Session["contexto"];
        string dirEsuite = DbnetDir.direESuite(DbnetContext.dbConnection);
        grabarDTE();
        string dirTed = dirEsuite + "in\\ted\\";
        string ArchivoTed = DbnetTool.Nombre_Estandar(DbnetContext, DbnetContext.Codi_empr.ToString(), Tipo_docu.Text, Foli_docu.Text) + ".ted";

        try
        {
            if (File.Exists(dirTed + ArchivoTed))
                File.Delete(dirTed + ArchivoTed);
        }
        catch
        {}
        string v_Vers_enca = string.Empty, p_existe = string.Empty;
        EliminaPrevisualizacion();
        DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "para_get_val",
                    "paem_codi_paem", "EGATE_VERS_ENCA", "VarChar", 30, "in",
                    "paem_valo_paem", "", "VarChar", 100, "out",
                    "p_existe", "", "VarChar", 1, "out",
                    "p_mensaje", "", "VarChar", 200, "out");

        p_existe = sp.return_String("p_existe");
        if (p_existe == "N")
        {
            lbMensaje.Text = sp.return_String("p_mensaje");
            return;
        }
        else
            v_Vers_enca = sp.return_String("paem_valo_paem");
        sLugar ="antes de grabar|";
        Grabar();
        sLugar += "despues de grabar|";
        query = string.Format("UPDATE dte_enca_docu SET esta_docu='ING',vers_enca='{0}' WHERE codi_empr={1} AND tipo_docu={2} AND foli_docu={3}",
                                v_Vers_enca,DbnetContext.Codi_empr,Tipo_docu.Text,Foli_docu.Text);
        sLugar += "despues de actualizar registro a ING|";
        DbnetTool.ctrlSqlInjection(this.Page.Form);
        DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, query);
        sLugar += "despues de actualizar registro a ING en BD|";
        registroLog(new Exception(sLugar), "",sLugar);
        DbnetTool.MsgAlerta("Se imprimio el Documento", this.Page);

        salir();
    }
    /// <summary>
    /// Elimina archivo pdf de previsualizacion
    /// </summary>
    private void EliminaPrevisualizacion()
    {
        var nomArchivo = DbnetTool.Nombre_Estandar(DbnetContext, DbnetContext.Codi_empr.ToString(), Tipo_docu.Text, Foli_docu.Text) + ".pdf";
        var path = DbnetDir.direESuite(DbnetContext.dbConnection) + "out\\pdf\\";        
        try
        {        
            if(File.Exists(path + nomArchivo))
                File.Delete(path + nomArchivo);
        }
        catch
        { }
    }
    protected void Tipo_docu_TextChanged(object sender, EventArgs e)
    {
        string queryIndServicio = "";

        try
        {
            tipo_ingre = (Session["user_tipo"].ToString().Contains("E")) ? "E" : "M";
            if (Convert.ToInt32(DbnetTool.SelectInto(DbnetContext.dbConnection, "select count(param_value) from sys_param where param_name = 'EGATE_USUA_DOCU' and param_value='N'")) > 0)
            {
                if (tipo_ingre == "E")
                    lvTipo_Docu.Query = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec='S' and indi_digi='S' union select 0,'[Seleccione]' from dual order by 2";
                else if (tipo_ingre == "M")
                    lvTipo_Docu.Query = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec!='S' and indi_digi='S' union select 0,'[Seleccione]' from dual order by 2";
            }
            else if (Convert.ToInt32(DbnetTool.SelectInto(DbnetContext.dbConnection, "select count(tipo_docu) from dte_usua_docu where codi_usua='" + DbnetContext.Codi_usua + "'")) > 0)
            {
                if (tipo_ingre == "E")
                    lvTipo_Docu.Query = "Select a.tipo_docu, " + DbnetContext.Auxdbo + "initcap(b.desc_tido) from dte_usua_docu a , dte_tipo_docu b where a.codi_usua='" + DbnetContext.Codi_usua.ToString() + "' and b.tipo_docu=a.tipo_docu and b.docu_elec='S' union select 0,'[Seleccione]' from dual order by 2";
                else if (tipo_ingre == "M")
                    lvTipo_Docu.Query = "Select a.tipo_docu, " + DbnetContext.Auxdbo + "initcap(b.desc_tido) from dte_usua_docu a , dte_tipo_docu b where a.codi_usua='" + DbnetContext.Codi_usua.ToString() + "' and b.tipo_docu=a.tipo_docu and b.docu_elec!='S' union select 0,'[Seleccione]' from dual order by 2";
            }
            else
            {
                if (tipo_ingre == "E")
                    lvTipo_Docu.Query = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec='S' and indi_digi='S' union select 0,'[Seleccione]' from dual order by 2";
                else if (tipo_ingre == "M")
                    lvTipo_Docu.Query = "Select tipo_docu, " + DbnetContext.Auxdbo + "initcap(desc_tido) from dte_tipo_docu where docu_elec!='S' and indi_digi='S' union select 0,'[Seleccione]' from dual order by 2";
            }
            lvTipo_Docu.Rescata(DbnetContext.dbConnection);
            lvTipo_Docu.SelectedValue = Tipo_docu.Text;
            Rutt_rece.Focus();


            if (Tipo_docu.Text.ToString() == "110" || Tipo_docu.Text.ToString() == "111" || Tipo_docu.Text.ToString() == "112")
            {
                string _qcombo = "";
                _qcombo = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=:PAR order by 3";
                cmbMoneda.Query = _qcombo.Replace(":PAR", "956");
                cmbMoneda.Rescata(DbnetContext.dbConnection);
                cmbMoneda.SelectedIndex = 32;
            }

            if (Tipo_docu.Text.ToString() == "52")
            {
                cmbIndServicio.Enabled = false;
                queryIndServicio = "select '0','[Seleccione]' ,'-' from dual ";
                cmbIndServicio.Query = queryIndServicio;
                cmbIndServicio.Rescata(DbnetContext.dbConnection);
                cmbIndServicio.SelectedIndex = 0;
            }
            else if (Tipo_docu.Text.ToString() == "56" || Tipo_docu.Text.ToString() == "110" || Tipo_docu.Text.ToString() == "111" || Tipo_docu.Text.ToString() == "112")
            {
                cmbIndServicio.Enabled = true;
                queryIndServicio = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=963 order by 1";
                cmbIndServicio.Query = queryIndServicio;
                cmbIndServicio.Rescata(DbnetContext.dbConnection);
                cmbIndServicio.SelectedIndex = 0;

                cmbTranVenta.Enabled = false;
                cmbTranCompra.Enabled = false;

                
            }
            else if (Tipo_docu.Text.ToString() == "61" )
            {
                cmbIndServicio.Enabled = true;
                queryIndServicio = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=963 order by 1";
                cmbIndServicio.Query = queryIndServicio;
                cmbIndServicio.Rescata(DbnetContext.dbConnection);
                cmbIndServicio.SelectedIndex = 0;
                cmbTranVenta.Enabled = true;
                cmbTranCompra.Enabled = false;


            }
            else
            {
                cmbIndServicio.Enabled = true;
                queryIndServicio = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=963 and code in (1,2,3) order by 1";
                cmbIndServicio.Query = queryIndServicio;
                cmbIndServicio.Rescata(DbnetContext.dbConnection);
                cmbIndServicio.SelectedIndex = 0;

                cmbTranVenta.Enabled = true;
                cmbTranCompra.Enabled = true;
            }

            // AM|23-10-2017|OT 9929029: Problemas de esquema de Ind.Servicio (Exportaciones)
            if (new string[] { "110", "111", "112" }.Contains(Tipo_docu.Text.ToString()))
            {
                queryIndServicio = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=963 and code in (3,4,5) order by 1";
                cmbIndServicio.Query = queryIndServicio;
                cmbIndServicio.Rescata(DbnetContext.dbConnection);
                cmbIndServicio.SelectedIndex = 0;
            }
            // AM|23-10-2017|OT 9929029: Problemas de esquema de Ind.Servicio (Documentos y Liquidacion)
            else if (new string[] { "33", "34", "43", "46", "52", "56", "61" }.Contains(Tipo_docu.Text.ToString()))
            {
                queryIndServicio = "select '0','[Seleccione]' ,'-' from dual union select code,  code_desc , code_desc from sys_code where domain_code=963 and code in (1,2,3) order by 1";
                cmbIndServicio.Query = queryIndServicio;
                cmbIndServicio.Rescata(DbnetContext.dbConnection);
                cmbIndServicio.SelectedIndex = 0;
            }


        }
        catch
        {
            DbnetTool.MsgError("El tipo de Documento no existe", this.Page);
            Tipo_docu.Text = "";
            lvTipo_Docu.SelectedIndex = 0;
        }
    }
    protected void btPreVisualizar_Click(object sender, EventArgs e)
    {
        lbMensaje.Text = "";
        if (!string.IsNullOrEmpty(Tipo_docu.Text) && !string.IsNullOrEmpty(Foli_docu.Text) && Foli_docu.Text != "0")
        {
            string cache = DbnetGlobal.Path + "cache\\";
            string dirEsuite = DbnetDir.direESuite(DbnetContext.dbConnection);
            string origen = dirEsuite + "out\\pdf\\";
            string archivo = DbnetTool.Nombre_Estandar(DbnetContext, DbnetContext.Codi_empr.ToString(), Tipo_docu.Text, Foli_docu.Text); ;
            string pScript = "";

            if (File.Exists(cache + archivo))
                File.Delete(cache + archivo);
            //proceso 
            string proceso = "egateDTE";
            string parametros = " -te bd -ts html -prev ";
            parametros += " -empr " + DbnetContext.Codi_empr;
            parametros += " -tdte " + Tipo_docu.Text;
            parametros += " -fdte " + Foli_docu.Text;
            lbMensaje.Text += DbnetTool.Ejecuta_Proceso_Espera(DbnetContext, proceso, parametros);

            archivo = DbnetTool.Nombre_Estandar(DbnetContext, DbnetContext.Codi_empr.ToString(), Tipo_docu.Text, Foli_docu.Text);
            archivo += ".pdf";
            origen += archivo;

            try
            {File.Copy(origen, cache + archivo, true);}
            catch (Exception ex)
            {
                String erroMensaje = registroLog(ex, "Menu: Ingreso de DTEs Propio. Previsualizar()", "");
                chkDespliega.Checked = true;
                lbMensaje.Enabled = true;
                lbMensaje.Visible = true;
                lbMensaje.Text = "<img src=\"../librerias/img/imgWarn.png\" border=\"0\" class=\"dbnEstado\" />";
                lbEx.Text = erroMensaje;
            }
            pScript = "<script type=\"text/javascript\"> " +
                                         "window.open(\"" + "../cache/" + archivo + "\",\"" + "_blank\",\"width=800,height=600,scrollbars=yes,toolbar=no,menubar=yes\");" +
                                         "</script>";
            ClientScript.RegisterStartupScript(typeof(Page), "PaginaDestino", pScript);
        }
        else
        {
            lbMensaje.Text += "Error : Debe generar documento antes de pre-visualizar PDF  <br>";
            MsgError(1);
        }
    }
    protected void DetaCodi_TextChanged(object sender, EventArgs e)
    {
        DetaNombre.Text = string.Empty;
        DetaPuni.Text = string.Empty;
        cargaSugerencia();

    }
    protected void bt_AddDeta_Click(object sender, EventArgs e)
    {
        cargaSugerencia();
    }
    /// <summary>
    /// Busqueda de producto, con combinacion de codigo y nombre, siendo excluyente el codigo.
    /// </summary>
    protected void cargaSugerencia()
    {
        var q = string.Empty;
        var codi = string.IsNullOrEmpty(DetaCodi.Text) ? "null" : "'" + DetaCodi.Text + "'";
        var desc = string.IsNullOrEmpty(DetaNombre.Text) ? "null" : "'" + DetaNombre.Text + "'";
        int k = 0;

        if (DetaCodi.Text != "" || DetaNombre.Text !="")
        {
            try
            {
                DbnetTool.ctrlSqlInjection(this.Page.Form);
                if (DbnetGlobal.Base_dato == "SQLSERVER")
                {
                    q = "select top 1 prod_desc,prod_codi,prod_valu from mant_prod where prod_codi = ISNULL(" + codi + ",prod_codi) AND prod_desc = ISNULL(" + desc + ",prod_desc)";                      
                }
                else
                {                     
                    q = "select prod_desc,prod_codi,prod_valu from mant_prod where prod_codi = NVL(" + codi + ",prod_codi) AND prod_desc = NVL(" + desc + ",prod_desc)" + " AND ROWNUM < 2"; ;
                }
                DbnetTool.ctrlSqlInjection(this.Page.Form);
                DataTable d = DbnetTool.Ejecuta_Select(DbnetContext.dbConnection, q);
                k = d.Rows.Count;
                if(k == 0)
                {
                   DetaNombre.Text = "";
                   DetaCodi.Text = "";
                   DetaPuni.Text = "";
                }
                else
                {
                   DataRow dr = d.Rows[0]; // RowNum = 1 y Top1 garantizan una sola fila.
                   DetaNombre.Text = dr[0].ToString().Trim();
                   DetaCodi.Text = dr[1].ToString().Trim();
                   DetaPuni.Text = dr[2].ToString().Trim();
                }
            }
            catch
            { }         
        }        
    }
    protected void DetaNombre_TextChanged(object sender, EventArgs e)
    {
        cargaSugerencia();
    }

    private String registroLog(Exception ex, String pprocErro, String sLugar)
    {
        String exMessage = ex.Message;
        String result = String.Empty;
        String p_mensaje = String.Empty;

        DbnetProcedure sp = new DbnetProcedure(DbnetContext.dbConnection, "PrcLogErro",
                              "pcodi_empr", DbnetContext.Codi_empr.ToString(), "VarChar", 3, "in",
                              "pproc_erro", pprocErro, "VarChar", 50, "in",
                              "pmsaj_erro", ex.Message + sLugar, "VarChar", 150, "in",
                              "pbin_erro", "WEB", "VarChar", 50, "in",
                              "p_mensaje", "", "VarChar", 200, "out");

        p_mensaje = sp.return_String("p_mensaje");

        if (exMessage.Contains("PRIMARY KEY") || exMessage.Contains("UNIQUE KEY")) 
        {
            result = "Error Documento Duplicado";
        }
        else if (exMessage.Contains("FOREIGN KEY"))
        {
            result = "Error en Dependencia del Documento";
        }
        else if (exMessage.Contains(", no permitido"))
        {
            result = "En Documento Contiene Palabras Peservada. Favor corregir para poder continuar";
        }
        else if( (exMessage.Contains("El campo ") && exMessage.Contains(" no puede estar vacio"))
              || (exMessage.Contains("El campo ") && exMessage.Contains(" debe ser numerico y distinto a vacio.")))
        {
            result = exMessage;
        }
        else
        {
            result = p_mensaje;
        }




        return result;
    }

    
    [WebMethod]
    [ScriptMethod(UseHttpGet = false)]
    public static object sSucu(string sId)
    {
        Sucursal oSucu = new Sucursal();
        var oContext = (DbnetSesion)HttpContext.Current.Session["contexto"];
        string sQuery = string.Format("select {2}(a.nomb_ofic,' '), {2}(a.DIRE_OFIC,' ') ,{2}(c.nomb_arge,' '), {2}(b.nomb_arge,' '), {2}(a.codi_ofic,' ') from oficina a, area_geog b, area_geog c " +
                                            "where a.CODI_CIUD = b.codi_arge and b.tipo_arge='04' " +
                                            "and a.codi_comu=c.codi_arge " +
                                            "and c.tipo_arge='05' and a.codi_empr={0} and a.codi_ofic={1}",
                                            oContext.Codi_empr, sId,
                                            (DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "ISNULL" : "NVL"));
        DataTable dt = DbnetTool.Ejecuta_Select(oContext.dbConnection, sQuery);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            DataRow dr = dt.Rows[i];
            oSucu.sNombre= dr[0].ToString();
            oSucu.sDireccion = dr[1].ToString();
            oSucu.sComuna = dr[2].ToString();
            oSucu.sCiudad = dr[3].ToString();
            oSucu.sCodigo = dr[4].ToString();
        }
        return oSucu;
    }
    [WebMethod]
    [ScriptMethod(UseHttpGet = false)]
    public static object sSucuLike(string sId)
    {
        Sucursal oSucu = new Sucursal();
        var oContext = (DbnetSesion)HttpContext.Current.Session["contexto"];
        string sQuery = string.Format("select {2}(a.nomb_ofic,' '), {2}(a.DIRE_OFIC,' ') ,{2}(c.nomb_arge,' '), {2}(b.nomb_arge,' '), {2}(a.codi_ofic,' ') from oficina a, area_geog b, area_geog c " +
                                            "where a.CODI_CIUD = b.codi_arge and b.tipo_arge='04' " +
                                            "and a.codi_comu=c.codi_arge " +
                                            "and c.tipo_arge='05' and a.codi_empr={0} and a.nomb_ofic like '%{1}%'",
                                            oContext.Codi_empr, sId,
                                            (DbnetGlobal.Base_dato.Equals("SQLSERVER") ? "ISNULL" : "NVL"));
        DataTable dt = DbnetTool.Ejecuta_Select(oContext.dbConnection, sQuery);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            DataRow dr = dt.Rows[i];
            oSucu.sNombre = dr[0].ToString();
            oSucu.sDireccion = dr[1].ToString();
            oSucu.sComuna = dr[2].ToString();
            oSucu.sCiudad = dr[3].ToString();
            oSucu.sCodigo = dr[4].ToString();
        }
        return oSucu;
    }
    protected void cmbMoneda_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbMoneda.SelectedValue.ToString() != "PESO CL")
        {
            tipo_camb.Enabled = true;
            ClientScript.RegisterStartupScript(typeof(Page), "Pagina", "<script type=\"text/javascript\"> javascript:mostrar('div_datoexportacion') </script>");
        }
    }
}
public class Sucursal
{
    public string sCodigo { get; set; }
    public string sNombre { get; set; }
    public string sDireccion { get; set; }
    public string sComuna { get; set; }
    public string sCiudad { get; set; }
}

public class asObjectException : asObject
{
    public asObjectException()
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
            myException = new Exception("El campo " + error + " no puede estar vacio.@");
            throw myException;
        }

        if (texto == 1)
            valor = valor.ToString().Replace(',', '.');
        string valor2 = valor.Replace("-", "");
        if (texto == 1 && !(System.Text.RegularExpressions.Regex.IsMatch(valor2, "^\\d{0,20}\\.{0,1}?\\d{1,4}$") ||
                             System.Text.RegularExpressions.Regex.IsMatch(valor2, "^\\d{0,20}\\,{0,1}?\\d{1,4}$")))
        {
            myException = new Exception("El campo " + error + " debe ser numerico y distinto a vacio.@");
            throw myException;
        }

        this.objeto[this.contador] = objeto;
        this.valor[this.contador] = valor;
        this.texto[this.contador] = texto;
        this.contador++;

    }


}