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
    /// Categories服务相关操作
    /// </summary>
    public abstract partial class CategoryService:ServiceBase<Category,int>,ICategoryService
    {

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Category repository</param>
        public CategoryService(IRepository<Category,int> repository):base(repository)
        {
        }

        #endregion
       
    }	    
}
