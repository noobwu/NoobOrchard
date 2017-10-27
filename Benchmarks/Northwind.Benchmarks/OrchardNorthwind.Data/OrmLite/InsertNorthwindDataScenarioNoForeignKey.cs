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
    public class InsertNorthwindDataScenarioNoForeignKey : InsertNorthwindDataScenarioBase
    {
        protected override void Run(IDbConnection db)
        {
            try
            {
                db.ExecuteSql("exec DeleteTables");
                db.ExecuteSql("exec DropForeignKeys");
                InsertDataByDb(db);
                db.ExecuteSql("exec CreateForeignKeys");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
