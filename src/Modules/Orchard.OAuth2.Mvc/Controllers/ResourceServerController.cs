using System.Web.Http;

namespace Orchard.OAuth2.Mvc.Controllers
{
    [Authorize]
    public class ResourceServerController : ApiController
    {
        // GET: ResourceServer
        [HttpGet]
        [HttpPost]
        public string Index()
        {
            return this.User.Identity.Name??"no user";
        }
    }
}