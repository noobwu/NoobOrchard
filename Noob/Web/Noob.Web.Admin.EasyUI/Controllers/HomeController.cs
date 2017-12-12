using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Caching;
using Noob.Web.Admin.Security;
using Noob.IServices;

namespace Noob.Web.Admin.EasyUI.Controllers
{

    public class HomeController : BaseAdminController
    {
        public HomeController(IAdmUserService  userService, ICacheManager cacheManager,ILoggerFactory loggerFactory)
            : base(cacheManager,loggerFactory)
        {
        }
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Message = "青青精选";
            return View();
        }
        public ActionResult Home()
        {
            string menuData = "[]";
            if (PassportDoc != null)
            {
                var root = PassportDoc.Root;
                if (root != null)
                {
                    var menu = root.Element("Menu");
                    if (menu != null)
                    {
                        menuData = menu.Value;
                    }
                }
            }
            ViewBag.MenuData = menuData;
            ViewBag.UserName = LoginUserName;
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
        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Welcome()
        {
            ViewBag.Message = "Welcome.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult NoAuthorize()
        {
            ViewBag.Title = "没有权限。";
            ViewBag.Message = "您没有权限该操作。";
            return View();
        }
        [AllowAnonymous]
        public ActionResult NoFoundData()
        {
            ViewBag.Title = "没有该数据。";
            ViewBag.Message = "没有找到相应的数据。";
            return View();
        }
    }
}