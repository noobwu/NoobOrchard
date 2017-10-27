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
    /// Products服务相关操作
    /// </summary>
    public abstract partial class ProductService:ServiceBase<Product,int>,IProductService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Product repository</param>
        public ProductService(IRepository<Product,int> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
