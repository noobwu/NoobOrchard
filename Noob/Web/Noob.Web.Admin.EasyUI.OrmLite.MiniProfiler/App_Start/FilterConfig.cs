using System.Web.Mvc;
using StackExchange.Profiling.Mvc;

namespace Noob.Web.Admin.EasyUI.OrmLite.MiniProfilers
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ProfilingActionFilter());
        }
    }
}
