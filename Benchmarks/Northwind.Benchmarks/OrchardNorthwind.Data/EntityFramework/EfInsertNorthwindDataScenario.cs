using OrchardNorthwind.Common.Data;
using OrchardNorthwind.Common.Entities;
using OrchardNorthwind.Data.EntityFramework;
using System;
using System.Data.Common;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Data.Entity;

namespace OrchardNorthwind.Data.EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class EfInsertNorthwindDataScenario : EfScenarioBase
    {
        static EfInsertNorthwindDataScenario()
        {
            NorthwindData.LoadData(false);
        }
        public void RunTests(long iterations = 1)
        {
            IsRunning = true;
            Console.WriteLine("NorthwindTest Test Starting");
            GC.Collect();
            //Server=.;Database=NorthwindTest;User ID=sa;Password=123456;Pooling=true;Max Pool Size=32767;Min Pool Size=0;Asynchronous Processing=True;MultipleActiveResultSets=True;
            var db = new EfTestDbContext(new SqlConnection("Data Source=.;Initial Catalog=NorthwindTest;User ID=sa;Password=123456"));
            var avgMs = Measure(() => { InsertData(db); }, iterations);
            GC.Collect();
            Console.WriteLine("NorthwindTest Test Ending,Avg: {0} ms", avgMs);
            IsRunning = false;
        }
        private static decimal Measure(Action action, decimal iterations)
        {
            GC.Collect();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var begin = stopWatch.ElapsedMilliseconds;

            for (var i = 0; i < iterations; i++)
            {
                action();
            }

            var end = stopWatch.ElapsedMilliseconds;

            return (end - begin) / iterations;
        }

        public void InsertData(DbContext dbContext)
        {
            dbContext.Database.ExecuteSqlCommand("exec ResetTables");
            dbContext.SaveChanges();
            //CreateTables(NorthwindFactory.ModelTypes.ToArray(), true,db);
            //NorthwindFactory.ModelTypes.ForEach(x => DeleteAll(x,db));
            //db.ExecuteSql("SET IDENTITY_INSERT [Categories] ON ");
            var categorySet = dbContext.Set<Category>();
            NorthwindData.Categories.ForEach(x => categorySet.Add(x));
            dbContext.SaveChanges();
            //db.ExecuteSql("SET IDENTITY_INSERT [Categories] OFF ");
            var customerSet = dbContext.Set<Customer>();
            NorthwindData.Customers.ForEach(x => customerSet.Add(x));
            dbContext.SaveChanges();

            var employeeSet = dbContext.Set<Employee>();
            NorthwindData.Employees.ForEach(x => employeeSet.Add(x));
            dbContext.SaveChanges();

            var supplierSet = dbContext.Set<Supplier>();
            NorthwindData.Suppliers.ForEach(x => supplierSet.Add(x));
            dbContext.SaveChanges();

            var productSet = dbContext.Set<Product>();
            NorthwindData.Products.ForEach(x => productSet.Add(x));
            dbContext.SaveChanges();

            var shipperSet = dbContext.Set<Shipper>();
            NorthwindData.Shippers.ForEach(x => shipperSet.Add(x));
            dbContext.SaveChanges();

            var orderSet = dbContext.Set<Order>();
            var orderDetailsSet = dbContext.Set<OrderDetail>();
            NorthwindData.Orders.ForEach(x =>
            {
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
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }
            });
            dbContext.SaveChanges();

            //NorthwindData.OrderDetails.ForEach(x => Insert(x, db));
            var customerCustomerDemoSet = dbContext.Set<CustomerCustomerDemo>();
            NorthwindData.CustomerCustomerDemos.ForEach(x => customerCustomerDemoSet.Add(x));
            dbContext.SaveChanges();

            var regionSet = dbContext.Set<Region>();
            NorthwindData.Regions.ForEach(x => regionSet.Add(x));
            dbContext.SaveChanges();

            var territorySet = dbContext.Set<Territory>();
            NorthwindData.Territories.ForEach(x => territorySet.Add(x));
            dbContext.SaveChanges();

            var employeeTerritorySet = dbContext.Set<EmployeeTerritory>();
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

        protected override void Run(DbContext dbContext)
        {
            try
            {
                InsertData(dbContext);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }

        }
    }
}
