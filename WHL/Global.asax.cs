using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;  
using System.Data.Entity.Infrastructure.Interception;

using WHL.DAL;
using WHL.Helpers;

namespace WHL
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DbInterception.Add(new DataInterceptorTransientErrors());
            DbInterception.Add(new DataInterceptorLogging());

        }

        protected void Application_BeginRequest()
        {
            var rawUrl = HttpContext.Current.Request.RawUrl;
            var lang = LangHelper.GetDefaultLang();

            if ((rawUrl.IndexOf("zh_CN") < 0) && (rawUrl.IndexOf("en_US") < 0) && (rawUrl.IndexOf("zh_HK") < 0) && (rawUrl.IndexOf("th_TH") < 0))
            {
                var newUrl = "/" + lang + rawUrl;
                //HttpContext.Current.RewritePath();

                Response.Redirect(newUrl);
                Response.End();
            }

        }
    }
}
