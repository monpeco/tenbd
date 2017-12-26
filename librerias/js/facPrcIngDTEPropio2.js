function Sucursal(oId, iDx) {
    var oDllTipoTraslado = document.getElementById(oId);
    var oValue = oDllTipoTraslado.options[oDllTipoTraslado.selectedIndex].value;
    var oData = JSON.stringify({ sId: oValue });
    jQuery.ajax({
        type: 'POST',
        async: false,
        url: window.location.pathname + '/sSucu',
        data: oData,
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        success: function(r) {
            var oSucursal = JSON.parse(r);
            if (iDx == 1) {
                document.getElementById("hdSucuOrig").value = oSucursal.d.sNombre;
                document.getElementById("txtDirecionOrigen").value = oSucursal.d.sDireccion;
                document.getElementById("txtComunaOrigen").value = oSucursal.d.sComuna;
                document.getElementById("txtCiudadOrigen").value = oSucursal.d.sCiudad;
            }
            else {
                document.getElementById("hdSucuDest").value = oSucursal.d.sNombre;
                document.getElementById("Dire_dest").value = oSucursal.d.sDireccion;
                document.getElementById("Comu_dest").value = oSucursal.d.sComuna;
                document.getElementById("Ciud_dest").value = oSucursal.d.sCiudad;
            }
        },
        error: function(e) {
        }
    });
}
function SucuDestLike(sData, iDx) {
    var oData = JSON.stringify({ sId: sData });
    jQuery.ajax({
        type: 'POST',
        async: false,
        url: window.location.pathname + '/sSucuLike',
        data: oData,
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        success: function(r) {
            var oSucursal = JSON.parse(r);
            if (iDx == 1) {
                jQuery("#ddlSucuOrigen").val(oSucursal.d.sCodigo).attr('selected', 'selected');
                document.getElementById("hdSucuOrig").value = oSucursal.d.sNombre;
                document.getElementById("txtDirecionOrigen").value = oSucursal.d.sDireccion;
                document.getElementById("txtComunaOrigen").value = oSucursal.d.sComuna;
                document.getElementById("txtCiudadOrigen").value = oSucursal.d.sCiudad;
            }
            else {
                jQuery("#ddlSucuDestino").val(oSucursal.d.sCodigo).attr('selected', 'selected');
                document.getElementById("hdSucuDest").value = oSucursal.d.sNombre;
                document.getElementById("Dire_dest").value = oSucursal.d.sDireccion;
                document.getElementById("Comu_dest").value = oSucursal.d.sComuna;
                document.getElementById("Ciud_dest").value = oSucursal.d.sCiudad;
            }
        },
        error: function(e) {
        }
    });
}

function CargaSucu() {
    validaTrasladoInterno();
    var sSucuOrig = document.getElementById("hdSucuOrig").value;
    var sSucuDest = document.getElementById("hdSucuDest").value;
    if (sSucuOrig != "") SucuDestLike(sSucuOrig, 1);
    if (sSucuDest != "") SucuDestLike(sSucuDest, 2);
}
function mostrar(pestana) {
    acepta();
    document.getElementById('div_comision').style.display = 'none';
    document.getElementById('div_detalle').style.display = 'none';
    document.getElementById('div_descuento').style.display = 'none';
    document.getElementById('div_impuesto').style.display = 'none';
    document.getElementById('div_referencia').style.display = 'none';
    document.getElementById('div_datoexportacion').style.display = 'none';
    document.getElementById('div_transporte').style.display = 'none';
    document.getElementById('div_resumen').style.display = 'none';
    document.getElementById('div_tipobulto').style.display = 'none';
    document.getElementById('div_msgschema').style.display = 'none';
    if (pestana != '')
        document.getElementById(pestana).style.display = 'block';
    else
        document.getElementById('div_detalle').style.display = 'block';
}

function agrega(tabla) {
    if (detacont <= detamax) {
        if ((document.getElementById('txtTotalBultos').value == "" || document.getElementById('txtTotalBultos').value == "0") && tabla == "tbl_bultos") {
            alert("Debe ingresar Total Bultos en Datos Exportacion ");
        }
        else {
            var Tbl = document.getElementById(tabla);
            var newRow = Tbl.insertRow(Tbl.rows.length);
            newRow.id = tabla + (Tbl.rows.length - 1);
            newRow.className = "TablaItem";
            newRow.onclick = function() { camps(tabla, this.id); };

            for (x = 0; x < Tbl.rows[0].cells.length - 1; x++)
                var oCell = newRow.insertCell(x);
            var oCell = newRow.insertCell((Tbl.rows[0].cells.length - 1));
            oCell.innerHTML = '<img height="15" border="0" src="../librerias/img/page_del.jpg" onclick=\"javascript:borrar(\'' + tabla + '\',this.id)\" id=\"' + newRow.id + '\"/>';
            detacont++;
        }
    }
    else {
        alert("excedio la cantidad de " + detamax + "lineas permitida ");
    }
}
function camps(tabla, celda) {
    var codi;
    var codi2;
    var Tbl = document.getElementById(tabla).rows[(celda.replace(tabla, ''))];
    if (Tbl != undefined) {
        switch (tabla) {
            case "tbl_detalle":
                if (Tbl.className != "TablaSelectedItem") {
                    acepta();
                    var cmb11 = '<select class="dbnLov" id="optdet11" style="width:140px;">';
                    for (x = 0; x < Arr['tipocodigo'].length; x++) {
                        codi = Tbl.getElementsByTagName('td')[0].innerHTML;
                        if (codi == "")
                            codi = "INT1";
                        cmb11 += '<option value="' + Arr['tipocodigo'][x].Codigo() + '"' + (codi == Arr['tipocodigo'][x].Codigo() ? "selected='selected'" : "") + '>' + Arr['tipocodigo'][x].Nombre() + '</option> \n';
                    }
                    cmb11 += '</select>';

                    var cmb12 = '<select class="dbnLov" id="optdet12" style="width:120px;">';
                    cmb12 += "<option value=\" \"></option>";
                    for (x2 = 0; x2 < Arr['tipocodigo'].length; x2++) {
                        codi2 = Tbl.getElementsByTagName('td')[12].innerHTML;
                        cmb12 += '<option value="' + Arr['tipocodigo'][x2].Codigo() + '"' + (codi2 == Arr['tipocodigo'][x2].Codigo() ? "selected='selected'" : "") + '>' + Arr['tipocodigo'][x2].Nombre() + '</option> \n';
                    }
                    cmb12 += '</select>';
                    var cmb1 = '<select class="dbnLov" id="optdet9" style="width:120px;">';
                    cmb1 += "<option value=\" \">[Sin Impuesto]</option>";
                    for (x = 0; x < Arr['impuesto'].length; x++) {
                        cmb1 += '<option value="' + Arr['impuesto'][x].Codigo() + '"' + (Tbl.getElementsByTagName('td')[7].innerHTML == Arr['impuesto'][x].Codigo() ? "selected='selected'" : "") + '>' + Arr['impuesto'][x].Nombre() + '</option> \n';
                    }
                    cmb1 += '</select>';
                    var cmb2 = '<select class="dbnLov" id="optdet4">';
                    for (x = 0; x < Arr['medida'].length; x++) {
                        cmb2 += '<option value="' + Arr['medida'][x].Codigo() + '"' + (Tbl.getElementsByTagName('td')[5].innerHTML == Arr['medida'][x].Codigo() ? "selected='selected'" : "") + '>' + Arr['medida'][x].Nombre() + '</option> \n';
                    }
                    cmb2 += '</select>';
                    var cmb3 = '<select class="dbnLov" id="optdet7"><option value="%" ' + (Tbl.getElementsByTagName('td')[8].innerHTML == "%" ? "selected='selected'" : "") + '>Porcentaje</option><option value="$" ' + (Tbl.getElementsByTagName('td')[8].innerHTML == "$" ? "selected='selected'" : "") + '>Monto</option></select>';
                    Tbl.getElementsByTagName('td')[0].innerHTML = cmb11;
                    Tbl.getElementsByTagName('td')[1].innerHTML = '<input class="txt" style="width:92%; position:relative;float:left;" id="txtdet0" value="' + Tbl.getElementsByTagName('td')[1].innerHTML + '" />';
                    Tbl.getElementsByTagName('td')[2].innerHTML = '<input maxlength="80" class="txt" style="width:98%; position:relative; float:left;" id="txtdet1" value="' + Tbl.getElementsByTagName('td')[2].innerHTML + '" />';
                    Tbl.getElementsByTagName('td')[3].innerHTML = '<input maxlength="1000" class="txt" style="width:98%; position:relative; float:left;" id="txtdet2" value="' + Tbl.getElementsByTagName('td')[3].innerHTML + '" />';
                    //calculo de precio unitario agregar hasta 6 decimales
                    Tbl.getElementsByTagName('td')[4].innerHTML = '<input maxlength="18" class="txt" style="width:92%;position:relative;float:left;" id="txtdet3" value="' + Tbl.getElementsByTagName('td')[4].innerHTML + '" onblur="multiplicar(\'' + tabla + '\',\'' + celda + '\',\'txtdet3\',\'txtdet5\',\'optdet7\',\'txtdet8\',\'txtdet6\')"/>';
                    Tbl.getElementsByTagName('td')[5].innerHTML = cmb2;
                    //calculo del total
                    var tipoDocu = document.getElementById('lvTipo_Docu').value;
                    Tbl.getElementsByTagName('td')[6].innerHTML = '<input class="txt" style="width:80px;position:relative;float:left;" id="txtdet5" value="' + Tbl.getElementsByTagName('td')[6].innerHTML + '" onblur="multiplicar(\'' + tabla + '\',\'' + celda + '\',\'txtdet3\',\'txtdet5\',\'optdet7\',\'txtdet8\',\'txtdet6\',' + tipoDocu + ')"/>';
                    //calculo de precio unitario agregar hasta 6 decimales
                    Tbl.getElementsByTagName('td')[7].innerHTML = '<input maxlength="18" class="txt" style="width:80px;position:relative;float:left;" id="txtdet6" value="' + Tbl.getElementsByTagName('td')[7].innerHTML + '" onblur="dividir(\'' + tabla + '\',\'' + celda + '\',\'txtdet6\',\'txtdet3\',\'txtdet5\')"/>';
                    Tbl.getElementsByTagName('td')[8].innerHTML = cmb3;
                    Tbl.getElementsByTagName('td')[9].innerHTML = '<input class="txt" style="width:92%;position:relative;float:left;" id="txtdet8" value="' + Tbl.getElementsByTagName('td')[9].innerHTML + '" onblur="multiplicar(\'' + tabla + '\',\'' + celda + '\',\'txtdet3\',\'txtdet5\',\'optdet7\',\'txtdet8\',\'txtdet6\')"/>';
                    Tbl.getElementsByTagName('td')[10].innerHTML = cmb1;
                    Tbl.getElementsByTagName('td')[11].innerHTML = '<input class="txt" id="chkdet10" type="checkbox" ' + (Tbl.getElementsByTagName('td')[11].innerHTML == "S" ? "checked" : "") + '>';
                    Tbl.getElementsByTagName('td')[12].innerHTML = cmb12;
                    Tbl.getElementsByTagName('td')[13].innerHTML = '<input class="txt" style="width:92%;position:relative;float:left;" id="txtdet13" value="' + Tbl.getElementsByTagName('td')[13].innerHTML + '" />';
                    var combo = document.getElementById('lvTipo_Docu');
                    if (combo.options[combo.selectedIndex].value == '43') {
                        Tbl.getElementsByTagName('td')[14].innerHTML = '<input class="txt" style="width:92%;position:relative;float:left;" maxlength=\'3\' id="txtdet14" value="' + Tbl.getElementsByTagName('td')[14].innerHTML + '" />';
                    }
                    Tbl.className = "TablaSelectedItem";
                }
                break;
            case "tbl_descuento":
                if (Tbl.className != "TablaSelectedItem") {
                    acepta();
                    var cmb1 = '<select class="dbnLov" id="optdes0"><option value="R" ' + (Tbl.getElementsByTagName('td')[0].innerHTML == "D" ? "selected='selected'" : "") + '>Recargo</option><option value="D" ' + (Tbl.getElementsByTagName('td')[0].innerHTML == "D" ? "selected='selected'" : "") + '>Descuento</option></select>';
                    var cmb2 = '<select class="dbnLov" id="optdes2"><option value="%" ' + (Tbl.getElementsByTagName('td')[2].innerHTML == "%" ? "selected='selected'" : "") + '>Porcentaje</option><option value="$" ' + (Tbl.getElementsByTagName('td')[2].innerHTML == "$" ? "selected='selected'" : "") + '>Monto</option></select>';
                    Tbl.getElementsByTagName('td')[0].innerHTML = cmb1;
                    Tbl.getElementsByTagName('td')[1].innerHTML = '<input class="txt" style="width:92%; position:relative; float:left;" id="txtdes1" value="' + Tbl.getElementsByTagName('td')[1].innerHTML + '" />';
                    Tbl.getElementsByTagName('td')[2].innerHTML = cmb2;
                    Tbl.getElementsByTagName('td')[3].innerHTML = '<input class="txt" style="width:92%; position:relative; float:left;" id="txtdes3" value="' + Tbl.getElementsByTagName('td')[3].innerHTML + '" onblur="descuentos(\'' + tabla + '\',\'' + celda + '\',\'optdes0\',\'optdes2\',4)"/>';
                    //4 calculo
                    Tbl.getElementsByTagName('td')[5].innerHTML = '<input class="txt" id="chkdes5" type="checkbox" ' + (Tbl.getElementsByTagName('td')[5].innerHTML == "S" ? "checked" : "") + '>';
                    Tbl.className = "TablaSelectedItem";
                }
                break;
            case "tbl_impuesto":
                if (Tbl.className != "TablaSelectedItem") {
                    acepta();
                    var sSelectTipo = (Tbl.getElementsByTagName('td')[0].innerHTML == "") ? "I" : Tbl.getElementsByTagName('td')[0].innerHTML;
                    var cmb1 = '<select class="dbnLov" id="optimp0" onchange="tipo_impuesto()"><option value="I" ' + (sSelectTipo == "I" ? "selected='selected'" : "") + '>Impuesto</option><option value="R" ' + (sSelectTipo == "R" ? "selected='selected'" : "") + '>Retencion</option></select>';
                    Tbl.getElementsByTagName('td')[0].innerHTML = cmb1;
                    var cmb2 = '<select class="dbnLov" id="optimp1" onchange="validaIva()">';
                    for (x = 0; x < Arr['impuesto'].length; x++) {
                        if (Arr['impuesto'][x].Tipo() == sSelectTipo)
                            cmb2 += '<option value="' + Arr['impuesto'][x].Codigo() + '" ' + (Tbl.getElementsByTagName('td')[1].innerHTML == Arr['impuesto'][x].Codigo() ? "selected='selected'" : "") + '>' + Arr['impuesto'][x].Nombre() + '</option>\n';
                    }
                    cmb2 += '</select>';
                    Tbl.getElementsByTagName('td')[1].innerHTML = cmb2;
                    //tipo_impuesto();
                    Tbl.getElementsByTagName('td')[2].innerHTML = '<input class="txt" style="width:92%; position:relative; float:left;" id="txtimp2" value="' + Tbl.getElementsByTagName('td')[2].innerHTML + '" onblur="impuesto(\'' + tabla + '\',\'' + celda + '\',\'optimp0\',3)"/>';
                    Tbl.getElementsByTagName('td')[3].innerHTML = '<input class="txt" style="width:92%; position:relative; float:left;" id="txtimp3" value="' + Tbl.getElementsByTagName('td')[3].innerHTML + '" onchange="validaIvaChange();"/>';
                    //4 calculo
                    Tbl.className = "TablaSelectedItem";
                    validaIva();
                }
                break;
            case "tbl_referencia":
                if (Tbl.className != "TablaSelectedItem") {
                    acepta();
                    var cmb1 = '<select class="dbnLov" id="optref0" onchange="validaSeleccionReferencia(event)">';
                    for (x = 0; x < Arr['documento'].length; x++) {
                        cmb1 += '<option value="' + Arr['documento'][x].Codigo() + '"' + (Tbl.getElementsByTagName('td')[0].innerHTML == Arr['documento'][x].Codigo() ? "selected='selected'" : "") + '>' + Arr['documento'][x].Nombre() + '</option> \n';
                    }
                    cmb1 += '</select>';
                    var cmb2 = '<select class="dbnLov" id="optref3" ' + (tipox_documento() ? "disabled" : "") + '><option value="1" ' + (Tbl.getElementsByTagName('td')[3].innerHTML == "1" ? "selected='selected'" : "") + '>1.- Anula Documentos de Referencia</option><option value="2" ' + (Tbl.getElementsByTagName('td')[3].innerHTML == "2" ? "selected='selected'" : "") + '>2.- Corrige Texto</option><option value="3" ' + (Tbl.getElementsByTagName('td')[3].innerHTML == "3" ? "selected='selected'" : "") + '>3.- Corrige Montos</option></select>';
                    Tbl.getElementsByTagName('td')[0].innerHTML = cmb1;
                    Tbl.getElementsByTagName('td')[1].innerHTML = '<input class="txt" style="width:92%; position:relative; float:left;" id="txtref1" value="' + Tbl.getElementsByTagName('td')[1].innerHTML + '"" />';
                    Tbl.getElementsByTagName('td')[2].innerHTML = '<input class="txt" style="width:92%; position:relative; float:left;" id="txtref2" value="' + Tbl.getElementsByTagName('td')[2].innerHTML + '" onclick="return showCalendar(\'txtref2\', \'%Y-%m-%d\');"/>';
                    Tbl.getElementsByTagName('td')[3].innerHTML = cmb2;
                    Tbl.getElementsByTagName('td')[4].innerHTML = '<input maxlength="90" class="txt" style="width:92%; position:relative; float:left;" id="txtref4" value="' + Tbl.getElementsByTagName('td')[4].innerHTML + '"/>';
                    Tbl.getElementsByTagName('td')[5].innerHTML = '<input class="txt" id="chkref5" type="checkbox" ' + (Tbl.getElementsByTagName('td')[5].innerHTML == "S" ? "checked" : "") + '>';
                    Tbl.className = "TablaSelectedItem";
                }
                break;
            case "tbl_bultos":
                if (Tbl.className != "TablaSelectedItem") {
                    acepta();

                    var cmb1 = '<select class="dbnLov" id="optpobul" onchange="container(\'' + tabla + '\',\'' + celda + '\',\'txtbulto3\',\'txtbulto4\',\'txtbulto5\',\'optpobul\')" >';
                    for (x = 0; x < Arr['tpobultos'].length; x++) {
                        cmb1 += '<option  value="' + Arr['tpobultos'][x].Codigo() + '"' + (Tbl.getElementsByTagName('td')[0].innerHTML == Arr['tpobultos'][x].Codigo() ? "selected='selected'" : "") + '>' + Arr['tpobultos'][x].Nombre() + '</option> \n';
                    }
                    cmb1 += '</select>';
                    Tbl.getElementsByTagName('td')[0].innerHTML = cmb1;
                    Tbl.getElementsByTagName('td')[1].innerHTML = '<input maxlength="10" class="txt" style="width:92%; position:relative; float:left;" id="txtbulto1" value="' + Tbl.getElementsByTagName('td')[1].innerHTML + '"/>';
                    Tbl.getElementsByTagName('td')[2].innerHTML = '<input maxlength="255" class="txt" style="width:92%; position:relative; float:left;" id="txtbulto2" value="' + Tbl.getElementsByTagName('td')[2].innerHTML + '"/>';
                    Tbl.getElementsByTagName('td')[3].innerHTML = '<input maxlength="25" class="txt" style="width:92%; position:relative; float:left;" id="txtbulto3" value="' + Tbl.getElementsByTagName('td')[3].innerHTML + '"/>';
                    Tbl.getElementsByTagName('td')[4].innerHTML = '<input maxlength="20" class="txt" style="width:92%; position:relative; float:left;" id="txtbulto4" value="' + Tbl.getElementsByTagName('td')[4].innerHTML + '"/>';
                    Tbl.getElementsByTagName('td')[5].innerHTML = '<input maxlength="70" class="txt" style="width:92%; position:relative; float:left;" id="txtbulto5" value="' + Tbl.getElementsByTagName('td')[5].innerHTML + '"/>';
                    Tbl.className = "TablaSelectedItem";
                }
                break;
            case "tbl_comision":
                if (Tbl.className != "TablaSelectedItem") {
                    acepta();
                    var sCombo = '<select class="dbnLov" id="optComision1">';
                    if (Tbl.getElementsByTagName('td')[0].innerHTML != "") {
                        if (Tbl.getElementsByTagName('td')[0].innerHTML == "C") {
                            sCombo += '<option value="C" selected="selected">Comisi&#243;n</option>';
                            sCombo += '<option value="O">Cargo</option>';
                        }
                        else {
                            sCombo += '<option value="C">Comisi&#243;n</option>';
                            sCombo += '<option value="O" selected="selected">Cargo</option>';
                        }
                    }
                    else {
                        sCombo += '<option value="C">Comisi&#243;n</option>';
                        sCombo += '<option value="O">Cargo</option>';
                    }
                    sCombo += '</select>';
                    Tbl.getElementsByTagName('td')[0].innerHTML = sCombo;
                    Tbl.getElementsByTagName('td')[1].innerHTML = '<input maxlength="60" class="txt" style="width:92%; position:relative; float:left;" id="txtcomi2" value="' + Tbl.getElementsByTagName('td')[1].innerHTML + '"/>';
                    Tbl.getElementsByTagName('td')[2].innerHTML = '<input maxlength="18" class="txt" style="width:92%; position:relative; float:left;" id="txtcomi3" value="' + Tbl.getElementsByTagName('td')[2].innerHTML + '"/>';
                    Tbl.getElementsByTagName('td')[3].innerHTML = '<input maxlength="18" class="txt" style="width:92%; position:relative; float:left;" id="txtcomi4" value="' + Tbl.getElementsByTagName('td')[3].innerHTML + '"/>';
                    Tbl.getElementsByTagName('td')[4].innerHTML = '<input maxlength="18" class="txt" style="width:92%; position:relative; float:left;" id="txtcomi5" value="' + Tbl.getElementsByTagName('td')[4].innerHTML + '"/>';
                    Tbl.getElementsByTagName('td')[5].innerHTML = '<input maxlength="18" class="txt" style="width:92%; position:relative; float:left;" id="txtcomi6" value="' + Tbl.getElementsByTagName('td')[5].innerHTML + '"/>';
                    Tbl.className = "TablaSelectedItem";
                }
                break;
        }
    }
}

function multiplicar(tabla, celda, item1, item2, item3, item4, resultado, tipoD) {
    if (document.getElementById(item4).value == "")
        document.getElementById(item4).value = 0;

    var Tbl = document.getElementById(tabla).rows[celda.replace(tabla, '')];
    //se realiza el calculo cuando el valor neto tiene o no valor
    if (document.getElementById(item2).value != "") {
        //item1 -> cantidad
        if (isNaN(document.getElementById(item1).value.replace(",", ".")))
            document.getElementById(item1).value = 0;
        //item2 -> precio unitario
        if (isNaN(document.getElementById(item2).value.replace(",", ".")))
            document.getElementById(item2).value = 0;
        //item3 -> tipo descuento
        item3 = document.getElementById(item3).value;
        //item4 -> descuento
        if (isNaN(document.getElementById(item4).value.replace(",", ".")))
            document.getElementById(item4).value = 0;

        var dPrecItem = document.getElementById(item2).value.replace(",", ".");
        //se quita el redondeo para el precio del item, dado que en el servicio es válido los decimales en este campo
        //document.getElementById(item2).value = Math.round(dPrecItem * Math.pow(10, 4)) / Math.pow(10, 4);
        if ((tipoD != "110") && (tipoD != "111") && (tipoD != "112")) {
            document.getElementById(item2).value = Math.round(dPrecItem * Math.pow(10, 4)) / Math.pow(10, 4);
        }
        else {
            document.getElementById(item2).value = parseFloat(dPrecItem).toFixed(6);
        }
        var res = document.getElementById(item1).value.replace(",", ".") * document.getElementById(item2).value.replace(",", ".");

        var desc = 0;
        //descuento no puede estar en blaco produce resultado NaN
        if (document.getElementById(item4).value != "")
            var desc = (item3 == "$" ? parseFloat(document.getElementById(item4).value.replace(",", ".")) : parseFloat(((res * document.getElementById(item4).value.replace(",", ".")) * 0.01)));
        //resultado -> monto total
        document.getElementById(resultado).value = res - desc;
        if (tipo_documentoRedon())
            document.getElementById(resultado).value = Math.round(document.getElementById(resultado).value);
        else
            document.getElementById(resultado).value = parseFloat(document.getElementById(resultado).value).toFixed(4); 
    }
}

function dividir(tabla, celda, item1, item2, resultado) {
    if (document.getElementById(resultado).value == "") {
        var Tbl = document.getElementById(tabla).rows[celda.replace(tabla, '')];

        if (isNaN(document.getElementById(item1).value.replace(",", ".")))
            document.getElementById(item1).value = 0;

        if (isNaN(document.getElementById(item2).value.replace(",", ".")))
            document.getElementById(item2).value = 0;

        if (document.getElementById(item2).value != "" && document.getElementById(item1).value != "") {
            var res = document.getElementById(item1).value.replace(",", ".") / document.getElementById(item2).value.replace(",", ".");
            if (celda = "txtdet5") {
                document.getElementById(resultado).value = res;
                var decimales = parseFloat(6);
                decimales = (!decimales ? 2 : decimales);
                document.getElementById(resultado).value = Math.round(document.getElementById(resultado).value * Math.pow(10, decimales)) / Math.pow(10, decimales);
            }
            else {
                document.getElementById(resultado).value = fix(res);
            }
        }
    }
}

function container(tabla, celda, item1, item2, item3, combo) {
    var Tbl = document.getElementById(tabla).rows[celda.replace(tabla, '')];

    if (isNaN(document.getElementById(item1).value.replace(",", ".")))
        document.getElementById(item1).value = 0;
    if (isNaN(document.getElementById(item2).value.replace(",", ".")))
        document.getElementById(item2).value = 0;
    if (isNaN(document.getElementById(item3).value.replace(",", ".")))
        document.getElementById(item3).value = 0;
    if (document.getElementById(combo).value == "75" || document.getElementById(combo).value == "78") {
        document.getElementById(item1).disabled = false;
        document.getElementById(item2).disabled = false;
        document.getElementById(item3).disabled = false;
    }
    else {
        document.getElementById(item1).value = "";
        document.getElementById(item2).value = "";
        document.getElementById(item3).value = "";
        document.getElementById(item1).disabled = true;
        document.getElementById(item2).disabled = true;
        document.getElementById(item3).disabled = true;
    }
}

function impuesto(tabla, celda, item1, resultado) {
    var Tbl = document.getElementById(tabla).rows[celda.replace(tabla, '')];
    var valor = document.getElementById('txtimp2').value.replace(",", ".");
    item1 = document.getElementById(item1).value;
    fnimpuesto(tabla, celda.replace(tabla, ''), item1, resultado, valor);
}

function fix(num) {
    num2 = new Number(num);
    cant = (document.getElementById('chkDecimal').checked ? 2 : 0);
    num2 = num2.toFixed(cant);
    return num2;
}

function fnimpuesto(tabla, celda, item1, resultado, valor) {
    var Tbl = document.getElementById(tabla).rows[celda];
    var ST = document.getElementById('lblDetalleST').innerHTML;
    var resulta = 0;
    if (item1 == "I")
        if (tipo_documentoRedon())
        resulta = Math.round(resulta);
    else
        resulta = fix(resulta);
    if (document.getElementById('txtimp3') != null) {
        if (document.getElementById('txtimp3').value == "0")
            document.getElementById('txtimp3').value = resulta;
    }
    else {
        if (Tbl.getElementsByTagName('td')[resultado].innerHTML == 0 || Tbl.getElementsByTagName('td')[resultado].innerHTML == "")
            Tbl.getElementsByTagName('td')[resultado].innerHTML = resulta;
    }
}
function tipo_impuesto() {
    var cmb = document.getElementById('optimp0');
    document.getElementById('optimp1').options.length = 0;
    var cmb2 = document.getElementById('optimp1');
    for (x = 0; x < Arr['impuesto'].length; x++) {
        if (Arr['impuesto'][x].Tipo() == cmb.value) {
            cmb2.options[cmb2.options.length] = new Option(Arr['impuesto'][x].Nombre(), Arr['impuesto'][x].Codigo());
        }
    }
    document.getElementById('txtimp3').disabled = false;
}

function descuentos(tabla, celda, item1, item2, resultado) {
    var Tbl = document.getElementById(tabla).rows[celda.replace(tabla, '')];
    var valor = document.getElementById('txtdes3').value; //Valor del descuento global
    var ST = document.getElementById('lblDetalleST').innerHTML;
    item1 = document.getElementById(item1).value;
    item2 = document.getElementById(item2).value;
    tipo = document.getElementById('chkdes5').checked ? "S" : "N";
    fndescuento(tabla, celda.replace(tabla, ''), item1, item2, resultado, valor, tipo);
}

function fndescuento(tabla, celda, item1, item2, resultado, valor, tipo) {
    var Tbl = document.getElementById(tabla).rows[celda];
    var deta_noexento = 0;
    var deta_exento = 0;
    var val1;
    var val2;
    var val3;
    var Tblaux = document.getElementById('tbl_detalle');
    for (xx = 1; xx < Tblaux.rows.length; xx++) {
        var Tbla = document.getElementById(Tblaux.rows[xx].id);
        if (Tbla.getElementsByTagName('td')[11].innerHTML == "N") {
            deta_noexento = parseFloat(deta_noexento) + parseFloat(Tbla.getElementsByTagName('td')[7].innerHTML);
            if (Tbla.getElementsByTagName('td')[9].innerHTML != "" && Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "%") {
                deta_noexento = parseFloat(deta_noexento);
                val1 = (parseFloat(Tbla.getElementsByTagName('td')[7].innerHTML) * (parseFloat(Tbla.getElementsByTagName('td')[9].innerHTML.replace(",", ".") * 0.01)));
            }
            if (Tbla.getElementsByTagName('td')[9].innerHTML != "" && Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "$") {
                deta_noexento = parseFloat(deta_noexento);
                val2 = parseFloat(Tbla.getElementsByTagName('td')[9].innerHTML.replace(",", "."));
            }
        }
        else {
            deta_exento = parseFloat(deta_exento) + parseFloat(Tbla.getElementsByTagName('td')[7].innerHTML);
            if (Tbla.getElementsByTagName('td')[9].innerHTML != "" || Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "%") {
                deta_exento = parseFloat(deta_exento);
            }
            if (Tbla.getElementsByTagName('td')[9].innerHTML != "" || Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "$") {
                deta_exento = parseFloat(deta_exento);
            }
        }
    }
    var ST = (tipo == "S" ? deta_exento : deta_noexento);
    var resulta = 0;
    if (item1 == "R") {
        if (item2 == "$") {
            valor = valor.replace(",", ".");
            resulta = Math.round(valor) * 1;
        }
        else {
            valor = valor.replace(",", ".");
            resulta = (ST * valor) * 0.01;
        }
    }
    else {
        if (item2 == "$") {
            valor = valor.replace(",", ".");
            resulta = Math.round(valor) * 1;
        }
        else {
            if (valor > 100) {
                return 0;
            }
            else {
                valor = valor.replace(",", ".");
                resulta = ((ST * valor) * 0.01);
            }
        }
    }
    if (tipo_documentoRedon()) {
        Tbl.getElementsByTagName('td')[resultado].innerHTML = resulta;
    }
    else {
        Tbl.getElementsByTagName('td')[resultado].innerHTML = fix(resulta);
    }
}
function borrar(tabla, xfila) {
    acepta();
    var tblEl = document.getElementById(tabla);
    for (x = 1; x < tblEl.rows.length; x++) {
        if (tblEl.rows[x].id == xfila) {
            tblEl.deleteRow(x);
            break;
        }
    }
    for (x = 1; x < tblEl.rows.length; x++) {
        tblEl.rows[x].cells[tblEl.rows[x].cells.length - 1].getElementsByTagName('img')[0].id = tabla + x;
        tblEl.rows[x].id = tabla + x;
    }
}

function refe_tipo() {
    var txt = document.getElementById('optref0').value;
    if (parseInt(txt) == 61 || parseInt(txt) == 56 || parseInt(txt) == 111 || parseInt(txt) == 112 || parseInt(txt) == 30 || parseInt(txt) == 60)
        document.getElementById('optref3').disabled = false;
    else
        document.getElementById('optref3').disabled = true;
}
function aceptanew() {
    if (document.getElementById('DetaPuni').value != "") {
        agrega('tbl_detalle');
        var Tbl = document.getElementById('tbl_detalle');
        var Tbla = document.getElementById(Tbl.rows[Tbl.rows.length - 1].id);
        Tbla.getElementsByTagName('td')[1].innerHTML = document.getElementById('DetaCodi').value;
        Tbla.getElementsByTagName('td')[2].innerHTML = document.getElementById('DetaNombre').value;
        Tbla.getElementsByTagName('td')[6].innerHTML = document.getElementById('DetaPuni').value;
        Tbla.className = "TablaItem";
        $("#DetaCodi").value('');
        $("#DetaNombre").value('');
        $("#DetaPuni").value('');
    }
    else
        alert("Antes de agregar debe obtener el Valor ");
}
function acepta() {
    //Detalle
    var Tbl = document.getElementById('tbl_detalle');
    var deta_noexento = 0;
    var deta_exento = 0;
    var desc_noexento = 0;
    var desc_exento = 0;
    for (x = 1; x < Tbl.rows.length; x++) {
        if (Tbl.rows[x].className == "TablaSelectedItem") {
            var Tbla = document.getElementById(Tbl.rows[x].id);
            Tbla.getElementsByTagName('td')[0].innerHTML = document.getElementById('optdet11').value;
            Tbla.getElementsByTagName('td')[1].innerHTML = document.getElementById('txtdet0').value;
            Tbla.getElementsByTagName('td')[2].innerHTML = document.getElementById('txtdet1').value;
            Tbla.getElementsByTagName('td')[3].innerHTML = document.getElementById('txtdet2').value;
            Tbla.getElementsByTagName('td')[4].innerHTML = document.getElementById('txtdet3').value;
            Tbla.getElementsByTagName('td')[5].innerHTML = document.getElementById('optdet4').value;
            Tbla.getElementsByTagName('td')[6].innerHTML = document.getElementById('txtdet5').value;
            Tbla.getElementsByTagName('td')[7].innerHTML = document.getElementById('txtdet6').value; //Total Item
            Tbla.getElementsByTagName('td')[8].innerHTML = document.getElementById('optdet7').value; //Tipo descuento
            Tbla.getElementsByTagName('td')[9].innerHTML = document.getElementById('txtdet8').value; //Valor Descuento
            Tbla.getElementsByTagName('td')[10].innerHTML = document.getElementById('optdet9').value;
            Tbla.getElementsByTagName('td')[11].innerHTML = (document.getElementById('chkdet10').checked ? "S" : "N");
            Tbla.getElementsByTagName('td')[12].innerHTML = document.getElementById('optdet12').value;
            Tbla.getElementsByTagName('td')[13].innerHTML = document.getElementById('txtdet13').value;
            var combo = document.getElementById('lvTipo_Docu');
            if (combo.options[combo.selectedIndex].value == '43') {
                Tbla.getElementsByTagName('td')[14].innerHTML = document.getElementById('txtdet14').value;
            }
            Tbla.className = "TablaItem";
            break;
        }
    }

    var ST = document.getElementById('lblDetalleST')
    ST.innerHTML = "0"; //Variable de Subtotal

    for (x = 1; x < Tbl.rows.length; x++) {
        var Tbla = document.getElementById(Tbl.rows[x].id);
        if (Tbla.getElementsByTagName('td')[7].innerHTML != "")
        //Suma el total de cada Item /
            ST.innerHTML = parseFloat(ST.innerHTML) + parseFloat(Tbla.getElementsByTagName('td')[7].innerHTML);
        //Resta el descuento de cada item con %
        if (Tbla.getElementsByTagName('td')[9].innerHTML != "" && Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "%") {
            var totalItem = Tbla.getElementsByTagName('td')[7].innerHTML;
            var totalPorc = Tbla.getElementsByTagName('td')[9].innerHTML;
            ST.innerHTML = parseFloat(ST.innerHTML);
        }
        //resta el descuento de cada item con $
        if (Tbla.getElementsByTagName('td')[9].innerHTML != "" && Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "$") {
            var totalItem = Tbla.getElementsByTagName('td')[7].innerHTML;
            var totalvalor = Tbla.getElementsByTagName('td')[9].innerHTML;
            ST.innerHTML = parseFloat(ST.innerHTML);
        }
        if (tipo_documentoRedon())
            ST.innerHTML = Math.round(ST.innerHTML);
        else
            ST.innerHTML = fix(ST.innerHTML);

        if (Tbla.getElementsByTagName('td')[11].innerHTML != "S") {
            deta_noexento = parseFloat(deta_noexento) + parseFloat(Tbla.getElementsByTagName('td')[7].innerHTML);
            if (Tbla.getElementsByTagName('td')[9].innerHTML != "" && Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "%") {
                var totalItem = Tbla.getElementsByTagName('td')[7].innerHTML;
                var totalPorc = Tbla.getElementsByTagName('td')[9].innerHTML;
                deta_noexento = parseFloat(deta_noexento);
            }
            if (Tbla.getElementsByTagName('td')[9].innerHTML != "" && Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "$") {
                var totalItem = Tbla.getElementsByTagName('td')[7].innerHTML;
                var totalvalor = Tbla.getElementsByTagName('td')[9].innerHTML;
                deta_noexento = parseFloat(deta_noexento);
            }
        }
        else {
            deta_exento = parseFloat(deta_exento) + parseFloat(Tbla.getElementsByTagName('td')[7].innerHTML);
            if (Tbla.getElementsByTagName('td')[9].innerHTML != "" && Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "%") {
                var totalItem = Tbla.getElementsByTagName('td')[7].innerHTML;
                var totalPorc = Tbla.getElementsByTagName('td')[9].innerHTML;
                deta_exento = parseFloat(deta_exento);
            }
            if (Tbla.getElementsByTagName('td')[9].innerHTML != "" && Tbla.getElementsByTagName('td')[9].innerHTML != 0 && Tbla.getElementsByTagName('td')[8].innerHTML == "$") {
                var totalItem = Tbla.getElementsByTagName('td')[7].innerHTML;
                var totalvalor = Tbla.getElementsByTagName('td')[9].innerHTML;
                deta_exento = parseFloat(deta_exento);
            }
        }

        if (tipo_documentoRedon()) {
            deta_noexento = Math.round(deta_noexento);
            deta_exento = Math.round(deta_exento);
        }
        else {
            deta_noexento = fix(deta_noexento);
            deta_exento = fix(deta_exento);
        }
    }
    document.getElementById('lblcantdeta').innerHTML = Tbl.rows.length - 1;
    document.getElementById('lblsubdeta').innerHTML = ST.innerHTML;

    //Descuento y Recargos Globales
    var Tbl = document.getElementById('tbl_descuento');

    for (x = 1; x < Tbl.rows.length; x++) {
        if (Tbl.rows[x].className == "TablaSelectedItem") {
            var Tbla = document.getElementById(Tbl.rows[x].id);
            Tbla.getElementsByTagName('td')[0].innerHTML = document.getElementById('optdes0').value;
            Tbla.getElementsByTagName('td')[1].innerHTML = document.getElementById('txtdes1').value;
            Tbla.getElementsByTagName('td')[2].innerHTML = document.getElementById('optdes2').value;
            Tbla.getElementsByTagName('td')[3].innerHTML = document.getElementById('txtdes3').value;
            Tbla.getElementsByTagName('td')[5].innerHTML = (document.getElementById('chkdes5').checked ? "S" : "N");
            Tbla.className = "TablaItem";
            break;
        }
    }
    var ST = document.getElementById('lblDescuentoST')
    ST.innerHTML = "0";
    for (x = 1; x < Tbl.rows.length; x++) {
        var Tbla = document.getElementById(Tbl.rows[x].id);
        fndescuento('tbl_descuento', x, Tbla.getElementsByTagName('td')[0].innerHTML,
                                    Tbla.getElementsByTagName('td')[2].innerHTML, 4,
                                    (Tbla.getElementsByTagName('td')[2].innerHTML == "%" ? Tbla.getElementsByTagName('td')[3].innerHTML : Tbla.getElementsByTagName('td')[4].innerHTML),
                                    Tbla.getElementsByTagName('td')[5].innerHTML);
        if (Tbla.getElementsByTagName('td')[4].innerHTML != "") //comprueba que exista valor en el campo total de cada item de descuentos
            if (Tbla.getElementsByTagName('td')[0].innerHTML == "R")
            ST.innerHTML = parseFloat(ST.innerHTML) + parseFloat(Tbla.getElementsByTagName('td')[4].innerHTML); //suma para recargo
        else
            ST.innerHTML = parseFloat(ST.innerHTML) - parseFloat(Tbla.getElementsByTagName('td')[4].innerHTML); //resta para descuento


        if (tipo_documentoRedon())
            ST.innerHTML = Math.round(ST.innerHTML);
        else
            ST.innerHTML = fix(ST.innerHTML);

        if (Tbla.getElementsByTagName('td')[5].innerHTML != "S") //si es distinto de exento
            if (Tbla.getElementsByTagName('td')[0].innerHTML == "R") //si es un recargo
            desc_noexento = parseFloat(desc_noexento) + parseFloat(Tbla.getElementsByTagName('td')[4].innerHTML); //suma para recargo
        else
            desc_noexento = parseFloat(desc_noexento) - parseFloat(Tbla.getElementsByTagName('td')[4].innerHTML); //resta para descuento
        else
            if (Tbla.getElementsByTagName('td')[0].innerHTML == "R")
            desc_exento = parseFloat(desc_exento) + parseFloat(Tbla.getElementsByTagName('td')[4].innerHTML); //suma para recargo
        else
            desc_exento = parseFloat(desc_exento) - parseFloat(Tbla.getElementsByTagName('td')[4].innerHTML); //resta para descuento


        if (tipo_documentoRedon()) {
            desc_noexento = Math.round(desc_noexento);
            desc_exento = Math.round(desc_exento);
        } else {
            desc_noexento = fix(desc_noexento);
            desc_exento = fix(desc_exento);
        }
    }
    document.getElementById('lblcantdesc').innerHTML = Tbl.rows.length - 1;
    document.getElementById('lblsubdesc').innerHTML = ST.innerHTML;  //sub-total de descuento
    var oIvaRetencion = false;
    //Impuestos y Retenciones Globales
    var Tbl = document.getElementById('tbl_impuesto');
    if (validaIvaChange() == 1) {
        for (x = 1; x < Tbl.rows.length; x++) {
            if (Tbl.rows[x].className == "TablaSelectedItem") {
                var Tbla = document.getElementById(Tbl.rows[x].id);
                Tbla.getElementsByTagName('td')[0].innerHTML = document.getElementById('optimp0').value;
                Tbla.getElementsByTagName('td')[1].innerHTML = document.getElementById('optimp1').value;
                if (Tbla.getElementsByTagName('td')[1].innerHTML == "32")
                    oIvaRetencion = true;
                Tbla.getElementsByTagName('td')[2].innerHTML = document.getElementById('txtimp2').value;
                Tbla.getElementsByTagName('td')[3].innerHTML = document.getElementById('txtimp3').value;
                Tbla.className = "TablaItem";
                break;
            }
        }
        var ST = document.getElementById('lblImpuestoST')
        ST.innerHTML = "0";
        for (x = 1; x < Tbl.rows.length; x++) {
            var Tbla = document.getElementById(Tbl.rows[x].id);
            fnimpuesto('tbl_impuesto', x, Tbla.getElementsByTagName('td')[0].innerHTML, 3, Tbla.getElementsByTagName('td')[2].innerHTML)

            if (Tbla.getElementsByTagName('td')[3].innerHTML != "")

                if (Tbla.getElementsByTagName('td')[0].innerHTML == "I")
                ST.innerHTML = parseFloat(ST.innerHTML) + parseFloat(Tbla.getElementsByTagName('td')[3].innerHTML);
            else
                ST.innerHTML = parseFloat(ST.innerHTML) - parseFloat(Tbla.getElementsByTagName('td')[3].innerHTML);

            if (tipo_documentoRedon())
                ST.innerHTML = Math.round(ST.innerHTML);
            else
                ST.innerHTML = fix(ST.innerHTML);
        }
        document.getElementById('lblcantimpu').innerHTML = Tbl.rows.length - 1;
        document.getElementById('lblsubimpu').innerHTML = ST.innerHTML;

        if (oIvaRetencion) {
            var iLIva = parseInt(document.getElementById('lblIVA').innerHTML);
            var iSTIva = parseInt(document.getElementById('lblImpuestoST').innerHTML);
            if (iSTIva > iLIva) {
                alert("No se puede ingresar un Iva Retenido mayor al Iva del Documento.");
            }
        }
    }

    //Referencias
    var Tbl = document.getElementById('tbl_referencia');
    for (x = 1; x < Tbl.rows.length; x++) {
        if (Tbl.rows[x].className == "TablaSelectedItem") {
            var Tbla = document.getElementById(Tbl.rows[x].id);
            Tbla.getElementsByTagName('td')[0].innerHTML = document.getElementById('optref0').value;
            Tbla.getElementsByTagName('td')[1].innerHTML = document.getElementById('txtref1').value;
            Tbla.getElementsByTagName('td')[2].innerHTML = document.getElementById('txtref2').value;
            Tbla.getElementsByTagName('td')[3].innerHTML = (document.getElementById('optref3').disabled ? "" : document.getElementById('optref3').value);
            Tbla.getElementsByTagName('td')[4].innerHTML = document.getElementById('txtref4').value;
            Tbla.getElementsByTagName('td')[5].innerHTML = (document.getElementById('chkref5').checked ? "S" : "N");
            Tbla.className = "TablaItem";
            break;
        }
    }

    //Bultos
    var Tbl = document.getElementById('tbl_bultos');
    for (x = 1; x < Tbl.rows.length; x++) {
        if (Tbl.rows[x].className == "TablaSelectedItem") {
            var Tbla = document.getElementById(Tbl.rows[x].id);
            Tbla.getElementsByTagName('td')[0].innerHTML = document.getElementById('optpobul').value;
            Tbla.getElementsByTagName('td')[1].innerHTML = document.getElementById('txtbulto1').value;
            Tbla.getElementsByTagName('td')[2].innerHTML = document.getElementById('txtbulto2').value;
            Tbla.getElementsByTagName('td')[3].innerHTML = document.getElementById('txtbulto3').value;
            Tbla.getElementsByTagName('td')[4].innerHTML = document.getElementById('txtbulto4').value;
            Tbla.getElementsByTagName('td')[5].innerHTML = document.getElementById('txtbulto5').value;
            Tbla.className = "TablaItem";
            break;
        }
    }

    document.getElementById('lblcantrefe').innerHTML = Tbl.rows.length - 1;
    document.getElementById('lblTotalAfecto').innerHTML = Math.round(deta_noexento + desc_noexento);
    document.getElementById('lblTotalExento').innerHTML = Math.round(deta_exento + desc_exento);

    if (tipo_documento() == false) {
        document.getElementById('lblTotalAfecto').innerHTML = "0";
        if (tipo_documentoRedon())
            document.getElementById('lblTotalExento').innerHTML = Math.round(parseFloat(deta_noexento) + parseFloat(desc_noexento) + parseFloat(deta_exento) + parseFloat(desc_exento));
        else
            document.getElementById('lblTotalExento').innerHTML = fix(parseFloat(deta_noexento) + parseFloat(desc_noexento) + parseFloat(deta_exento) + parseFloat(desc_exento));
    }

    document.getElementById('lblIVA').innerHTML = parseFloat(document.getElementById('lblTotalAfecto').innerHTML) * 0.19;
    var total = parseFloat(document.getElementById('lblIVA').innerHTML) +
                parseFloat(document.getElementById('lblTotalAfecto').innerHTML) +
                parseFloat(document.getElementById('lblTotalExento').innerHTML) +
                parseFloat(document.getElementById('lblsubimpu').innerHTML) -
                parseFloat(document.getElementById('lblTotalNetoAsig').innerHTML) -
                parseFloat(document.getElementById('lblTotalExentoAsig').innerHTML) -
                parseFloat(document.getElementById('lblTotalIvaAsig').innerHTML);
    //by AROJAS
    var iCredec = document.getElementById('txtCredec').value == "" ? 0 : parseFloat(document.getElementById('txtCredec').value);
    var tTotal = (total - iCredec);
    document.getElementById('lblTotal').innerHTML = tTotal;
    //MntMargenCom
    var iMontMntMargenCom = document.getElementById('txtMntMargenCom').value = "" ? 0 : parseFloat(document.getElementById('txtMntMargenCom').value);
    if (iMontMntMargenCom > 0) {
        document.getElementById('lblResuMargenCom').style.removeProperty('display');
        document.getElementById('lblMargenCom').style.removeProperty('display');
        document.getElementById('lblResuMargenCom').innerHTML = Math.round(iMontMntMargenCom);
    }
    else {
        document.getElementById('lblResuMargenCom').style.display = 'none';
        document.getElementById('lblMargenCom').style.display = 'none';
    }
    if (tipo_documentoRedon()) {
        document.getElementById('lblIVA').innerHTML = Math.round(document.getElementById('lblIVA').innerHTML);
        document.getElementById('lblTotal').innerHTML = Math.round(document.getElementById('lblTotal').innerHTML);
    }
    else {
        document.getElementById('lblIVA').innerHTML = fix(document.getElementById('lblIVA').innerHTML);
        document.getElementById('lblTotal').innerHTML = fix(document.getElementById('lblTotal').innerHTML);
    }

/*calculo totales de exportacion*/
var _noaf_otmo = 0;
var _mont_otmo = 0;
var temp= document.getElementById('tipo_camb').value.replace(",",".");

var _tipo_camb = parseFloat(temp);

var _lblTotalExento = document.getElementById('lblTotalExento').innerHTML.replace(",",".");
var _lblTotal= document.getElementById('lblTotal').innerHTML.replace(",",".");

if ( _tipo_camb > 0.0)
{
//_noaf_otmo = parseFloat(document.getElementById('lblTotalExento').innerHTML) * parseFloat(document.getElementById('tipo_camb').value);
_noaf_otmo = parseFloat(_lblTotalExento) * _tipo_camb;
_mont_otmo = parseFloat(_lblTotal) * _tipo_camb;
}

document.getElementById('txtTotales').value = document.getElementById('lblTotalAfecto').innerHTML + "|" + document.getElementById('lblTotalExento').innerHTML + "|" + document.getElementById('lblIVA').innerHTML + "|" + document.getElementById('lblTotal').innerHTML + "|" + _noaf_otmo.toFixed(4) + "|" + _mont_otmo.toFixed(4);
    document.getElementById('lblTotalAfecto').innerHTML = document.getElementById('lblTotalAfecto').innerHTML.replace(".", ",");
    document.getElementById('lblcantrefe').innerHTML = document.getElementById('lblcantrefe').innerHTML.replace(".", ",");
    document.getElementById('lblTotalExento').innerHTML = document.getElementById('lblTotalExento').innerHTML.replace(".", ",");
    document.getElementById('lblTotal').innerHTML = document.getElementById('lblTotal').innerHTML.replace(".", ",");
    document.getElementById('lblIVA').innerHTML = document.getElementById('lblIVA').innerHTML.replace(".", ",");
    ST.innerHTML = ST.innerHTML.replace(".", ",");

    //Comisiones o Cargos
    var Tbl = document.getElementById('tbl_comision');
    var nValor1 = 0;
    var nValor2 = 0;
    var nValor3 = 0;
    var nTotalNeto = 0;
    var nTotalExento = 0;
    var nTotalIva = 0;
    for (x = 1; x < Tbl.rows.length; x++) {
        if (Tbl.rows[x].className == "TablaSelectedItem") {
            var Tbla = document.getElementById(Tbl.rows[x].id);
            Tbla.getElementsByTagName('td')[0].innerHTML = document.getElementById('optComision1').value;
            Tbla.getElementsByTagName('td')[1].innerHTML = document.getElementById('txtcomi2').value;
            Tbla.getElementsByTagName('td')[2].innerHTML = document.getElementById('txtcomi3').value;
            nValor1 = document.getElementById('txtcomi4').value;
            Tbla.getElementsByTagName('td')[3].innerHTML = nValor1;
            nValor2 = document.getElementById('txtcomi5').value;
            Tbla.getElementsByTagName('td')[4].innerHTML = nValor2;
            nValor3 = document.getElementById('txtcomi6').value;
            Tbla.getElementsByTagName('td')[5].innerHTML = nValor3;

            Tbla.className = "TablaItem";
            break;
        }
    }
    for (x = 1; x < Tbl.rows.length; x++) {
        nValor1 = Tbl.rows[x].cells[3].childNodes[0];
        if (nValor1 != undefined) {
            nTotalNeto = (parseFloat(nValor1.data) + parseFloat(nTotalNeto));
            document.getElementById('lblTotalNetoAsig').innerHTML = nTotalNeto;
        }

        nValor2 = Tbl.rows[x].cells[4].childNodes[0];
        if (nValor2 != undefined) {
            nTotalExento = (parseFloat(nValor2.data) + parseFloat(nTotalExento));
            document.getElementById('lblTotalExentoAsig').innerHTML = nTotalExento;
        }

        nValor3 = Tbl.rows[x].cells[5].childNodes[0];
        if (nValor3 != undefined) {
            nTotalIva = (parseFloat(nValor3.data) + parseFloat(nTotalIva));
            document.getElementById('lblTotalIvaAsig').innerHTML = nTotalIva;
        }
    }
}

function toaspcomi() {
    var txt = document.getElementById('txtAuxComiNEI');
    var nNeto = document.getElementById('lblTotalNetoAsig').innerHTML;
    var nExento = document.getElementById('lblTotalExentoAsig').innerHTML;
    var nIva = document.getElementById('lblTotalIvaAsig').innerHTML;
    txt.value = nNeto + "|" + nExento + "|" + nIva;
}

function toaspf(tabla, campo) {
    var Tbl = document.getElementById(tabla);
    var Txt = document.getElementById(campo);
    Txt.value = "";
    for (x = 1; x < Tbl.rows.length; x++) {
        for (y = 0; y < Tbl.rows[x].cells.length - 1; y++)
        { Txt.value += Tbl.rows[x].cells[y].innerHTML + "|"; }
        Txt.value += "#$#";
    }
}
function toasp() {
    acepta();
    toaspf('tbl_detalle', 'txtAuxDetalle');
    toaspf('tbl_descuento', 'txtAuxDescuento');
    toaspf('tbl_impuesto', 'txtAuxImpuesto');
    toaspf('tbl_referencia', 'txtAuxReferencia');
    toaspf('tbl_bultos', 'txtAuxTpoBulto');
    toaspf('tbl_comision', 'txtAuxComision');
    toaspcomi();
}

function datos() {
    asptojs('tbl_detalle', 'txtAuxDetalle');
    asptojs('tbl_descuento', 'txtAuxDescuento');
    asptojs('tbl_impuesto', 'txtAuxImpuesto');
    asptojs('tbl_referencia', 'txtAuxReferencia');
    asptojs('tbl_bultos', 'txtAuxTpoBulto');
    asptojs('tbl_comision', 'txtAuxComision');
    acepta();
    CargaSucu();
    if (document.getElementById('Rutt_rece').value == "55555555") {
        document.getElementById('txtNomb_rece').style.zIndex = 134;
    }
    else {
        document.getElementById('txtNomb_rece').style.zIndex = 133;
    }
}

function CargaSucursal() {
    var oSelOrig = document.getElementById("ddlSucuOrigen");
    var oSelDest = document.getElementById("ddlSucuDestino");
    for (ix = 0; ix <= Arr['oficina'].length -1; ix++) {
        var option = document.createElement('option');
        option.value = Arr['oficina'][ix].Codigo();
        option.text = Arr['oficina'][ix].Nombre();
        oSelOrig.add(option);
    }
    for (ix = 0; ix <= Arr['oficina'].length - 1; ix++) {
        var option = document.createElement('option');
        option.value = Arr['oficina'][ix].Codigo();
        option.text = Arr['oficina'][ix].Nombre();
        oSelDest.add(option);
    }
}

function asptojs(tabla, campo) {
    var Txt = document.getElementById(campo).value;
    if (Txt != "") {
        var lineas = Txt.split("#$#");
        for (x1 = 0; x1 < lineas.length - 1; x1++) {
            agrega(tabla);
            var Tbl = document.getElementById(tabla).rows[document.getElementById(tabla).rows.length - 1];
            var datos = lineas[x1];
            datos = datos.split("|");
            for (y = 0; y < datos.length - 1; y++) {
                Tbl.cells[y].innerHTML = datos[y];
            }
        }
    }
}

function ValoresComision() {
    var oData = document.getElementById('txtAuxComiNEI').value;
    if (oData != "") {
        var oLineas = oData.split("#$#");
        for (x1 = 0; x1 < oLineas.length - 1; x1++) {
            var datos = oLineas[x1];
            datos = datos.split('|');
            document.getElementById('lblTotalNetoAsig').innerHTML = datos[0];
            document.getElementById('lblTotalExentoAsig').innerHTML = datos[1];
            document.getElementById('lblTotalIvaAsig').innerHTML = datos[2];
        }
    }
}

function tipo_documento() {
    var DocAux = document.getElementById('Tipo_docu').value;
    switch (DocAux) {
        case "34":
        case "110":
        case "111":
        case "112":
            return false;
            break;
        default:
            return true;
    }
}
function tipo_documentoRedon() {
    var DocAux = document.getElementById('Tipo_docu').value;
    switch (DocAux) {
        case "110":
        case "111":
        case "112":
            return false;
            break;
        default:
            return true;
    }
}

function tipox_documento() {
    var DocAux = document.getElementById('Tipo_docu').value;
    switch (DocAux) {
        case "61":
        case "56":
        case "111":
        case "112":
            return false;
            break;
        default:
            return true;
    }
}

//By GDIAZ
//Cada vez que se cambie el tipo, se perdera el folio previamente ingresado
function validaSeleccionReferencia(e) {
    var codigoref = document.getElementById('optref0').value;
    for (x = 0; x < Arr['documento'].length; x++) {
        if (Arr['documento'][x].Codigo() == codigoref)
            document.getElementById('txtref1').value = '';
    }
}

function validaTipoReferencia(e) {
    var codigoref = document.getElementById('optref0').value;
    for (x = 0; x < Arr['documento'].length; x++) {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 8) return true; //Tecla de retroceso (para poder borrar)
        if (tecla == 0) return true;

        if (Arr['documento'][x].Codigo() == codigoref) {
            if (parseInt(Arr['documento'][x].Codigo()) < 800) {
                patron = /\d/;
                te = String.fromCharCode(tecla);
                return patron.test(te);
            }
            else {
                patron = /\w/;
                te = String.fromCharCode(tecla);
                return patron.test(te);
            }
        }
    }
}

function validaIva() {
    var oSelect0 = document.getElementById('optimp0');
    var oValue = document.getElementById('optimp1').value;
    var DocAux = document.getElementById('Tipo_docu').value;
    document.getElementById('txtimp3').disabled = false;
    if (oSelect0.value == "R") {
        if (DocAux == "46") {
            if (oValue == "15") {
                document.getElementById('txtimp3').value = document.getElementById('lblIVA').innerHTML;
                document.getElementById('txtimp3').disabled = true;
            }
            if (oValue == "16") {
                if (document.getElementById('txtimp3').value == "0") {
                    document.getElementById('txtimp3').value = document.getElementById('lblIVA').innerHTML;
                }
            }
        }
    }
}

function validaIvaChange() {
    if (document.getElementById('optimp1') != null) {
        var oIdx = document.getElementById('optimp1');
        var oValue = oIdx.options[oIdx.selectedIndex].value;
        var DocAux = document.getElementById('Tipo_docu').value;
        var oSelect0 = document.getElementById('optimp0').value;
        if (DocAux != null) {
            if (oSelect0 == "R") {
                var iIva = document.getElementById('lblIVA').innerHTML;
                var iIvaReten = document.getElementById('txtimp3').value;
                if (iIva != null && iIvaReten != null) {
                    if (parseInt(iIvaReten) > 0) {
                        if (parseInt(iIvaReten) > parseInt(iIva)) {
                            document.getElementById("barRun").disabled = true;
                            alert("No se puede retener mas iva que el emitido en el documento. Favor modificar el monto");
                            return 0;
                        }
                    }
                    else {
                        document.getElementById("barRun").disabled = true;
                        alert("El monto de impuesto o retencion debe ser mayor a 0");
                        return 0;
                    }
                }
            }
        }
    }
    document.getElementById("barRun").disabled = false;
    return 1;
}

function validaTrasladoInterno() {
    var oDllTipoTraslado = document.getElementById("lvIndi_vegd");
    var oValue = oDllTipoTraslado.options[oDllTipoTraslado.selectedIndex].value;
    if (oValue == 5) {
        document.getElementById("ddlSucuOrigen").removeAttribute("disabled");
        document.getElementById("ddlSucuDestino").removeAttribute("disabled");
        CargaSucursal();
    }
    else {
        document.getElementById("ddlSucuOrigen").setAttribute("disabled", "disabled");
        document.getElementById("ddlSucuDestino").setAttribute("disabled", "disabled");
    }
}