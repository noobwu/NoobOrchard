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
    /// Customers服务相关操作
    /// </summary>
    public abstract partial class CustomerService:ServiceBase<Customer,string>,ICustomerService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Customer repository</param>
        public CustomerService(IRepository<Customer,string> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
