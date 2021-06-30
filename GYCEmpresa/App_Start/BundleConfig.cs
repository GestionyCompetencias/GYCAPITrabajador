using System.Web;
using System.Web.Optimization;

namespace GYCEmpresa
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", 
                        "~/Scripts/select2.min.js",
                        "~/Scripts/bootstrap.bundle.min.js",
                        "~/Scripts/recorrer.js",
                          "~/Content/vendor/jquery-easing/jquery.easing.min.js",
                          "~/Content/js/main/jquery.min.js",
                        "~/Content/js/main/bootstrap.bundle.min.js",
                        "~/Content/js/plugins/loaders/blockui.min.js",
                        "~/Content/js/plugins/visualization/d3/d3.min.js",
                        "~/Content/js/plugins/visualization/d3/d3_tooltip.js",
                        "~/Content/js/plugins/forms/styling/switchery.min.js",
                        "~/Content/js/plugins/ui/moment/moment.min.js",
                        "~/Content/js/plugins/pickers/daterangepicker.js",
                        "~/Content/js/app.js",
                        "~/Content/js/demo_pages/dashboard.js",
                        "~/Content/datatables.min.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/streamgraph.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/sparklines.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/lines.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/areas.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/donuts.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/bars.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/progress.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/heatmaps.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/pies.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/pies.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/pies.js",
                        "~/Content/js/demo_charts/pages/dashboard/light/bullets.js",
                        "~/Content/vendor/chart.js/Chart.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                          "~/Scripts/sb-admin-2.min.js",
                          "~/Scripts/demo/chart-area-demo.js",
                          "~/Scripts/demo/chart-pie-demo.js"));
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/css/icons/icomoon/styles.min.css",
                "~/Content/css/bootstrap.min.css",
                "~/Content/css/bootstrap_limitless.min.css",
                "~/Content/css/layout.min.css",
                "~/Content/css/components.min.css",
                "~/Content/datatables.min.css",
                "~/Content/css/colors.min.css"));

            var cdnGoogleFont = "https://fonts.googleapis.com/css?family=Nunito:200,200i,300,300i,400,400i,600,600i,700,700i,800,800i,900,900i";
            bundles.Add(new StyleBundle("~/Content/css", cdnGoogleFont).Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/vendor/fontawesome-free/css/all.min.css",
                      "~/Content/css/select2.min.css",
                      "~/Content/css/sb-admin-2.min.css"));
        }
    }
}
