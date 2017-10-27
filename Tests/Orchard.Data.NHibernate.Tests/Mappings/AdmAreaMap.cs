using Orchard.Data.NHibernate.EntityMappings;
using Orchard.Data.NHibernate.Tests.Entities;
using System;

namespace Orchard.Data.NHibernate.Tests.Mappings
{
    /// <summary>
    /// 地区 A shortcut of <see cref="EntityMap{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="AdmArea">Entity map</typeparam>
    [Serializable]
    public class AdmAreaMap : EntityMap<AdmArea, int>
    {
        /// <summary>
        /// 地区 Constructor.
        /// </summary>
        /// <param name="tableName">Table name</param>
        public AdmAreaMap() : base("wt_adm_area")
        {
            //Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.AreaID).Column("AreaID").Length(50);
            Map(x => x.AreaName).Column("AreaName").Length(50);
            Map(x => x.ParentId).Column("ParentId").Length(50);
            Map(x => x.ShortName).Column("ShortName").Length(50);
            Map(x => x.LevelType).Column("LevelType");
            Map(x => x.CityCode).Column("CityCode").Length(50).Nullable();
            Map(x => x.ZipCode).Column("ZipCode").Length(50).Nullable();
            Map(x => x.AreaNamePath).Column("AreaNamePath").Length(500);
            Map(x => x.AreaIDPath).Column("AreaIDPath").Length(500);
            Map(x => x.Lng).Column("lng");
            Map(x => x.Lat).Column("Lat");
            Map(x => x.PinYin).Column("PinYin").Length(50).Nullable();
            Map(x => x.ShortPinYin).Column("ShortPinYin").Length(20).Nullable();
            Map(x => x.PYFirstLetter).Column("PYFirstLetter").Length(10).Nullable();
            Map(x => x.SortOrder).Column("SortOrder");
            Map(x => x.Status).Column("Status");
            Map(x => x.CreateTime).Column("CreateTime");
            Map(x => x.CreateUser).Column("CreateUser");
            Map(x => x.UpdateTime).Column("UpdateTime");
            Map(x => x.UpdateUser).Column("UpdateUser");
        }
    }
}
