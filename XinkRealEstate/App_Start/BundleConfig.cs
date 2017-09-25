using System.Web;
using System.Web.Optimization;

namespace XinkRealEstate
{
    public class BundleConfig
    {
#if DEBUG
        const bool DEBUG = true;
        const string MIN = "";
#else
        const bool DEBUG = false;
        const string MIN = "min.";
#endif
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            // AdminLTE and necessary stylesheet
            bundles.Add(new StyleBundle("~/AdminLTE/css").Include(
                      $"~/Content/AdminLTE/bower_components/bootstrap/dist/css/bootstrap.{MIN}css",
                      // Font Awesome
                      $"~/Content/AdminLTE/bower_components/font-awesome/css/font-awesome.{MIN}css",
                      // Ionicons
                      $"~/Content/AdminLTE/bower_components/Ionicons/css/ionicons.{MIN}css",
                      // Theme style 
                      $"~/Content/AdminLTE/dist/css/AdminLTE.{MIN}css",
                      // AdminLTE skin
                      $"~/Content/AdminLTE/dist/css/skins/skin-green.{MIN}css"));

            // AdminLTE and necessary script
            bundles.Add(new ScriptBundle("~/AdminLTE/js").Include(
                      // jQuery 3
                      $"~/Content/AdminLTE/bower_components/jquery/dist/jquery.{MIN}js",
                      // Bootstrap 3.3.7
                      $"~/Content/AdminLTE/bower_components/bootstrap/dist/js/bootstrap.{MIN}js",
                      // AdminLTE App
                      $"~/Content/AdminLTE/dist/js/adminlte.{MIN}js"));

            // Datatable
            bundles.Add(new StyleBundle("~/AdminLTE/css/dataTables").Include(
                      $"~/Content/AdminLTE/bower_components/datatables.net-bs/css/dataTables.bootstrap.{MIN}css"));
            bundles.Add(new ScriptBundle("~/AdminLTE/js/dataTables").Include(
                      // dataTables
                      $"~/Content/AdminLTE/bower_components/datatables.net/js/jquery.dataTables.{MIN}js",
                      // dataTables treeGrid
                      $"~/Content/AdminLTE/bower_components/datatables.net/js/dataTables.treeGrid.{MIN}js",
                      // dataTables-bootstrap
                      $"~/Content/AdminLTE/bower_components/datatables.net-bs/js/dataTables.bootstrap.{MIN}js"));

            // bootstrap-datepicker
            bundles.Add(new StyleBundle("~/AdminLTE/css/datepicker").Include(
                      $"~/Content/AdminLTE/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.{MIN}css"));
            bundles.Add(new ScriptBundle("~/AdminLTE/js/datepicker").Include(
                      // moment js
                      $"~/Content/AdminLTE/bower_components/moment/min/moment.min.js",
                      // dataTables-bootstrap
                      $"~/Content/AdminLTE/bower_components/datatables.net-bs/js/dataTables.bootstrap.{MIN}js"));
            //

        }
    }
}
