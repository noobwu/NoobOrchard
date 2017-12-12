using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Autofac;

namespace Noob.Web.Admin.EasyUI.EntityFrameworkCore
{
    public class Global :NoobAdminEasyUIApplication
    {
        public override void Register(ContainerBuilder builder)
        {
            builder.RegisterModule(new NoobAdminEntityFrameworkCoreDataModule());
        }

    }
}