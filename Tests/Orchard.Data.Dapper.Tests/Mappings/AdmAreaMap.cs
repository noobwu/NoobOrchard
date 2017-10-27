using DapperExtensions.Mapper;
using Orchard.Data.Dapper.Tests.Entities;

namespace Orchard.Data.Dapper.Tests.Mappings
{
    public sealed class AdmAreaMap : ClassMapper<AdmArea>
    {
        public AdmAreaMap()
        {
            Table("wt_adm_area");
            Map(t => t.Id).Key(KeyType.Identity);
            AutoMap();
        }
    }
}
