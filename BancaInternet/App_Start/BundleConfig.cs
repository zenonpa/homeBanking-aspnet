using System.Web;
using System.Web.Optimization;

namespace BancaInternet
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, consulte http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Ingreso
            bundles.Add(new ScriptBundle("~/credinka/jsIngreso").Include(
                "~/Content/Scripts/jquery-2.1.4.js",
                "~/Content/Scripts/bootstrap.js",
                 "~/Content/Scripts/login.js"));
 
            bundles.Add(new StyleBundle("~/credinka/cssIngreso").Include(
                "~/Content/Styles/bootstrap.css",
                "~/Content/Styles/font-awesome/css/font-awesome.css",
                "~/Content/Styles/login.css"));

            //Principal
            bundles.Add(new StyleBundle("~/credinka/cssGeneral").Include(
                "~/Content/Styles/bootstrap.css",
                "~/Content/Styles/font-awesome/css/font-awesome.css",
                "~/Content/Styles/master.css"));

            bundles.Add(new ScriptBundle("~/credinka/jsGeneral").Include(
                "~/Content/Scripts/jquery-2.1.4.js",
                "~/Content/Scripts/bootstrap.js",
                 "~/Content/Scripts/index.js"));

            //ClaveObligatorio
            bundles.Add(new ScriptBundle("~/credinka/jsCambio").Include(
                "~/Content/Scripts/jquery-2.1.4.js",
                "~/Content/Scripts/bootstrap.js",
                 "~/Content/Scripts/CambioClave.js"));


            //Actualizacion de Datos
            bundles.Add(new ScriptBundle("~/credinka/jsDatos").Include(
                "~/Content/Scripts/bootstrap-datepicker.js",
                "~/Content/Scripts/ActualizarDatos.js"));

            bundles.Add(new StyleBundle("~/credinka/cssDatos").Include(
                "~/Content/Styles/datepicker.css"));

        }
    }
}
