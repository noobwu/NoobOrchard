using Newtonsoft.Json;
using Orchard.WebApi.OAuth2.OwinOAuth.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace Orchard.WebApi.OAuth2.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRequest
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ValuesController : ApiController
    {
        // GET api/values
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [Authorize]
        public string Get(int id)
        {
            //IEnumerable<Claim> claims = access_token.ToClaims();

            return "value";
        }

        // post api/values/Register
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request.UserName"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public string Register(UserRequest request)
        {
            //IEnumerable<Claim> claims = access_token.ToClaims();

            return request.UserName;
        }

        // post api/values/Upload
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public virtual async Task<HttpResponseMessage> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {

            }
            var tempPath = HostingEnvironment.MapPath("~/Uploads/Temp");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            List<string> files = new List<string>();
            var provider = new CustomMultipartFormDataStreamProvider(tempPath);
            return await Request.Content.ReadAsMultipartAsync(provider)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        files.Add(Path.GetFileName(file.LocalFileName));
                    }

                    // Send OK Response along with saved file names to the client.  
                    return Request.CreateResponse(HttpStatusCode.OK, files);
                    //返回上传后的文件全路径
                    //return new HttpResponseMessage() { Content = new StringContent(fileName) };
                });
        }

        /// <summary>
        /// 
        /// </summary>
        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path)
                : base(path)
            { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                //var sb = new StringBuilder((headers.ContentDisposition.FileName ?? DateTime.Now.Ticks.ToString()).Replace("\"", "").Trim().Replace(" ", "_"));
                //Array.ForEach(Path.GetInvalidFileNameChars(), invalidChar => sb.Replace(invalidChar, '-'));
                //return sb.ToString();
                //截取文件扩展名
                string exp = Path.GetExtension(headers.ContentDisposition.FileName.TrimStart('\"').TrimEnd('\"'));
                string name = base.GetLocalFileName(headers);
                return name + exp;
            }
        }
    }
}
