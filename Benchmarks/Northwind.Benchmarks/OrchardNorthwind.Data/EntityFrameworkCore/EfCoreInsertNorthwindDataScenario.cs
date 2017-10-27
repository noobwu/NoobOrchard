using System;
using Microsoft.EntityFrameworkCore;
namespace OrchardNorthwind.Data.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class EfCoreInsertNorthwindDataScenario: EfCoreInsertNorthwindDataScenarioBase
    {
      
        protected override void Run(DbContext dbContext)
        {
            try
            {
                dbContext.Database.ExecuteSqlCommand("exec ResetTables");
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
