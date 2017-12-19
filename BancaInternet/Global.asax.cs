using System;
using System.Web;
using System.Drawing;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.Collections.Concurrent;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace BancaInternet
{
    public class MvcApplication : HttpApplication
    {
        #region Fields

        public const string MultipleParameterKey = "_TypeKey_";
        public const string CookieParameterKey = "_Typecookie_";
        public static string sKey_Master = System.Web.Configuration.WebConfigurationManager.AppSettings["KeyMaestra"].ToString();
        public static string TimeLogueo = System.Web.Configuration.WebConfigurationManager.AppSettings["TimerCerrar"].ToString();
        public static string TimerCaduca = System.Web.Configuration.WebConfigurationManager.AppSettings["TimerCaduca"].ToString();
        public static string TimerSistema = System.Web.Configuration.WebConfigurationManager.AppSettings["TimerSistema"].ToString();

        private static readonly ConcurrentDictionary<int, ICaptchaManager> CaptchaManagers =
            new ConcurrentDictionary<int, ICaptchaManager>();

        private static readonly DefaultCaptchaManager CookieCaptchaManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.HttpApplication"/> class.
        /// </summary>
        static MvcApplication()
        {
            CookieCaptchaManager = new DefaultCaptchaManager(new CookieStorageProvider());
            CookieCaptchaManager.ImageUrlFactory =
                (helper, pair) => helper.Action("Generate", "DefaultCaptcha",
                                                new RouteValueDictionary
                                                    {
                                                        {
                                                            CookieCaptchaManager.TokenParameterName,
                                                            pair.Key
                                                        },
                                                        {
                                                            CookieParameterKey, "t"
                                                        }
                                                    });

            CookieCaptchaManager.RefreshUrlFactory =
                (helper, pair) => helper.Action("Refresh", "DefaultCaptcha",
                                                new RouteValueDictionary
                                                    {
                                                        {
                                                            CookieParameterKey, "t"
                                                        }
                                                    });
        }

        #endregion

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CaptchaUtils.CaptchaManagerFactory = GetCaptchaManager;
            /*********************************************/
            var imageGenerator = CaptchaUtils.ImageGenerator;
            Color redColor = Color.FromArgb(245, 124, 31);
            imageGenerator.FontColor = redColor;
            imageGenerator.Width = 95;
            imageGenerator.Height = 28;
        }

        private static ICaptchaManager GetCaptchaManager(IParameterContainer parameterContainer)
        {
            int numberOfCaptcha;
            if (parameterContainer.TryGet(MultipleParameterKey, out numberOfCaptcha))
                return CaptchaManagers.GetOrAdd(numberOfCaptcha, CreateCaptchaManagerByNumber);

            if (parameterContainer.IsContains(CookieParameterKey))
                return CookieCaptchaManager;
            //If not found parameter return default manager.
            return CaptchaUtils.CaptchaManager;
        }

        private static ICaptchaManager CreateCaptchaManagerByNumber(int i)
        {
            var captchaManager = new DefaultCaptchaManager(new SessionStorageProvider());
            captchaManager.ImageElementName += i;
            captchaManager.InputElementName += i;
            captchaManager.TokenElementName += i;
            captchaManager.ImageUrlFactory = (helper, pair) =>
            {
                var dictionary = new RouteValueDictionary();
                dictionary.Add(captchaManager.TokenParameterName, pair.Key);
                dictionary.Add(MultipleParameterKey, i);
                return helper.Action("Generate", "DefaultCaptcha", dictionary);
            };
            captchaManager.RefreshUrlFactory = (helper, pair) =>
            {
                var dictionary = new RouteValueDictionary();
                dictionary.Add(MultipleParameterKey, i);
                return helper.Action("Refresh", "DefaultCaptcha", dictionary);
            };
            return captchaManager;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var error = 999;
            var exception = Server.GetLastError();
            Response.Clear();
            var httpException = exception as HttpException;
            HttpContextBase httpContext = new HttpContextWrapper(HttpContext.Current);
            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContext, new RouteData()));
            if (httpException != null) {
                error = httpException.GetHttpCode();
            }
            string redirectUrl = urlHelper.Action("redirect", "credinka", new { Error = error  });
            httpContext.Response.Redirect(redirectUrl, true);
        }


    }
}
