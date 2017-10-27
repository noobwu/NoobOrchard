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
    /// Shippers服务相关操作
    /// </summary>
    public abstract partial class ShipperService:ServiceBase<Shipper,int>,IShipperService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Shipper repository</param>
        public ShipperService(IRepository<Shipper,int> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
