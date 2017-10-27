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
    /// Suppliers服务相关操作
    /// </summary>
    public abstract partial class SupplierService:ServiceBase<Supplier,int>,ISupplierService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Supplier repository</param>
        public SupplierService(IRepository<Supplier,int> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
