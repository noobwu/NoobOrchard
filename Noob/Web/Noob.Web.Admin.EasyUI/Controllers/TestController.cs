using Autofac;
using Noob.IServices;
using Orchard.Caching;
using Orchard.Logging;
using Orchard.Web.Mvc.Controllers;
using Orchard.Web.Mvc.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Noob.Web.Admin.EasyUI.Controllers
{
    public class TestController : BaseController
    {
        //private readonly IAdmUserService _service;
        //private readonly ICacheManager _cacheManager;
        public TestController(ILoggerFactory loggerFactory):base(loggerFactory)
        {
            //_service = userService;
        }
        // GET: Test
        public async Task<string> Index()
        {
            IAdmUserService userService= ContainerContext.Current.Resolve<IAdmUserService>();
            ILoggerFactory loggerFactory= ContainerContext.Current.Resolve<ILoggerFactory>();
            ICacheManager cacheManager = ContainerContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), GetType()));
            return await Task.FromResult("Test"); ;
        }
    }
}