using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Autofac;
using System.Data.Common;
using Noob.Data.EntityFramework;
using Orchard.Data.EntityFramework;
using System.Data.SqlClient;

namespace Noob.Web.Admin.EasyUI.EntityFramework
{
    public class Global : NoobAdminEasyUIApplication
    {
        public override void Register(ContainerBuilder builder)
        {
            builder.RegisterModule(new NoobAdminEntityFrameworkDataModule());
        }
    }
}