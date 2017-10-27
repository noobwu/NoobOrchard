using OrchardNorthwind.Common.Data;
using OrchardNorthwind.Common.Entities;
using System;
using System.Data.Entity;
using System.Linq;

namespace OrchardNorthwind.Data.EntityFramework
{
    public class EfInsertNorthwindDataScenarioNoForeignKey : EfInsertNorthwindDataScenarioBase
    {
        protected override void Run(DbContext dbContext)
        {
            try
            {
                dbContext.Database.ExecuteSqlCommand("exec DropForeignKeys");
                dbContext.Database.ExecuteSqlCommand("exec DeleteTables");
                dbContext.SaveChanges();
                InsertData(dbContext);
                dbContext.Database.ExecuteSqlCommand("exec CreateForeignKeys");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        public override void InsertData(DbContext dbContext)
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
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Categories] ON ");
                categorySet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Categories] OFF ");
            });

            NorthwindData.Customers.ForEach(x => customerSet.Add(x));

            NorthwindData.Employees.ForEach(x =>
            {
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Employees] ON ");
                employeeSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Employees] ON ");
            });

            NorthwindData.Suppliers.ForEach(x =>
            {
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Suppliers] ON ");
                supplierSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Suppliers] OFF ");
            });
          
            NorthwindData.Products.ForEach(x =>
            {
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Products] ON ");
                productSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Products] OFF ");
            });

            NorthwindData.Shippers.ForEach(x =>
            {
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Shippers] ON ");
                shipperSet.Add(x);
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Shippers] OFF ");
            });
          
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Orders] OFF ");
            });
          
            //NorthwindData.OrderDetails.ForEach(x => Insert(x, db));

            NorthwindData.CustomerCustomerDemos.ForEach(x => customerCustomerDemoSet.Add(x));
          
            NorthwindData.Regions.ForEach(x => regionSet.Add(x));

            var territorySet = dbContext.Set<Territory>();
            NorthwindData.Territories.ForEach(x => territorySet.Add(x));

            NorthwindData.EmployeeTerritories.ForEach(x => employeeTerritorySet.Add(x));

            dbContext.SaveChanges();

        }
    }
}
