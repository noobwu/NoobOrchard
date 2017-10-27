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
    /// Employees服务相关操作
    /// </summary>
    public abstract partial class EmployeeService:ServiceBase<Employee,int>,IEmployeeService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Employee repository</param>
        public EmployeeService(IRepository<Employee,int> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
