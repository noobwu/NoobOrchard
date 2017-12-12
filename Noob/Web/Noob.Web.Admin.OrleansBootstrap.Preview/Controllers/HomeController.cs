using Orchard.Web.Mvc.Filters;
using System.Web.Mvc;

namespace Noob.Web.Admin.Bootstrap.Controllers
{

    public class HomeController : Controller
    {
        //[MvcPerformance("~/App_Data/Configs/GeneralConfigs.config", "GeneralConfigs/AdminConfig")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Error()
        {
            ViewBag.Message = "Error";

            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult ServerError()
        {
            return View();
        }
    }
}
