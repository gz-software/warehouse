using System.Web;
using System.Web.Mvc;
using WHL.Models.Virtual;
using WHL.Helpers;

namespace WHL
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LangFilterAttribute(LangHelper.GetThreadLang()), 0); // 语言过滤器
        }
    }
}
