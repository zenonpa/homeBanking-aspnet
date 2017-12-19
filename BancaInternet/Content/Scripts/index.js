var contador = 0;
var timer = 0;
var out = 0;
var interval = null;

var timerExp = {
    get: function () {
        var min = (Math.floor(timer / 60));
        var seg = (timer % 60);
        var sseg = seg.toString();
        if (sseg.length == 1) {
            sseg = "0" + sseg;
        }
        return "" + min + ":" + sseg;
    }
};

$(document).ready(function () {

    var m = $("#hddMessage").html();
    if (m != "") {
        $("#myModal2").modal("show");
    }

    document.onmousewheel = function () { timer = $("#hddContador").val(); $("#spseconds").html(timerExp.get()); }
    document.onkeydown = function () { timer = $("#hddContador").val(); $("#spseconds").html(timerExp.get()); }
    document.onkeypress = function () { timer = $("#hddContador").val(); $("#spseconds").html(timerExp.get()); }
    document.onkeyup = function () { timer = $("#hddContador").val(); $("#spseconds").html(timerExp.get()); }
    document.onclick = function () { timer = $("#hddContador").val(); $("#spseconds").html(timerExp.get()); }
    timer = $("#hddContador").val();
    $("#sptimer").html(timerExp.get());
    interval = setInterval(fn_timer, 1000);

    $("input[id^='CaptchaInputText']").attr("maxlength", 7);

    $("#dvCuenta").click(function () {
        if (contador == 0) {
            $("#dvOption").slideDown("fast");
            $("#dvCuenta").css("background-color", "#EA6C1D");
            $("#dvCuenta").css("color", "#fff");
            var obj = $("#dvCuenta.css-option-micuenta::after");
            contador = 1;
        } else {
            $("#dvOption").slideUp("fast");
            $("#dvCuenta").css("background-color", "#eee");
            $("#dvCuenta").css("color", "#333");
            contador = 0;
        }
    });

    $("[class='css-modal-option']").mouseleave(function () {
        $("#dvOption").slideUp("fast");
        $("#dvCuenta").css("background-color", "#eee");
        $("#dvCuenta").css("color", "#333");
        contador = 0;
    });

   // $("#hrefPF").attr("href", VG_RUTA_SERVIDOR + 'credinka/PreguntasFrecuentes');
    
    $.each($("[name='clave']"), function (i, l) {
        $(this).click(function () {
            $("[for]").removeClass("css-input-select");
            $("[for='" + this.id + "']").addClass("css-input-select");
        });
    });

    var val = $("#hddaction").val();
    if (val == 0) {
        $("#0").addClass("active");
    } else if (val == 2) {
        $("#2").addClass("active");
    } else if (val == 3) {
        $("#3").addClass("active");
    }

    $("#aSeSe").click(function () {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        LoadingDialog.show();
        $.ajax({
            url: VG_RUTA_SERVIDOR + 'credinka/SellosSeg',
            type: 'POST',
            cache: false,
            data: {
                __RequestVerificationToken: token
            },
            success: function (data) {
                $("#dvContent").html(data);
                $("#mIMG").modal("show");
                LoadingDialog.hide();
            }
        });
        return false;
        $("#mIMG").modal("show");
    });

    $("#aCronPag").click(function () {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        LoadingDialog.show();
        $.ajax({
            url: VG_RUTA_SERVIDOR + 'credinka/CronogramaPagos',
            type: 'POST',
            cache: false,
            data: {
                __RequestVerificationToken: token
            },
            success: function (data) {
                $("#dvContent").html(data);
                $("#mIMG").modal("show");
                LoadingDialog.hide();
            }
        });
        $("#mIMG").modal("show");
        return false;
    });


    $('#ex_x_d_f').click(function (event) {
        event.preventDefault();
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        var htmlxt = $("#conteiner").clone();
        $(htmlxt).find(".option").remove();
        $(htmlxt).find("select").remove();
        $(htmlxt).find("button").remove();
        $(htmlxt).find("input").remove();
        $(htmlxt).find(".css-text-title-option").css("fontSize", "12px");
        $(htmlxt).find("td").css("fontSize", "11px");
        $(htmlxt).find("tr").css("fontSize", "11px");
        $(htmlxt).find("th").remove(":contains('Detalle')");
        $(htmlxt).find("th").css("fontSize", "11px");

        var strDatos = $(htmlxt).html().replace(/</g, '[').replace(/>/g, ']');
        var newForm = $('<form>', {
            'action': VG_RUTA_SERVIDOR + 'credinka/ExportarInformacion',
            'target': '_top',
            'method': 'post'
        }).append($('<input>', {
            'name': 'hddevent1',
            'value': $("#spUsuario").text(),
            'type': 'hidden'
        })).append($('<input>', {
            'name': 'hddevent2',
            'value': strDatos,
            'type': 'hidden'
        })).append($('<input>', {
            'name': '__RequestVerificationToken',
            'value': token,
            'type': 'hidden'
        }));
        newForm.submit();
    });

});

function fn_option(val) {
    $("#hddaction").val(val);
    LoadingDialog.show();
    $("#hddsubmit").click();
}

function fn_timer() {

    var out = $("#hddtimeout").val();

    $("#sptimer").html(timerExp.get());
    timer--;
    if (timer < out) {
        $("#myModal").modal("show");
    }
    if (timer < 0) {
        clearInterval(interval);
        fn_server();
        $("#myModal").modal("hide");
        $("#myModal1").modal("show");
    }
}

function fn_server() {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: VG_RUTA_SERVIDOR + 'credinka/end',
        type: 'POST',
        data: {
            __RequestVerificationToken: token
        },
        success: function (data) { },
        error: function (request, status, error) {
        }
    });
    return false;
}

function fn_ir(var1, var2) {
    $("#hddevent1").val(var1);
    $("#hddevent2").val(var2);
    if (var1 != "3" && var1 != "4" && var1 != "5" && var2 != "3" && var2 != "4" && var2 != "5") {
        LoadingDialog.show();
    }
    if (var1 == "5" || var2 == "5") {
        var strDatosPadre = $(var2).html().replace(/</g, '[').replace(/>/g, ']');
        $("#hddevent2").val(strDatosPadre);
    }
    $("#hddsubmit_b").click();
}

function fn_ex(var1, var2) {
    $("#hddevent1").val(var1);
    $("#hddsubmit_b").click();
}

function fn_click() {
    $("#hddMessage").html("");
    var code = false;

    var t = $("#hddpin1").val();

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
    if (code) {
        $("#myModal2").modal("show");
        return false;
    } else {
        LoadingDialog.show();
    }

    $("#btnSubmit").click();
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

function fn_codping2(cod) {

    if (cod == -1) {
        $("#hddpin").val("");
        $("#psdclave").val("");
        return;
    }

    var psdval = $("#psdclave").val();
    var hddval = $("#hddpin").val();
    if (hddval.length == 6) {
        return;
    }
    if (hddval.length < 6) {
        $("#hddpin").val(hddval + cod);
        $("#psdclave").val(psdval + "*");
        return;
    }
}
