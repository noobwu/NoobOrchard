using OrchardNorthwind.Common.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchardNorthwind.Data.OrmLite
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class InsertNorthwindDataScenarioBase : OrmLiteScenarioBase
    {
        static InsertNorthwindDataScenarioBase()
        {
            NorthwindData.LoadData(false);
        }
        public void InsertDataByDb(IDbConnection db)
        {
            NorthwindData.Categories.ForEach(x => Insert(x, db));
            NorthwindData.Customers.ForEach(x => Insert(x, db));

            NorthwindData.Employees.ForEach(x => Insert(x, db));

            NorthwindData.Suppliers.ForEach(x => Insert(x, db));

            NorthwindData.Products.ForEach(x => Insert(x, db));

            NorthwindData.Shippers.ForEach(x => Insert(x, db));

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
            //NorthwindData.OrderDetails.ForEach(x => Insert(x, db));

            NorthwindData.CustomerCustomerDemos.ForEach(x => Insert(x, db));

            NorthwindData.Regions.ForEach(x => Insert(x, db));

            NorthwindData.Territories.ForEach(x => Insert(x, db));

            NorthwindData.EmployeeTerritories.ForEach(x => Insert(x, db));
        }
        public void Insert<T>(T obj, IDbConnection db)
        {
            try
            {
                db.Insert(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
