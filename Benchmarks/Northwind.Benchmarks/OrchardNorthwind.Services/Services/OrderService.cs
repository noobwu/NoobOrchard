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
    /// Orders服务相关操作
    /// </summary>
    public abstract partial class OrderService:ServiceBase<Order,int>,IOrderService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Order repository</param>
        public OrderService(IRepository<Order,int> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
