using System.Web;
using System.Web.Optimization;

namespace Product_catalog_and_Warehouse_inventory1
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Admin_Templete/css/bootstrap.min.css",                     
                      "~/Admin_Templete/font-awesome/css/font-awesome.css",
                      "~/Admin_Templete/css/plugins/dataTables/datatables.min.css",
                      "~/Admin_Templete/css/animate.css",
                      "~/Admin_Templete/css/style.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Admin_Templete/js/jquery-2.1.1.js",
                      "~/Admin_Templete/js/bootstrap.min.js",
                      "~/Admin_Templete/js/plugins/metisMenu/jquery.metisMenu.js",
                      "~/Admin_Templete/js/plugins/slimscroll/jquery.slimscroll.min.js",
                      "~/Admin_Templete/js/inspinia.js",
                      "~/Admin_Templete/js/plugins/pace/pace.min.js",
                      "~/Admin_Templete/js/plugins/dataTables/datatables.min.js"));
         

        }
    }
}
