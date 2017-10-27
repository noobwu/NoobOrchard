using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OrchardNorthwind.Data.EntityFrameworkCore
{
    public class EfCoreInsertNorthwindDataScenarioDeleteTable : EfCoreInsertNorthwindDataScenarioBase
    {
        protected override void Run(DbContext dbContext)
        {
            try
            {
                dbContext.SaveChanges();
                dbContext.Database.ExecuteSqlCommand("exec DeleteTables");
                InsertData(dbContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
          
        }
    }
}
