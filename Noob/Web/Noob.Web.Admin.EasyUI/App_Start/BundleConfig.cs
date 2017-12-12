using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace Noob.Web.Admin.EasyUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/jquery/jquery-{version}.js")
                .Include("~/Scripts/bootstrap.js","~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            var easyuiVersion = "1.5.2";
            easyuiVersion = "1.4.3";
            bundles.Add(new ScriptBundle("~/script/easyui")
             .Include(string.Format("~/Scripts/jquery-easyui-{0}/jquery.min.js", easyuiVersion))
             .Include(string.Format("~/Scripts/jquery-easyui-{0}/jquery.easyui.min.js", easyuiVersion))
             .Include(string.Format("~/Scripts/jquery-easyui-{0}/locale/easyui-lang-zh_CN.js", easyuiVersion))
             .Include(string.Format("~/Scripts/jquery-easyui-{0}/jquery.easyui.extend.js", easyuiVersion))
             );
            bundles.Add(new StyleBundle("~/style/easyui").Include(
                string.Format("~/Scripts/jquery-easyui-{0}/themes/gray/easyui.css", easyuiVersion),
                string.Format("~/Scripts/jquery-easyui-{0}/themes/color.css", easyuiVersion),
                string.Format("~/Scripts/jquery-easyui-{0}/themes/icon.css", easyuiVersion))
              );

            bundles.Add(new ScriptBundle("~/script/admin")
             .Include("~/Scripts/Admin/common.js")
             );
            bundles.Add(new StyleBundle("~/style/admin").Include(
                        "~/Content/Admin/css/common.css"));

            //List<string> cdnList = new List<string>();
            //BundleTable.EnableOptimizations = true;
            //bundles.UseCdn = true;
            //cdnList.ForEach(a=> {
            //    bundles.Add(new ScriptBundle("~/bundles/jquery", a)
            //        .Include("~/Scripts/jquery-{version}.js"));
            //});
        }
    }
}
