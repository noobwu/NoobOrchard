using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OrchardNorthwind.Data.EntityFramework
{
    public class EfInsertNorthwindDataScenarioDeleteTable : EfInsertNorthwindDataScenarioBase
    {
        protected override void Run(DbContext dbContext)
        {
            try
            {
                dbContext.Database.ExecuteSqlCommand("exec DeleteTables");
                dbContext.SaveChanges();
                InsertData(dbContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        
        }
    }
}
