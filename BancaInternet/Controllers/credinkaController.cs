using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Web.Mvc;
using BancaInternet.EN;
using System.Reflection;
using BancaInternet.UTL;
using System.Configuration;
using CaptchaMvc.Attributes;
using BancaInternet.wsCuenta;
using BancaInternet.wsUbigeo;
using BancaInternet.wsUsuario;
using BancaInternet.Funciones;
using BancaInternet.wsPrestamo;
using BancaInternet.wsTipoCambio;
using System.Collections.Generic;
using BancaInternet.wsDescripcion;
using System.Web.Script.Serialization;

using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Globalization;

namespace BancaInternet.Controllers
{
    public class credinkaController : Controller
    {
        private HttpRequestBase _requestBase;
        string sKey_TimerCerrar = MvcApplication.TimeLogueo;
        private string KEY_TEC = "LOBJ_";
        private bool bCambioContra = true;

        /*TEMPORAL*/
        string ip = "10.10.10.10";
        string mac = "F2-F2-F2-F2";
        

        public ActionResult PreLoad()
        {
            Session.Clear();
            string sForm_validate = utlFunciones.P_RJD_Encriptar("PreLoad", MvcApplication.sKey_Master); 
            ViewBag.form_validate = sForm_validate;
            return View();
        }

        [HttpPost, CaptchaVerify("Captcha no es valido")]
        [ValidateAntiForgeryToken]
        public ActionResult BancaPorInternet(FormCollection fcFormulario)
        {
            string sForm_validate = "";
            string sForm_Interno = "";
            
            try
            {
                sForm_validate = Request.Form["form_name"] == null ? "" : Request.Form["form_name"];
                if (!sForm_validate.Equals("")) {
                    sForm_validate = utlFunciones.P_RJD_Desencriptar(sForm_validate, MvcApplication.sKey_Master);
                }
                sForm_Interno = Request.Form["_form_name"] == null ? "" : Request.Form["_form_name"];
                if (!sForm_Interno.Equals("")) {
                    sForm_Interno = utlFunciones.P_RJD_Desencriptar(sForm_Interno, MvcApplication.sKey_Master);
                }
                

                if (sForm_validate.Equals("") && sForm_Interno.Equals(""))
                {
                    Session.Clear();
                    ViewBag.ruta = GenerarTeclado(0);
                    sForm_validate = utlFunciones.P_RJD_Encriptar("Logueo",MvcApplication.sKey_Master);
                    ViewBag.form_validate = sForm_validate;
                    List<string> imgDimanic = ObtenerImagenes(1);
                    ViewBag.imgDimanic = imgDimanic;
                    ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                    TempData["Message"] = "Datos incorrectos";
                    ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                    ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                    return View("Logueo");
                }

                if (sForm_validate.Equals("PreLoad"))
                {
                    Session.Clear();
                    ViewBag.ruta = GenerarTeclado(0);
                    sForm_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                    ViewBag.form_validate = sForm_validate;
                    List<string> imgDimanic = ObtenerImagenes(1);
                    ViewBag.imgDimanic = imgDimanic;
                    ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                    ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                    ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                    return View("Logueo");
                }
                else if (sForm_validate.Equals("Logueo"))
                {
                    return View(Logueo());
                }
                else if (sForm_validate.Equals("ClaveObligatorio"))
                {
                    return View(CambioClaveObligatoria());
                }
                else if (sForm_validate.Equals("Menu"))
                {
                    var obj = Session["session01"];
                    if (obj == null)
                    {
                        ViewBag.ruta = GenerarTeclado(0);
                        ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                        List<string> imgDimanic = ObtenerImagenes(1);
                        ViewBag.imgDimanic = imgDimanic;
                        ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                        TempData["Message"] = "Al parecer su tiempo de sesión expiró.";
                        ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                        ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                        return View("Logueo");
                    }

                    string sAction = Request.Form["hddaction"] == null ? "" : Request.Form["hddaction"];
                    if (sAction == null || sAction.Equals("0"))
                    {
                        CargarMaster();
                        RedirectPrincipal();
                        return View("Principal");
                    }
                    /*Cerrar Sesion*/
                    else if (sAction.Equals("1"))
                    {
                        foreach (System.Collections.DictionaryEntry entry in HttpContext.Cache)
                        {
                            HttpContext.Cache.Remove((string)entry.Key);
                        }

                        Session.Clear();
                        ViewBag.ruta = GenerarTeclado(0);
                        ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                        List<string> imgDimanic = ObtenerImagenes(1);
                        ViewBag.imgDimanic = imgDimanic;
                        ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                        TempData["Message"] = "Se cerró con éxito la sesión";
                        ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                        ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                        return View("Logueo");
                    }
                    /*Mis cuentas*/
                    else if (sAction.Equals("2"))
                    {
                        CargarMaster();
                        RedirectMisCuentas();
                        return View("MisCuentas");
                    }
                    /*Mis prestamos*/
                    else if (sAction.Equals("3"))
                    {
                        CargarMaster();
                        RedirectObtenerPrestamos();
                        return View("MisPrestamos");
                    }
                    /*Actualizar Datos*/
                    else if (sAction.Equals("4"))
                    {
                        CargarMaster();
                        CargarActualizarDatos();
                        return View("ActualizarDatos");
                    }
                    /*Cambio de contrasena*/
                    else if (sAction.Equals("5"))
                    {
                        CargarMaster();
                        ViewBag.ruta = GenerarTeclado(0);
                        ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
                        ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("CambiaClave", MvcApplication.sKey_Master);
                        ViewBag.hddaction = "5";
                        return View("CambioContrasena");
                    }

                    /*Acciones internas*/
                    if (sForm_Interno.Equals("Principal"))
                    {
                        return View(action_click_Principal());
                    }
                    else if (sForm_Interno.Equals("CambiaClave"))
                    {
                        CargarMaster();
                        return View(action_click_CambioClave());
                    }
                    else if (sForm_Interno.Equals("ActualizarDatos"))
                    {
                        CargarMaster();
                        return View(action_click_ActualizarDatos());
                    }
                    else if (sForm_Interno.Equals("CambioSello"))
                    {
                        return View(action_click_CambioSello());
                    }
                    else if (sForm_Interno.Equals("CronogramaPagos"))
                    {
                        return View(Action_click_CronogramaPagos());
                    }
                    else if (sForm_Interno.Equals("Prestamos"))
                    {
                        return View(action_click_DetallePrestamo());
                    }
                    else if (sForm_Interno.Equals("Cuentas"))
                    {
                        return View(action_click_DetalleCuenta());
                    }
                    else if (sForm_Interno.Equals("MisCuentasDetalle"))
                    {
                        return View(action_click_MovimientosCuenta());
                    }
                    else if (sForm_Interno.Equals("MisPrestamosDetalle"))
                    {
                        return View(action_click_CuentaExportar());
                    }
                }

                TempData["Message"] = "Datos Incorrectos.";
                return Redirect("PreLoad");
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                Session.Clear();
                string sKey_TimerCerrar = MvcApplication.TimeLogueo;
                ViewBag.ruta = GenerarTeclado(0);
                ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                List<string> imgDimanic = ObtenerImagenes(1);
                ViewBag.imgDimanic = imgDimanic;
                ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                TempData["Message"] = "Datos Incorrectos.";
                ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                return View("Logueo");
            }
        }

        public FileContentResult PreguntasFrecuentes()
        {
            FileContentResult fcResult;
            try
            {
               
                string sKey_file_server = System.Web.Configuration.WebConfigurationManager.AppSettings["urlFilepdf"].ToString();
                byte[] bFile = System.IO.File.ReadAllBytes(sKey_file_server + "preguntas-frecuentes.pdf");
                fcResult = File(bFile, System.Net.Mime.MediaTypeNames.Application.Octet, "preguntas-frecuentes.pdf");                
                return fcResult;
                
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "No se encuentra el archivo";
                byte[] bFile = new byte[0];
                fcResult = File(bFile, System.Net.Mime.MediaTypeNames.Application.Octet, "preguntas-frecuentes.pdf");
                return fcResult;
            }
        }

        public ActionResult redirect(string Error = "")
        {
            return View("Error");
        }

        [HttpPost, CaptchaVerify("Captcha no es valido")]
        [ValidateAntiForgeryToken]
        public ActionResult Generar()
        {
            var val = Request.Form["hddValor"];
            try
            {
                if (val == null || val.Equals("1"))
                {
                    ViewBag.ruta = GenerarTeclado(1);
                    ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                    ViewBag.ValueOption = Session.SessionID;
                    return PartialView("GenerarClave1");
                }
                else if (val.Equals(Session.SessionID))
                {
                    if (ModelState.IsValid)
                    {
                        //Campos a utlizar  ObtenerPin
                        string sT1 = "", sT2 = "", sT3 = "", sT4 = "", sTarjeta = "", sPosiciones = "", sPin = "";
                        int iTipoTarjeta = 0;
                        sT1 = Request.Form["txtcaja5"];
                        sT2 = Request.Form["txtcaja6"];
                        sT3 = Request.Form["txtcaja7"];
                        sT4 = Request.Form["txtcaja8"];
                        sPosiciones = Request.Form["hddpin1"];
                        iTipoTarjeta = int.Parse(Request.Form["selectTarjeta"]);
                        sTarjeta = sT1 + sT2 + sT3 + sT4;
                        sPin = ObtenerPin(1, sPosiciones);
                        if (sTarjeta.Length != 16 || sPin.Length != 4)
                        {
                            ViewBag.textValor = "Los datos son incorrectos";
                            ViewBag.ruta = GenerarTeclado(1);
                            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                            ViewBag.ValueOption = Session.SessionID;
                            return PartialView("GenerarClave1");
                        }
                        else
                        {

                            /*Encripta la informacion*/
                            rnSegRSA ornSegRSA = new rnSegRSA();
                            string strEncriptado = ornSegRSA.RSA_Encriptar(sTarjeta);
                            string strEncriptado1 = ornSegRSA.RSA_Encriptar(sPin);
                            /*********************/

                            enUsuarioValida enUsuario = new enUsuarioValida();
                            using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
                            {
                                enUsuario = wsUsuario.WSValidaClaveUnibanca(strEncriptado, strEncriptado1);
                            }
                            if (enUsuario.iTipoResultado == 1)
                            {
                                enUsuario.iTipoTarjeta = iTipoTarjeta;
                                enUsuario.sNumeroTarjeta = sTarjeta;

                                /*Encriptacion Session*/
                                string ObjetoSerializado = new JavaScriptSerializer().Serialize(enUsuario);
                                string objEncriptado = utlFunciones.P_RJD_Encriptar(ObjetoSerializado, MvcApplication.sKey_Master);
                                Session["Valor1"] = objEncriptado;

                                DestruirTeclado(1);
                                /*Redirecciona*/

                                ViewBag.textValor = "";
                                ViewBag.ruta = GenerarTeclado(2);
                                ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                                ViewBag.ValueOption = "_" + Session.SessionID;
                                return PartialView("GenerarClave2");
                            }
                            else if (enUsuario.iTipoResultado == 3)
                            {
                                DestruirTeclado(1);
                                ViewBag.textValor = "El cliente ya se encuentra asociado, utilice la opción de \"Olvidé mi clave\" si no recuerda su clave de acceso.";
                                return PartialView("GenerarClave3");
                            }
                            else if (enUsuario.iTipoResultado == 4)
                            {
                                ViewBag.textValor = "El cliente no se encuentra afiliado.";
                                ViewBag.ruta = GenerarTeclado(1);
                                ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                                ViewBag.ValueOption = Session.SessionID;
                                return PartialView("GenerarClave1");
                            }
                            else
                            {
                                ViewBag.textValor = "Los datos son incorrectos";
                                ViewBag.ruta = GenerarTeclado(1);
                                ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                                ViewBag.ValueOption = Session.SessionID;
                                return PartialView("GenerarClave1");
                            }
                        }
                    }
                    else
                    {
                        ViewBag.textValor = "El texto de la imagen no es correcto";
                        ViewBag.ruta = GenerarTeclado(1);
                        ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                        ViewBag.ValueOption = Session.SessionID;
                        return PartialView("GenerarClave1");
                    }
                }
                else if (val.Equals("_" + Session.SessionID))
                {
                    if (ModelState.IsValid)
                    {
                        //Campos a utlizar  ObtenerPin
                        string sPosicion = "", sPosicion2 = "";
                        int iRespuesta = 0;
                        sPosicion = Request.Form["hddpin1"];
                        sPosicion2 = Request.Form["hddpin2"];
                        if (sPosicion.Length != 6)
                        {
                            ViewBag.textValor = "Su clave debe tener 6 dígitos";
                            ViewBag.ruta = GenerarTeclado(2);
                            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                            ViewBag.ValueOption = "_" + Session.SessionID;
                            return PartialView("GenerarClave2");
                        }
                        else if (!sPosicion.Equals(sPosicion2))
                        {
                            ViewBag.textValor = "Su Clave no coincide.";
                            ViewBag.ruta = GenerarTeclado(2);
                            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                            ViewBag.ValueOption = "_" + Session.SessionID;
                            return PartialView("GenerarClave2");
                        }
                        else
                        {
                            //Obtiene el objeto de sessio
                            var obj = Session["Valor1"];
                            if (obj != null)
                            {
                                /*obtiene pin*/
                                string sPin = ObtenerPin(2, sPosicion);
                                string ojbSerializado = "";
                                if (!sPin.Equals("")){
                                    ojbSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                                }
                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                enUsuarioValida oenUsuario = serializer.Deserialize<enUsuarioValida>(ojbSerializado);
                                enUsuarioValida poenUsuario = new enUsuarioValida();
                                poenUsuario.sCodigoCliente = oenUsuario.sCodigoCliente;
                                poenUsuario.iTipoTarjeta = oenUsuario.iTipoTarjeta;
                                poenUsuario.sNumeroTarjeta = oenUsuario.sNumeroTarjeta;
                                poenUsuario.sPing = sPin;
                                poenUsuario.vAudIPCreacion = ip;
                                poenUsuario.vAudMACCreacion = mac;

                                /*Encripta la informacion*/
                                string ObjetoSerializado = new JavaScriptSerializer().Serialize(poenUsuario);
                                rnSegRSA ornSegRSA = new rnSegRSA();
                                string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                                /*********************/

                                using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
                                {
                                    iRespuesta = wsUsuario.WSRegistrarAsociaconUsuario(strEncriptado);
                                }
                                if (iRespuesta <= 0)
                                {
                                    DestruirTeclado(2);
                                    TempData["Message"] = "No se pudo generar su clave de 6 dígitos.";
                                    return Redirect("PreLoad");
                                }
                                else
                                {
                                    DestruirTeclado(2);
                                    ViewBag.textValor = "Su clave de Internet (6 dígitos) se ha generado correctamente. Haga click en continuar e ingresa los datos para acceder a la Banca por Internet.";
                                    return PartialView("GenerarClave3");
                                }
                            }
                            else
                            {
                                ViewBag.textValor = "Valide su cuenta.";
                                ViewBag.ruta = GenerarTeclado(1);
                                ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                                ViewBag.ValueOption = Session.SessionID;
                                return PartialView("GenerarClave1");
                            }
                        }
                    }
                    else
                    {
                        ViewBag.textValor = "El texto de la imagen no es correcto";
                        ViewBag.ruta = GenerarTeclado(2);
                        ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Generar", MvcApplication.sKey_Master);
                        ViewBag.ValueOption = "_" + Session.SessionID;
                        return PartialView("GenerarClave2");
                    }
                }
                else
                {
                    return Redirect("PreLoad");
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "Los datos son incorrectos";
                return Redirect("PreLoad");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SellosSeg()
        {
            try
            {
                var parameterContainer = Request == null ? _requestBase : Request;
                if (Request.IsAjaxRequest())
                {
                    var obj = Session["session01"];
                    if (obj == null)
                    {
                        TempData["Message"] = "Al parecer su tiempo de sesión expiró.";
                        return Redirect("PreLoad");
                    }
                    string sUsuarioSerializado = "";
                    if (!obj.Equals("")) {
                        sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                    }

                    
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);
                    List<enSellos> loensellos = new List<enSellos>();
                    using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
                    {
                        loensellos = wsUsuario.WSListarSellos().ToList<enSellos>();
                    }
                    Random rng = new Random();
                    int n = loensellos.Count;
                    while (n > 0)
                    {
                        n--;
                        int k = rng.Next(n + 1);
                        enSellos value = loensellos[k];
                        loensellos[k] = loensellos[n];
                        loensellos[n] = value;
                        if (value.vRutaCompleta == oenSesion.valor7)
                        {
                            value.bSeleccion = true;
                        }
                    }
                    ViewBag.lstIMG = loensellos;
                    ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
                    ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("CambioSello", MvcApplication.sKey_Master);
                    return PartialView("CambiarSello");
                }
                else
                {
                    return Redirect("PreLoad");
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "Los datos son incorrectos";
                return Redirect("PreLoad");
            }
        }

        //CronogramaPagos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CronogramaPagos()
        {
            try
            {
                var sNumeroCredito = Convert.ToString(Session["PrestamosDetalle"]);

                if (Request.IsAjaxRequest())
                {
                    var obj = Session["session01"];
                    string sUsuarioSerializado = "";
                    if (!obj.Equals(""))
                    {
                        sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                    }

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);
                    enCuenta oenCuenta = new enCuenta
                    {
                        sCodigoSiscredinka = oenSesion.valor6,
                        sNumeroCuenta = sNumeroCredito
                    };

                    /*Encripta la informacion*/
                    string ObjetoSerializado = new JavaScriptSerializer().Serialize(oenCuenta);
                    rnSegRSA ornSegRSA = new rnSegRSA();
                    string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                    /*********************/
                    List<enCronograma> loCronograma;


                    using (IwsPrestamoClient wsPrestamo = new IwsPrestamoClient())
                    {
                        loCronograma = wsPrestamo.WSObtenerCronograma(strEncriptado).ToList<enCronograma>(); ;
                    }

                    ViewBag.lstIMG = loCronograma.OrderBy(o => o.nCuota).ToList();
                    return PartialView("CronogramaPagos");
                }
                else
                {
                    return base.Redirect("PreLoad");
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "Los datos son incorrectos";
                return base.Redirect("PreLoad");
            }
        }

        #region IngresoSistema

        private string Logueo()
        {
            string sForm_contra = Request.Form["hddcod"];
            if (sForm_contra != null && sForm_contra.Equals("1"))
            {
                /*Envio de correo - Olvide mi contraseña*/
                if (ModelState.IsValid)
                {
                    int Resultado = EnvioCorreo();
                    if (Resultado == 1)
                    {
                        ViewBag.ruta = GenerarTeclado(0);
                        ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                        List<string> imgDimanic = ObtenerImagenes(1);
                        ViewBag.imgDimanic = imgDimanic;
                        ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                        TempData["Message"] = "Se le envió un correo electronico para poder cambiar su contraseña.";
                        ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                        ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                        return "Logueo";
                    }
                    else
                    {
                        ViewBag.ruta = GenerarTeclado(0);
                        ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                        List<string> imgDimanic = ObtenerImagenes(1);
                        ViewBag.imgDimanic = imgDimanic;
                        ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                        TempData["Message"] = "Los datos son incorrectos";
                        ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                        ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                        return "Logueo";
                    }
                }
                else
                {
                    ViewBag.ruta = GenerarTeclado(0);
                    ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                    List<string> imgDimanic = ObtenerImagenes(1);
                    ViewBag.imgDimanic = imgDimanic;
                    ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                    TempData["Message"] = "El texto de la imagen no es correcto";
                    ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                    ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                    return "Logueo";
                }
            }

            /*Session logueada*/
            var obj = Session["session01"];
            if (obj == null)
            {
                /*Validacion de Ingreso*/
                if (ModelState.IsValid)
                {
                    if (ValidarAccesos())
                    {
                        DestruirTeclado(0);
                        DestruirTeclado(1);

                        if (bCambioContra)
                        {
                            /*Redirecciona al principal*/
                            CargarMaster();
                            RedirectPrincipal();
                            return "Principal";
                        }
                        else
                        {
                            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("ClaveObligatorio", MvcApplication.sKey_Master);
                            ViewBag.ruta = GenerarTeclado(3);
                            return "ClaveObligatorio";
                        }
                    }
                    else
                    {
                        ViewBag.ruta = GenerarTeclado(0);
                        ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                        List<string> imgDimanic = ObtenerImagenes(1);
                        ViewBag.imgDimanic = imgDimanic;
                        ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                        ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                        ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                        return "Logueo";
                    }
                }
                else
                {
                    ViewBag.ruta = GenerarTeclado(0);
                    ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                    List<string> imgDimanic = ObtenerImagenes(1);
                    ViewBag.imgDimanic = imgDimanic;
                    ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                    TempData["Message"] = "El texto de la imagen no es correcto";
                    ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                    ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                    return "Logueo";
                }
            }
            else
            {
                string sUsuarioSerializado = "";
                if (!obj.Equals("")) {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);
                /*Ingreso correctamente - Validaciones dentro del sistema*/
                if (oenSesion.bCambioClave)
                {
                    /*Informacion para cargar el master*/
                    CargarMaster();
                    RedirectPrincipal();
                    return "Principal";
                }
                else
                {
                    ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("ClaveObligatorio", MvcApplication.sKey_Master);
                    ViewBag.ruta = GenerarTeclado(3);
                    return "ClaveObligatorio";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        private bool ValidarAccesos()
        {
            //Variables Globales
            string sPin = "", sNumTarjeta1 = "", sNumTarjeta2 = "", sNumTarjeta3 = "", sNumTarjeta4 = "", sPinPosiciones = "", sTarjeta = "", selectTarjeta = "";

            enSesion oenSesion = new enSesion();
            try
            {
                //Obtiene Datos Formulario
                sPinPosiciones = Request.Form["hddpin"];
                sNumTarjeta1 = Request.Form["txtcaja1"];
                sNumTarjeta2 = Request.Form["txtcaja2"];
                sNumTarjeta3 = Request.Form["txtcaja3"];
                sNumTarjeta4 = Request.Form["txtcaja4"];
                selectTarjeta = Request.Form["selectTarjeta"];

                sTarjeta = sNumTarjeta1 + sNumTarjeta2 + sNumTarjeta3 + sNumTarjeta4;
                sPin = ObtenerPin(0, sPinPosiciones);


                if (sTarjeta.Length != 16 || sPin.Length != 6)
                {
                    TempData["Message"] = "Los datos son incorrectos";
                    return false;
                }
                enUsuario poenUsuario = new enUsuario();
                poenUsuario.iTipoTarjeta = int.Parse(selectTarjeta);
                poenUsuario.sPing = sPin;
                poenUsuario.sNumeroTarjeta = sTarjeta;
                string userip = Request.UserHostAddress;
                poenUsuario.vAudIPCreacion = userip;
                poenUsuario.vAudMACCreacion = mac;

                /*Encripta la informacion*/
                string ObjetoSerializado = new JavaScriptSerializer().Serialize(poenUsuario);
                rnSegRSA ornSegRSA = new rnSegRSA();
                string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                /*********************/
                using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
                {
                    bCambioContra = true;
                    oenSesion = wsUsuario.WSValidaAccesoHomeBanking(strEncriptado);
                }

                if (oenSesion.iTipoResultado == 1)
                {
                    bCambioContra = oenSesion.bCambioClave;
                    /*Se guarda al usuario en sesion*/
                    ObjetoSerializado = new JavaScriptSerializer().Serialize(oenSesion);
                    string objEncriptado = utlFunciones.P_RJD_Encriptar(ObjetoSerializado, MvcApplication.sKey_Master);
                    Session["session01"] = objEncriptado;
                    return true;

                }
                else if (oenSesion.iTipoResultado == 3)
                {
                    /*El cliente no se encuentra afiliado*/
                    TempData["Message"] = "Los datos son incorrectos";
                    return false;
                }
                else
                {
                    TempData["Message"] = "Los datos son incorrectos";
                    return false;
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "Los datos son incorrectos";
                return false;
            }
        }

        /// <summary>
        /// Se implementa el codigo para envio de correo, caso olvido contrasena
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        private int EnvioCorreo()
        {
            int respuesta = -1;
            enUsuarioValida oenUsuario;
            try
            {
                string sTipoDocumento = "0", sNumeroDocumento = "", sCorreo = "";
                sTipoDocumento = Request.Form["selectTipo"];
                sNumeroDocumento = Request.Form["txtdocumento"];
                sCorreo = Request.Form["txtemail"];
                if (sTipoDocumento == null || sNumeroDocumento == null || sCorreo == null || sTipoDocumento.Equals("0") || sNumeroDocumento.Length == 0 || sCorreo.Length == 0)
                {
                    TempData["Message"] = "Los datos son incorrectos";
                    respuesta = -1;
                    return respuesta;
                }
                else
                {
                    oenUsuario = new enUsuarioValida();
                    oenUsuario.iTipoTarjeta = int.Parse(sTipoDocumento);
                    oenUsuario.sNumeroTarjeta = sNumeroDocumento;
                    oenUsuario.sEmail = sCorreo;

                    /*Encripta la informacion*/
                    string ObjetoSerializado = new JavaScriptSerializer().Serialize(oenUsuario);
                    rnSegRSA ornSegRSA = new rnSegRSA();
                    string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                    /*********************/

                    using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
                    {
                        respuesta = wsUsuario.WSOlvideMiClave(strEncriptado);
                    }
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "Los datos son incorrectos";
                return -1;
            }
        }

        #endregion

        #region Util

        /// <summary>
        /// Se genera el teclado Virtual
        /// </summary>
        /// <returns></returns>
        private ActionResult GenerarTeclado(int key)
        {
            string KEY = KEY_TEC + key;
            FileContentResult result;
            System.Drawing.Image imgFondo = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-fondo.jpg"), false);
            System.Drawing.Image imgLimpiar = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-limpiar.jpg"), false);
            System.Drawing.Image img1 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-1.jpg"), false);
            System.Drawing.Image img2 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-2.jpg"), false);
            System.Drawing.Image img3 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-3.jpg"), false);
            System.Drawing.Image img4 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-4.jpg"), false);
            System.Drawing.Image img5 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-5.jpg"), false);
            System.Drawing.Image img6 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-6.jpg"), false);
            System.Drawing.Image img7 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-7.jpg"), false);
            System.Drawing.Image img8 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-8.jpg"), false);
            System.Drawing.Image img9 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-9.jpg"), false);
            System.Drawing.Image img0 = System.Drawing.Image.FromFile(HttpContext.Server.MapPath("~/Content/Images/" + "img-numero-0.jpg"), false);

            List<System.Drawing.Image> lstImgs = new List<System.Drawing.Image>();
            lstImgs.Add(img0);
            lstImgs.Add(img1);
            lstImgs.Add(img2);
            lstImgs.Add(img3);
            lstImgs.Add(img4);
            lstImgs.Add(img5);
            lstImgs.Add(img6);
            lstImgs.Add(img7);
            lstImgs.Add(img8);
            lstImgs.Add(img9);

            int iHeight = 102;
            int iWidth = 87;
            int iHeight1 = 22;
            int iWidth1 = 25;
            int iHeight2 = 22;
            int iWidth2 = 52;

            Bitmap bitmap = new Bitmap(imgFondo, iWidth, iHeight);
            Graphics g = Graphics.FromImage(bitmap);
            List<int> theList = Enumerable.Range(0, 10).ToList();
            Random rng = new Random();
            int n = theList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = theList[k];
                theList[k] = theList[n];
                theList[n] = value;
            }

            g.DrawImage(lstImgs[theList[0]], 4, 4, iWidth1, iHeight1);
            g.DrawImage(lstImgs[theList[1]], 31, 4, iWidth1, iHeight1);
            g.DrawImage(lstImgs[theList[2]], 58, 4, iWidth1, iHeight1);
            g.DrawImage(lstImgs[theList[3]], 4, 28, iWidth1, iHeight1);
            g.DrawImage(lstImgs[theList[4]], 31, 28, iWidth1, iHeight1);
            g.DrawImage(lstImgs[theList[5]], 58, 28, iWidth1, iHeight1);
            g.DrawImage(lstImgs[theList[6]], 4, 52, iWidth1, iHeight1);
            g.DrawImage(lstImgs[theList[7]], 31, 52, iWidth1, iHeight1);
            g.DrawImage(lstImgs[theList[8]], 58, 52, iWidth1, iHeight1);
            g.DrawImage(imgLimpiar, 4, 76, iWidth2, iHeight2);
            g.DrawImage(lstImgs[theList[9]], 58, 76, iWidth1, iHeight1);

            string ObjetoSerializado = new JavaScriptSerializer().Serialize(theList);
            string objEncriptado = utlFunciones.P_RJD_Encriptar(ObjetoSerializado, MvcApplication.sKey_Master);
            Session[KEY] = objEncriptado;

            using (var memStream = new System.IO.MemoryStream())
            {
                bitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                result = this.File(memStream.ToArray(), "image/jpg");
                Response.Buffer = false;
                Response.Clear();
            }

            return result;
        }

        private string ObtenerPin(int Key, string sPosiciones)
        {
            string KEY = KEY_TEC + Key;
            string sPin = "";
            try
            {
                //Obtiene ListaSession
                var lobj = Session[KEY];
                if (lobj != null)
                {
                    string sListaSerializada = "";
                    if (!lobj.Equals("")) {
                        sListaSerializada = utlFunciones.P_RJD_Desencriptar(lobj.ToString(), MvcApplication.sKey_Master);
                    }
                    
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    List<int> ListaImgPin = serializer.Deserialize<List<int>>(sListaSerializada);
                    //ValidaPin
                    for (int i = 0; i < sPosiciones.Length; i++)
                    {
                        int valor = int.Parse(sPosiciones.Substring(i, 1));
                        sPin = sPin + ListaImgPin[valor];
                    }
                }

            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                    ex.Message.ToString());
            }
            return sPin;
        }

        private void DestruirTeclado(int KEY)
        {
            string LLAVE = KEY_TEC + KEY;
            Session[LLAVE] = null;
        }

        /// <summary>
        /// Obtiene las imagenes dinamicas para mostrar.
        /// </summary>
        /// <param name="iTipoImagen"></param>
        /// <returns></returns>
        private List<string> ObtenerImagenes(int iTipoImagen)
        {
            string sKey_file_server = System.Web.Configuration.WebConfigurationManager.AppSettings["urlFile"].ToString();
            List<string> imgDimanic = new List<string>();
            try
            {
                /* Imagenes Logueo == 1
                 * Banner-Bienvenido == 2
                 * bannerLateral == 3 
                 * bannerLateral_1 == 4
                 */
                using (IwsDescripcionClient wsDescripcion = new IwsDescripcionClient())
                {
                    imgDimanic = wsDescripcion.WSObtenerArchivosHomeBanking(iTipoImagen).ToList<string>();
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "No se pudieron cargar las imagenes.";
            }
            return imgDimanic;
        }

        private void CargarMaster()
        {
            try
            {
                bCambioContra = true;
                var obj = Session["session01"];
                string sUsuarioSerializado = "";
                if (!obj.Equals("")) {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

                /*Compra y venta: Tipo cambio*/
                enTipoCambio oenTipoCambio = new enTipoCambio();
                using (IwsTipoCambioClient wsTipoCambio = new IwsTipoCambioClient())
                {
                    oenTipoCambio = wsTipoCambio.WSObtenerTipoCambioDelDia();
                }

                bCambioContra = oenSesion.bCambioClave;
                ViewBag.nombre = oenSesion.valor1 + " " + oenSesion.valor2;
                ViewBag.apellidos = oenSesion.valor3 + " " + oenSesion.valor4;
                List<string> lstImg = ObtenerImagenes(2);
                ViewBag.bannerBienvenida = lstImg[0];
                lstImg = ObtenerImagenes(3);
                ViewBag.bannerLateral = lstImg[0];
                lstImg = ObtenerImagenes(4);
                ViewBag.bannerLateral_1 = lstImg[0];
                ViewBag.imagenContenido = oenSesion.valor7;
                ViewBag.sTipoCambioCompra = oenTipoCambio.nTipoCompraME.ToString("0.00", CultureInfo.InvariantCulture);
                ViewBag.sTipoCambioVenta = oenTipoCambio.nTipoVentaME.ToString("0.00", CultureInfo.InvariantCulture);
                ViewBag.sFechaCambio = "(" + oenTipoCambio.sFechaTipoCambio + ")";
                ViewBag.sUltimoAcceso = oenSesion.valor5;
                ViewBag.TimerSistema = MvcApplication.TimerSistema;
                ViewBag.TimerCaduca = MvcApplication.TimerCaduca;
                var year = DateTime.Now.Year;
                var month = (DateTime.Now.Month.ToString().Length < 2 ? "0" : "") + DateTime.Now.Month.ToString();
                var day = (DateTime.Now.Day.ToString().Length < 2 ? "0" : "") + DateTime.Now.Day.ToString();
                var hour = (DateTime.Now.Hour.ToString().Length < 2 ? "0" : "") + DateTime.Now.Hour.ToString();
                var Min = (DateTime.Now.Minute.ToString().Length < 2 ? "0" : "") + DateTime.Now.Minute.ToString();
                ViewBag.GetDate = day + "/" + month + "/" + year + " " + hour + ":" + Min;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<enDescripcion> ObtenerLista(int Cod, bool bCerrar = false)
        {
            try
            {
                List<enDescripcion> oenlista = new List<enDescripcion>();
                using (IwsDescripcionClient wsDescripcion = new IwsDescripcionClient())
                {
                    oenlista = wsDescripcion.WSListarValoresParametros(Cod, bCerrar).ToList<enDescripcion>();
                }
                return oenlista;
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                throw ex;
            }
        }

        private void CargarActualizarDatos()
        {
            var obj = Session["session01"];
            string sUsuarioSerializado = "";
            if (!obj.Equals("")) {
                sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
            }
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

            /*Obtiene los datos del cliente.*/
            enUsuario oenUsuario = new enUsuario();
            List<enUbigeo> list1 = new List<enUbigeo>();
            List<enUbigeo> list2 = new List<enUbigeo>();
            List<enUbigeo> list3 = new List<enUbigeo>();
            List<enUbigeo> list4 = new List<enUbigeo>();
            List<enUbigeo> list5 = new List<enUbigeo>();
            List<enUbigeo> list6 = new List<enUbigeo>();
            List<enUbigeo> list7 = new List<enUbigeo>();
            List<enDescripcion> lstTipoCuenta = new List<enDescripcion>();
            List<enDescripcion> lstEstadoCuenta = new List<enDescripcion>();
            List<enDescripcionSiscredinka> lstCargos = new List<enDescripcionSiscredinka>();
            List<enDescripcionSiscredinka> lstVia = new List<enDescripcionSiscredinka>();
            List<enDescripcionSiscredinka> lstZona = new List<enDescripcionSiscredinka>();
            using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
            {
                oenUsuario = wsUsuario.WSListarDatosCliente(oenSesion.valor6);
            }
            using (IwsUbigeoClient wsUbigeo = new IwsUbigeoClient())
            {
                list1 = wsUbigeo.WSObtenerDepartamentos().ToList<enUbigeo>();
                list2 = wsUbigeo.WSObtenerProvinciaPorDepartamentos(oenUsuario.sUbigeoNacimiento_Dep).ToList<enUbigeo>();
                list3 = wsUbigeo.WSObtenerDistritosPorProvincia(oenUsuario.sUbigeoNacimiento_Prov).ToList<enUbigeo>();
                list4 = wsUbigeo.WSObtenerProvinciaPorDepartamentos(oenUsuario.sUbigeoCasa_Dep).ToList<enUbigeo>();
                list5 = wsUbigeo.WSObtenerDistritosPorProvincia(oenUsuario.sUbigeoCasa_Prov).ToList<enUbigeo>();
                list6 = wsUbigeo.WSObtenerProvinciaPorDepartamentos(oenUsuario.sUbigeoLaboral_Dep).ToList<enUbigeo>();
                list7 = wsUbigeo.WSObtenerDistritosPorProvincia(oenUsuario.sUbigeoLaboral_Prov).ToList<enUbigeo>();
            }
            using (IwsDescripcionClient wsDescripcion = new IwsDescripcionClient())
            {
                /*Tipo de cuenta*/
                lstTipoCuenta = wsDescripcion.WSListarValoresParametros(5,false).ToList<enDescripcion>();
                /*Estado de cuenta*/
                lstEstadoCuenta = wsDescripcion.WSListarValoresParametros(4,false).ToList<enDescripcion>();
                /*Mensaje de Autorizacion*/
                ViewBag.TextoAutorizacion = wsDescripcion.WSListarValoresParametros(6,false).ToList<enDescripcion>();
                /*Mensaje de envio EECC*/
                ViewBag.EnvioEECC = wsDescripcion.WSListarValoresParametros(7,false).ToList<enDescripcion>();
                /*Lista cargos*/
                lstCargos = wsDescripcion.WSListarValoresParametrosSiscredinka(750).ToList<enDescripcionSiscredinka>();
                /*Lista Via*/
                lstVia = wsDescripcion.WSListarValoresParametrosSiscredinka(751).ToList<enDescripcionSiscredinka>();
                /*Lista Zona*/
                lstZona = wsDescripcion.WSListarValoresParametrosSiscredinka(752).ToList<enDescripcionSiscredinka>();

            }

            ViewBag.list1 = new SelectList(list1, "sCodigo", "sDescripcion", oenUsuario.sUbigeoNacimiento_Dep);
            ViewBag.list2 = new SelectList(list2, "sCodigo", "sDescripcion", oenUsuario.sUbigeoNacimiento_Prov);
            ViewBag.list3 = new SelectList(list3, "sCodigo", "sDescripcion", oenUsuario.sUbigeoNacimiento_Dist);
            ViewBag.list4 = new SelectList(list1, "sCodigo", "sDescripcion", oenUsuario.sUbigeoCasa_Dep);
            ViewBag.list5 = new SelectList(list4, "sCodigo", "sDescripcion", oenUsuario.sUbigeoCasa_Prov);
            ViewBag.list6 = new SelectList(list5, "sCodigo", "sDescripcion", oenUsuario.sUbigeoCasa_Dist);
            ViewBag.list7 = new SelectList(list1, "sCodigo", "sDescripcion", oenUsuario.sUbigeoLaboral_Dep);
            ViewBag.list8 = new SelectList(list6, "sCodigo", "sDescripcion", oenUsuario.sUbigeoLaboral_Prov);
            ViewBag.list9 = new SelectList(list7, "sCodigo", "sDescripcion", oenUsuario.sUbigeoLaboral_Dist);

            /*Agrega valores siscredinka*/
            lstVia.Add(new enDescripcionSiscredinka() { sValor = "0", sDescripcion = "Sin Vía" });
            lstZona.Add(new enDescripcionSiscredinka() { sValor = "0", sDescripcion = "Sin Tipo de Zona" });
            ViewBag.listV = new SelectList(lstVia, "sValor", "sDescripcion", oenUsuario.sCodigoTipoVia);
            ViewBag.listZ = new SelectList(lstZona, "sValor", "sDescripcion", oenUsuario.sCodigoTipoZona);
            ViewBag.Cargos = new SelectList(lstCargos, "sValor", "sDescripcion", oenUsuario.sCodigoCargo);
            ViewBag.TipoCuenta = lstTipoCuenta;
            ViewBag.EstadoCuenta = lstEstadoCuenta;
            ViewBag.poenUsuario = oenUsuario;
            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
            ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("ActualizarDatos", MvcApplication.sKey_Master);;
            ViewBag.ruta = GenerarTeclado(5);
            ViewBag.hddaction = "4";
        }

        #region Eventos HttpPost

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult end()
        {
            var parameterContainer = Request == null ? _requestBase : Request;
            if (Request.IsAjaxRequest())
            {
                Session.Clear();
                return null;
            }
            else
            {
                return Redirect("PreLoad");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetCodigoCIIU()
        {
            var objResult = new object();
            List<object> lobject = new List<object>();
            try
            {
                var parameterContainer = Request == null ? _requestBase : Request;
                string par1 = Request.Form["par1"];
                string par2 = Request.Form["par2"];
                if (Request.IsAjaxRequest())
                {
                    List<enDescripcionSiscredinka> list1 = new List<enDescripcionSiscredinka>();

                    using (IwsDescripcionClient wsdescripcion = new IwsDescripcionClient())
                    {
                        list1 = wsdescripcion.WSBusquedaActividadesEconomicasCIIU(par1, par2).ToList<enDescripcionSiscredinka>();
                    }
                    if (list1.Count > 0)
                    {
                        foreach (enDescripcionSiscredinka item in list1)
                        {
                            var objResult1 = new object();
                            objResult1 = new { valor1 = item.sValor, valor2 = item.sDescripcion };
                            lobject.Add(objResult1);
                        }
                    }
                    objResult = new { lst = lobject };
                    return Json(objResult);
                }
                else
                {
                    objResult = new { lst = lobject };
                    return Json(objResult);
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                objResult = new { lst = lobject };
                return Json(objResult);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetInformacionUbigeo()
        {
            var objResult = new object();
            try
            {
                var parameterContainer = Request == null ? _requestBase : Request;
                string par1 = Request.Form["par1"];
                string par2 = Request.Form["par2"];
                if (Request.IsAjaxRequest())
                {
                    List<enUbigeo> list1 = new List<enUbigeo>();
                    using (IwsUbigeoClient wsUbigeo = new IwsUbigeoClient())
                    {
                        if (par1.Equals("1"))
                        {
                            list1 = wsUbigeo.WSObtenerProvinciaPorDepartamentos(par2).ToList<enUbigeo>();
                        }
                        else
                        {
                            list1 = wsUbigeo.WSObtenerDistritosPorProvincia(par2).ToList<enUbigeo>();
                        }
                    }
                    string htmlCombo = "<option value='0'>Seleccione</option>";
                    if (list1.Count > 0)
                    {
                        foreach (enUbigeo item in list1)
                        {
                            htmlCombo = htmlCombo + "<option value='" + item.sCodigo + "'>" + item.sDescripcion + "</option>";
                        }
                    }
                    objResult = new { list1 = htmlCombo };
                    return Json(objResult);
                }
                else
                {
                    objResult = new { list1 = "" };
                    return Json(objResult);
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                objResult = new { list1 = "" };
                return Json(objResult);
            }
        }

        #endregion

        #endregion

        #region Mis Cuentas

        private void RedirectMisCuentas()
        {
            try
            {
                var obj = Session["session01"];
                string sUsuarioSerializado = "";
                if (!obj.Equals("")) {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

                /*Encripta la informacion*/
                rnSegRSA ornSegRSA = new rnSegRSA();
                string sValor1 = ornSegRSA.RSA_Encriptar(oenSesion.valor6);
                /*********************/

                List<enCuenta> loenCuenta = new List<enCuenta>();
                using (IwsCuentaClient wsCuenta = new IwsCuentaClient())
                {
                    loenCuenta = wsCuenta.WSObtenerCuentasPasivasCliente(sValor1).ToList<enCuenta>();
                }

                ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
                ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("Cuentas", MvcApplication.sKey_Master);
                ViewBag.hddaction = "2";
                ViewBag.ListaCuentas = loenCuenta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RedirectDetalleCuenta(string codCuenta)
        {
            var obj = Session["session01"];
            string sUsuarioSerializado = "";
            if (!obj.Equals("")) {
                sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
            }
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);
            enCuenta oenCuenta = new enCuenta();
            oenCuenta.sCodigoSiscredinka = oenSesion.valor6;
            oenCuenta.sNumeroCuenta = codCuenta;

            /*Encripta la informacion*/
            string ObjetoSerializado = new JavaScriptSerializer().Serialize(oenCuenta);
            rnSegRSA ornSegRSA = new rnSegRSA();
            string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
            /*********************/

            using (IwsCuentaClient wsCuenta = new IwsCuentaClient())
            {
                oenCuenta = wsCuenta.WSObtenerDetalleCuentasPasivasCliente(strEncriptado);
            }

            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
            ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("MisCuentasDetalle", MvcApplication.sKey_Master);
            ViewBag.enCuenta = oenCuenta;
            ViewBag.hddaction = "2";
        }

        private void RedirectDetallePrestamo(string codCuenta)
        {
            var obj = Session["session01"];
            string sUsuarioSerializado = "";
            if (!obj.Equals("")) {
                sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
            }
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);
            enCuenta oenCuenta = new enCuenta();
            oenCuenta.sCodigoSiscredinka = oenSesion.valor6;
            oenCuenta.sNumeroCuenta = codCuenta;

            /*Encripta la informacion*/
            string ObjetoSerializado = new JavaScriptSerializer().Serialize(oenCuenta);
            rnSegRSA ornSegRSA = new rnSegRSA();
            string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
            /*********************/

            enPrestamo oenPrestamo = new enPrestamo();

            using (IwsPrestamoClient wsPrestamo = new IwsPrestamoClient())
            {
                oenPrestamo = wsPrestamo.WSObtenerDetalleCreditosActivosCliente(strEncriptado);
            }
            Session["PrestamosDetalle"] = oenCuenta.sNumeroCuenta;
            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
            ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("MisPrestamosDetalle", MvcApplication.sKey_Master);
            ViewBag.enPrestamo = oenPrestamo;
            ViewBag.hddaction = "3";
        }


        #endregion

        #region Mis Prestamos

        private void RedirectObtenerPrestamos()
        {
            try
            {
                var obj = Session["session01"];
                string sUsuarioSerializado = "";
                if (!obj.Equals("")) {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

                /*Encripta la informacion*/
                rnSegRSA ornSegRSA = new rnSegRSA();
                string sValor1 = ornSegRSA.RSA_Encriptar(oenSesion.valor6);
                /*********************/

                List<enPrestamo> loenPrestamo = new List<enPrestamo>();
                using (IwsPrestamoClient wsPrestamo = new IwsPrestamoClient())
                {
                    loenPrestamo = wsPrestamo.WSObtenerCuentasActivasCliente(sValor1).ToList<enPrestamo>();
                }
                ViewBag.ListaPrestamos = loenPrestamo;
                ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
                ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("Prestamos", MvcApplication.sKey_Master);
                ViewBag.hddaction = "3";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        private void RedirectPrincipal()
        {
            RedirectMisCuentas();
            RedirectObtenerPrestamos();
            ViewBag.hddaction = "0";
            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
            ViewBag.form_interno = utlFunciones.P_RJD_Encriptar("Principal", MvcApplication.sKey_Master);
            ViewBag.sTituloOpcion = "Bienvenido,";
        }

        private string CambioClaveObligatoria()
        {
            string accion = Request.Form["hddcod"];
            if (accion.Equals("0"))
            {
                Session.Clear();
                ViewBag.ruta = GenerarTeclado(0);
                ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                List<string> imgDimanic = ObtenerImagenes(1);
                ViewBag.imgDimanic = imgDimanic;
                ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                TempData["Message"] = "Se cerró con éxito la sesión";
                ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                return "Logueo";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    bool b = false;
                    string ping1 = "", ping2 = "", ping3 = "";
                    ping1 = Request.Form["hddpin1"];
                    ping2 = Request.Form["hddpin2"];
                    ping3 = Request.Form["hddpin3"];

                    if (ping1.Replace(" ", "").Length != 6)
                    {
                        b = true;
                    }

                    if (ping2.Replace(" ", "").Length != 6)
                    {
                        b = true;
                    }

                    if (ping3.Replace(" ", "").Length != 6)
                    {
                        b = true;
                    }

                    if (!ping2.Equals(ping3))
                    {
                        b = true;
                    }

                    if (b)
                    {
                        ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("ClaveObligatorio", MvcApplication.sKey_Master);
                        ViewBag.ruta = GenerarTeclado(3);
                        TempData["Message"] = "Los datos son incorrectos";
                        return "ClaveObligatorio";
                    }
                    else
                    {
                        /*Restablecer clave*/
                        int iResultado = -1;

                        var obj = Session["session01"];
                        string sUsuarioSerializado = "";
                        if (!obj.Equals("")) {
                            sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                        }
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

                        enUsuarioValida poenUsuario = new enUsuarioValida();
                        poenUsuario.sPing = ObtenerPin(3, ping1);
                        poenUsuario.sPingNuevo = ObtenerPin(3, ping3);
                        poenUsuario.sCodigoCliente = oenSesion.valor6;
                        poenUsuario.vAudIPCreacion = ip;
                        poenUsuario.vAudMACCreacion = mac;

                        /*Encripta la informacion*/
                        string ObjetoSerializado = new JavaScriptSerializer().Serialize(poenUsuario);
                        rnSegRSA ornSegRSA = new rnSegRSA();
                        string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                        /*********************/

                        using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
                        {
                            iResultado = wsUsuario.WSCambiarClaveTarjeta(strEncriptado);
                        }

                        if (iResultado == 1)
                        {
                            Session.Clear();

                            ViewBag.ruta = GenerarTeclado(0);
                            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                            List<string> imgDimanic = ObtenerImagenes(1);
                            ViewBag.imgDimanic = imgDimanic;
                            ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                            TempData["Message"] = "Se cambio correctamente su clave, ingrese con su nueva clave.";
                            ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                            ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                            return "Logueo";
                        }
                        else
                        {
                            ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("ClaveObligatorio", MvcApplication.sKey_Master);
                            ViewBag.ruta = GenerarTeclado(3);
                            TempData["Message"] = "Los datos son incorrectos";
                            return "ClaveObligatorio";
                        }


                    }
                }
                else
                {
                    ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("ClaveObligatorio", MvcApplication.sKey_Master);
                    ViewBag.ruta = GenerarTeclado(3);
                    TempData["Message"] = "El texto de la imagen no es correcto";
                    return "ClaveObligatorio";
                }
            }
        }

        private string action_click_Principal()
        {
            try
            {
                string action = Request.Form["hddevent1"] == null ? "" : Request.Form["hddevent1"];
                /*Mis cuentas*/
                if (action.Equals("1"))
                {
                    return action_click_DetalleCuenta();
                }
                /*Mis Prestamos*/
                else if (action.Equals("2"))
                {
                    return action_click_DetallePrestamo();
                }
                /*Imprimir*/
                else if (action.Equals("3"))
                {
                    CargarMaster();
                    RedirectPrincipal();
                    return "Principal";
                }
                /*PDF*/
                else if (action.Equals("4"))
                {
                    var obj = Session["session01"];
                    string sUsuarioSerializado = "";
                    if (!obj.Equals("")) {
                        sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                    }
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);


                    using (MemoryStream ms = new MemoryStream())
                    using (Document document = new Document(PageSize.A4, 30, 30, 40, 40))
                    using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                    {
                        Exportar exportar = new Exportar();
                        exportar.ExportarPDFPrincipal(document, writer, oenSesion.valor6, oenSesion.valor1 + " " + oenSesion.valor2 + "" + oenSesion.valor3 + " " + oenSesion.valor4);
                        document.Close();
                        writer.Close();
                        ms.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("Mi_Informacion_{0}", DateTime.Now.ToString("ddMMyyyyHHmmss")) + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    }

                    CargarMaster();
                    RedirectPrincipal();
                    return "Principal";
                }
                else
                {
                    CargarMaster();
                    RedirectPrincipal();
                    return "Principal";
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "Ocurrio un problemar al procesar la información";
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
        }

        private string action_click_CambioClave()
        {
            var obj = Session["session01"];
            bool b = false;
            string ping1 = "", ping2 = "", ping3 = "";
            ping1 = Request.Form["hddpin1"];
            ping2 = Request.Form["hddpin2"];
            ping3 = Request.Form["hddpin3"];

            if (ping1.Replace(" ", "").Length != 6)
            {
                b = true;
            }

            if (ping2.Replace(" ", "").Length != 6)
            {
                b = true;
            }

            if (ping3.Replace(" ", "").Length != 6)
            {
                b = true;
            }

            if (!ping2.Equals(ping3))
            {
                b = true;
            }

            if (b)
            {
                TempData["Message"] = "Los datos son incorrectos";
                ViewBag.ruta = GenerarTeclado(0);
                ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
                ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("CambiaClave", MvcApplication.sKey_Master);
                ViewBag.hddaction = "5";
                return "CambioContrasena";
            }
            else
            {
                /*Restablecer clave*/
                int iResultado = -1;
                string sUsuarioSerializado = "";
                if (!obj.Equals("")) {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

                enUsuarioValida poenUsuario = new enUsuarioValida();
                poenUsuario.sPing = ObtenerPin(0, ping1);
                poenUsuario.sPingNuevo = ObtenerPin(0, ping3);
                poenUsuario.sCodigoCliente = oenSesion.valor6;
                poenUsuario.vAudIPCreacion = ip;
                poenUsuario.vAudMACCreacion = mac;

                /*Encripta la informacion*/
                string ObjetoSerializado = new JavaScriptSerializer().Serialize(poenUsuario);
                rnSegRSA ornSegRSA = new rnSegRSA();
                string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                /*********************/

                using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
                {
                    iResultado = wsUsuario.WSCambiarClaveTarjeta(strEncriptado);
                }

                if (iResultado == 1)
                {
                    Session.Clear();

                    ViewBag.ruta = GenerarTeclado(0);
                    ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Logueo", MvcApplication.sKey_Master);
                    List<string> imgDimanic = ObtenerImagenes(1);
                    ViewBag.imgDimanic = imgDimanic;
                    ViewBag.key_TimerCerrar = sKey_TimerCerrar;
                    TempData["Message"] = "Se cambio correctamente su clave, ingrese con su nueva clave.";
                    ViewBag.lista1 = new SelectList(ObtenerLista(1), "iValor1", "sNombre");
                    ViewBag.lista2 = new SelectList(ObtenerLista(2), "iValor1", "sNombre");
                    return "Logueo";
                }
                else
                {
                    TempData["Message"] = "Los datos son incorrectos";
                    ViewBag.ruta = GenerarTeclado(0);
                    ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
                    ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("CambiaClave", MvcApplication.sKey_Master);
                    ViewBag.hddaction = "5";
                    return "CambioContrasena";
                }
            }
        }

        private string action_click_ActualizarDatos()
        {
            var obj = Session["session01"];
            /*Validacion para actualizacion de informacion*/
            string sApePat = "", sApeMat = "", sNombre = "", snombre2 = "", sFecNac = "", sUbigeoNac_Dep = "", sUbigeoNac_Pro = "",
            sUbigeoNac_Dis = "", sTipoVia = "", sDireccion = "", sNroDir = "", sReferencia = "", sTipoZona = "", sValorZona = "",
            sUbigeoDir_Dep = "", sUbigeoDir_Pro = "", sUbigeoDir_Dis = "", sEmail = "", sNumero = "", sTipoRelacion = "", sCodigoCIIU = "",
            sFechaInicioAI = "", sRUCI = "", sDireccionNeg = "", sUbigeoNeg_Dep = "", sUbigeoNeg_Pro = "", sUbigeoNeg_Dis = "",
            sTelefonoNeg1 = "", sTelefonoNeg2 = "", sRazonSocial = "", sSelectCargo = "", sPing = "", CargoP = "", txtDetalleCargo = "",
            txtDetalleInstitucion = "", sAutUsoDatos = "", sTipoCuentaUsuario = "", sRecibirTipoCuenta = "", sMedioRecibir = "";
            bool result = false;

            sNombre = Request.Form["txtNombre"];
            snombre2 = Request.Form["txtNombre1"];
            sApePat = Request.Form["txtApep"];
            sApeMat = Request.Form["txtApem"];
            sFecNac = Request.Form["txtfecNac"];
            sUbigeoNac_Dep = Request.Form["selectNacDep"];
            sUbigeoNac_Pro = Request.Form["selectNacPro"];
            sUbigeoNac_Dis = Request.Form["selectNacDis"];
            sTipoVia = Request.Form["selectTipoVia"];
            sDireccion = Request.Form["txtDirP"];
            sNroDir = Request.Form["txtnro"];
            sReferencia = Request.Form["txtRefP"];
            sTipoZona = Request.Form["selectTipoZona"];
            sValorZona = Request.Form["txtZona"];
            sUbigeoDir_Dep = Request.Form["selectDirDep"];
            sUbigeoDir_Pro = Request.Form["selectDirPro"];
            sUbigeoDir_Dis = Request.Form["selectDirDis"];
            sEmail = Request.Form["txtEmail"];
            sNumero = Request.Form["txtNumero"];
            sTipoRelacion = Request.Form["RelacionIngreso"];
            sPing = Request.Form["hddpin"];

            /*Independiente*/
            if (sTipoRelacion.Equals("01"))
            {
                sCodigoCIIU = Request.Form["txthActvGiroI"];
                sFechaInicioAI = Request.Form["fecInicioAI"];
                sRUCI = Request.Form["txtRUCI"];
                sDireccionNeg = Request.Form["txtdirNegI"];
                sUbigeoNeg_Dep = Request.Form["selectDirLDepI"];
                sUbigeoNeg_Pro = Request.Form["selectDirLProI"];
                sUbigeoNeg_Dis = Request.Form["selectDirLDisI"];
                sTelefonoNeg1 = Request.Form["fono1I"];
                sTelefonoNeg2 = Request.Form["fono2I"];

            }
            /*Dependiente*/
            else if (sTipoRelacion.Equals("02"))
            {
                sRazonSocial = Request.Form["txtRazon"];
                sSelectCargo = Request.Form["selectCargos"];
                sDireccionNeg = Request.Form["txtdirNeg"];
                sUbigeoNeg_Dep = Request.Form["selectDirLDep"];
                sUbigeoNeg_Pro = Request.Form["selectDirLPro"];
                sUbigeoNeg_Dis = Request.Form["selectDirLDis"];
                sTelefonoNeg1 = Request.Form["fono1"];
                sTelefonoNeg2 = Request.Form["fono2"];
                sFechaInicioAI = Request.Form["fecIngreso"];
            }
            /*Otro -> Error */
            else
            {
                TempData["Message"] = "Los datos son incorrectos";
                CargarActualizarDatos();
                return "ActualizarDatos";
            }

            sAutUsoDatos = Request.Form["AutUso"];
            sTipoCuentaUsuario = Request.Form["CuentaUsuario"];
            sRecibirTipoCuenta = Request.Form["EstadoCuenta"];
            sMedioRecibir = Request.Form["RecibirCuenta"];

            if (sTipoCuentaUsuario == null || sRecibirTipoCuenta == null)
            {
                TempData["Message"] = "Los datos son incorrectos";
                CargarActualizarDatos();
                return "ActualizarDatos";
            }

            if (sTipoCuentaUsuario.Equals("4"))
            {
                sRecibirTipoCuenta = "4";
                sMedioRecibir = "0";
            }

            if (sRecibirTipoCuenta.Equals("4"))
            {
                sMedioRecibir = "0";
            }

            CargoP = Request.Form["CargoP"];
            txtDetalleCargo = Request.Form["txtDetalleCargo"];
            txtDetalleInstitucion = Request.Form["txtDetalleInstitucion"];

            if (sNombre == null || snombre2 == null || sApePat == null || sApeMat == null || sFecNac == null || sUbigeoNac_Dep == null ||
                sUbigeoNac_Pro == null || sUbigeoNac_Dis == null || sTipoVia == null || sDireccion == null || sNroDir == null || sReferencia == null ||
                sTipoZona == null || sValorZona == null || sUbigeoDir_Dep == null || sUbigeoDir_Dep == null || sUbigeoDir_Pro == null ||
                sUbigeoDir_Dis == null || sEmail == null || sNumero == null || sTipoRelacion == null || sCodigoCIIU == null ||
                sFechaInicioAI == null || sRUCI == null || sDireccionNeg == null || sUbigeoNeg_Dep == null || sUbigeoNeg_Pro == null ||
                sUbigeoNeg_Dis == null || sTelefonoNeg1 == null || sTelefonoNeg2 == null || sRazonSocial == null || sSelectCargo == null ||
                sPing == null || CargoP == null || txtDetalleCargo == null || txtDetalleInstitucion == null || sAutUsoDatos == null ||
                sTipoCuentaUsuario == null || sRecibirTipoCuenta == null || sMedioRecibir == null)
            {
                TempData["Message"] = "Los datos son incorrectos";
                CargarActualizarDatos();
                return "ActualizarDatos";
            }
            string sUsuarioSerializado = "";
            if (!obj.Equals("")) {
                sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

            enUsuario poenUsuario = new enUsuario();
            poenUsuario.sApellidoPaterno = sApePat;
            poenUsuario.sApellidoMaterno = sApeMat;
            poenUsuario.sPrimerNombre = sNombre;
            poenUsuario.sSegundoNombre = snombre2;
            poenUsuario.sFechaNacimiento = sFecNac;
            poenUsuario.sUbigeoLaboral_Dist = sUbigeoNac_Dis;
            poenUsuario.sDireccion = sDireccion;
            poenUsuario.sReferenciaUbicacion = sReferencia;
            poenUsuario.sUbigeoCasa_Dist = sUbigeoDir_Dis;
            poenUsuario.sEmail = sEmail;
            poenUsuario.nNumeroCasa = decimal.Parse(sNumero);
            poenUsuario.sCodigoRelacionLaboral = sTipoRelacion;
            poenUsuario.sCodigoCIIU = sCodigoCIIU;
            poenUsuario.sRazonSocial = sRazonSocial;
            poenUsuario.sRuc = sRUCI;
            poenUsuario.sCodigoCargo = sSelectCargo;
            poenUsuario.sFechaIngreso = sFechaInicioAI;
            poenUsuario.sDireccionLaboral = sDireccionNeg;
            poenUsuario.sUbigeoLaboral_Dist = sUbigeoNeg_Dis;
            poenUsuario.nTelefonoLaboral1 = decimal.Parse(sTelefonoNeg1);
            poenUsuario.nTelefonoLaboral2 = decimal.Parse(sTelefonoNeg2);
            poenUsuario.iIdentificarPep = int.Parse(CargoP);
            poenUsuario.iIdentificarAut = int.Parse(sAutUsoDatos);
            poenUsuario.sCodigoSisCredinkaCliente = oenSesion.valor6;
            poenUsuario.vAudIPCreacion = ip;
            poenUsuario.vAudMACCreacion = mac;
            poenUsuario.iCorrespondencia = int.Parse(sMedioRecibir);
            poenUsuario.iEnvioCuenta = int.Parse(sTipoCuentaUsuario);
            poenUsuario.iEnvioReciboEECC = int.Parse(sRecibirTipoCuenta);
            poenUsuario.sPing = ObtenerPin(5, sPing);
            poenUsuario.sPepOcupacion = txtDetalleCargo;
            poenUsuario.sPepOrganismo = txtDetalleInstitucion;
            poenUsuario.sCodigoTipoVia = sTipoVia;
            poenUsuario.sCodigoTipoZona = sTipoZona;
            poenUsuario.sNombreZona = sValorZona;
            poenUsuario.sNroVia = sNroDir;

            /*Encripta la informacion*/
            string ObjetoSerializado = new JavaScriptSerializer().Serialize(poenUsuario);
            rnSegRSA ornSegRSA = new rnSegRSA();
            string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
            /*********************/

            using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
            {
                result = wsUsuario.WSActualizarInformacion(ObjetoSerializado);
            }
            if (result)
            {
                TempData["Message"] = "Se actualizó su información correctamente.";
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
            else
            {
                TempData["Message"] = "Los datos son incorrectos";
                CargarActualizarDatos();
                return "ActualizarDatos";
            }
        }

        private string Action_click_CronogramaPagos()
        {
            try
            {
                var obj = Session["session01"];
                string valor = Request.Form["select"];
                string sUsuarioSerializado = "";
                if (!obj.Equals(""))
                {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);
                enSesion poenSesion = new enSesion
                {
                    valor1 = oenSesion.valor6,
                    valor2 = valor,
                    valor3 = ip,
                    valor4 = mac
                };
                string sRuta = "";

                /*Encripta la informacion*/
                string ObjetoSerializado = new JavaScriptSerializer().Serialize(poenSesion);
                rnSegRSA ornSegRSA = new rnSegRSA();
                string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                /*********************/

                using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
                {
                    sRuta = wsUsuario.WSActualizarSelloSeguridad(strEncriptado);
                }
                if (sRuta == null || sRuta == "")
                {
                    TempData["Message"] = "La información no es correcta";
                }
                else
                {
                    oenSesion.valor7 = sRuta;
                    /*Se guarda al usuario en sesion*/
                    ObjetoSerializado = new JavaScriptSerializer().Serialize(oenSesion);
                    string objEncriptado = utlFunciones.P_RJD_Encriptar(ObjetoSerializado, MvcApplication.sKey_Master);
                    Session["session01"] = objEncriptado;
                    ViewBag.imagenContenido = sRuta;
                    TempData["Message"] = "Se actualizó correctamente su seguridad";
                }
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                    ex.Message.ToString());
                TempData["Message"] = "La información no es correcta";
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
        }

        private string action_click_CambioSello()
        {
            try
            {
                var obj = Session["session01"];
                string valor = Request.Form["select"];
                string sUsuarioSerializado = "";
                if (!obj.Equals("")) {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);
                enSesion poenSesion = new enSesion();
                poenSesion.valor1 = oenSesion.valor6;
                poenSesion.valor2 = valor;
                poenSesion.valor3 = ip;
                poenSesion.valor4 = mac;
                string sRuta = "";

                /*Encripta la informacion*/
                string ObjetoSerializado = new JavaScriptSerializer().Serialize(poenSesion);
                rnSegRSA ornSegRSA = new rnSegRSA();
                string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                /*********************/

                using (IwsUsuarioClient wsUsuario = new IwsUsuarioClient())
                {
                    sRuta = wsUsuario.WSActualizarSelloSeguridad(strEncriptado);
                }
                if (sRuta == null || sRuta == "")
                {
                    TempData["Message"] = "La información no es correcta";
                }
                else
                {
                    oenSesion.valor7 = sRuta;
                    /*Se guarda al usuario en sesion*/
                    ObjetoSerializado = new JavaScriptSerializer().Serialize(oenSesion);
                    string objEncriptado = utlFunciones.P_RJD_Encriptar(ObjetoSerializado, MvcApplication.sKey_Master);
                    Session["session01"] = objEncriptado;
                    ViewBag.imagenContenido = sRuta;
                    TempData["Message"] = "Se actualizó correctamente su seguridad";
                }
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                    ex.Message.ToString());
                TempData["Message"] = "La información no es correcta";
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
        }

        private string action_click_DetalleCuenta()
        {
            try
            {
                string action = Request.Form["hddevent1"] == null ? "" : Request.Form["hddevent1"];
                string codCuenta = Request.Form["hddevent2"] == null ? "" : Request.Form["hddevent2"];
                /*Mis cuentas*/
                if (action.Equals("1"))
                {
                    CargarMaster();
                    if (codCuenta == null)
                    {
                        RedirectPrincipal();
                        return "Principal";
                    }
                    RedirectDetalleCuenta(codCuenta);
                    return "MisCuentasDetalle";
                }
                /*Imprimir*/
                else if (action.Equals("3"))
                {
                    CargarMaster();
                    RedirectPrincipal();
                    return "Principal";
                }
                /*PDF*/
                else if (action.Equals("4"))
                {
                    var obj = Session["session01"];
                    string sUsuarioSerializado = "";
                    if (!obj.Equals("")) {
                        sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                    }
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);


                    using (MemoryStream ms = new MemoryStream())
                    using (Document document = new Document(PageSize.A4, 30, 30, 40, 40))
                    using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                    {
                        Exportar exportar = new Exportar();
                        exportar.ExportarPDFMisCuentas(document, writer, oenSesion.valor6, oenSesion.valor1 + " " + oenSesion.valor2 + "" + oenSesion.valor3 + " " + oenSesion.valor4);
                        document.Close();
                        writer.Close();
                        ms.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("Mis_Cuentas_{0}", DateTime.Now.ToString("ddMMyyyyHHmmss")) + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    }

                    CargarMaster();
                    RedirectPrincipal();
                    return "Principal";
                }
                /*Excel*/
                else if (action.Equals("5"))
                {

                    CargarMaster();
                    RedirectPrincipal();
                    return "Principal";
                }
                else
                {
                    CargarMaster();
                    RedirectPrincipal();
                    return "Principal";
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "Ocurrio un problemar al procesar la información";
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
        }

        public string action_click_MovimientosCuenta()
        {
            try
            {
                var obj = Session["session01"];
                if (obj == null)
                {
                    TempData["Message"] = "Al parecer su tiempo de sesión expiró.";
                    return "PreLoad";
                }
                string sUsuarioSerializado = "";
                if (!obj.Equals("")) {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

                string codCuenta = Request.Form["hddevent1"];
                string codAccion = Request.Form["hddevent2"];
                string NumeroRegistros = Request.Form["selectTotal"];
                if (codCuenta == null || codCuenta.Length == 0)
                {
                    TempData["Message"] = "Datos Incorrectos.";
                    CargarMaster();
                    RedirectPrincipal();
                    return "Principal";
                }
                /*Ver movimientos*/
                if (codAccion.Equals("1"))
                {
                    List<enCuenta> loencuenta = new List<enCuenta>();
                    enCuenta oenCuenta = new enCuenta();
                    oenCuenta.sCodigoSiscredinka = oenSesion.valor6;
                    oenCuenta.sNumeroCuenta = codCuenta;
                    oenCuenta.NumeroRegistros = int.Parse(NumeroRegistros);


                    /*Encripta la informacion*/
                    string ObjetoSerializado = new JavaScriptSerializer().Serialize(oenCuenta);
                    rnSegRSA ornSegRSA = new rnSegRSA();
                    string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                    /*********************/

                    using (IwsCuentaClient wsCuenta = new IwsCuentaClient())
                    {
                        loencuenta = wsCuenta.WSObtenerMovimientosCuentaPasivasCliente(strEncriptado).ToList<enCuenta>();
                    }
                    using (IwsCuentaClient wsCuenta = new IwsCuentaClient())
                    {
                        oenCuenta = wsCuenta.WSObtenerDetalleCuentasPasivasCliente(strEncriptado);
                    }
                    ViewBag.enCuenta = oenCuenta;
                    ViewBag.form_validate = utlFunciones.P_RJD_Encriptar("Menu", MvcApplication.sKey_Master);
                    ViewBag.Form_Interno = utlFunciones.P_RJD_Encriptar("MisCuentasDetalle", MvcApplication.sKey_Master);
                    ViewBag.lstDetalleCuenta = loencuenta;
                    ViewBag.hddaction = "2";
                    List<object> lista = new List<object>();
                    lista.Add(new { valor = "5", descripcion = "Últimos 5" });
                    lista.Add(new { valor = "10", descripcion = "Últimos 10" });
                    lista.Add(new { valor = "20", descripcion = "Últimos 20" });
                    ViewBag.list = new SelectList(lista, "valor", "descripcion", NumeroRegistros);
                    CargarMaster();
                    return "MisCuentasDetalle";
                }
                /*Imprimir*/
                else if (codAccion.Equals("3"))
                {
                    CargarMaster();
                    RedirectDetalleCuenta(codCuenta);
                    return "MisCuentasDetalle";
                }
                /*PDF*/
                else if (codAccion.Equals("4"))
                {
                    string accion_exportar = Request.Form["hdc_f"];
                    using (MemoryStream ms = new MemoryStream())
                    using (Document document = new Document(PageSize.A4, 30, 30, 40, 40))
                    using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                    {
                        Exportar exportar = new Exportar();
                        if (accion_exportar.Equals("1"))
                        {
                            exportar.ExportarPDFMovimientos(document, writer, oenSesion.valor6, oenSesion.valor1 + " " + oenSesion.valor2 + "" + oenSesion.valor3 + " " + oenSesion.valor4, codCuenta, NumeroRegistros);
                            Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("Movimientos_Cuenta_{0}", DateTime.Now.ToString("ddMMyyyyHHmmss")) + ".pdf");
                        }
                        else
                        {
                            exportar.ExportarPDFDetalleCuenta(document, writer, oenSesion.valor6, oenSesion.valor1 + " " + oenSesion.valor2 + "" + oenSesion.valor3 + " " + oenSesion.valor4, codCuenta);
                            Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("Detalle_Cuenta_{0}", DateTime.Now.ToString("ddMMyyyyHHmmss")) + ".pdf");
                        }
                        document.Close();
                        writer.Close();
                        ms.Close();
                        Response.ContentType = "pdf/application";
                        
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    }
                    CargarMaster();
                    RedirectDetalleCuenta(codCuenta);
                    return "MisCuentasDetalle";
                }
                /*Excel*/
                else if (codAccion.Equals("5"))
                {
                    CargarMaster();
                    RedirectDetalleCuenta(codCuenta);
                    return "MisCuentasDetalle";
                }
                else
                {
                    CargarMaster();
                    RedirectDetalleCuenta(codCuenta);
                    return "MisCuentasDetalle";
                }
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "Datos Incorrectos.";
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
        }

        private string action_click_DetallePrestamo()
        {
            string action = Request.Form["hddevent1"] == null ? "" : Request.Form["hddevent1"];
            string codCuenta = Request.Form["hddevent2"] == null ? "" : Request.Form["hddevent2"];
            /*Mis Prestamos*/
            if (action.Equals("2"))
            {
                CargarMaster();
                if (codCuenta == null)
                {
                    RedirectPrincipal();
                    return "Principal";
                }
                RedirectDetallePrestamo(codCuenta);
                return "MisPrestamosDetalle";
            }
            /*Imprimir*/
            else if (action.Equals("3"))
            {
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
            /*PDF*/
            else if (action.Equals("4"))
            {
                var obj = Session["session01"];
                string sUsuarioSerializado = "";
                if (obj != null || !obj.Equals("")) {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

                using (MemoryStream ms = new MemoryStream())
                using (Document document = new Document(PageSize.A4, 30, 30, 40, 40))
                using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                {
                    Exportar exportar = new Exportar();
                    exportar.ExportarPDFMisPrestamos(document, writer, oenSesion.valor6, oenSesion.valor1 + " " + oenSesion.valor2 + "" + oenSesion.valor3 + " " + oenSesion.valor4);
                    document.Close();
                    writer.Close();
                    ms.Close();
                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("Mis_Prestamos_{0}", DateTime.Now.ToString("ddMMyyyyHHmmss")) + ".pdf");
                    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                }
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
            /*Excel*/
            else if (action.Equals("5"))
            {
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
            else
            {
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileContentResult ExportarInformacion()
        {
            FileContentResult fcResult;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                using (Document document = new Document(PageSize.A4, 30, 30, 40, 40))
                using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                {
                    StringBuilder sbCAdena = new StringBuilder();
                    StringBuilder sbHead = new StringBuilder();
                    string valueText = Request.Form["hddevent2"];
                    string NameCli = Request.Form["hddevent1"];
                    var urlLocalhost = Request.Url.Authority;
                    var docRecurse = "";
                    docRecurse += "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' /><!DOCTYPE html>";
                    docRecurse += "<html>";
                    docRecurse += "<head>";
                    docRecurse += "<style>";
                    docRecurse += " hr {color: #f00;background-color: #00008B;height: 1px; width:600px;}";
                    docRecurse += "</style>";
                    docRecurse += "</head>";
                    docRecurse += "<body style='background-color:#ffffff;margin:10px;'>";
                    docRecurse += "<table>";
                    docRecurse += "<colgroup>";
                    docRecurse += "<col width='30%'>";
                    docRecurse += "<col width='15%'>";
                    docRecurse += "<col width='15%'>";
                    docRecurse += "<col width='40%'>";
                    docRecurse += "</colgroup>";
                    docRecurse += "<tr>";
                    docRecurse += "<td>";
                    docRecurse += "<img class='css-banner-image' src='http://10.10.113.75/Homebanking/Content/Images/logo-intranet.png'>";
                    docRecurse += "</td>";
                    docRecurse += "<td>";
                    docRecurse += "</td>";
                    docRecurse += "<td>";
                    docRecurse += "</td>";
                    docRecurse += "<td colspan='2'>";
                    docRecurse += "<span style='color: #fc8104;float: right;margin-top: 17px;font-size: 30px;'>Banca por Internet</span>";
                    docRecurse += "</td>";
                    docRecurse += "</tr>";
                    docRecurse += "</table>";
                    docRecurse += "<div>";
                    docRecurse += "</div>";
                    docRecurse += "<div class='css-margin-top'>";
                    docRecurse += "<span class='fa-stack'>";
                    docRecurse += "<i class='fa fa-square-o fa-stack-2x' style='color:#EA6C1D;'></i>";
                    docRecurse += "<i class='fa fa-square fa-stack-1x' style='color:#EA6C1D;'></i>";
                    docRecurse += "</span><span class='css-text-title-option' style='font-size:10px;'>DATOS CLIENTE</span>";
                    docRecurse += "</div>";
                    docRecurse += "<div>";
                    docRecurse += "<table class='table-responsive css-table-detalle'>";
                    docRecurse += "<colgroup>";
                    docRecurse += "<col width='30%'>";
                    docRecurse += "<col width='70%'>";
                    docRecurse += "</colgroup>";
                    docRecurse += "<tbody>";
                    docRecurse += "<tr>";
                    docRecurse += "<td class='cabecera' style='font-size:9px;'>Nombre del cliente</td>";
                    docRecurse += "<td colspan='3' style='font-size:9px;'>"+NameCli+"</td>";
                    docRecurse += "</tr>";
                    docRecurse += "</tbody>";
                    docRecurse += "</table>";
                    docRecurse += "</div>";
                    docRecurse += (valueText).Replace("[", "<").Replace("]", ">");
                    docRecurse += "</div>";
                    docRecurse += "</body>";
                    docRecurse += "</html>";

                    sbCAdena.Append(docRecurse);

                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("Informacion_Datos_{0}", DateTime.Now.ToString("ddMMyyyyHHmmss")) + ".xls");
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.Output.Write(sbCAdena.ToString());
                    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    fcResult = this.File(ms.ToArray(), "application/vnd.ms-excel");
                    Response.Flush();
                    Response.End();
                }

                return fcResult;
            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                TempData["Message"] = "No se encuentra el archivo";
                byte[] bFile = new byte[0];
                fcResult = File(bFile, System.Net.Mime.MediaTypeNames.Application.Octet, "preguntas-frecuentes.pdf");
                return fcResult;
            }
        }

        private string action_click_CuentaExportar()
        {
            string action = Request.Form["hddevent1"] == null ? "" : Request.Form["hddevent1"];
            string codCuenta = Request.Form["hddevent2"] == null ? "" : Request.Form["hddevent2"];
            /*Imprimir*/
            if (action.Equals("3"))
            {
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
            /*PDF*/
            else if (action.Equals("4"))
            {
                var obj = Session["session01"];
                string sUsuarioSerializado = "";
                if (obj != null || !obj.Equals("")) {
                    sUsuarioSerializado = utlFunciones.P_RJD_Desencriptar(obj.ToString(), MvcApplication.sKey_Master);
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                enSesion oenSesion = serializer.Deserialize<enSesion>(sUsuarioSerializado);

                using (MemoryStream ms = new MemoryStream())
                using (Document document = new Document(PageSize.A4, 30, 30, 40, 40))
                using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                {
                    Exportar exportar = new Exportar();
                    exportar.ExportarPDFDetallePrestamo(document, writer, oenSesion.valor6, oenSesion.valor1 + " " + oenSesion.valor2 + "" + oenSesion.valor3 + " " + oenSesion.valor4, codCuenta);
                    document.Close();
                    writer.Close();
                    ms.Close();
                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("Detalle_Prestamo_{0}", DateTime.Now.ToString("ddMMyyyyHHmmss")) + ".pdf");
                    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                }
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
            /*Excel*/
            else if (action.Equals("5"))
            {
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
            else
            {
                CargarMaster();
                RedirectPrincipal();
                return "Principal";
            }
        }


    }
}