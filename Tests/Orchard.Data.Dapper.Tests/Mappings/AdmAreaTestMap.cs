using DapperExtensions.Mapper;
using Orchard.Tests.Common.Domain.Entities;

namespace Orchard.Data.Dapper.Tests.Mappings
{
    /// <summary>
    /// wt_adm_area_test
    /// </summary>
    public class AdmAreaTestMap : ClassMapper<AdmAreaTest>
    {
        /// <summary>
        /// wt_adm_area_test
        /// </summary>
        public AdmAreaTestMap()
        {
            Table("wt_adm_area_test");
            Map(t => t.Id).Column("ID").Key(KeyType.Identity);

            Map(t => t.AreaId).Column("AreaID");
            Map(t => t.AreaIdPath).Column("AreaIDPath");
            Map(t => t.Lng).Column("lng");
            AutoMap();
        }
    }
}
