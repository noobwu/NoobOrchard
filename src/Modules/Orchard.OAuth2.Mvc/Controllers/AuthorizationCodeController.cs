using DotNetOpenAuth.OAuth2;
using Orchard.OAuth2.OwinOAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Orchard.OAuth2.Mvc.Controllers
{
    public class AuthorizationCodeController : Controller
    {
        private WebServerClient _webServerClient;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            ViewBag.AccessToken = Request.Form["AccessToken"] ?? "";
            ViewBag.RefreshToken = Request.Form["RefreshToken"] ?? "";
            ViewBag.Action = "";
            ViewBag.ApiResponse = "";

            InitializeWebServerClient();
            var accessToken = Request.Form["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                var authorizationState = _webServerClient.ProcessUserAuthorization(Request);
                if (authorizationState != null)
                {
                    ViewBag.AccessToken = authorizationState.AccessToken;
                    ViewBag.RefreshToken = authorizationState.RefreshToken;
                    ViewBag.Action = Request.Path;
                }
            }

            if (!string.IsNullOrEmpty(Request.Form.Get("submit.Authorize")))
            {
                var userAuthorization = _webServerClient.PrepareRequestUserAuthorization(new[] { "test1scope", "test2scope" },
                    new Uri(TokenConstants.AuthorizeCodeCallBackPath));
                userAuthorization.Send(HttpContext);
                Response.End();
            }
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.Refresh")))
            {
                var state = new AuthorizationState
                {
                    AccessToken = Request.Form["AccessToken"],
                    RefreshToken = Request.Form["RefreshToken"]
                };
                if (_webServerClient.RefreshAuthorization(state))
                {
                    ViewBag.AccessToken = state.AccessToken;
                    ViewBag.RefreshToken = state.RefreshToken;
                }
            }
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.CallApi")))
            {
                var resourceServerUri = new Uri(TokenConstants.AuthorizationServerBaseAddress);
                var client = new HttpClient(_webServerClient.CreateAuthorizingHandler(accessToken));
                var body =await client.GetStringAsync(new Uri(resourceServerUri, TokenConstants.MePath));
                ViewBag.ApiResponse = body;
            }

            return View();
        }

        private void InitializeWebServerClient()
        {
            var authorizationServerUri = new Uri(TokenConstants.AuthorizationServerBaseAddress);
            var authorizationServer = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(authorizationServerUri, TokenConstants.AuthorizePath),
                TokenEndpoint = new Uri(authorizationServerUri, TokenConstants.TokenPath)
            };
            _webServerClient = new WebServerClient(authorizationServer, Clients.ApiCodeGrantClient.AppId, Clients.ApiCodeGrantClient.AppSecret);
        }
    }
}