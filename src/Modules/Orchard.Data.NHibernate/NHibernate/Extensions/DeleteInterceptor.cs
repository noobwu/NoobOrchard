using NHibernate;
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orchard.Data.NHibernate.Extensions
{
    //DeleteInterceptor类
    public class DeleteInterceptor : EmptyInterceptor
    {
        private static readonly Regex regex = new Regex("\\s+from\\s+([^\\s]+)\\s+([^\\s]+)\\s+");

        public override SqlString OnPrepareStatement(SqlString sql)
        {
            Match match = regex.Match(sql.ToString());
            String tableName = match.Groups[1].Value;
            String tableAlias = match.Groups[2].Value;

            sql = sql.Substring(match.Groups[2].Index);
            sql = sql.Replace(tableAlias, tableName);
            sql = sql.Insert(0, "delete from ");

            Int32 orderByIndex = sql.IndexOfCaseInsensitive(" order by ");

            if (orderByIndex > 0)
            {
                sql = sql.Substring(0, orderByIndex);
            }

            //不知道为何多了“limit 1”，不知道github作者知道不知道
            //投机取巧的在这里去除了“limit 1”
            int limitIndex = sql.IndexOfCaseInsensitive(" limit ");
            if (limitIndex > 0)
            {
                sql = sql.Substring(0, limitIndex);
            }

            return sql;
        }
    }
}
