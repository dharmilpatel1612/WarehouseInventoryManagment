using System.Web;
using System.Web.Optimization;

namespace Product_CatalogAndWarehouse_Inventory
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            // Style sheets
            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Admin_Template/css/bootstrap.min.css",
                    "~/Admin_Template/font-awesome/css/font-awesome.css",
                    "~/Admin_Template/css/plugins/dataTables/datatables.min.css",
                    "~/Admin_Template/css/animate.css",                   
                    "~/Admin_Template/css/plugins/jasny/jasny-bootstrap.min.css",                   
                    "~/Admin_Template/css/style.css",
                    "~/Admin_Template/css/ImageStyle.css"));

            // Mainly scripts Custom and plugin javascript
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/Admin_Template/js/jquery-2.1.1.js",
                    "~/Admin_Template/js/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                    "~/Admin_Template/js/plugins/metisMenu/jquery.metisMenu.js",
                    "~/Admin_Template/js/plugins/slimscroll/jquery.slimscroll.min.js",
                    "~/Admin_Template/js/plugins/dataTables/datatables.min.js",
                    "~/Admin_Template/js/inspinia.js",
                    "~/Admin_Template/js/plugins/pace/pace.min.js",
                    "~/Admin_Template/js/plugins/jasny/jasny-bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            "~/Admin_Template/js/plugins/validate/jquery.validate.min.js",
            "~/Scripts/jquery.validate.unobtrusive.js"));
        }
    }
}
