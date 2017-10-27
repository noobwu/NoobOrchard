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
    /// EmployeeTerritories服务相关操作
    /// </summary>
    public abstract partial class EmployeeTerritoryService:ServiceBase<EmployeeTerritory,string>,IEmployeeTerritoryService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">EmployeeTerritory repository</param>
        public EmployeeTerritoryService(IRepository<EmployeeTerritory,string> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
