
$(document).ready(function () {
    $("input[id^='CaptchaInputText']").attr("maxlength", 7);


    //Cambio de contraseña
    $.each($("[name='clave']"), function (i, l) {
        $(this).click(function () {
            $("[for]").removeClass("css-input-select");
            $("[for='" + this.id + "']").addClass("css-input-select");
        });
    });

    $("#myModal1").modal("show");
   
    var m = $("#hddMessage").html();
    if (m != "") {
        $("#myModal").modal("show");
    }
});

function fn_click(valor) {
    $("#hddMessage").html("");
    var code = false;
    $("#hddcod").val(valor);
    if (valor == 1) {
        var t = $("#hddpin1").val();
        var v = $("#CaptchaInputText2").val();
        if ($.trim(t).length == 0) {
            $("#hddMessage").append("Ingrese su clave actual (6 d&iacute;gitos) <br/>");
            code = true;
        }
        else if ($.trim(t).length != 6) {
            $("#hddMessage").append("La clave actual debe ser de 6 d&iacute;gitos <br/>");
            code = true;
        }
        t = $("#hddpin2").val();
        if ($.trim(t).length == 0) {
            $("#hddMessage").append("Ingrese su clave nueva (6 d&iacute;gitos) <br/>");
            code = true;
        }
        else if ($.trim(t).length != 6) {
            $("#hddMessage").append("La clave nueva debe ser de 6 d&iacute;gitos <br/>");
            code = true;
        }
        var t1 = $("#hddpin3").val();
        if ($.trim(t1).length == 0) {
            $("#hddMessage").append("Repita su clave nueva <br/>");
            code = true;
        }

        if ($.trim(t) != $.trim(t1)) {
            $("#hddMessage").append("La clave no coincide <br/>");
            code = true;
        }
        if ($.trim(v).length == 0) {
            $("#hddMessage").append("Ingrese el texto de la imagen <br/>");
            code = true;
        }
        if (code) {
            $("#myModal").modal("show");
            return false;
        } else {
            LoadingDialog.show();
        }
    }
    btnSubmit.click();
}


function fn_codping(cod) {

    var a = $("#rClave1").is(":checked");
    var b = $("#rClave2").is(":checked");
    
    if (a) {
        if (cod == -1) {
            $("#hddpin1").val("");
            $("#txtClave1").val("");
            return;
        }

        var psdval = $("#txtClave1").val();
        var hddval = $("#hddpin1").val();
        if (hddval.length == 6) {
            return;
        }
        if (hddval.length < 6) {
            $("#hddpin1").val(hddval + cod);
            $("#txtClave1").val(psdval + "*");
            return;
        }
    }
    else if (b) {
        if (cod == -1) {
            $("#hddpin2").val("");
            $("#txtClave2").val("");
            return;
        }

        var psdval = $("#txtClave2").val();
        var hddval = $("#hddpin2").val();
        if (hddval.length == 6) {
            return;
        }
        if (hddval.length < 6) {
            $("#hddpin2").val(hddval + cod);
            $("#txtClave2").val(psdval + "*");
            return;
        }
    } else {
        if (cod == -1) {
            $("#hddpin3").val("");
            $("#txtClave3").val("");
            return;
        }

        var psdval = $("#txtClave3").val();
        var hddval = $("#hddpin3").val();
        if (hddval.length == 6) {
            return;
        }
        if (hddval.length < 6) {
            $("#hddpin3").val(hddval + cod);
            $("#txtClave3").val(psdval + "*");
            return;
        }
    }
}