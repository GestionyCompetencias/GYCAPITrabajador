
$(document).ready(function () {
    AgregarDatos2();
    //$("body").on('click', ".autHHEE", autorizaHHEE);
    //$("body").on('click', ".fa-check-square", PareaEvento);
    //$(".autHHEE").click(HELLO);
});

$(".faena").on("click", function () {
    var FaenaId = $(this).val();
    $(".trabajador").select2({
        placeholder: "Seleccione Trabajador(es)",
        ajax: {
            url: "/AsAdminist/GetTrabajadoresList?FaenaId=" + FaenaId,
            dataType: "json",
            data: function (params) {
                return {
                    searchTerm: params.term
                };
            },
            processResults: function (data, params) {
                return {
                    results: data
                };
            }
        }
    });
});

$(".trabajador").on("change", function () {
    var TrabRut = $(this).val();
    $("#txtTrabajadores").val(TrabRut);
    var txtTrabajadoresData = $("#txtTrabajadores").val();

    $.ajax({
        url: '/AsAdminist/GetTrabajadoresSeleccionados?trabajadores=' + txtTrabajadoresData,
        dataType: 'json',
        type: 'post',
    });
});

//$(".faena").on("click", function () {
//    var FaenaId = $(this).val();
//    $.ajax({
//        url: "GetTrabajadoresList?FaenaId=" + FaenaId,
//        dataType: 'json',
//        type: 'post',
//        success: function (data) {
//            $.each(data, function (index, row) {
//                $(".trabajador").append("<option value='" + row + "'>" + row + "</option>")
//            });
//        }
//    });
//});



function AgregarDatos() {

    var tabla = document.getElementById("tablaX");
    //Ejemplos de datos
    for (var i = 0; i < 5; i++) {
        var fila = document.createElement("tr");
        fila.align = "center";
        fila.id = "fila" + i;

        //Creamos las celdas de las columnas
        var rut = document.createElement("th");
        var apellidos = document.createElement("td");
        var nombres = document.createElement("td");

        var calcFiniquito = document.createElement("td");
        calcFiniquito.innerHTML = '<input class="form-check-input" type="checkbox" value="" id="col6-' + i + '">';

        var pagoFiniquito = document.createElement("td");
        pagoFiniquito.innerHTML = '<input class="form-check-input" type="checkbox" value="" id="col7-' + i + '">';

        var infoComparendo = document.createElement("td");
        infoComparendo.innerHTML = '<input class="form-check-input" type="checkbox" value="" id="col8-' + i + '">';

        var presentarse = document.createElement("td");
        presentarse.innerHTML = '<input class="form-check-input" type="checkbox" value="" id="col9-' + i + '">';


        //Asignamos valores X
        rut.innerHTML = "11222333-k";
        apellidos.innerHTML = "PEREZ SOTO";
        nombres.innerHTML = "ANDRES ALBERTO";

        fila.appendChild(rut);
        fila.appendChild(apellidos);
        fila.appendChild(nombres);

        fila.appendChild(calcFiniquito);
        fila.appendChild(pagoFiniquito);
        fila.appendChild(infoComparendo);
        fila.appendChild(presentarse);

        tabla.getElementsByTagName("tbody")[0].appendChild(fila);
    }

}

function clickearVerificacion() {
    //Esta funcion devuelve los checkbox y sus estados en base a la fila
    for (var i = 0; i < 5; i++) {
        var filaID = i;

        var fila = document.getElementById("fila" + filaID);

        var stringReturn = "RUT " + fila.childNodes[0].innerHTML + "\nCheckboxes: \n";

        var acred = document.getElementById("col1-" + filaID);
        var acredAnexo = document.getElementById("col2-" + filaID);
        var confAnexo = document.getElementById("col3-" + filaID);
        var gestODI = document.getElementById("col4-" + filaID);
        var evalODI = document.getElementById("col5-" + filaID);

        if (acred.checked) {
            stringReturn += "\n Calculo Finiquito checked";
        }
        else {
            stringReturn += "\n Calculo Finiquito not checked";
        }

        if (acredAnexo.checked) {
            stringReturn += "\n Pago Finiquito checked";
        }
        else {
            stringReturn += "\n Pago Finiquito not checked";
        }

        if (confAnexo.checked) {
            stringReturn += "\n Informacion de Comparendo checked";
        }
        else {
            stringReturn += "\n Informacion de comparendo not checked";
        }

        if (gestODI.checked) {
            stringReturn += "\n Presentarse Comparendo checked";
        }
        else {
            stringReturn += "\n Presentarse Comparendo not checked";
        }

        if (evalODI.checked) {
            stringReturn += "\n Presentarse Comparendo checked";
        }
        else {
            stringReturn += "\n Presentarse Comparendo not checked";
        }

        alert(stringReturn);
    }


}