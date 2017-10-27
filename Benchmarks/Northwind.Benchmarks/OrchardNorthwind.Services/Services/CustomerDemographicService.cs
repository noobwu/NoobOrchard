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
    /// CustomerDemographics服务相关操作
    /// </summary>
    public abstract partial class CustomerDemographicService:ServiceBase<CustomerDemographic,string>,ICustomerDemographicService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">CustomerDemographic repository</param>
        public CustomerDemographicService(IRepository<CustomerDemographic,string> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
