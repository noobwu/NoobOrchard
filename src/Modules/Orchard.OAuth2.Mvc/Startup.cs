using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(Orchard.OAuth2.Mvc.Startup))]

namespace Orchard.OAuth2.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           new  OwinOAuth.OwinOAuthStartup().ConfigureAuth(app);
        }
    }
}
