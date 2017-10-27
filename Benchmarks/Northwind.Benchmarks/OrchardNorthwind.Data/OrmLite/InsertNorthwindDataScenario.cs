using OrchardNorthwind.Common.Data;
using ServiceStack.OrmLite;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace OrchardNorthwind.Data.OrmLite
{
    public class InsertNorthwindDataScenario
        : InsertNorthwindDataScenarioBase
    {

        protected override void Run(IDbConnection db)
        {
            try
            {
                //InsertData(db);
                InsertDataByResetTable(db);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        public void InsertDataByResetTable(IDbConnection db)
        {
            db.ExecuteSql("exec ResetTables");
            //CreateTables(NorthwindFactory.ModelTypes.ToArray(), true,db);
            //NorthwindFactory.ModelTypes.ForEach(x => DeleteAll(x,db));
            InsertDataByDb(db);
        }

        public void RunTests(IDbConnection db,long iterations = 1)
        {
            IsRunning = true;
            Console.WriteLine("NorthwindTest Test Starting");
            GC.Collect();
            var avgMs = Measure(() => { InsertDataByResetTable(db); }, iterations);
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


        public void InsertDataByStep(IDbConnection db)
        {
            //CreateTables(NorthwindFactory.ModelTypes.ToArray(), true,db);
            //NorthwindFactory.ModelTypes.ForEach(x => DeleteAll(x,db));
            Console.WriteLine("\nPress Enter to Run Categories Insert");
            Console.ReadLine();
            //db.ExecuteSql("SET IDENTITY_INSERT [Categories] ON ");
            NorthwindData.Categories.ForEach(x => Insert(x, db));
            //db.ExecuteSql("SET IDENTITY_INSERT [Categories] OFF ");
            Console.WriteLine("\nPress Enter to Run Customers Insert");
            Console.ReadLine();
            NorthwindData.Customers.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run Employees Insert");
            Console.ReadLine();
            NorthwindData.Employees.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run Suppliers Insert");
            Console.ReadLine();
            NorthwindData.Suppliers.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run Products Insert");
            Console.ReadLine();
            NorthwindData.Products.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run Shippers Insert");
            Console.ReadLine();
            NorthwindData.Shippers.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run Orders Insert");
            Console.ReadLine();
            NorthwindData.Orders.ForEach(x =>
            {
                try
                {
                    var orderDetails = NorthwindData.OrderDetails.Where(a => a.OrderId == x.OrderId).ToList();
                    var orderId = (int)db.Insert(x, true);
                    orderDetails.ForEach(a => a.OrderId = orderId);
                    db.InsertAll(orderDetails);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
            //Console.WriteLine("\nPress Enter to Run OrderDetails Insert");
            //Console.ReadLine();
            //NorthwindData.OrderDetails.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run CustomerCustomerDemos Insert");
            Console.ReadLine();
            NorthwindData.CustomerCustomerDemos.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run Regions Insert");
            Console.ReadLine();
            NorthwindData.Regions.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run Territories Insert");
            Console.ReadLine();
            NorthwindData.Territories.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run EmployeeTerritories Insert");
            Console.ReadLine();
            NorthwindData.EmployeeTerritories.ForEach(x => Insert(x, db));
            Console.WriteLine("\nPress Enter to Run  Exit");
            Console.ReadLine();

        }
        public void CreateTables(Type[] tableTypes, bool overwrite, IDbConnection db)
        {
            foreach (var tableType in tableTypes)
            {
                try
                {
                    db.CreateTable(overwrite, tableType);
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"{tableType.GetType()} {ex}");
                }
            }
        }
        public void DeleteAll(Type tableType, IDbConnection db)
        {
            try
            {
                db.DeleteAll(tableType);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"{tableType.GetType()} {ex}");
            }

        }

        public void InsertData(IDbConnection db)
        {
            if (this.IsFirstRun)
            {
                db.CreateTables(true, NorthwindFactory.ModelTypes.ToArray());
            }
            else
            {
                NorthwindFactory.ModelTypes.ForEach(x => db.DeleteAll(x));
            }
            NorthwindData.Categories.ForEach(x => db.Insert(x));
            NorthwindData.Customers.ForEach(x => db.Insert(x));
            NorthwindData.Employees.ForEach(x => db.Insert(x));
            NorthwindData.Suppliers.ForEach(x => db.Insert(x));
            NorthwindData.Products.ForEach(x => db.Insert(x));
            NorthwindData.Shippers.ForEach(x => db.Insert(x));
            NorthwindData.Orders.ForEach(x => db.Insert(x));
            NorthwindData.OrderDetails.ForEach(x => db.Insert(x));
            NorthwindData.CustomerCustomerDemos.ForEach(x => db.Insert(x));
            NorthwindData.Regions.ForEach(x => db.Insert(x));
            NorthwindData.Territories.ForEach(x => db.Insert(x));
            NorthwindData.EmployeeTerritories.ForEach(x => db.Insert(x));
        }
    }
}