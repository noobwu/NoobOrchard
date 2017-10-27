using OrchardNorthwind.Common.Data;
using OrchardNorthwind.Common.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace OrchardNorthwind.Data.EntityFramework
{
    public abstract class EfInsertNorthwindDataScenarioBase : EfScenarioBase
    {
        private static bool loadImages = false;
        static EfInsertNorthwindDataScenarioBase()
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
            try
            {
                categorySet.AddRange(NorthwindData.Categories);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            NorthwindData.Customers.ForEach(x => customerSet.Add(x));
            dbContext.SaveChanges();

            NorthwindData.Employees.ForEach(x =>
            {
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Employees] ON ");
                employeeSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Employees] ON ");
            });
            dbContext.SaveChanges();


            NorthwindData.Suppliers.ForEach(x =>
            {
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Suppliers] ON ");
                supplierSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Suppliers] OFF ");
            });
            dbContext.SaveChanges();


            NorthwindData.Products.ForEach(x =>
            {
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Products] ON ");
                productSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Products] OFF ");
            });
            dbContext.SaveChanges();

            NorthwindData.Shippers.ForEach(x =>
            {
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
                    var order = orderSet.Add(x);
                    dbContext.SaveChanges();
                    orderDetails.ForEach(a => a.OrderId = order.OrderId);
                    orderDetailsSet.AddRange(orderDetails);
                }
                catch (DbEntityValidationException ex)
                {
                    LogDbEntityValidationException(ex);
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
        protected virtual void LogDbEntityValidationException(DbEntityValidationException exception)
        {
            foreach (var ve in exception.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
            {
                Console.WriteLine(" - " + ve.PropertyName + ": " + ve.ErrorMessage);
            }
        }
    }
}
