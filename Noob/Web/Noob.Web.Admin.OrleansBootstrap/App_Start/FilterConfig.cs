using Orchard.Environment.Configuration;
using Orchard.Logging.NLogger;
using Orchard.Web.Mvc.Filters;
using System.Web;
using System.Web.Mvc;

namespace Noob.Web.Admin.Bootstrap
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            string configPath = "~/App_Data/Configs/GeneralConfigs.config", sectionName = "GeneralConfigs/AdminConfig";
            WebConfig webConfig = WebConfig.GetInstance(configPath,sectionName);
            filters.Add(new HandleErrorAttribute());
            if (webConfig.PerformanceEnabled)
            {
                var logFactory = new NLogFactory("~/App_Data/Configs/NLog.config");
                filters.Add(new MvcPerformanceAttribute(webConfig,logFactory));
            }
            //filters.Add(new WebApiPerformanceAttribute(configPath,
            //   sectionName));
        }
    }
}
