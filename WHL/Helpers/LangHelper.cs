using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using System.Reflection;

namespace WHL.Helpers
{
    /// <summary>
    /// Author: Pango
    /// LangHelper:  this class impletment all language resource helper methods.
    /// </summary>
    public static class LangHelper
    {

        /// <summary>
        /// default language come from current system setting.
        /// </summary>
        /// <returns></returns>
        public static String GetDefaultLang()
        {
            return GetThreadLang();//"en_US";
        }

       /// <summary>
       /// get current language by this machine system setting.
       /// </summary>
       /// <returns></returns>
        public static String GetThreadLang()
        {
            String uiLang = Thread.CurrentThread.CurrentUICulture.Name;
            switch (uiLang)
            {
                case "en-US":
                    return "en_US";
                case "zh-CN":
                    return "zh_CN";
                case "zh-HK":
                    return "zh_HK";
                case "th-TH":
                    return "th_TH";
                default:
                    return "en_US";
            }
        }

        /// <summary>
        /// get the language resouce file
        /// </summary>
        /// <returns></returns>
        public static Type GetLangType()
        {
            String lang = GetThreadLang();
            switch (lang)
            {
                case "en_US":
                    return typeof(Resources.en_US);
                case "zh_CN":
                    return typeof(Resources.zh_CN);
                case "zh_HK":
                    return typeof(Resources.zh_HK);
                case "th_TH":
                    return typeof(Resources.th_TH);
                default:
                    return typeof(Resources.en_US);
            }
        }

        /// <summary>
        /// get the language value by key for UI
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetLangByKey(string key)
        {
            Type resourceType = GetLangType();
            PropertyInfo p = resourceType.GetProperty(key);
            if (p != null)
                return p.GetValue(null, null).ToString();
            else
                return key;
        }

        /// <summary>
        /// get the language value by key for UI
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            return GetLangByKey(key);
        }


        /// <summary>
        /// Generate the java script language resource
        /// </summary>
        /// <returns></returns>
        public static string GenerateJsResource()
        {
            Type resourceType = GetLangType();

            PropertyInfo[] props = resourceType.GetProperties();
            String jsStr = "";
            foreach (PropertyInfo p in props)
            {
                if (p.PropertyType == typeof(string))
                {
                    var key = (p.Name);
                    var value = p.GetValue(null, null).ToString();

                    if (value != null)
                    {
                        jsStr += string.Format("var {0} = '{1}';", key, value);
                        //jsStr += " /r/n";
                    }
                    else
                    {
                        jsStr += string.Format("var {0} = '{1}';", key, key);
                        //jsStr += " /r/n";

                    }
                }
            }
            return jsStr;
        }
    }
}