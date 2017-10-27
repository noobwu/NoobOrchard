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
    /// Region服务相关操作
    /// </summary>
    public abstract partial class RegionService:ServiceBase<Region,int>,IRegionService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Region repository</param>
        public RegionService(IRepository<Region,int> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
