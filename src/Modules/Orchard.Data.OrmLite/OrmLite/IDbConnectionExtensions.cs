using ServiceStack.OrmLite;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Orchard.Data.OrmLite
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDbConnectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static List<string> GetColumnNames(IDbConnection db, string tableName)
        {
            var columns = new List<string>();
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "exec sp_columns " + tableName;
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var ordinal = reader.GetOrdinal("COLUMN_NAME");
                    columns.Add(reader.GetString(ordinal));
                }
                reader.Close();
            }
            return columns;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        public static void AlterTable<T>(this IDbConnection db) where T : new()
        {
            var model = ModelDefinition<T>.Definition;

            // just create the table if it doesn't already exist
            if (db.TableExists(model.ModelName) == false)
            {
                db.CreateTable<T>(overwrite: false);
                return;
            }

            // find each of the missing fields
            var columns = GetColumnNames(db, model.ModelName);
            var missing = ModelDefinition<T>.Definition.FieldDefinitions
                .Where(field => columns.Contains(field.FieldName) == false)
                .ToList();
        }
    }
}
