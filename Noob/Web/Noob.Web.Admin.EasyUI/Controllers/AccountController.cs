using Noob.Domain.Entities;
using Noob.IServices;
using Noob.Web.Admin.Models.Account;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Security.Providers;
using Orchard.Utility;
using Orchard.Web.Mvc.Controllers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace Noob.Web.Admin.EasyUI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountController : BaseController
    {
        #region Members
        private readonly IAdmUserService _service;
        private readonly IAdmMenuService _menuService;
        private readonly IAdmUserRightsService _userRightsService;
        private readonly string uploadUrl;
        private readonly string _certifyPoolDir;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public AccountController(IAdmUserService service, IAdmUserRightsService userRightsService,IAdmMenuService menuService
            ,ICacheManager cacheManager,ILoggerFactory loggerFactory):base(loggerFactory)
        {
            _service = service;
            _menuService = menuService;
            _userRightsService = userRightsService;
            string appPath = RequestHelper.GetApplicationPath();
            WebConfig webConfig = Services.Caching.WebConfigService.GetWebConfig("/App_Data/Configs/GeneralConfigs.config", "GeneralConfigs/WebConfig", cacheManager);
            uploadUrl = webConfig.UploadUrl;
            if (!uploadUrl.Contains(appPath))
            {
                uploadUrl = appPath + uploadUrl;
            }
            string rootUrl = webConfig.RootUrl;
            if (!rootUrl.Contains(appPath))
            {
                rootUrl = appPath + rootUrl;
            }
            ViewBag.RootUrl = rootUrl;
            ViewBag.UploadUrl = uploadUrl;
            _certifyPoolDir = Utils.GetMapPath("~/Files/CertifyPool/");
            if (!Directory.Exists(_certifyPoolDir))
            {
                Utils.CreateDir(_certifyPoolDir);
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["User"] = null;
            Session["UserRights"] = null;
            return RedirectToAction("Login", "Account");
        }
        public ActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string url)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = GetErrorMsgFromModelState();
                return View(model);
            }
            else
            {
                //url = Request.RequestContext.HttpContext.Server.UrlTokenDecode(url);
                string tmpPwd = Utils.MD5(model.Password);
                AdmUser userInfo = await _service.SingleAsync(a => a.Password == tmpPwd && a.UserName == model.UserName);
                if (userInfo == null)
                {
                    ModelState.AddModelError("UserName", "账号名或者密码错误");
                    return View(model);
                }
                if (userInfo.Status == 0)
                {
                    ModelState.AddModelError("UserName", "该账号已被禁用,请联系管理员");
                    return View(model);
                }

                await SetUserRights(userInfo.UserID);

                FormsAuthenticationUtils.SignIn(HttpContext,new UserIdentifier(userInfo.UserName,userInfo.UserID),30);

                return RedirectToAction("Home", "Home");
            }
        }

        [NonAction]
        private async Task SetUserRights(int userId)
        {
            List<AdmUserRightsExt> userRights = await _userRightsService.GetAdmUserRightsExtListAsync(
                a => a.UserID == userId);
            if (userRights != null && userRights.Count > 0)
            {
                userRights = userRights.Where(a => !string.IsNullOrEmpty(a.RightsCode)).ToList();
                XDocument xDoc =
                      new XDocument(
                          new XDeclaration("1.0", "utf-8", "yes"), new XElement("Passport",
                              new XElement("RightsList",
                                  from entity in userRights
                                  select new XElement("Rights",
                                      new XElement("RightsCode", entity.RightsCode),
                                      new XElement("RightsName", entity.RightsName)
                                  )
                              )
                      ));
                if (xDoc.Root != null)
                {
                    xDoc.Root.Add(new XElement("Menu", await GetMenuJson(userRights)));
                }

                string filePath = _certifyPoolDir + userId + ".cer";
                xDoc.Save(filePath);
            }
            else
            {
                XDocument xDoc =
                     new XDocument(
                         new XDeclaration("1.0", "utf-8", "yes"), new XElement("Passport",
                             new XElement("RightsList", null
                             )
                     ));
                if (xDoc.Root != null)
                {
                    xDoc.Root.Add(new XElement("Menu", GetMenuJson(userRights)));
                }
                string filePath = _certifyPoolDir + userId + ".cer";
                xDoc.Save(filePath);
            }
        }
        /// <summary>
        /// 获取管理员菜单JSON数据
        /// </summary>
        /// <param name="id">用户ID</param>
        [NonAction]
        private async Task<string> GetMenuJson(List<AdmUserRightsExt> userRights)
        {
            if (userRights == null || userRights.Count == 0) return "{}";
            IOrderByExpression<AdmMenu>[] orderByExpressions =  {
                new OrderByExpression<AdmMenu,int>(a=>a.SortOrder),
                  new OrderByExpression<AdmMenu,int>(a=>a.MenuID),
            };
            List<AdmMenu> menus = await _menuService.GetListAsync(a => a.MenuID > 0, orderByExpressions);
            if (menus != null && menus.Count > 0)
            {
                //使用组连接
                var userMenus = (from menu in menus
                                 join rights in userRights
                                   on menu.RightsID equals rights.RightsID
                                 select menu).ToList();
                if (userMenus == null || userMenus.Count == 0) return "{}";
                StringBuilder jsonBuilder = new StringBuilder();
                jsonBuilder.Append("{");
                foreach (var menuItem in menus.Where(a => a.ParentID == 0))
                {
                    if (userMenus.Exists(a => a.IDPath.Contains("^" + menuItem.MenuID + "^")))
                    {
                        jsonBuilder.Append("\"" + menuItem.SortOrder + "_" + menuItem.MenuID + "\":{");
                        jsonBuilder.Append("\"code\":\"" + menuItem.MenuCode + "\",\"name\":\"" + menuItem.MenuName + "\",");
                        if (menuItem.MenuType == 1)
                        {
                            jsonBuilder.Append("\"url\":\"" + menuItem.MenuUrl + "\"");
                        }
                        else
                        {
                            jsonBuilder.Append("\"menus\":[");
                            var secondMenus = menus.Where(b => b.ParentID == menuItem.MenuID).ToList();
                            if (secondMenus.Count == 0)
                            {
                                jsonBuilder.Append("]");
                            }
                            else
                            {
                                for (int j = 0; j < secondMenus.Count; j++)
                                {
                                    if (userMenus.Exists(a => a.IDPath.Contains("^" + secondMenus[j].MenuID + "^")))
                                    {
                                        jsonBuilder.Append("{\"id\":" + secondMenus[j].MenuID + ",");
                                        jsonBuilder.Append("\"code\":\"" + secondMenus[j].MenuCode + "\",\"name\":\"" + secondMenus[j].MenuName + "\",");
                                        if (secondMenus[j].MenuType == 1)
                                        {
                                            jsonBuilder.Append("\"url\":\"" + secondMenus[j].MenuUrl + "\"");
                                        }
                                        else
                                        {
                                            jsonBuilder.Append("\"menus\":[");
                                            var thirdMenus = menus.Where(c => c.ParentID == secondMenus[j].MenuID && c.MenuType == 1).ToList();
                                            if (thirdMenus.Count == 0)
                                            {
                                                jsonBuilder.Append("]");
                                            }
                                            else
                                            {
                                                StringBuilder tmpJsonBuilder = new StringBuilder();
                                                for (int k = 0; k < thirdMenus.Count; k++)
                                                {
                                                    if (userMenus.Exists(a => a.IDPath.Contains("^" + thirdMenus[k].MenuID + "^")))
                                                    {
                                                        tmpJsonBuilder.Append("{\"id\":" + thirdMenus[k].MenuID + ",");
                                                        tmpJsonBuilder.Append("\"code\":\"" + thirdMenus[k].MenuCode + "\",\"name\":\"" + thirdMenus[k].MenuName + "\",");
                                                        tmpJsonBuilder.Append("\"url\":\"" + thirdMenus[k].MenuUrl + "\"");
                                                        tmpJsonBuilder.Append("}");
                                                        tmpJsonBuilder.Append(",");
                                                    }
                                                }
                                                jsonBuilder.Append(tmpJsonBuilder.ToString().Trim(','));
                                                jsonBuilder.Append("]");
                                            }
                                        }
                                        jsonBuilder.Append("}");
                                        if (j != secondMenus.Count - 1)
                                        {
                                            jsonBuilder.Append(",");
                                        }
                                    }
                                }
                                jsonBuilder.Append("]");
                            }
                        }
                        jsonBuilder.Append("},");
                    }
                }
                return jsonBuilder.ToString().Trim(',') + "}";

            }
            else
            {
                return "{}";
            }

        }
    }
}
