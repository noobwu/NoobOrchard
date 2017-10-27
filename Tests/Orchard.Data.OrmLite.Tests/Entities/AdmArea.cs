using Orchard.Domain.Entities;
using System;

namespace Orchard.Data.OrmLite.Tests.Entities
{
    /// <summary>
    /// 地区
    /// </summary>
    [Serializable]
    public class AdmArea : Entity<int>, ISoftDelete
    {
        /// <summary>
        /// 自增长ID
        /// </summary>
        public override int Id { get; set; }
        /// <summary>
        /// 地区ID
        /// </summary>
        public virtual string AreaID { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        public virtual string AreaName { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public virtual string ParentId { get; set; }
        /// <summary>
        /// 地区简称
        /// </summary>
        public virtual string ShortName { get; set; }
        /// <summary>
        /// 地区类型(0:国家,1:直辖市或省份 2:市 3:区或者县)
        /// </summary>
        public virtual byte LevelType { get; set; }
        /// <summary>
        /// 区号
        /// </summary>
        public virtual string CityCode { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public virtual string ZipCode { get; set; }
        /// <summary>
        /// 完整地区名称
        /// </summary>
        public virtual string AreaNamePath { get; set; }
        /// <summary>
        /// 完整地区名称
        /// </summary>
        public virtual string AreaIDPath { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public virtual decimal Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public virtual decimal Lat { get; set; }
        /// <summary>
        /// 拼音
        /// </summary>
        public virtual string PinYin { get; set; }
        /// <summary>
        /// 拼音缩写
        /// </summary>
        public virtual string ShortPinYin { get; set; }
        /// <summary>
        /// 拼音第一个字母
        /// </summary>
        public virtual string PYFirstLetter { get; set; }
        /// <summary>
        /// 排序值
        /// </summary>
        public virtual int SortOrder { get; set; }
        /// <summary>
        /// 状态(1:启用,0:禁用)
        /// </summary>
        public virtual byte Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public virtual int CreateUser { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 最后更新人
        /// </summary>
        public virtual int UpdateUser { get; set; }

        /// <summary>
        /// 删除状态
        /// </summary>
        public virtual bool DeleteFlag { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public virtual DateTime DeleteTime { get; set; }
        /// <summary>
        /// 删除人
        /// </summary>
        public virtual int DeleteUser { get; set; }
    }
}
