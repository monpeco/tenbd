<%@ Page Language="C#" AutoEventWireup="true" CodeFile="facPrcIngDTEPropio.aspx.cs" Inherits="facPrcIngDTEPropio" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="cc1" Namespace="DbnetWebControl" Assembly="DbnetWebControl" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Suite Electr&#243;nica -<% = lbTitulo.Text %></title>
    <meta charset="UTF-8" />
    <meta http-equiv="pragma" content="no-cache" />
    <link rel="stylesheet" type="text/css" href="../librerias/css/dbnEstilo.css" />
    <link rel="stylesheet" type="text/css" href="../librerias/js/jquery/jquery.autocomplete.css" />
    <link rel="stylesheet" type="text/css" href="../librerias/js/jquery/thickbox.css" />
    <link rel="stylesheet" type="text/css" href="../librerias/css/jscal2.css" />
    <link rel="stylesheet" type="text/css" href="../librerias/css/border-radius.css" />
    <link rel="stylesheet" type="text/css" href="../librerias/css/steel/steel.css" />
    <link rel="stylesheet" type="text/css" href="../librerias/css/StyleSheetChrome.css" />
    <script src="../librerias/js/jquery/jquery.js" type="text/javascript"></script>
    <script src="../librerias/js/jquery/jquery.bgiframe.min.js" type="text/javascript"></script>
    <script src="../librerias/js/jquery/jquery.ajaxQueue.js" type="text/javascript"></script>
    <script src="../librerias/js/jquery/thickbox-compressed.js" type="text/javascript"></script>
    <script src="../librerias/js/jquery/jquery.autocomplete.js" type="text/javascript"></script>
    <script src="../librerias/js/jscal2.js" type="text/javascript"></script>
    <script src="../librerias/js/lang/es.js" type="text/javascript"></script>
    <script src="../librerias/js/enter.js" type="text/javascript"></script>
    <script src="../librerias/js/busqueda.js" type="text/javascript"></script>
    <script src="../librerias/js/rollover.js" type="text/javascript"></script>
    <script src="../librerias/js/overlib.js" type="text/javascript"></script>
    <script src="../librerias/js/calendar.js" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/librerias/js/jquery/jquery.json.min.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        var detacont = 1;
        var detamax = <%=lbDetalleMax.Text%>;
        function waitx(){
            if (document.getElementById)
                document.getElementById('hidepage').style.visibility = 'visible';
            else {
                if (document.layers)
                    document.hidepage.visibility = 'visible';
                else
                    document.all.hidepage.style.visibility = 'visible';
            }
        }
        $().ready(function() {
            $("#DetaNombre").autocomplete(productos);
        });
        function lista_tpoBultos() {
            mywindow = window.open("list_tipoBultos.aspx", "mywindow", "location=1,status=1,scrollbars=1,width=300,height=350");
            mywindow.moveTo(0, 0);
        }
        
        function verOcultarError(controlDiv, controlCheck, checkDespliega) {
            if (this.document.getElementById(checkDespliega).checked == true) {
                this.document.getElementById(controlCheck).checked = !this.document.getElementById(controlCheck).checked;
                if (this.document.getElementById(controlCheck).checked == true) {
                    Effect.BlindDown(controlDiv);
                    return false;
                }
                else {
                    Effect.BlindUp(controlDiv);
                    return false;
                }
            }
        }
    </script>
    <style type="text/css">
        .hidden{display:none;}
        .style1
        {
            width: 30px;
        }
    </style>
    <script src="../librerias/js/facPrcIngDTEPropio2.js" type="text/javascript"></script>
</head>
<body onload="MM_load()">
    <form id="dbnForm" runat="server" onsubmit="toasp()" style="padding-left: 10px">
        <div style="z-index: 101; position: relative; height: 90px; width: 680px;" class="fondo1">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="lbTitulo" runat="server" BackColor="Transparent" BorderColor="Transparent"
                            CssClass="barTitulo" Design_Time_Lock="True" Style="z-index: 101" Width="603px">Titulo</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbDetalleMax" runat="server" BackColor="Transparent" BorderColor="Transparent"
                            CssClass="barTitulo" Design_Time_Lock="True" Style="z-index: 101" Width="1px"
                            Visible="False"></asp:Label>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Image ID="imgTitulo" runat="server" Design_Time_Lock="True" Height="34px" ImageUrl="../librerias/img/titulo_01.jpg"
                            Style="z-index: 100" Width="568px" Visible="False" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table border="0" cellpadding="0">
                            <tr>
                                <td width="33px">
                                    <asp:ImageButton ID="barRun" runat="server" ToolTip="Ejecutar" Design_Time_Lock="True"
                                        ImageUrl="../librerias/img/page_run.png" onmouseout="this.src='../librerias/img/page_run.png'"
                                        onmouseover="this.src='../librerias/img/page_run_over.png'" Style="z-index: 110"
                                        OnClientClick="javascript:waitx();" />
                                </td>
                                <td width="33px">
                                    <asp:ImageButton ID="barExit" runat="server" onmouseout="this.src='../librerias/img/page_exit.png'"
                                        onmouseover="this.src='../librerias/img/page_exit_over.png'" Style="z-index: 103"
                                        ToolTip="Salir" ImageUrl="../librerias/img/page_exit.png" Design_Time_Lock="True"
                                        OnClick="barExit_Click"></asp:ImageButton>
                                </td>
                                <td width="33px">
                                    <asp:ImageButton ID="barAyuda" runat="server" ToolTip="Ayuda" Design_Time_Lock="True"
                                        ImageUrl="../librerias/img/page_help.png" OnClientClick="barAyuda_Click" onmouseout="this.src='../librerias/img/page_help.png'"
                                        onmouseover="this.src='../librerias/img/page_help_over.png'" Style="z-index: 103"
                                        OnClick="barAyuda_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="hidepage" style="position: absolute; left: 100px; top: 100px; background-color: #ffffff; height: 20%; width: 59%; visibility: hidden; z-index: 200; filter: Alpha(Opacity=85);">
            <img src="../librerias/img/cargando.gif" border="0" />
        </div>
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <div id="ayuda" style="display: none; left: 0px; overflow: auto; width: 680px; position: relative; height: 25px">
                        <asp:Label ID="barDescripcion" runat="server" CssClass="dbnTexto" Height="15px" Style="z-index: 103"
                            Width="630px">Permite subir a Suite Electr&#243;nica el archivo del libro de compra venta y de guias de despacho</asp:Label>
                    </div>
                    <asp:ToolkitScriptManager ID="ScriptManager1" runat="server">
                        <Scripts>
                            <asp:ScriptReference Path="../librerias/js/prototype.js" />
                            <asp:ScriptReference Path="../librerias/js/effects.js" />
                        </Scripts>
                    </asp:ToolkitScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="Div3" style="width: 630px; position: relative; top: 0px; height: auto;">
                                <asp:Label ID="lbMensaje" runat="server" CssClass="dbnError" Style="height: auto; width: 630px; position: relative; top: 0px;"
                                    Width="630px" Enabled="False" onclick="verOcultarError('divEx', 'chkEx', 'chkDespliega'); return false;">
                                </asp:Label>
                            </div>
                            <div id="divEx" style="display: none; background-color: #FFFF80; width: 631px; position: relative; top: 0px; height: auto">
                                <asp:Label ID="lbEx" runat="server" CssClass="dbnError" Style="z-index: 101; height: auto;"
                                    Width="630px">
                                </asp:Label>
                                <asp:CheckBox ID="chkEx" runat="server" Style="visibility: hidden;" />
                                <asp:CheckBox ID="chkDespliega" runat="server" Style="visibility: hidden;" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <div id="Busqueda">
            <table border="0" cellspacing="1" style="border-spacing: 5px">
                <tr>
                    <td class="style3">
                        <asp:Label ID="lb_Tipo_auto" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 111" Width="100px">Tipo Documento</asp:Label>
                    </td>
                    <td colspan="4">
                        <asp:TextBox ID="Tipo_docu" runat="server" AutoPostBack="True" Height="19px"
                            MaxLength="3" Style="z-index: 112" TabIndex="1" Width="35px" OnTextChanged="Tipo_docu_TextChanged"/>
                        <asp:FilteredTextBoxExtender ID="Tipo_docu_filtro" runat="server"
                            Enabled="True" TargetControlID="Tipo_docu" ValidChars="0123456789">
                        </asp:FilteredTextBoxExtender>
                        <cc1:DbnetWebLov ID="lvTipo_Docu" runat="server" Height="17px" Style="z-index: 113" TabIndex="2" 
                            Width="298px" AutoPostBack="True" OnSelectedIndexChanged="lvTipo_Docu_SelectedIndexChanged"/>
                    </td>
                    <td>
                        <asp:Label ID="lblFolio" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 106" Width="38px">Folio</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="Foli_docu" runat="server" CssClass="dbnTextBox_Numero"
                            Enabled="False" Formato="0" Height="19px" Width="108px">0</asp:TextBox>
                        <asp:FilteredTextBoxExtender ID="Foli_docu_filtro" runat="server"
                            Enabled="True" TargetControlID="Foli_docu" ValidChars="0123456789">
                        </asp:FilteredTextBoxExtender>
                    </td>
                </tr>
                <tr>
                    <td class="style3">
                        <asp:Label ID="lb_Fein_auto" runat="server" CssClass="dbnLabel" Height="4px"
                            Width="95px">Se&#241;ores (s)</asp:Label>
                    </td>
                    <td colspan="4">
                        <asp:TextBox ID="Rutt_rece" runat="server" AutoPostBack="True" CssClass="dbnTextBox_Numero"
                            Formato="0" Height="19px" MaxLength="10" Style="z-index: 133" TabIndex="3" Width="59px"
                            OnTextChanged="Rutt_rece_TextChanged">0</asp:TextBox>
                        <asp:FilteredTextBoxExtender ID="Rutt_rece_filtro" runat="server"
                            Enabled="True" TargetControlID="Foli_docu" ValidChars="0123456789">
                        </asp:FilteredTextBoxExtender>
                        <asp:TextBox ID="Digi_rece" runat="server" AutoPostBack="True" Height="19px"
                            MaxLength="1" Style="z-index: 135;" TabIndex="4" Width="14px"></asp:TextBox>
                        <asp:FilteredTextBoxExtender ID="Digi_rece_filtro" runat="server"
                            Enabled="True" TargetControlID="Digi_rece" ValidChars="0123456789Kk">
                        </asp:FilteredTextBoxExtender>
                        <cc1:DbnetWebTextBox ID="txtNomb_rece" runat="server" Height="19px" MaxLength="100"
                            Style="z-index: 133;" TabIndex="5" Width="255px"></cc1:DbnetWebTextBox>
                    </td>
                    <td>
                        <asp:Label ID="Dbnetweblabel12" runat="server" CssClass="dbnLabel" Height="4px"
                            Width="65px">Folio ERP</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="Foli_docu_rp" runat="server" CssClass="dbnTextBox_Numero"
                            Height="19px" Width="108px" TabIndex="14">0</asp:TextBox>
                        <asp:FilteredTextBoxExtender ID="Foli_docu_rp_filtro" runat="server"
                            Enabled="True" TargetControlID="Foli_docu_rp" ValidChars="0123456789">
                        </asp:FilteredTextBoxExtender>
                    </td>
                </tr>
                <tr>
                    <td class="style3">
                        <asp:Label ID="lb_Fete_auto" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 103" Width="101px">Direcci&#243;n</asp:Label>
                    </td>
                    <td colspan="4" style="border-spacing: 80px">
                        <cc1:DbnetWebTextBox ID="Dire_rece" runat="server" Height="19px" MaxLength="70" TabIndex="5"
                            Width="339px"></cc1:DbnetWebTextBox>
                    </td>
                    <td>
                        <asp:Label ID="Dbnetweblabel2" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 107" Width="62px">Telefono/Contacto</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="Fono" runat="server" Height="19px" MaxLength="40" Style="z-index: 105"
                            TabIndex="15" Width="108px" />
                    </td>
                </tr>
                <tr>
                    <td class="style3">
                        <asp:Label ID="Dbnetweblabel5" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="left: 0px; top: 0px;" Width="101px">Comuna</asp:Label>
                    </td>
                    <td class="style4">
                        <cc1:DbnetWebTextBox ID="Comu_rece" runat="server" Height="19px" MaxLength="20" Style="z-index: 121"
                            TabIndex="6" Width="125px"></cc1:DbnetWebTextBox>
                    </td>
                    <td colspan="2" class="style1">&nbsp;<asp:Label ID="Dbnetweblabel9" runat="server" CssClass="dbnLabel" Height="15px"
                        Style="z-index: 127; left: 184px; top: 80px" Width="38px">Ciudad</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox ID="Ciud_rece" runat="server" Height="19px" MaxLength="20" Style="left: 0px; top: 0px;"
                            TabIndex="7" Width="110px"></cc1:DbnetWebTextBox>
                    </td>
                    <td>
                        <asp:Label ID="lb_Defe_auto" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 104" Width="93px">Fecha Emisi&#243;n</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox_Fecha ID="Fech_Emis" runat="server" Formato="1" Height="19px"
                            MaxLength="10" Style="z-index: 116" TabIndex="16" Width="76px">30-04-2007</cc1:DbnetWebTextBox_Fecha>
                        <img id="Fecha_desdeCal" src="../librerias/img/cal.jpg" style="z-index: 901; cursor: pointer;"
                            height="19px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lb_Esta_auto" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 102; left: 20px; top: 103px" Width="92px">Giro</asp:Label>
                    </td>
                    <td colspan="4">
                        <cc1:DbnetWebTextBox ID="Giro_rece" runat="server" Height="19px" MaxLength="6" TabIndex="8"
                            Width="57px" AutoPostBack="True" OnTextChanged="Giro_rece_TextChanged"></cc1:DbnetWebTextBox>
                        <cc1:DbnetWebLov ID="lvGiros" runat="server" AutoPostBack="True" Height="17px" TabIndex="9"
                            Width="276px" OnSelectedIndexChanged="lvGiros_SelectedIndexChanged">
                        </cc1:DbnetWebLov>
                    </td>
                    <td>
                        <asp:Label ID="Dbnetweblabel8" runat="server" CssClass="dbnLabel" Height="4px"
                            Width="110px">Fecha Vencimiento</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox_Fecha ID="fech_venc" runat="server" Formato="1" Height="19px"
                            MaxLength="10" TabIndex="17" Width="76px">30-04-2007</cc1:DbnetWebTextBox_Fecha>
                        <img id="Fech_vencal" src="../librerias/img/cal.jpg" style="z-index: 126; cursor: pointer"
                            height="19px" />

                        <script type="text/javascript">
                            var cal = Calendar.setup({
                                onSelect: function(cal) { cal.hide() }
                            });
                            cal.manageFields("Fecha_desdeCal", "Fech_Emis", "%Y-%m-%d");
                            cal.manageFields("Fech_vencal", "fech_venc", "%Y-%m-%d");
                        </script>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Dbnetweblabel3" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 108; left: 21px; top: 125px" Width="85px">Medio Pago</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebLov ID="lvModa_Pago" runat="server" Height="17px" TabIndex="10" Width="140px">
                        </cc1:DbnetWebLov>
                    </td>
                    <td>
                        <asp:Label ID="Dbnetweblabel4" runat="server" CssClass="dbnLabel" Height="12px"
                            Width="72px">Forma Pago</asp:Label>
                    </td>
                    <td colspan="2">
                        <cc1:DbnetWebLov ID="lvForm_Pago" runat="server" Height="17px" Style="z-index: 128; left: 355px; top: 125px"
                            TabIndex="11" Width="116px">
                        </cc1:DbnetWebLov>
                    </td>
                    <td colspan="2">
                        <asp:Label ID="lblschema" runat="server" CssClass="dbnLabel" ForeColor="Red"
                            Height="12px" Width="190px"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Dbnetweblabel7" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="93px">Estado</asp:Label>
                    </td>
                    <td colspan="4">
                        <cc1:DbnetWebTextBox ID="Esta_docu" runat="server" AutoPostBack="True" Enabled="False"
                            Height="19px" MaxLength="3" ReadOnly="True" Style="z-index: 119" Width="29px"
                            OnTextChanged="Esta_docu_TextChanged"></cc1:DbnetWebTextBox>
                        <cc1:DbnetWebLov ID="LvEstado" runat="server" AutoPostBack="True" Enabled="False"
                            Height="19px" Style="z-index: 120;" Width="302px" OnSelectedIndexChanged="LvEstado_SelectedIndexChanged"
                            TabIndex="12">
                        </cc1:DbnetWebLov>
                    </td>
                    <td colspan="2">&nbsp;&nbsp;&nbsp;
                    <input id="bt_msg" onclick="javascript:mostrar('div_msgschema')" style="left: 477px; top: -55px; width: 182px; height: 19px;"
                        type="button" value="ver mensaje" tabindex="21" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel43" runat="server" CssClass="dbnLabel" Height="12px"
                            Style="z-index: 109; left: 23px; top: 175px" Width="88px">Impresora</asp:Label>
                    </td>
                    <td colspan="4">
                        <cc1:DbnetWebLov ID="cmbImpresora" runat="server" Height="17px" Style="z-index: 128"
                            TabIndex="13" Width="339px">
                        </cc1:DbnetWebLov>
                    </td>
                    <td>&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btPreVisualizar" runat="server" CausesValidation="False" CssClass="dbnBoton"
                        Height="23px" OnClick="btPreVisualizar_Click" Style="z-index: 132" Text="Pre visualizar"
                        UseSubmitBehavior="False" Width="96px" TabIndex="23" />
                    </td>
                    <td>
                        <asp:Button ID="btEnvSii" runat="server" CssClass="dbnBoton" Height="23px" Text="Enviar SII"
                            Width="87px" OnClick="btEnvSii_Click" Enabled="False" TabIndex="27" />
                    </td>
                </tr>
                
                 <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" CssClass="dbnLabel" Height="12px"
                            Style="z-index: 109; left: 23px; top: 175px" Width="93px">Ind.Servicio</asp:Label>
                    </td>
                    <td colspan="4">
                        <cc1:DbnetWebLov ID="cmbIndServicio" runat="server" Height="17px" Style="z-index: 128"
                            TabIndex="13" Width="339px">
                        </cc1:DbnetWebLov>
                    </td>
                    <td>&nbsp;&nbsp;&nbsp;
                   
                    </td>
                    <td>
                       
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" CssClass="dbnLabel" Height="12px"
                            Style="z-index: 109; left: 23px; top: 175px" Width="100px">Tipo Tran. Venta</asp:Label>
                    </td>
                    <td colspan="4">
                        <cc1:DbnetWebLov ID="cmbTranVenta" runat="server" Height="17px" Style="z-index: 128"
                            TabIndex="13" Width="339px">
                        </cc1:DbnetWebLov>
                    </td>
                    <td>&nbsp;&nbsp;&nbsp;
                   
                    </td>
                    <td>
                       
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" CssClass="dbnLabel" Height="12px"
                            Style="z-index: 109; left: 23px; top: 175px" Width="110px">Tipo Tran. Compra</asp:Label>
                    </td>
                    <td colspan="4">
                        <cc1:DbnetWebLov ID="cmbTranCompra" runat="server" Height="17px" Style="z-index: 128"
                            TabIndex="13" Width="339px">
                        </cc1:DbnetWebLov>
                    </td>
                    <td>&nbsp;&nbsp;&nbsp;
                   
                    </td>
                    <td>
                       
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label5" runat="server" CssClass="dbnLabel" Height="12px"
                            Style="z-index: 109; left: 23px; top: 175px" Width="110px">Valor a pagar</asp:Label>
                    </td>
                    <td colspan="4">
                       <asp:TextBox ID="valo_paga" runat="server" CssClass="dbnTextBox_Numero" Height="19px" TabIndex="19"
                                Width="102px" MaxLength="18">0</asp:TextBox>
                        <asp:FilteredTextBoxExtender ID="valo_paga_filtro" runat="server"
                            Enabled="True" TargetControlID="valo_paga" ValidChars="0123456789">
                        </asp:FilteredTextBoxExtender>
                    </td>
                    <td>&nbsp;&nbsp;&nbsp;
                   
                    </td>
                    <td>
                       
                    </td>
                </tr>
            </table>
        </div>
        <div id="DIV1">
            <table border="0" cellspacing="1">
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>
                        <input type="button" onclick="javascript:mostrar('div_detalle')" id="btn_det" style="width: 60px;"
                            value="Detalle" tabindex="24" />
                        <input type="button" onclick="javascript:mostrar('div_descuento')" id="btn_des" style="width: 238px;"
                            value="Descuento y/o Recargos Globales" tabindex="25" />
                        <input type="button" onclick="javascript:mostrar('div_impuesto')" id="btn_imp" style="width: 232px;"
                            value="Impuestos y/o Retenciones Globales" tabindex="26" />
                        <input type="button" onclick="javascript:mostrar('div_referencia')" id="btn_ref"
                            style="width: 92px;" value="Referencias" tabindex="27" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>
                        <input type="button" onclick="javascript:mostrar('div_datoexportacion')" id="btn_dat"
                            style="width: 118px;" value="Datos Exportaci&#243;n" tabindex="28" />
                        <input type="button" onclick="javascript:mostrar('div_transporte')" id="btn_tra"
                            style="width: 185px;" value="Transporte y/o Valores Libres" tabindex="29" />
                        <input type="button" onclick="javascript:mostrar('div_tipobulto')" id="btn_bultos"
                            style="width: 100px;" value="Tipos Bultos" tabindex="30" />
                        <input type="button" onclick="javascript:mostrar('div_comision')" id="btn_comisiones"
                            style="width: 148px;" value="Comisiones o Cargos" tabindex="30" />
                        <input type="button" onclick="javascript:mostrar('div_resumen')" id="btn_res" style="width: 100px;"
                            value="Resumen" tabindex="31" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <div id="div_detalle" style="width: 680px; position: relative; height: 332px;">
            <table border="0" cellspacing="1">
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel10" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125" Width="150px">Detalle</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lb_tipo" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125;" Width="150px" Visible="False">TIPO</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lb_modo" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125; width: 150px;" Visible="False">MODO</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <div style="width: 558px; height: 49px;">
                            <table border="0" class="dbnLabel">
                                <tr>
                                    <th style="width: 71px; height: 12px">Codigo</th>
                                    <th style="height: 12px; width: 155px;">Nombre</th>
                                    <th style="height: 12px; width: 115px;">Valor Unitario</th>
                                    <th style="width: 148px; height: 12px"></th>
                                    <th style="width: 148px;">&nbsp;</th>
                                </tr>
                                <tr>
                                    <td style="width: 71px; height: 17px">
                                        <asp:TextBox ID="DetaCodi" runat="server" CssClass="dbnTextBox" Height="19px"
                                            MaxLength="14" Width="64px" AutoPostBack="True" OnTextChanged="DetaCodi_TextChanged"
                                            TabIndex="18"></asp:TextBox>
                                    </td>
                                    <td style="height: 17px; width: 155px;">
                                        <asp:TextBox ID="DetaNombre" runat="server" AutoPostBack="True" CssClass="dbnTextBox"
                                            Height="19px" MaxLength="20" Width="143px" TabIndex="19"
                                            OnTextChanged="DetaNombre_TextChanged"></asp:TextBox>
                                    </td>
                                    <td style="height: 17px; width: 115px;">
                                        <asp:TextBox ID="DetaPuni" runat="server" CssClass="dbnTextBox" Height="19px"
                                            MaxLength="14" Width="101px" Enabled="False"></asp:TextBox>
                                    </td>
                                    <td style="height: 17px; width: 148px;">
                                        <asp:Button ID="bt_AddDeta" runat="server" OnClick="bt_AddDeta_Click" Text="Obtener Valor"
                                            UseSubmitBehavior="False" TabIndex="20" />
                                    </td>
                                    <th style="width: 148px;">
                                        <img src="../librerias/img/agregar.png" alt="Agregar" id="Img1" onclick="javascript:aceptanew();" />
                                    </th>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="right">
                        <img src="../librerias/img/aceptar.png" alt="Aceptar" id="img_acepta_detalle" onclick="javascript:acepta();" />
                        <img src="../librerias/img/agregar.png" alt="Agregar" id="img_agrega_detalle" onclick="javascript:agrega('tbl_detalle');" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <div style="visibility: hidden">
                            <asp:TextBox ID="txtAuxDetalle" runat="server" Height="10px" MaxLength="40"
                                Style="z-index: 115;" TabIndex="11" Width="10px" />
                            <asp:TextBox ID="txtAgility" runat="server"></asp:TextBox>
                        </div>
                        <div style="width: 700px; height: 174px; overflow: scroll;" class="DataGridPanel">
                            <div style="left: 4px; width: 550px; position: relative; top: 0px;">
                                <table id="tbl_detalle" border='1' class='Tabla' style="width: 1179px;">
                                    <tr class='TablaHeader'>
                                        <th class='dbnLabel' style="height: 20px; width: 140px;">Tipo C&#243;digo</th>
                                        <th class='dbnLabel' style="height: 20px; width: 60px;">C&#243;digo</th>
                                        <th class='dbnLabel' style="height: 20px; width: 150px;">Nombre</th>
                                        <th class='dbnLabel' style="text-align: center; height: 20px; width: 150px;">Descripci&#243;n</th>
                                        <th class='dbnLabel' style="height: 20px; width: 60px;">Cantidad</th>
                                        <th class='dbnLabel' style="height: 20px; width: 130px;">Unid.Medida</th>
                                        <th class='dbnLabel' style="height: 20px; width: 82px;">P.Unitario</th>
                                        <th class='dbnLabel' style="height: 20px; width: 82px;">Total</th>
                                        <th class='dbnLabel' style="height: 20px; width: 80px;">Tipo Desc</th>
                                        <th class='dbnLabel' style="height: 20px; width: 100px;">Descuento</th>
                                        <th class='dbnLabel' style="height: 20px; width: 100px;">Impuesto</th>
                                        <th class='dbnLabel' style="height: 20px; width: 40px;">Exento</th>
                                        <th class='dbnLabel' style="height: 20px; width: 120px;">Tipo C&#243;digo</th>
                                        <th class='dbnLabel' style="height: 20px; width: 50px;">C&#243;digo</th>
                                        <th class='dbnLabel' style="height: 20px; width: 100px;">Tipo Liquidaci&#243;n</th>
                                        <th class='dbnLabel' style="height: 20px">&nbsp;</th>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="right">
                        <asp:Label ID="DbnetWebLabel11" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="100px">Subtotal</asp:Label>
                        <asp:Label ID="lblDetalleST" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="83px">0</asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <!-- Fin Detalle -->
        <div id="div_descuento" style="width: 680px; height: 235px">
            <table border="0" cellspacing="1">
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel13" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125" Width="300px">Descuento y/o recargos Globales</asp:Label>
                    </td>
                    <td align="right">
                        <img src="../librerias/img/aceptar.png" alt="Aceptar" onclick="javascript:acepta();"
                            id="img_acepta_descuento" />
                        <img src="../librerias/img/agregar.png" alt="Agregar" onclick="javascript:agrega('tbl_descuento');"
                            id="img_agrega_descuento" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="visibility: hidden">
                            <asp:TextBox ID="txtAuxDescuento" runat="server" Height="10px" MaxLength="40"
                                Style="z-index: 115" TabIndex="11" Width="10px" />
                        </div>
                        <div style="width: 700px; height: 174px; overflow: scroll;" class="DataGridPanel">
                            <div style="left: 4px; width: 550px; position: relative; top: 0px;">
                                <table id="tbl_descuento" border='1' class='Tabla' style="width: 720px">
                                    <tr class='TablaHeader'>
                                        <th width='80px' class='dbnLabel' style="height: 20px">Tipo</th>
                                        <th width='300px' class='dbnLabel' style="text-align: center; height: 20px;">Descripci&#243;n</th>
                                        <th width='80px' class='dbnLabel' style="height: 20px">Tipo Valor</th>
                                        <th width='100px' class='dbnLabel' style="height: 20px">Valor</th>
                                        <th width='100px' class='dbnLabel' style="height: 20px">Total</th>
                                        <th width='30px' class='dbnLabel' style="height: 20px">Exento</th>
                                        <th width='30px' class='dbnLabel' style="height: 20px">&nbsp;</th>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Label ID="lblSubtotal" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125;" Width="100px">Subtotal</asp:Label>
                        <asp:Label ID="lblDescuentoST" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125;" Width="83px">0</asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div id="div_impuesto" style="width: 680px; position: relative; height: 235px; left: 0px; top: 0px;">
            <table border="0" cellspacing="1">
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel16" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125" Width="300px">Impuestos y/o Retenciones Globales</asp:Label>
                    </td>
                    <td align="right">
                        <img src="../librerias/img/aceptar.png" alt="Aceptar" id="img_acepta_impuesto" onclick="javascript:acepta();" />
                        <img src="../librerias/img/agregar.png" alt="Agregar" id="img_acepta_impuesto2" onclick="javascript:agrega('tbl_impuesto');" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="visibility: hidden">
                            <cc1:DbnetWebTextBox ID="txtAuxImpuesto" runat="server" Height="10px" MaxLength="40"
                                Style="z-index: 115;" TabIndex="11" Width="10px"></cc1:DbnetWebTextBox>
                        </div>
                        <div style="width: 700px; height: 174px; overflow: scroll;" class="DataGridPanel">
                            <div style="left: 4px; width: 550px; position: relative; top: 0px;">
                                <table id="tbl_impuesto" border='1' class='Tabla' style="width: 620px">
                                    <tr class='TablaHeader'>
                                        <th width='80px' class='dbnLabel' style="height: 20px">Tipo</th>
                                        <th width='300px' class='dbnLabel' style="text-align: center; height: 20px;">Descripci&#243;n</th>
                                        <th width='100px' class='dbnLabel' style="height: 20px">Tasa</th>
                                        <th width='100x' class='dbnLabel' style="height: 20px">Total</th>
                                        <th width='30px' class='dbnLabel' style="height: 20px">&nbsp;</th>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Label ID="DbnetWebLabel17" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="100px">Subtotal</asp:Label>
                        <asp:Label ID="lblImpuestoST" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="83px">0</asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div id="div_referencia" style="width: 450px; position: relative; height: 235px; left: 0px; top: 0px;">
            <table border="0" cellspacing="1">
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel19" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125" Width="69px">Referencias</asp:Label>
                    </td>
                    <td align="right">
                        <img src="../librerias/img/aceptar.png" alt="Aceptar" id="img_acepta_referencia"
                            onclick="javascript:acepta();" />
                        <img src="../librerias/img/agregar.png" alt="Agregar" id="img_acepta_referencia2"
                            onclick="javascript:agrega('tbl_referencia');" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="visibility: hidden">
                            <cc1:DbnetWebTextBox ID="txtAuxReferencia" runat="server" Height="10px" MaxLength="40"
                                Style="z-index: 115;" TabIndex="11" Width="10px"></cc1:DbnetWebTextBox>
                        </div>
                        <div style="width: 700px; height: 174px; overflow: scroll;" class="DataGridPanel">
                            <div style="left: 4px; width: 600px; position: relative; top: 0px;">
                                <table id="tbl_referencia" border='1' class='Tabla' style="width: 840px">
                                    <tr class='TablaHeader'>
                                        <th width='150px' class='dbnLabel'>Tipo</th>
                                        <th width='80px' class='dbnLabel'>Folio</th>
                                        <th width='80px' class='dbnLabel'>Fecha</th>
                                        <th width='200px' class='dbnLabel'>Codigo Referencia</th>
                                        <th width='250px' class='dbnLabel'>Razon</th>
                                        <th width='50px' class='dbnLabel'>Ref.Global</th>
                                        <th width='30px' class='dbnLabel'>&nbsp;</th>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="div_datoexportacion" style="width: 680px; position: relative; height: 440px; left: 0px; top: 0px;">
            <table border="0" cellspacing="1" style="border-spacing: 5px">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="DbnetWebLabel22" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125" Width="142px">Datos Exportaci&#243;n</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblFormaPago" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="91px">Forma de Pago Exportación</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebLov ID="cmbFormaPago" runat="server" Height="17px" Style="z-index: 113"
                            TabIndex="2" Width="406px">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblClausula" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="63px">Cl&#225;usula</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebLov ID="cmbClausula" runat="server" Height="17px" Style="z-index: 113"
                            TabIndex="2" Width="406px">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblModa" runat="server" CssClass="dbnLabel" Height="4px" Style="z-index: 125"
                            Width="122px">Modalidad de Venta</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebLov ID="cmbModalidad" runat="server" Height="17px" Style="z-index: 113"
                            TabIndex="2" Width="406px">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblTotalClausula" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="119px">Total Cl&#225;usula</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox ID="txtTotalClausula" runat="server" Height="19px" TabIndex="19"
                            Width="102px" MaxLength="18"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel1" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="80px">Peso Bruto</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox ID="txtPesoBruto" runat="server" Height="19px" TabIndex="19"
                            Width="40px" MaxLength="10">0</cc1:DbnetWebTextBox>
                        <cc1:DbnetWebLov ID="cmbPesoBruto" runat="server" Height="17px" Style="z-index: 113"
                            TabIndex="2" Width="353px">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel20" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="80px">Peso Neto</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox ID="txtPesoNeto" runat="server" Height="19px" TabIndex="19"
                            Width="40px" MaxLength="10">0</cc1:DbnetWebTextBox>
                        <cc1:DbnetWebLov ID="cmbPesoNeto" runat="server" Height="17px" Style="z-index: 113;"
                            TabIndex="2" Width="353px">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblBoking" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="119px">Booking</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox ID="txtBoking" runat="server" Height="19px" MaxLength="20" TabIndex="19"
                            Width="399px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblOperador" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="119px">Operador</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox ID="txtOperador" runat="server" Height="19px" MaxLength="20"
                            TabIndex="19" Width="399px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblViaTransporte" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="135px">V&#237;a de Transporte </asp:Label>
                    </td>
                    <td style="border-left-width: 0px">
                        <cc1:DbnetWebLov ID="cmbTransporte" runat="server" Height="17px" Style="z-index: 113"
                            TabIndex="2" Width="406px">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblNombreTransp" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="130px">Nombre de Transporte</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox ID="txtNombreTransp" runat="server" Height="19px" MaxLength="40"
                            TabIndex="19" Width="399px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblPuerto" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125;" Width="135px">Puerto de Embarque </asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebLov ID="cmbPuerto" runat="server" Height="17px" Style="z-index: 113"
                            TabIndex="2" Width="406px">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblPuerto2" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="135px">Puerto Destino</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebLov ID="cmbPuerto2" runat="server" Height="17px" Style="z-index: 113"
                            TabIndex="2" Width="406px">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblCodigo" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125;" Width="135px">C&#243;digo Pa&#237;s Receptor </asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebLov ID="cmbPais" runat="server" Height="17px" Style="z-index: 113" TabIndex="2"
                            Width="406px">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblTipoMoneda" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="145px">Tipo Moneda Transacci&#243;n </asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebLov ID="cmbMoneda" runat="server" Height="17px" Style="z-index: 113"
                            TabIndex="2" Width="406px" AutoPostBack="True"
                            onselectedindexchanged="cmbMoneda_SelectedIndexChanged">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                 <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="145px">Tipo Cambio</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tipo_camb" runat="server" Height="19px" TabIndex="19"
                            Width="102px" MaxLength="18"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="DbnetWebLabel21" runat="server" CssClass="dbnLabel" Height="12px"
                            Style="z-index: 109" Width="305px">Usar decimales en los calculos de exportaci&#243;n</asp:Label><asp:CheckBox
                                ID="chkDecimal" runat="server" Checked="True" CssClass="dbnLov" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblFlete" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="47px">Flete</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtFlete" runat="server" Height="19px" TabIndex="19" Width="273px" />
                        <asp:FilteredTextBoxExtender ID="txtFleteFtbe" TargetControlID="txtFlete"
                            runat="server" ValidChars="1234567980." />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblMntSeguro" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="47px">Monto Seguro</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtMntSeguro" runat="server" CssClass="dbnTextBox_Numero" Height="19px" TabIndex="19" Width="273px" MaxLength="19" Text="0" />
                        <asp:FilteredTextBoxExtender ID="txtMntSeguroFtbe" TargetControlID="txtMntSeguro"
                            runat="server" ValidChars="1234567980," />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblBultos" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="98px">Total Bultos </asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtTotalBultos" runat="server" Height="19px" TabIndex="19"
                            Width="102px" MaxLength="18"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <div id="div_transporte" style="width: 680px; position: relative; height: 400px; left: 0px; top: 0px;">
            <table border="0" cellspacing="1" style="border-spacing: 5px" id="tbl_transporte">
                <tr>
                    <td colspan="4">
                        <asp:Label ID="DbnetWebLabel28" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125; left: 12px" Width="142px">Transporte</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Dbnetweblabel23" runat="server" CssClass="dbnLabel" Height="4px"
                            Width="96px">Ind. de Traslado</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebLov ID="lvIndi_vegd" runat="server" Height="21px" TabIndex="49" Width="301px" onchange="javascript:validaTrasladoInterno();">
                        </cc1:DbnetWebLov>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Dbnetweblabel24" runat="server" CssClass="dbnLabel" Height="4px"
                            Width="104px">Rut Transportista</asp:Label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="Rutt_tran" runat="server" CssClass="dbnTextBox_Numero"
                            Formato="0" Height="19px" MaxLength="10" TabIndex="50" Width="67px">0</asp:TextBox>
                        <asp:FilteredTextBoxExtender ID="txtRutt_Tran_filtro" runat="server"
                            Enabled="True" TargetControlID="Rutt_tran" ValidChars="0123456789">
                        </asp:FilteredTextBoxExtender>
                        <asp:TextBox ID="Digi_tran" runat="server" Height="19px" MaxLength="1" TabIndex="51"
                            Width="21px"></asp:TextBox>
                        <asp:FilteredTextBoxExtender ID="txtFigiTran_filtro" runat="server"
                            Enabled="True" TargetControlID="Digi_tran" ValidChars="0123456789Kk">
                        </asp:FilteredTextBoxExtender>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lblPatente" runat="server" CssClass="dbnLabel" Height="4px"
                            Width="104px">Patente</asp:Label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="Patente_tran" runat="server" Height="19px" MaxLength="8"
                            TabIndex="52" Width="65px" />
                        <asp:FilteredTextBoxExtender ID="Patente_tran_filtro" runat="server"
                            Enabled="True" TargetControlID="Patente_tran"
                            ValidChars="0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-">
                        </asp:FilteredTextBoxExtender>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">&nbsp;</td>
                    <td colspan="3">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="4">
                        <div id="divSucursal" runat="server">
                            <div id="divContentSucu">
                                <div>
                                    <div class="primero"><label class="dbnLabel">Sucursal Origen</label></div>
                                    <div>
                                        <select class="dbnLov" ID="ddlSucuOrigen" runat="server" style="width:300px; height:19px;"
                                            tabindex="53" onchange="Sucursal(this.id,1);" disabled="disabled"/>
                                            <asp:TextBox id="hdSucuOrig" runat="server" CssClass="hidden"/>
                                    </div>
                                </div>
                                <div>
                                    <div class="primero"><label class="dbnLabel" style="width:104px;">Dirección Origen</label></div>
                                    <div>
                                        <asp:TextBox ID="txtDirecionOrigen" CssClass="dbnTextBox" runat="server" Width="300px" Height="19px" TabIndex="54" Enabled="false"/>
                                    </div>
                                </div>
                                <div>
                                    <div class="primero"><label class="dbnLabel" style="width:104px;">Comuna Origen</label></div>
                                    <div>
                                        <asp:TextBox ID="txtComunaOrigen" CssClass="dbnTextBox" runat="server" Width="102px" Height="19px" TabIndex="55" Enabled="false"/>
                                    </div>
                                    <div id="divCiudOrig">
                                        <label class="dbnLabel" style="width:104px;">Ciudad Origen</label>
                                    </div>
                                    <div style="margin-left: 62px;">
                                        <asp:TextBox ID="txtCiudadOrigen" CssClass="dbnTextBox" runat="server" Width="102px" Height="19px" TabIndex="56" Enabled="false"/>
                                    </div>
                                </div>
                                <div>
                                    <div class="primero"><label class="dbnLabel">Sucursal Destino</label></div>
                                    <div>
                                        <select class="dbnLov" ID="ddlSucuDestino" runat="server" style="width:300px; height:19px;"
                                            tabindex="57" onchange="Sucursal(this.id,2);" disabled="disabled"/>
                                            <asp:TextBox id="hdSucuDest" runat="server" CssClass="hidden" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Dbnetweblabel29" runat="server" CssClass="dbnLabel" Height="4px"
                            Width="104px">Direcci&#243;n Destino</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="Dire_dest" runat="server" Height="19px" TabIndex="58" Width="300px"
                            MaxLength="60"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Dbnetweblabel30" runat="server" CssClass="dbnLabel" Height="4px"
                            Width="101px">Comuna Destino</asp:Label>
                    </td>
                    <td>
                        <cc1:DbnetWebTextBox ID="Comu_dest" runat="server" Height="19px" TabIndex="59" Width="102px"
                            MaxLength="20"></cc1:DbnetWebTextBox>
                    </td>
                    <td>
                        <asp:Label ID="Dbnetweblabel32" runat="server" CssClass="dbnLabel" Width="88px">Cuidad Destino</asp:Label>
                    </td>
                    <td class="style3">
                        <cc1:DbnetWebTextBox ID="Ciud_dest" runat="server" Style="height: 19px;" TabIndex="60"
                            Width="102px" MaxLength="20"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">&nbsp;
                    </td>
                    <td colspan="3">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lblCredec" CssClass="dbnLabel" Width="102" runat="server">Crédito Empr. Constructoras</asp:Label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtCredec" CssClass="dbnTextBox_Numero number" runat="server" MaxLength="18" Width="150px" Height="19px" TabIndex="61">0</asp:TextBox>
                        <asp:FilteredTextBoxExtender ID="txtCredec_filtro" runat="server"
                            Enabled="True" TargetControlID="txtCredec" FilterType="Numbers"
                            ValidChars="0123456789">
                        </asp:FilteredTextBoxExtender>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lblMntMargenCom" runat="server" CssClass="dbnLabel" Height="4px" Width="104px">Monto Margen Comercial</asp:Label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtMntMargenCom" runat="server" CssClass="dbnTextBox_Numero" Height="19px" MaxLength="18" TabIndex="62" Width="150px" Text="0" />
                        <asp:FilteredTextBoxExtender ID="txtMntMargenCom_filtro" runat="server"
                            Enabled="True" TargetControlID="txtMntMargenCom" ValidChars="0123456789">
                        </asp:FilteredTextBoxExtender>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">&nbsp;
                    </td>
                    <td colspan="3">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Label ID="DbnetWebLabel25" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125; left: 12px" Width="132px">Valores Libres</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lvlval1" runat="server" CssClass="dbnLabel" Height="4px" Width="36px">Val1</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="txtVal1" runat="server" Height="19px" TabIndex="63" MaxLength="300" Width="350px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lvlval2" runat="server" CssClass="dbnLabel" Height="4px" Width="36px">Val 2</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="txtVal2" runat="server" Height="19px" TabIndex="64" MaxLength="300" Width="350px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lvlval3" runat="server" CssClass="dbnLabel" Height="4px" Width="36px">Val 3</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="txtVal3" runat="server" Height="19px" TabIndex="65" Width="350px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lvlval4" runat="server" CssClass="dbnLabel" Height="4px" Width="36px">Val 4</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="txtVal4" runat="server" Height="19px" TabIndex="66" MaxLength="300" Width="350px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lvlval5" runat="server" CssClass="dbnLabel" Height="4px" Width="36px">Val 5</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="txtVal5" runat="server" Height="19px" TabIndex="67" MaxLength="300" Width="350px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lvlval6" runat="server" CssClass="dbnLabel" Height="4px" Width="36px">Val 6</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="txtVal6" runat="server" Height="19px" TabIndex="68" MaxLength="300" Width="350px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lvlval7" runat="server" CssClass="dbnLabel" Height="4px" Width="36px">Val 7</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="txtVal7" runat="server" Height="19px" TabIndex="69" MaxLength="300" Width="350px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lvlval8" runat="server" CssClass="dbnLabel" Height="4px" Width="36px">Val 8</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="txtVal8" runat="server" Height="19px" TabIndex="70" MaxLength="300" Width="350px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="lvlval9" runat="server" CssClass="dbnLabel" Height="4px" Width="36px">Val 9</asp:Label>
                    </td>
                    <td colspan="3">
                        <cc1:DbnetWebTextBox ID="txtVal9" runat="server" Height="19px" TabIndex="71" MaxLength="300" Width="350px"></cc1:DbnetWebTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">&nbsp;
                    </td>
                    <td colspan="3">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">&nbsp;
                    </td>
                    <td colspan="3">&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <div id="div_resumen" style="width: 680px; position: relative; height: 235px; left: 0px; top: 0px;">
            <table border="0" cellspacing="1">
                <tr>
                    <td colspan="5">
                        <asp:Label ID="DbnetWebLabel31" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125" Width="142px">Resumen</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="5">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Label ID="DbnetWebLabel15" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="75px">Cantidad</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="DbnetWebLabel18" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="85px">Subtotal</asp:Label>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel33" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="142px">Detalles</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblcantdeta" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: center" Width="58px">0</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblsubdeta" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: center" Width="85px">0</asp:Label>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel34" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="215px">Descuento y/o recargos Globales</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblcantdesc" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: center" Width="58px">0</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblsubdesc" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: center" Width="85px">0</asp:Label>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel35" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="219px">Impuestos y/o Retenciones Globales</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblcantimpu" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: center" Width="58px">0</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblsubimpu" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: center" Width="85px">0</asp:Label>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel36" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="69px">Referencias</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblcantrefe" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: center" Width="58px">0</asp:Label>
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td>
                        <asp:Label ID="lbldeAfecto" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="82px">Total Afecto</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblTotalAfecto" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: right" Width="92px">0</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblTotalNoAfecto" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: right" Width="92px">0</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td>
                        <asp:Label ID="DbnetWebLabel45" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="82px">Total Exento</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblTotalExento" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: right" Width="92px">0</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td>
                        <asp:Label ID="lvldeIva" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="82px">IVA 19%</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblIVA" runat="server" CssClass="dbnLabel" Height="4px" Style="z-index: 125; text-align: right"
                            Width="92px">0</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td>
                        <asp:Label ID="lblMargenCom" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; display: none;" Width="82px">Margen Comer.</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblResuMargenCom" runat="server" CssClass="dbnLabel" Height="4px" Style="z-index: 125; text-align: right; display: none;"
                            Width="92px">0</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td>
                        <asp:Label ID="lblIvaMargenCom" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; display: none;" Width="82px">I.V.A. M. Comer. 19%</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblResuIvaMargenCom" runat="server" CssClass="dbnLabel" Height="4px" Style="z-index: 125; text-align: right; display: none;"
                            Width="92px">0</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td>
                        <asp:Label ID="lblTotaltiu" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125" Width="82px">Total </asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblTotal" runat="server" CssClass="dbnLabel" Height="4px"
                            Style="z-index: 125; text-align: right" Width="92px">0</asp:Label>
                    </td>
                </tr>
            </table>
            <div style="visibility: hidden;">
                <table border="0" cellspacing="1">
                    <tr>
                        <td>
                            <cc1:DbnetWebTextBox ID="txtTotales" runat="server" Height="10px" MaxLength="40"
                                Style="z-index: 115" TabIndex="11" Width="10px"></cc1:DbnetWebTextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="div_comision" style="width: 680px; position: relative; height: 235px; left: 0px; top: 0px;">
            <table border="0" cellspacing="1">
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel26" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125" Width="69px">Comisiones o Cargos</asp:Label>
                    </td>
                    <td align="right">
                        <img src="../librerias/img/aceptar.png" alt="Aceptar" id="img3" onclick="javascript:acepta();" />
                        <img src="../librerias/img/agregar.png" alt="Agregar" id="img4" onclick="javascript:agrega('tbl_comision');" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="visibility: hidden">
                            <asp:TextBox ID="txtAuxComision" runat="server"></asp:TextBox>
                            <asp:TextBox ID="txtAuxComiNEI" runat="server"></asp:TextBox>
                        </div>
                        <div style="width: 700px; height: 174px; overflow: scroll;" class="DataGridPanel">
                            <div style="left: 4px; width: 550px; position: relative; top: 0px;">
                                <input id="hdValNumero" runat="server" type="hidden" value="1" />
                                <table id="tbl_comision" border='1' class='Tabla' style="width: 840px">
                                    <tr class='TablaHeader'>
                                        <th width='80px' class='dbnLabel'>Tipo Mov.</th>
                                        <th width='180px' class='dbnLabel'>Glosa</th>
                                        <th width='80px' class='dbnLabel'>Tasa</th>
                                        <th width='80px' class='dbnLabel'>Neto</th>
                                        <th width='80px' class='dbnLabel'>Exento</th>
                                        <th width='80px' class='dbnLabel'>IVA</th>
                                        <th width='30px' class='dbnLabel'>&nbsp;</th>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="position: relative; float: right; width: 200px; height: 80px;">
                            <div style="position: relative; float: right; width: 170px;">
                                <label id="lbTotalNeto" class="dbnLabel" style="position: relative; float: left;">Total Neto: </label>
                                <asp:Label ID="lblTotalNetoAsig" runat="server" CssClass="dbnLabel" Style="position: relative; float: right;">0</asp:Label>
                            </div>
                            <div style="position: relative; float: right; width: 170px;">
                                <label id="lbTotalExento" class="dbnLabel" style="position: relative; float: left;">Total Exento: </label>
                                <asp:Label ID="lblTotalExentoAsig" runat="server" CssClass="dbnLabel" Style="position: relative; float: right;">0</asp:Label>
                            </div>
                            <div style="position: relative; float: right; width: 170px;">
                                <label id="lbTotalIva" class="dbnLabel" style="position: relative; float: left;">Total Iva: </label>
                                <asp:Label ID="lblTotalIvaAsig" runat="server" CssClass="dbnLabel" Style="position: relative; float: right;">0</asp:Label>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="div_msgschema" style="width: 680px; position: relative; height: 235px; left: 0px; top: 0px;">
            <table border="0" cellspacing="1">
                <tr>
                    <td>
                        <asp:Label ID="lb_msgschema" runat="server" CssClass="dbnLabel" Height="211px"
                            Style="z-index: 109" Width="659px" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div id="div_tipobulto" style="width: 680px; height: 235px;">
            <table border="0" cellspacing="1">
                <tr>
                    <td>
                        <asp:Label ID="DbnetWebLabel6" runat="server" CssClass="barTitulo" Height="4px"
                            Style="z-index: 125; text-align: left" Width="142px">Tipo de Bultos </asp:Label>
                    </td>
                    <td align="right">
                        <img src="../librerias/img/aceptar.png" alt="Aceptar" onclick="javascript:acepta();"
                            id="Img2" />
                        <img src="../librerias/img/agregar.png" alt="Agregar" onclick="javascript:agrega('tbl_bultos');"
                            id="img_acepta_bultos" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="visibility: hidden">
                            <cc1:DbnetWebTextBox ID="txtAuxTpoBulto" runat="server" Height="10px" MaxLength="40"
                                Style="z-index: 115" TabIndex="11" Width="10px"></cc1:DbnetWebTextBox>
                        </div>
                        <div style="left: 10px; width: 700px; height: 174px; overflow: scroll;" class="DataGridPanel">
                            <div style="left: 4px; width: 550px; position: relative" id="DIV2">
                                <table id="tbl_bultos" border='1' class='Tabla'>
                                    <tr class='TablaHeader' style="width:800px;">
                                        <th class='dbnLabel' style="width: 187px">Codigo</th>
                                        <th width='80px' class='dbnLabel'>Cantidad</th>
                                        <th width='80px' class='dbnLabel'>Marcas</th>
                                        <th width='120px' class='dbnLabel'>Id Container</th>
                                        <th width='80px' class='dbnLabel'>Sello</th>
                                        <th width='250px' class='dbnLabel'>Emisor Sello</th>
                                        <th width='50px' class='dbnLabel'></th>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>

        <% =Session["javascript"] %>
        <script type="text/javascript">
            mostrar('');
            datos();
            if (tipo_documento()) {
                document.getElementById('lbldeAfecto').style.visibility = 'visible';
                document.getElementById('lblTotalAfecto').style.visibility = 'visible';
                document.getElementById('lblTotalNoAfecto').style.visibility = 'visible';
                document.getElementById('lvldeIva').style.visibility = 'visible';
                document.getElementById('lblIVA').style.visibility = 'visible';
            }
            else 
            {
                document.getElementById('lbldeAfecto').style.visibility = 'hidden';
                document.getElementById('lblTotalAfecto').style.visibility = 'hidden';
                document.getElementById('lblTotalNoAfecto').style.visibility = 'hidden';
                document.getElementById('lvldeIva').style.visibility = 'hidden';
                document.getElementById('lblIVA').style.visibility = 'hidden';
            }  
        </script>
    </form>
</body>
</html>
