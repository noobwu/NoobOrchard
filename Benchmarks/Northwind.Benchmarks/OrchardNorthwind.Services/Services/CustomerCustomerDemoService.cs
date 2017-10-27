using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using OrchardNorthwind.Common.Entities;
using Orchard.Domain.Repositories;
using Orchard.Services;
using OrchardNorthwind.IServices;
namespace OrchardNorthwind.Services
{

    /// <summary>
    /// CustomerCustomerDemo服务相关操作
    /// </summary>
    public abstract partial class CustomerCustomerDemoService:ServiceBase<CustomerCustomerDemo,string>,ICustomerCustomerDemoService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">CustomerCustomerDemo repository</param>
        public CustomerCustomerDemoService(IRepository<CustomerCustomerDemo,string> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
