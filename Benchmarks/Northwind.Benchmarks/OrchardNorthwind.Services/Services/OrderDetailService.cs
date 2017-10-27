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
    /// Order Details服务相关操作
    /// </summary>
    public abstract partial class OrderDetailService:ServiceBase<OrderDetail,string>,IOrderDetailService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">OrderDetail repository</param>
        public OrderDetailService(IRepository<OrderDetail,string> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
