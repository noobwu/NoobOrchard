using Microsoft.Owin.Security;
using Orchard.OAuth2.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Orchard.OAuth2.Mvc.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var authentication = HttpContext.GetOwinContext().Authentication;
            var isPersistent = model.RememberMe;
            await Task.Factory.StartNew(() =>
            {
                authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = isPersistent },
                    new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType,
                    model.Email) }, "Application"));
            });
            return RedirectToAction("Index","Home");
        }

        public ActionResult Logout()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> External()
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            if (Request.HttpMethod == "POST")
            {
                foreach (var key in Request.Form.AllKeys)
                {
                    if (key.StartsWith("submit.External.") && !string.IsNullOrEmpty(Request.Form.Get(key)))
                    {
                        var authType = key.Substring("submit.External.".Length);
                        authentication.Challenge(authType);
                        return new HttpUnauthorizedResult();
                    }
                }
            }
            var authResult =await authentication.AuthenticateAsync("External");
            if (authResult != null)
            {
                var identity = authResult.Identity;
                authentication.SignOut("External");
                authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = true },
                    new ClaimsIdentity(identity.Claims, "Application", identity.NameClaimType, identity.RoleClaimType));
                return Redirect(Request.QueryString["ReturnUrl"]);
            }

            return View();
        }
    }
}