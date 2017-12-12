using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;

namespace Noob.Web.Admin.EasyUI.OrmLite.MySql
{
    /// <summary>
    /// 
    /// </summary>
    public class MvcApplication : NoobAdminEasyUIApplication
    {
        public override void Register(ContainerBuilder builder)
        {
            builder.RegisterModule(new NoobAdminOrmLiteDataModule());
        }
    }
}
