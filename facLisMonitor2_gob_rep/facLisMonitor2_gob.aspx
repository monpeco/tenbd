<%@ Register TagPrefix="cc1" Namespace="DbnetWebControl" Assembly="DbnetWebControl" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="facLisMonitor2_GOB.aspx.cs" Inherits="facLisMonitor2_GOB" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Suite Electronica - <% = lbTitulo.Text %></title>
		<meta content="True" name="vs_snapToGrid">
		<meta content="True" name="vs_showGrid">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../librerias/css/dbnEstilo.css" type="text/css" rel="stylesheet">
  		<link rel="stylesheet" type="text/css" href="../librerias/css/jscal2.css" />
        <link rel="stylesheet" type="text/css" href="../librerias/css/border-radius.css" />
        <link rel="stylesheet" type="text/css" href="../librerias/css/steel/steel.css" />
        
        <script language="javascript" type="text/javascript">

            function waitx()
            {
                if (this.document.getElementById)
                    {  // DOM3 = IE5, NS6
                        this.document.getElementById("hidepage").style.visibility = "visible";
                    }
                else 
                    {
                        if (this.document.layers) 
                        {  // Netscape 4
                            this.document.hidepage.visibility = "visible";
                        }
                        else 
                        {  // IE 4
                            this.document.all.hidepage.style.visibility = "visible";
                        }
                    }
            }
         
            function verOcultarError(controlDiv, controlCheck, checkDespliega)
           {
               if(this.document.getElementById(checkDespliega).checked == true)
               {
                   this.document.getElementById(controlCheck).checked = !this.document.getElementById(controlCheck).checked;
                   if(this.document.getElementById(controlCheck).checked == true)
                   {
                       Effect.BlindDown(controlDiv);
                       return false;
                   }
                   else
                   {
                       Effect.BlindUp(controlDiv);
                       return false;
                   }
               }
           }
        </script>

	</head>
	<body bottomMargin="0" topMargin="0" onload="MM_load()" rightMargin="0">		
		<form id="dbnForm" title="Suite Electronica" method="post" runat="server">
		<script src="../librerias/js/jscal2.js"></script>
        <script src="../librerias/js/lang/es.js"></script>		
		<script src="../librerias/js/facLisMonitor2.js" type="text/javascript"></script>	
		<script src="../librerias/js/enter.js" type="text/javascript"></script>
	   <script src="../librerias/js/busqueda.js" type="text/javascript"></script>
	   <script src="../librerias/js/rollover.js" type="text/javascript"></script>
	   <script src="../librerias/js/overlib.js" type="text/javascript"></script>
			
			<div style="WIDTH: 680px; POSITION: relative; HEIGHT: 90px; top: 0px; left: 0px;"  
            class="fondo1">
			<table border="0" cellpadding="0" cellspacing="0">
			<tr>
			<td><asp:label id="lbTitulo" style="Z-INDEX: 110; Design_Time_Lock: True"
					runat="server" Design_Time_Lock="True" BorderColor="Transparent" BackColor="Transparent" 
                    CssClass="barTitulo" Width="624px">Titulo</asp:label></td>
             </tr>
             <tr>
             <td>&nbsp;</td>  
             </tr>
             <tr>
			<td><asp:image id="imgTitulo" style="Z-INDEX: 101; LEFT: 2px; TOP: 0px" runat="server"
					Width="656px" ImageUrl="../librerias/img/titulo_01.jpg" Height="34px" Visible="False"></asp:image></td>
             </tr>
             <tr>
			<td><table border="0" cellpadding="0" >
             <tr>
             <td width="33px"><asp:imagebutton id="barFirst" onmouseover="this.src='../librerias/img/page_first_over.png'"
					style="Z-INDEX: 108" onmouseout="this.src='../librerias/img/page_first.png'" runat="server" Design_Time_Lock="True" ToolTip="Primera Pagina"
					ImageUrl="../librerias/img/page_first.png" CausesValidation="False" ></asp:imagebutton></td>
					
              <td width="33px"><asp:imagebutton id="barBack" onmouseover="this.src='../librerias/img/page_back_over.png'"
					style="Z-INDEX: 102" onmouseout="this.src='../librerias/img/page_back.png'" runat="server" Design_Time_Lock="True" 
					ToolTip="Pagina anterior" ImageUrl="../librerias/img/page_back.png"
					CausesValidation="False" ></asp:imagebutton></td>
					
              <td width="33px"><asp:imagebutton id="barNext" onmouseover="this.src='../librerias/img/page_next_over.png'"
					style="Z-INDEX: 103" onmouseout="MM_swapImgRestore()" runat="server"
					Design_Time_Lock="True" ToolTip="Pagina siguiente" ImageUrl="../librerias/img/page_next.png" CausesValidation="False" ></asp:imagebutton></td>
					
              <td width="33px"><asp:imagebutton id="barLast" onmouseover="this.src='../librerias/img/page_last_over.png'"
					style="Z-INDEX: 109" onmouseout="this.src='../librerias/img/page_last.png'" runat="server" Design_Time_Lock="True" 
                    ToolTip="Ultima Pagina" ImageUrl="../librerias/img/page_last.png"
                    CausesValidation="False" ></asp:imagebutton></td>
                    
              <td width="33px"><asp:imagebutton id="barExcel" onmouseover="this.src='../librerias/img/page_save_over.png'"
					style="Z-INDEX: 112" onmouseout="this.src='../librerias/img/page_save.png'" runat="server" ToolTip="Dercargar en Formato Excel"
					ImageUrl="../librerias/img/page_save.png" ></asp:imagebutton></td>
             <td width="33px"></td>
             <td width="33px"></td>
             <td width="33px"></td>
             <td width="33px"></td>
             <td><asp:Label ID="lbPag" runat="server" CssClass="barLabel" Design_Time_Lock="True">Paginas:</asp:Label> </td>
             <td><asp:Label ID="lbPagina" runat="server" CssClass="barLabel" Design_Time_Lock="True" Style="z-index: 105; design_time_lock: True"></asp:Label></td>
             <td width="33px"></td>
             <td><asp:Label ID="lbReg" runat="server" CssClass="barLabel" Design_Time_Lock="True"
                Style="z-index: 106; design_time_lock: True">Registros:</asp:Label></td>
             <td><asp:Label ID="lbRegistro" runat="server" CssClass="barLabel" Design_Time_Lock="True"
                Style="z-index: 107; design_time_lock: True"></asp:Label></td>
             <td width="100px"></td>
             <td><asp:Label ID="lbRegPag" runat="server" CssClass="barLabel" Design_Time_Lock="True"
                Style="z-index: 111; design_time_lock: True">Reg/Pag:</asp:Label></td>
             <td width="12px"></td>
             <td> <cc1:DbnetWebLov ID="txtRegPag" runat="server" AutoPostBack="True" Design_Time_Lock="True"
                Style="z-index: 114; design_time_lock: True"
                Width="50px">
                <asp:ListItem Value="10">10</asp:ListItem>
                <asp:ListItem Value="15">15</asp:ListItem>
                <asp:ListItem Value="25">25</asp:ListItem>
                <asp:ListItem Value="30">30</asp:ListItem>
                <asp:ListItem Value="50">50</asp:ListItem>
                <asp:ListItem Value="100">100</asp:ListItem>
                <asp:ListItem Value="200">200</asp:ListItem>
            </cc1:DbnetWebLov></td>
             </tr>
            </table>
             </td>
             </tr>
    </table>
    </div>
    <table border="0" cellpadding="0" cellspacing="0">
             <tr>
			<td><div id="ayuda" style="display: none; left: 0px; overflow: auto;
                width: 680px; position: relative; top: 0px; height: 25px">
                <asp:Label ID="barDescripcion" runat="server" CssClass="dbnTexto" Height="15px" Style="z-index: 103" Width="630px">Permite subir a Suite Electronica el archivo del libro de compra venta y de guias de despacho</asp:Label>
            </div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
                <Scripts>
                     <asp:ScriptReference Path="../librerias/js/prototype.js" />
                    <asp:ScriptReference Path="../librerias/js/effects.js" />
                </Scripts>
            </asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
	            <ContentTemplate>
	                <div id="Div2" style="width: 630px; position: relative; top: 0px; height: auto;">
                        <asp:Label ID="lbMensaje" runat="server" CssClass="dbnError" 
                            Style="height: auto;  width: 630px; position: relative; top: 0px;" Width="630px" 
                            enabled="False" 
                            onclick="verOcultarError('divEx', 'chkEx', 'chkDespliega'); return false;">
                        </asp:Label>
                    </div>
	                <div id="divEx" style="display: none; background-color: #FFFF80; width: 631px; position: relative; top: 0px; height: auto">
                        <asp:Label ID="lbEx" runat="server" CssClass="dbnError" Style="z-index: 101; height: auto;" Width="630px">
                        </asp:Label>
                        <asp:CheckBox ID="chkEx" runat="server" style="visibility: hidden;"/>
                        <asp:CheckBox ID="chkDespliega" runat="server" style="visibility: hidden;"/>
                    </div>
	            </ContentTemplate>
		    </asp:UpdatePanel></td>
             </tr>
            </table>
 

<div id="Busqueda" style="WIDTH: 800px; HEIGHT: 150px; top: 0px; left: 0px;" class="fondo3">
                                
    <table id="tbListador" border="0" cellspacing="1">
                            <tr>
                                <td class="style6">
                            <cc1:dbnetweblabel id="DbnetWebLabel1" CssClass="dbnLabel" style=""
								runat="server" Width="75px">Fecha Desde</cc1:dbnetweblabel></td>
                                <td class="style2">
                                    <cc1:dbnetwebtextbox_fecha id="Fecha_desde" style="Z-INDEX: 103; Design_Time_Lock: True"
								runat="server" Design_Time_Lock="True" Width="74px" Formato="1" Height="19px" MaxLength="10">2010-11-01</cc1:dbnetwebtextbox_fecha>
                               <img id="Fecha_desdeCal" src="../librerias/img/cal.gif" 
                                        Style="z-index: 900; cursor: pointer;"/></td>
                                <td>
                            <cc1:dbnetweblabel id="DbnetWebLabel4" Width="75px"  CssClass="dbnLabel" style=""
								runat="server">Folio Desde</cc1:dbnetweblabel></td>
                                <td><cc1:dbnetwebtextbox_numero id="Foli_docu1" 
								   style="Z-INDEX: 101; Design_Time_Lock: True"
								   runat="server" Design_Time_Lock="True" CssClass="dbnTextBox_Numero" Width="79px">0</cc1:dbnetwebtextbox_numero>
								   <cc1:dbnetwebtextbox id="Foli_erp1" style="Z-INDEX: 125;" runat="server"
								   CssClass="dbnTextBox_Numero" Width="80px" Height="19px" Visible="False" AutoPostBack="True" MaxLength="10">1</cc1:dbnetwebtextbox>
                                <cc1:dbnetwebtextbox id="Foli_erp2" style="Z-INDEX: 124;" runat="server"
								CssClass="dbnTextBox_Numero" Width="80px" Height="19px" Visible="False" AutoPostBack="True" MaxLength="10">9999999999</cc1:dbnetwebtextbox></td>
                                <td>
                            <cc1:dbnetweblabel id="DbnetWebLabel5"
								runat="server">Tipo</cc1:dbnetweblabel></td>
                                <td>
                            <cc1:dbnetwebtextbox_numero id="Tipo_docu" style=""
								runat="server" Design_Time_Lock="True" CssClass="dbnTextBox_Numero" Width="23px" Height="19px">0</cc1:dbnetwebtextbox_numero>
                            <cc1:dbnetweblov id="lvTipo_docu" runat="server" Design_Time_Lock="True" Width="239px" AutoPostBack="True" Height="17px"></cc1:dbnetweblov></td>
                            </tr>
                            <tr>
                                <td>
                            <cc1:dbnetweblabel id="DbnetWebLabel2" CssClass="dbnLabel" style="" Width="75px"
								runat="server" Height="19px">Fecha Hasta</cc1:dbnetweblabel>
                                </td>
                                <td>
                                    <cc1:dbnetwebtextbox_fecha id="Fecha_hasta" style="Z-INDEX: 104; Design_Time_Lock: True"
								runat="server" Design_Time_Lock="True" Width="74px" Formato="1" Height="19px" MaxLength="10">2010-11-01</cc1:dbnetwebtextbox_fecha>
                           <img id="Fecha_hastaCal" src="../librerias/img/cal.jpg" 
                                        Style="z-index: 901; cursor: pointer;"/>
                                        <script type="text/javascript">//<![CDATA[
                                    var cal = Calendar.setup({
                                          onSelect: function(cal) { cal.hide() }
                                          
                                      });
                                      cal.manageFields("Fecha_desdeCal", "Fecha_desde", "%Y-%m-%d");
                                      cal.manageFields("Fecha_hastaCal", "Fecha_hasta", "%Y-%m-%d"); 
                                      var skin = document.getElementById("skinhelper-compact");
                                      skin.rel = "stylesheet";
                                      skin.disabled = true;                              
                                             //]]></script></td>
                                <td>
                            <cc1:dbnetweblabel id="DbnetWebLabel3" style=""
								runat="server">Folio Hasta</cc1:dbnetweblabel></td>
                                <td>
                                    <cc1:dbnetwebtextbox_numero id="Foli_docu2" 
								style="Z-INDEX: 102; Design_Time_Lock: True"
								runat="server" Design_Time_Lock="True" CssClass="dbnTextBox_Numero" Width="79px">9999999999</cc1:dbnetwebtextbox_numero></td>
                                <td><cc1:dbnetweblabel id="lbCodi_pers" Width="60px" runat="server">Receptor</cc1:dbnetweblabel></td>
                                <td><cc1:dbnetwebtextbox_numero id="Codi_pers" style=""
								runat="server" Design_Time_Lock="True" CssClass="dbnTextbox_Numero" Width="64px" 
                                AutoPostBack="True" MaxLength="16" Height="19px">0</cc1:dbnetwebtextbox_numero>
                                <cc1:dbnetweblov id="lvCodi_pers" style=""
								tabIndex="6" runat="server" Design_Time_Lock="True" Width="197px" AutoPostBack="True" 
                                Height="17px"></cc1:dbnetweblov><asp:ImageButton ID="lvCodi_persLoad" runat="server" ImageUrl="../librerias/img/load.jpg"
                                OnClick="lvCodi_persLoad_Click" Style="" 
                                ToolTip="Carga Receptores" Width="16px" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                            <cc1:DbnetWebLabel ID="DbnetWebLabel20" runat="server" CssClass="dbnLabel" 
                                Style="" Width="30px" Visible="False">Para:</cc1:DbnetWebLabel>
                                </td>
                                <td>
                            <asp:RadioButton id="rdEmision" style=""
				                runat="server" CssClass="dbnRadio" Width="81px" Height="20px" GroupName="Mensajes" Text="Emision"
				                Checked="True" Visible="False"></asp:RadioButton>                                
                                
				                </td>
                                <td>
                                    </td>
                                <td>
                                    </td>
                                <td>
                                    <cc1:dbnetweblabel id="lbCodi_sucu" style="Z-INDEX: 115" Width="60px"
								runat="server" CssClass="dbnLabel" Visible="False">Sucursal</cc1:dbnetweblabel></td>
                                <td>
                            <cc1:dbnetwebtextbox_numero id="Codi_sucu" style=""
								runat="server" Design_Time_Lock="True" CssClass="dbnTextbox_Numero" Width="64px" 
                                AutoPostBack="True" MaxLength="16" Height="19px" Visible="False">0</cc1:dbnetwebtextbox_numero>
                            <cc1:dbnetweblov id="lvCodi_sucu" style=""
								tabIndex="6" runat="server" Design_Time_Lock="True" Width="197px" AutoPostBack="True" 
                                Height="17px" Visible="False"></cc1:dbnetweblov></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td colspan="5"><asp:RadioButton id="rdRecepcion" style=""
				                runat="server" CssClass="dbnRadio" Width="92px" Height="20px" GroupName="Mensajes" Text="Recepcion"
				                Checked="True" Visible="False"></asp:RadioButton>
				                &nbsp;<cc1:dbnetweblabel id="LbFoli_clie" style="Z-INDEX: 122"
								runat="server" Visible="False">Folio ERP</cc1:dbnetweblabel>
                            <asp:checkbox id="ChkFoli_clie" style="POSITION: relative;"
								runat="server" CssClass="dbnTextbox" AutoPostBack="True" Text=" " Visible="False"></asp:checkbox>
                                    <cc1:dbnetweblabel id="Dbnetweblabel6" style="Z-INDEX: 118"
								runat="server">Estados Anormales</cc1:dbnetweblabel>
                                    <asp:checkbox id="EstadoAnormal" style="Z-INDEX: 119"
								runat="server" Design_Time_Lock="True" CssClass="dbnTextbox" Text=" "></asp:checkbox>
                                    <cc1:dbnetweblabel id="Dbnetweblabel7" style="Z-INDEX: 121"
								runat="server">Ver Respuesta en Formato XML</cc1:dbnetweblabel>
                                    <asp:checkbox id="FormatoRespuesta" style="Z-INDEX: 120"
								runat="server" CssClass="dbnTextbox" Width="137px" Height="16px"></asp:checkbox></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td align="center"><asp:imagebutton id="btBuscar" onmouseover="this.src='../librerias/img/bt_buscar_over.png'"
								style="Z-INDEX: 100; LEFT: 605px; TOP: 72px" onmouseout="this.src='../librerias/img/bt_buscar.png'" runat="server" 
                                    ToolTip="Buscar" ImageUrl="../librerias/img/bt_buscar.png" 
                                    ></asp:imagebutton></td>
                            </tr>
                        </table>
   </div>
				    
	<table border="0" cellspacing="1">
				<tr>
				<TD colSpan="3">&nbsp;</TD>
				</tr>
				<TR>
					<TD colSpan="3">
					        <div style='left: 0px; overflow: auto;
					         width: 680px; position: relative; top: 0px; overflow: scroll;'>
					        <asp:datagrid id="Grilla" runat="server" CssClass="dbnGrilla" Width="2500px"
							AllowSorting="True" PageSize="15" AllowPaging="True" AutoGenerateColumns="False" OnSelectedIndexChanged="Grilla_SelectedIndexChanged">
							<ItemStyle CssClass="dbnGrillaDetalle"></ItemStyle>
							<HeaderStyle Width="18px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
							<Columns>
							<asp:ButtonColumn Text="&lt;img src=../librerias/img/xml.gif border=0 alt=XML&gt;" 
									CommandName="XML">
							<HeaderStyle Width="18px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
							</asp:ButtonColumn>
								<asp:ButtonColumn Text="&lt;img src=../librerias/img/pdf.gif border=0 alt=PDF&gt;" 
									CommandName="PDF">
							<HeaderStyle Width="18px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
								</asp:ButtonColumn>
							<asp:ButtonColumn Text="&lt;img src=../librerias/img/html.gif border=0 alt=HTML&gt;" 
									CommandName="HTML">
							<HeaderStyle Width="18px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
								</asp:ButtonColumn>
								<asp:ButtonColumn Text="&lt;img src=../librerias/img/file.jpg border=0 alt=Archivo&gt;" 
									CommandName="ARHE">
							<HeaderStyle Width="18px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
								</asp:ButtonColumn>
								<asp:ButtonColumn Text="&lt;img src=../librerias/img/help.gif border=0 alt=Ayuda&gt;" 
									CommandName="HELP">
							<HeaderStyle Width="18px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
								</asp:ButtonColumn>
							<asp:TemplateColumn HeaderText="Lin">
							<HeaderStyle Width="18px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="lblLin" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Lin") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Imp">
							<HeaderStyle Width="18px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="lblImp" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Imp") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Con">
							<HeaderStyle Width="18px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="lblCon" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Con") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Tipo" SortExpression="Tipo">
									<HeaderStyle Width="20px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemStyle CssClass="dbnGrillaDetalle" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="lbTipo" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Tipo") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Folio" SortExpression="Folio">
									<HeaderStyle Width="60px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbFolio" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Folio") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Folio ERP" SortExpression="Folio-ERP">
									<HeaderStyle Width="60px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbFolioERP" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Folio-ERP") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Rut" SortExpression="Rut">
									<HeaderStyle Width="75px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbRut" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Rut") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Nombre" SortExpression="Nombre">
									<HeaderStyle Width="250px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbNombre" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Nombre") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Total" SortExpression="Total">
									<HeaderStyle Width="85px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbTotal" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Total") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Emision" SortExpression="Emision">
									<HeaderStyle Width="85px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbEmision" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Emision") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Carga">
									<HeaderStyle Width="45px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbCarga" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Carga") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Envio SII">
									<HeaderStyle Width="75px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbEnvioSII" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Envio SII") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:ButtonColumn Text="&lt;img src=../librerias/img/resp_sii.gif border=0 alt=&quot;Respuesta SII&quot;&gt;"
									HeaderText="Resp. SII" CommandName="RESP1">
									<HeaderStyle Width="20px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
								</asp:ButtonColumn>
								<asp:TemplateColumn HeaderText="Envio Contribuyente">
									<HeaderStyle Width="120px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbEnvioContr" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Envio Contribuyente") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Recep.DTE">
									<HeaderStyle Width="80px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbRecepcion" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Recep DTE") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Resp.Tecnica">
									<HeaderStyle Width="80px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbRespTecnica" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Resp Tecnica") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:ButtonColumn Text="&lt;img src=../librerias/img/resp_tec.gif border=0 alt=&quot;Respuesta Tecnica&quot;&gt;"
									HeaderText="Resp. Tecnica" CommandName="RESP2">
									<HeaderStyle CssClass="dbnGrillaEncabezado" Width="20px" ></HeaderStyle>
								</asp:ButtonColumn>
								<asp:ButtonColumn Text="&lt;img src=../librerias/img/pdf_trazo.jpg border=0 alt=&quot;Respuesta Lapiz Digital&quot;&gt;"
									HeaderText="Trazo" CommandName="TRAZO">
									<HeaderStyle CssClass="dbnGrillaEncabezado" Width="18px"></HeaderStyle>
								</asp:ButtonColumn>
								<asp:TemplateColumn HeaderText="Resp.Comercial">
									<HeaderStyle Width="80px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbRespComercial" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Resp Comercial") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:ButtonColumn Text="&lt;img src=../librerias/img/resp_com.gif border=0 alt=&quot;Respuesta Comercial&quot;&gt;"
									HeaderText="Resp. Comercial" CommandName="RESP3">
									<HeaderStyle Width="20px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
								</asp:ButtonColumn>
								<asp:TemplateColumn HeaderText="Estado Documento" SortExpression="Estado Doc">
									<HeaderStyle Width="180px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbEstadoDoc" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Estado Doc") %>
										</asp:Label>
										-
										<asp:Label id="lbDescripDoc" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Descripcion Doc") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Estado Envio" SortExpression="Estado Env">
									<HeaderStyle Width="180px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbEstadoEnv" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Estado Env") %>
										</asp:Label>
										-
										<asp:Label id="lbDescripEnv" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Descripcion Env") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Correlativo" SortExpression="Correlativo">
									<HeaderStyle Width="60px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbCorrel" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Correlativo") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Track-id" SortExpression="Track-id">
									<HeaderStyle Width="60px" CssClass="dbnGrillaEncabezado"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lbTrackId" runat="server">
											<%# DataBinder.Eval(Container.DataItem, "Track-id") %>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle Visible="False" NextPageText="Siguiente" PrevPageText="Anterior" HorizontalAlign="Left"
								Position="Top" PageButtonCount="3"></PagerStyle>
                        <AlternatingItemStyle CssClass="dbnGrillaIntercala" />
						</asp:datagrid>
						
						</div>
						</TD>
				</TR>
		</TABLE>
		</form>
	</body>
</html>
