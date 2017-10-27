using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Data;
using ServiceStack.OrmLite;
using Orchard.Data;
using Orchard.Data.OrmLite.Repositories;
using OrchardNorthwind.Common.Entities;
using Orchard.Domain.Uow;
using Orchard.Data.OrmLite.Uow;
namespace OrchardNorthwind.Services.OrmLite
{

    /// <summary>
    /// CustomerCustomerDemo服务相关操作
    /// </summary>
    public partial class CustomerCustomerDemoService:OrchardNorthwind.Services.CustomerCustomerDemoService
    {
        /// <summary>
        /// 
        /// </summary>
        private OrmLiteRepositoryBase<CustomerCustomerDemo, string> _repository;
        
        /// <summary>
        /// 
        /// </summary>
        private ITransactionUnitOfWork<IDbConnection> _trans;
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public CustomerCustomerDemoService(OrmLiteRepositoryBase<CustomerCustomerDemo, string> repository) : base(repository)
        {
            _repository = repository;
           _trans = new OrmLiteTransactionUnitOfWork();
        }
    }	    
}
