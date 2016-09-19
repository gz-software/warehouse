using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

namespace WHL.Models.Virtual
{
    /// <summary>
    /// ActionFilterAttribute Extension: we add the multilanguage support for the request action filter
    /// Author: Pango
    /// </summary>
    public class LangFilterAttribute : ActionFilterAttribute
    {
        private string _DefaultLanguage = "en_US";

        public LangFilterAttribute(string defaultLanguage)
        {
            _DefaultLanguage = defaultLanguage;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string lang = (string)filterContext.RouteData.Values["lang"] ?? _DefaultLanguage;
            if (lang != _DefaultLanguage)
            {
                try
                {
                    CultureInfo info;
                    switch (lang)
                    {
                        case "en_US":
                            info = new CultureInfo("en-US");
                            break;
                        case "zh_CN":
                            info = new CultureInfo("zh-CN");
                            break;
                        case "zh_HK":
                            info = new CultureInfo("zh-HK");
                            break;
                        case "th_TH":
                            info = new CultureInfo("th-TH");
                            break;
                        default:
                            info = new CultureInfo("en-US");
                            break;
                    }
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = info;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw new NotSupportedException(String.Format("ERROR: Invalid language code '{0}'.", lang));
                }
            }
        }
    }
}