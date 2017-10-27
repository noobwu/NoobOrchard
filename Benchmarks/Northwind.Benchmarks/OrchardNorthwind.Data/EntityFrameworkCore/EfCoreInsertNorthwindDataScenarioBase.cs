using Microsoft.EntityFrameworkCore;
using OrchardNorthwind.Common.Data;
using OrchardNorthwind.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchardNorthwind.Data.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EfCoreInsertNorthwindDataScenarioBase : EfCoreScenarioBase
    {
        private static bool loadImages = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="loadImages"></param>
        static EfCoreInsertNorthwindDataScenarioBase()
        {
            NorthwindData.LoadData(loadImages);
        }
        protected DbSet<Category> categorySet;
        protected DbSet<Customer> customerSet;
        protected DbSet<Employee> employeeSet;
        protected DbSet<Supplier> supplierSet;
        protected DbSet<Product> productSet;
        protected DbSet<Shipper> shipperSet;
        protected DbSet<Order> orderSet;
        protected DbSet<OrderDetail> orderDetailsSet;
        protected DbSet<CustomerCustomerDemo> customerCustomerDemoSet;
        protected DbSet<Region> regionSet;
        protected DbSet<EmployeeTerritory> employeeTerritorySet;
        public virtual void InsertData(DbContext dbContext)
        {
            if (IsFirstRun)
            {
                categorySet = dbContext.Set<Category>();
                customerSet = dbContext.Set<Customer>();
                employeeSet = dbContext.Set<Employee>();
                supplierSet = dbContext.Set<Supplier>();
                productSet = dbContext.Set<Product>();
                shipperSet = dbContext.Set<Shipper>();
                orderSet = dbContext.Set<Order>();
                orderDetailsSet = dbContext.Set<OrderDetail>();
                customerCustomerDemoSet = dbContext.Set<CustomerCustomerDemo>();
                regionSet = dbContext.Set<Region>();
                employeeTerritorySet = dbContext.Set<EmployeeTerritory>();
            }
            //CreateTables(NorthwindFactory.ModelTypes.ToArray(), true,db);
            //NorthwindFactory.ModelTypes.ForEach(x => DeleteAll(x,db));
            NorthwindData.Categories.ForEach(x =>
            {
                x.CategoryId = 0;
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Categories] ON ");
                categorySet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Categories] OFF ");
            });
            dbContext.SaveChanges();

            NorthwindData.Customers.ForEach(x => customerSet.Add(x));
            dbContext.SaveChanges();

            NorthwindData.Employees.ForEach(x =>
            {
                x.EmployeeId = 0;
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Employees] ON ");
                employeeSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Employees] ON ");
            });
            dbContext.SaveChanges();


            NorthwindData.Suppliers.ForEach(x =>
            {
                x.SupplierId = 0;
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Suppliers] ON ");
                supplierSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Suppliers] OFF ");
            });
            dbContext.SaveChanges();


            NorthwindData.Products.ForEach(x =>
            {
                x.ProductId = 0;
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Products] ON ");
                productSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Products] OFF ");
            });
            dbContext.SaveChanges();

            NorthwindData.Shippers.ForEach(x =>
            {
                x.ShipperId = 0;
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Shippers] ON ");
                shipperSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Shippers] OFF ");
            });
            dbContext.SaveChanges();


            NorthwindData.Orders.ForEach(x =>
            {
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Orders] ON ");
                try
                {
                    var orderDetails = NorthwindData.OrderDetails.Where(a => a.OrderId == x.OrderId).ToList();
                    x.OrderId = 0;
                    var order = orderSet.Add(x).Entity;
                    dbContext.SaveChanges();
                    orderDetails.ForEach(a => a.OrderId = order.OrderId);
                    orderDetailsSet.AddRange(orderDetails);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Orders] OFF ");
            });
            dbContext.SaveChanges();
            //NorthwindData.OrderDetails.ForEach(x => Insert(x, db));

            NorthwindData.CustomerCustomerDemos.ForEach(x => customerCustomerDemoSet.Add(x));
            dbContext.SaveChanges();


            NorthwindData.Regions.ForEach(x => regionSet.Add(x));
            dbContext.SaveChanges();

            var territorySet = dbContext.Set<Territory>();
            NorthwindData.Territories.ForEach(x => territorySet.Add(x));
            dbContext.SaveChanges();


            NorthwindData.EmployeeTerritories.ForEach(x => employeeTerritorySet.Add(x));
            dbContext.SaveChanges();

        }
    }
}
