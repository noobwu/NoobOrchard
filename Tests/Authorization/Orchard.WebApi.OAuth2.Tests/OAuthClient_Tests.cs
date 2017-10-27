using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Orchard.WebApi.OAuth2.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenConstants
    {
        /// <summary>
        /// 
        /// </summary>
        public const string UserName = "test";
        /// <summary>
        /// 
        /// </summary>

        public const string Password = "test";
        /// <summary>
        /// 
        /// </summary>
        public const string BaseAddress = "http://localhost:10070";
        /// <summary>
        /// 
        /// </summary>
        public const string LoginPath = "";
        /// <summary>
        /// 
        /// </summary>

        public const string LogoutPath = "";

        /// <summary>
        /// 
        /// </summary>
        public const string AuthorizePath = "/Api/OAuth/Authorize";
        /// <summary>
        /// 
        /// </summary>
        public const string TokenPath = "/Api/OAuth/Token";

        /// <summary>
        /// 
        /// </summary>
        public const string ValuesPath = "/Api/Values";
        /// <summary>
        /// 
        /// </summary>
        public const string AuthorizeGrantCodePath = "/Api/AuthorizeGrant/Index";
        /// <summary>
        /// 
        /// </summary>
        public const string AuthorizeGrantTokenPath = "/Api/AuthorizeGrant/Token";
        /// <summary>
        /// The period of time the access token remains valid after being issued(unit for minute) 
        /// </summary>

        public const int AccessTokenExpireTime = 7 * 24 * 60;
        /// <summary>
        /// 
        /// </summary>
        public const string ClientId = "E937A9C70E6B7AE8612B9887461A5353";
        /// <summary>
        /// 
        /// </summary>
        public const string ClientSecret = "E7ED5DE668C20E3414F92A31866E12C9";
    }
    [TestFixture]
    public class OAuthClient_Tests
    {
        private static HttpClient _httpClient;
        //private IDisposable _webApp;
        public OAuthClient_Tests()
        {
            Console.WriteLine("Web API started!");
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(TokenConstants.BaseAddress);
            Console.WriteLine("HttpClient started!");

        }

        #region Method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grantType"></param>
        /// <param name="refreshToken"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>

        private static async Task<TokenResponse> GetToken(string grantType, string refreshToken = null, string userName = null, string password = null, string authorizationCode = null)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("client_id", TokenConstants.ClientId);
            parameters.Add("client_secret", TokenConstants.ClientSecret);
            parameters.Add("grant_type", grantType);
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                parameters.Add("username", userName);
                parameters.Add("password", password);
            }
            if (!string.IsNullOrEmpty(authorizationCode))
            {
                parameters.Add("code", authorizationCode);
                parameters.Add("redirect_uri", TokenConstants.BaseAddress + TokenConstants.AuthorizeGrantCodePath); //和获取 authorization_code 的 redirect_uri 必须一致，不然会报错
            }
            if (!string.IsNullOrEmpty(refreshToken))
            {
                parameters.Add("refresh_token", refreshToken);
            }

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            //    "Basic",
            //    Convert.ToBase64String(Encoding.UTF8.GetBytes(TokenConstants.ClientId + ":" + TokenConstants.ClientSecret)));

            var response = await _httpClient.PostAsync(TokenConstants.TokenPath, new FormUrlEncodedContent(parameters));
            var responseValue = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("error,RequestUri:" + response.RequestMessage.RequestUri);
                Console.WriteLine("error,StatusCode:" + response.StatusCode);
                //try
                //{
                //    Console.WriteLine("ExceptionMessage:"+(await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                //}
                //catch (Exception)
                //{
                //    Console.WriteLine("Content:"+await response.Content.ReadAsStringAsync());
                //}
                Console.WriteLine("error,Content:" +await response.Content.ReadAsStringAsync());
                return null;
            }
            Console.WriteLine("GetToken:"+await response.Content.ReadAsStringAsync());
            return await response.Content.ReadAsAsync<TokenResponse>();
        }
        private static async Task<string> GetAuthorizationCode()
        {
            string url = TokenConstants.AuthorizePath + $"?grant_type=authorization_code&response_type=code&client_id={TokenConstants.ClientId}&client_secret={TokenConstants.ClientSecret}&redirect_uri={HttpUtility.UrlEncode(TokenConstants.BaseAddress + TokenConstants.AuthorizeGrantCodePath)}";
            Console.WriteLine("url:"+url);
            var response = await _httpClient.GetAsync(url);
            var authorizationCode = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("error,RequestUri:" + response.RequestMessage.RequestUri);
                Console.WriteLine("error,StatusCode:" + response.StatusCode);
                //try
                //{
                //    Console.WriteLine("ExceptionMessage:"+(await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                //}
                //catch (Exception)
                //{
                //    Console.WriteLine("Content:"+await response.Content.ReadAsStringAsync());
                //}
                Console.WriteLine("error,Content:" +await response.Content.ReadAsStringAsync());
                return null;
            }
            Console.WriteLine("GetAuthorizationCode:" + await response.Content.ReadAsStringAsync());
            return authorizationCode;
        }
        #endregion Method

        [Test]
        public async Task OAuth_ClientCredentials_Test()
        {
            var tokenResponse = await GetToken("client_credentials"); //获取 access_token
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken)) return;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _httpClient.GetAsync(TokenConstants.ValuesPath);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("error,RequestUri:" + response.RequestMessage.RequestUri);
                Console.WriteLine("error,StatusCode:" + response.StatusCode);
                //try
                //{
                //    Console.WriteLine("ExceptionMessage:"+(await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                //}
                //catch (Exception)
                //{
                //    Console.WriteLine("Content:"+await response.Content.ReadAsStringAsync());
                //}
                Console.WriteLine("error,Content:" + await response.Content.ReadAsStringAsync());
            }
            Console.WriteLine("response:" + await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            //Thread.Sleep(3000);

            var tokenResponseTwo = await GetToken("refresh_token", tokenResponse.RefreshToken);
            if (tokenResponseTwo == null || string.IsNullOrEmpty(tokenResponseTwo.RefreshToken)) return;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseTwo.AccessToken);
            var responseTwo = await _httpClient.GetAsync(TokenConstants.ValuesPath);
            Console.WriteLine("responseTwo:" + await responseTwo.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, responseTwo.StatusCode);
        }

        [Test]
        public async Task OAuth_Password_Test()
        {
            var tokenResponse = await GetToken("password", userName: TokenConstants.UserName, password: TokenConstants.Password); //获取 access_token
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken)) return;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _httpClient.GetAsync(TokenConstants.ValuesPath);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("error,RequestUri:" + response.RequestMessage.RequestUri);
                Console.WriteLine("error,StatusCode:" + response.StatusCode);
                //try
                //{
                //    Console.WriteLine("ExceptionMessage:"+(await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                //}
                //catch (Exception)
                //{
                //    Console.WriteLine("Content:"+await response.Content.ReadAsStringAsync());
                //}
                Console.WriteLine("error,Content:" + await response.Content.ReadAsStringAsync());
            }
            Console.WriteLine("response:"+await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            //Thread.Sleep(3000);

            var tokenResponseTwo = await GetToken("refresh_token", tokenResponse.RefreshToken);
            if (tokenResponseTwo == null || string.IsNullOrEmpty(tokenResponseTwo.AccessToken)) return;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseTwo.AccessToken);
            var responseTwo = await _httpClient.GetAsync(TokenConstants.ValuesPath);
            Console.WriteLine("responseTwo:" + await responseTwo.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, responseTwo.StatusCode);
        }


        [Test]
        public async Task OAuth_AuthorizationCode_Test()
        {
            var authorizationCode = await GetAuthorizationCode(); //获取 authorization_code
            var tokenResponse = await GetToken("authorization_code", null, null, null, authorizationCode); //根据 authorization_code 获取 access_token
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken)) return;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _httpClient.GetAsync(TokenConstants.ValuesPath);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("error,RequestUri:" + response.RequestMessage.RequestUri);
                Console.WriteLine("error,StatusCode:" + response.StatusCode);
                //try
                //{
                //    Console.WriteLine("ExceptionMessage:"+(await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                //}
                //catch (Exception)
                //{
                //    Console.WriteLine("Content:"+await response.Content.ReadAsStringAsync());
                //}
                Console.WriteLine("error,Content:" + await response.Content.ReadAsStringAsync());
            }
            Console.WriteLine("response:"+await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            //Thread.Sleep(10000);

            var tokenResponseTwo = GetToken("refresh_token", tokenResponse.RefreshToken).Result; //根据 refresh_token 获取 access_token
            if (tokenResponseTwo == null || string.IsNullOrEmpty(tokenResponseTwo.AccessToken)) return;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseTwo.AccessToken);
            var responseTwo = await _httpClient.GetAsync(TokenConstants.ValuesPath);
            Console.WriteLine("responseTwo:" + await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, responseTwo.StatusCode);
        }

    }
}
