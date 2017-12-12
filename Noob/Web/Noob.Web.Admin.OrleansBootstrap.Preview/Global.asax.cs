using Autofac;
using Noob.Web.Admin.Bootstrap.OrmLite;
using Orchard.Environment.Configuration;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Noob.Web.Admin.Bootstrap
{
    public class MvcApplication : NoobAdminBootstrapApplication
    {
        public override void Register(ContainerBuilder builder)
        {
            builder.RegisterModule(new NoobAdminOrmLiteDataModule());
        }
    }
}
