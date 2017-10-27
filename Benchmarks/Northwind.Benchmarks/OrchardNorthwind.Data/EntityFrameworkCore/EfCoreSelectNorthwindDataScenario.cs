using Microsoft.EntityFrameworkCore;
using OrchardNorthwind.Common.Entities;
using System.Linq;

namespace OrchardNorthwind.Data.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class EfCoreSelectNorthwindDataScenario : EfCoreScenarioBase
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
