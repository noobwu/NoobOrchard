using OrchardNorthwind.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchardNorthwind.Data.EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class EfSelectNorthwindDataScenario : EfScenarioBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        protected override void Run(DbContext dbContext)
        {
            dbContext.Set<Category>().ToList();
            dbContext.Set<Customer>().ToList();
            dbContext.Set<Employee>().ToList();
            dbContext.Set<Shipper>().ToList();
            dbContext.Set<Order>().ToList();
            dbContext.Set<Product>().ToList();
            dbContext.Set<OrderDetail>().ToList();
            dbContext.Set<CustomerCustomerDemo>().ToList();
            dbContext.Set<Region>().ToList();
            dbContext.Set<Territory>().ToList();
            dbContext.Set<EmployeeTerritory>().ToList();
        }
    }
}
