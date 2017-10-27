using OrchardNorthwind.Common.Data;
using OrchardNorthwind.Common.Entities;
using ServiceStack.OrmLite;
using System.Data;

namespace OrchardNorthwind.Data.OrmLite
{
    public class SelectNorthwindSupplierScenario
		: OrmLiteScenarioBase
	{
		private const int SupplierId = 1;

        protected override void Run(IDbConnection db)
		{
			if (this.IsFirstRun)
			{
				db.CreateTable<Supplier>(true);

				db.Insert(NorthwindFactory.Supplier(SupplierId, "Exotic Liquids", "Charlotte Cooper", "Purchasing Manager", "49 Gilbert St.", "London", null, "EC1 4SD", "UK", "(171) 555-2222", null, null));
			}

            db.SingleById<Supplier>(SupplierId);
		}

	}
}