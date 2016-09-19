using System.Web;
using System.Web.Optimization;

namespace WHL
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            ///////////////// common script
            bundles.Add(new ScriptBundle("~/Scripts/Common/jquery").Include(
                "~/Scripts/Common/jquery-{version}.js",
                "~/Scripts/Common/jquery.object.toObject.js",
                "~/Scripts/Common/jquery.object.js2form.js",
                "~/Scripts/Common/jquery.object.form2js.js",
                "~/Scripts/Common/jquery.toaster.js" )
                );

            bundles.Add(new ScriptBundle("~/Scripts/Common/jqueryVal").Include("~/Scripts/Common/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/Scripts/Common/jqueryTable").Include(
                "~/Scripts/Common/jquery.dataTables.min.js", 
                "~/Scripts/Common/jquery.dataTables.scroller.min.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/Common/modernizr").Include("~/Scripts/Common/modernizr-*"));

            bundles.Add(new ScriptBundle("~/Scripts/Common/bootstrap")
                .Include(
                "~/Scripts/Common/bootstrap.js",
                "~/Scripts/Common/respond.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/Common/tools")
              .Include(
              "~/Scripts/Common/gzsoftware.tools.js"
              ));

            bundles.Add(new ScriptBundle("~/Scripts/Common/laydate").Include("~/Scripts/Common/laydate.js"));

            ///////////////// WHL script
            bundles.Add(new ScriptBundle("~/Scripts/WHL/menu").Include("~/Scripts/WHL/menu.js"));
            bundles.Add(new ScriptBundle("~/Scripts/WHL/inventory").Include("~/Scripts/WHL/inventory.js"));
            bundles.Add(new ScriptBundle("~/Scripts/WHL/outbound").Include("~/Scripts/WHL/outbound.js"));
            bundles.Add(new ScriptBundle("~/Scripts/WHL/inbound").Include("~/Scripts/WHL/inbound.js"));
            bundles.Add(new ScriptBundle("~/Scripts/WHL/import").Include("~/Scripts/WHL/import.js"));
            bundles.Add(new ScriptBundle("~/Scripts/WHL/export").Include("~/Scripts/WHL/export.js"));
            bundles.Add(new ScriptBundle("~/Scripts/WHL/epacket").Include("~/Scripts/WHL/epacket.js"));

            ///////////////// common css
            bundles.Add(new StyleBundle("~/Styles/Common/bootstrap")
                .Include(
                "~/Styles/Common/bootstrap.css",
                "~/Styles/Common/site.css"
                ));

            ///////////////// jquery table css
            bundles.Add(new StyleBundle("~/Styles/Common/jqueryTable")
                .Include(
                "~/Styles/Common/jquery.dataTables.css",
                "~/Styles/Common/jquery.dataTables.scroller.min.css"
                ));

            ///////////////// laydate css
            bundles.Add(new StyleBundle("~/Styles/Common/laydate").Include("~/Styles/Common/laydate.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
