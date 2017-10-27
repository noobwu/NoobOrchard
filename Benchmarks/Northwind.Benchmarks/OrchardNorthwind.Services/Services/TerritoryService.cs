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
    /// Territories服务相关操作
    /// </summary>
    public abstract partial class TerritoryService:ServiceBase<Territory,string>,ITerritoryService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Territory repository</param>
        public TerritoryService(IRepository<Territory,string> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
