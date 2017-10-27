using OrchardNorthwind.Common.Entities;
using ServiceStack.OrmLite;
using System.Data;

namespace OrchardNorthwind.Data.OrmLite
{
    public class SelectNorthwindDataScenario
		: OrmLiteScenarioBase
    {
        protected override void Run(IDbConnection dbCmd)
		{
			//if (this.IsFirstRun)
			//{
			//	new InsertNorthwindDataScenario().InsertData(dbCmd);
			//}

			dbCmd.Select<Category>();
			dbCmd.Select<Customer>();
			dbCmd.Select<Employee>();
			dbCmd.Select<Shipper>();
			dbCmd.Select<Order>();
			dbCmd.Select<Product>();
			dbCmd.Select<OrderDetail>();
			dbCmd.Select<CustomerCustomerDemo>();
			dbCmd.Select<Region>();
			dbCmd.Select<Territory>();
			dbCmd.Select<EmployeeTerritory>();
		}
	}
}