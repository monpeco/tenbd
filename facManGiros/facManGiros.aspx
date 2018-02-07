<%@ Register TagPrefix="cc1" Namespace="DbnetWebControl" Assembly="DbnetWebControl" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="facManGiros.aspx.cs" Inherits="facManGiros" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Suite Electr&#243;nica - <% = lbTitulo.Text %></title>
		<meta content="False" name="vs_snapToGrid">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../librerias/css/dbnEstilo.css" type="text/css" rel="stylesheet">
		
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
	<body onLoad="MM_load()" topMargin="0" MS_POSITIONING="GridLayout">
		<form id="dbnForm" method="post" runat="server">
			
				 <script src="../librerias/js/enter.js" type="text/javascript"></script>
	   <script src="../librerias/js/busqueda.js" type="text/javascript"></script>
	   <script src="../librerias/js/rollover.js" type="text/javascript"></script>
	   <script src="../librerias/js/overlib.js" type="text/javascript"></script>
		
	<div style="WIDTH: 680px; POSITION: relative; HEIGHT: 90px" ms_positioning="GridLayout" class="fondo1">
	<table border="0" cellpadding="0" cellspacing="0">
        <tr>
        <td><asp:label id="lbTitulo" style="Z-INDEX: 101;"
					runat="server" Width="535px" CssClass="barTitulo" BorderColor="Transparent" BackColor="Transparent"
					Design_Time_Lock="True">Titulo</asp:label></td>
        </tr>
         <tr>
        <td>&nbsp;</td>  
        </tr>
         <tr>
        <td><asp:image id="imgTitulo" style="Z-INDEX: 100"
					runat="server" Width="568px" Height="34px" ImageUrl="../librerias/img/titulo_01.jpg"
					Design_Time_Lock="True" Visible="False"></asp:image></td>  
        </tr>
         <tr>
        <td><table border="0" cellpadding="0" >
                  <tr>
                     <td width="33px"><asp:imagebutton id="barRun" onmouseout="this.src='../librerias/img/page_run.png'"
                     onmouseover="this.src='../librerias/img/page_run_over.png'" style="Z-INDEX: 110;"
					runat="server"   Design_Time_Lock="True" ImageUrl="../librerias/img/page_run.png" ToolTip="Ejecutar" TabIndex="9"></asp:imagebutton></td>
                     <td width="33px"><asp:imagebutton id="barDel" onmouseout="this.src='../librerias/img/page_del.png'" 
                     onmouseover="this.src='../librerias/img/page_del_over.png'" style="Z-INDEX: 104"
					runat="server"   Design_Time_Lock="True" ImageUrl="../librerias/img/page_del.png" ToolTip="Eliminar" TabIndex="10"></asp:imagebutton></td>
                     <td width="33px"><asp:imagebutton id="barExit" onmouseout="this.src='../librerias/img/page_exit.png'"
                     onmouseover="this.src='../librerias/img/page_exit_over.png'" style="Z-INDEX: 103"
					runat="server"   Design_Time_Lock="True" ImageUrl="../librerias/img/page_exit.png" ToolTip="Salir" TabIndex="11"></asp:imagebutton></td>
                     <td width="33px">
                     
                     <asp:ImageButton ID="barAyuda" runat="server" ToolTip="Ayuda" Design_Time_Lock="True"
                     ImageUrl="../librerias/img/page_help.png" onmouseout="this.src='../librerias/img/page_help.png'"
                    onmouseover="this.src='../librerias/img/page_help_over.png'"
                    Style="z-index: 103" TabIndex="12" /></td>
                  </tr>
            </table>
        </td>               
        </tr>
        </table>
        </div>
        
        <table border="0" cellpadding="0" cellspacing="0">
         <tr>
        <td><div id="ayuda" ms_positioning="GridLayout" style="display: none; left: 0px; overflow: auto;
                width: 680px; position: relative; top: 0px; height: 25px">
                <asp:Label ID="barDescripcion" runat="server" CssClass="dbnTexto" Height="15px" Style="z-index: 103" Width="630px">Permite subir a Suite Electr&#243;nica el archivo del libro de compra venta y de guias de despacho</asp:Label>
           </div>
           
            <asp:ScriptManager ID="ScriptManager1" runat="server">
                <Scripts>
                     <asp:ScriptReference Path="../librerias/js/prototype.js" />
                    <asp:ScriptReference Path="../librerias/js/effects.js" />
                </Scripts>
            </asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
	            <ContentTemplate>
	                <div id="Div2" style="left: 23px; width: 630px; position: relative; top: 0px; height: auto;">
                        <asp:Label ID="lbMensaje" runat="server" CssClass="dbnError" Style="z-index: 101; height: auto;" Width="630px" 
                            enabled="False" onclick="verOcultarError('divEx', 'chkEx', 'chkDespliega'); return false;">
                        </asp:Label>
                    </div>
	                <div id="divEx" style="display: none; background-color: #FFFF80; left: 23px; width: 631px; position: relative; top: 0px; height: auto">
                        <asp:Label ID="lbEx" runat="server" CssClass="dbnError" Style="z-index: 101; height: auto;" Width="630px">
                        </asp:Label>
                        <asp:CheckBox ID="chkEx" runat="server" style="visibility: hidden;"/>
                        <asp:CheckBox ID="chkDespliega" runat="server" style="visibility: hidden;"/>
                    </div>
	            </ContentTemplate>
		    </asp:UpdatePanel></td>  
        </tr>
    </table>    
 
 <div id="Busqueda" style="WIDTH: 680px; POSITION: relative; HEIGHT: 156px; left: 0px; top: 0px;" ms_positioning="GridLayout">
	<table  border="0" cellspacing="1">
      <tr>
      <td><cc1:dbnetweblabel id="lb_Codiramo" style="Z-INDEX: 102"
				runat="server" Width="85px" Height="16px" CssClass="dbnLabel">Codigo Giro</cc1:dbnetweblabel></td>
      <td><cc1:DbnetWebTextBox id="Codi_ramo" style="Z-INDEX: 107"
				runat="server" Width="98px" Height="19px" MaxLength="12" tabIndex="1"></cc1:DbnetWebTextBox></td>
      </tr>
      <tr>
      <td><cc1:dbnetweblabel id="lbNombramo" style="Z-INDEX: 108"
				runat="server" Height="16px" Width="85px" CssClass="dbnLabel">Nombre Giro</cc1:dbnetweblabel></td>
      <td><cc1:DbnetWebTextBox id="Nomb_ramo" style="Z-INDEX: 109"
				tabIndex="3" runat="server" MaxLength="80" Height="19px" Width="254px"></cc1:DbnetWebTextBox></td>
      </tr>
    </table>  	
	</div>
    </form>
	</body>
</html>
