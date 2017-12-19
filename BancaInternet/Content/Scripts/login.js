var attr = 0;
var time = $("#hddsecond").val();
var interval = null;

$(document).ready(function () {

    $("input[id^='CaptchaInputText']").attr("maxlength", 7);

    if ($("#hddpin").val() != "") {
        location.reload();
        return;
    }
    time = $("#hddsecond").val();
    $("#spseconds").html(time);

    var m = $("#hddMessage").html();
    if (m != "") {
        $("#myModal").modal("show");
    }

    $("#txtcaja1,#txtcaja2,#txtcaja3,#txtcaja4").keydown(function () {
		attr = $(this).attr("value-text");
	});

    $("#txtcaja1,#txtcaja2,#txtcaja3,#txtcaja4").keyup(function () {
        if (this.value.match(/[^0-9]/g)) {
            this.value = this.value.replace(/[^0-9]/g, '');
        }
		fn_saltab(this);
	});

	$("#hddpin").val("");
	$("#psdclave").val("");
    
	$(".carousel-inner .item:first").addClass("active");
	$(".carousel-indicators li:first").addClass("active");
	
	//$("#hrefPF").attr("href", VG_RUTA_SERVIDOR + 'credinka/PreguntasFrecuentes');
	
	document.onmousewheel=function(){ time = $("#hddsecond").val(); $("#spseconds").html(time);	 }
    document.onkeydown=function(){  time = $("#hddsecond").val(); $("#spseconds").html(time);	 }
	document.onkeypress=function(){  time = $("#hddsecond").val(); $("#spseconds").html(time);	 }
    document.onkeyup=function(){  time = $("#hddsecond").val(); $("#spseconds").html(time);	 }
    document.onclick=function(){ time = $("#hddsecond").val(); $("#spseconds").html(time);	  }
    interval = setInterval(fn_seconds, 1000);

    $("#btnsubmit").click(function () {
        $("#hddMessage").html("");
        var code = false;
        var cap = $("#CaptchaInputText0").val();
        var hddval = $("#hddpin").val();
        var t1 = $.trim($("#txtcaja1").val());
        var t2 = $.trim($("#txtcaja2").val());
        var t3 = $.trim($("#txtcaja3").val());
        var t4 = $.trim($("#txtcaja4").val());
        var tj = t1 + t2 + t3 + t4;
        if (tj.length < 16) {
            $("#hddMessage").append("El número de tarjeta es invalido <br/>");
            code = true;
        }
        if ($.trim(hddval).length != 6) {
            $("#hddMessage").append("La clave de acceso debe ser de 6 dígitos <br/>");
            code = true;
        }
        if ($.trim(cap).length == 0) {
            $("#hddMessage").append("Ingrese el texto de la imagen <br/>");
            code = true;
        }
        if (code) {
            $("#myModal").modal("show");
            return false;
        } else {
            LoadingDialog.show();
            return true;
        }
    });

    $("#selectTipo").change(function () {
        var val = $(this).val();
        if (val == 1) {
            $("#txtdocumento").attr("maxlength", 8).val("");
        } else {
            $("#txtdocumento").attr("maxlength", 11).val("");
        }
    });

    $('#txtdocumento').keyup(function () {
        if (this.value.match(/[^0-9]/g)) {
            this.value = this.value.replace(/[^0-9]/g, '');
        }
    });

    $("#selectTipo").change();

    $("#btnOlvide").click(function () {
        $("#hddMessage").html("");
        var code = false;
        var re = /\S+@\S+\.\S+/;
        var tc = $("#selectTipo").val();
        var cap = $("#CaptchaInputText1").val();
        var doc = $("#txtdocumento").val();
        var email = $.trim($("#txtemail").val());

        if ($.trim(doc).length == 0) {
            $("#hddMessage").append("Ingrese su Número de documento<br/>");
            code = true;
        }else if(($.trim(doc).length < 8 && tc == 1) || ($.trim(doc).length < 11 && tc == 2)) {
            $("#hddMessage").append("Su número de documento esta incompleto<br/>");
            code = true;
        }
        if ($.trim(email).length == 0) {
            $("#hddMessage").append("Ingrese su correo electrónico<br/>");
            code = true;
        }else if(!re.test(email)) {
            $("#hddMessage").append("El correo electrónico ingresado no es valido<br/>");
            code = true;
        }
        if ($.trim(cap).length == 0) {
            $("#hddMessage").append("Ingrese el texto de la imagen <br/>");
            code = true;
        }
        if (code) {
            $("#myModal").modal("show");
            return false;
        } else {
            LoadingDialog.show();
            return true;
        }
    })

    $('#rdModal2').click(function (ev) {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        LoadingDialog.show();
        $.ajax({
            url: VG_RUTA_SERVIDOR+'credinka/Generar',
            type: 'POST',
            cache: false,
            data: {
                __RequestVerificationToken: token
            },
            success: function (data) {
                $("#dvContent").html(data);
                $("#myModal2").modal("show");
                LoadingDialog.hide();
            }
        });
        return false;
    })

    $("#dvOlvide").click(function (ev) {
        $("#txtdocumento").val("");
        $("#txtemail").val("");
        $("#CaptchaInputText1").val("");
    });
    
});

function fn_saltab(obj){	

	var tolval = obj.value;
	tolval = tolval.replace(/\s/g,"");

	if(tolval.length > 4 && obj.id == "txtcaja1"){
		$("#txtcaja1").val(tolval.substring(0,4));
		$("#txtcaja2").val(tolval.substring(4,8));
		$("#txtcaja3").val(tolval.substring(8,12));
		$("#txtcaja4").val(tolval.substring(12,16));	
	}

	if(attr != 0){
		if(obj.id == "txtcaja1"){
			if(tolval.length <= 16 && tolval.length >= 12){
				$("#txtcaja4").focus();
			}else if(tolval.length <= 12 && tolval.length >= 8){
				$("#txtcaja3").focus();
			}else if(tolval.length <= 8 && tolval.length >= 4){
				$("#txtcaja2").focus();
			}else if(tolval.length < 4){
				$("#txtcaja1").focus();
			}
		}else{
			if(obj.value.length == 4){
				attr++;
				$("[value-text="+attr+"]").focus();
			}
			if(obj.value.length == 0){
				attr--;
				$("[value-text="+attr+"]").focus();
			}
		}
	}
	attr = 0;
	
}

function fn_seconds(){
	time--;
	if(time < 0){
        this.window.open('', '_self', '');
        this.window.close();
        $("#modal-close").click();
	    $("#modal-close1").click();
		$("[class='container css-container-center']").html('<div class="css-banner-top-destroy"><img class="css-banner-image"'+
			' src="'+ VG_RUTA_SERVIDOR+'Content/Images/logo-intranet.png"></div><div class="row"><div class="col-md-4"></div><div class="col-md-4"><div ' +
			'class="css-container-principal-2x"><div class="row css-row-margin0"><div class="col-xs-12 css-fondo-blanco css-t'+
			'ext-destroy">El tiempo ha expirado, cierre la ventana y vuelva a entrar a la opción de "Banca por Internet".</di'+
			'v></div></div></div><div class="col-md-4"></div></div>');
		clearInterval(interval);
		return false;
	}else{
		$("#spseconds").html(time);	
	}
}

function fn_codping(cod) {

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
