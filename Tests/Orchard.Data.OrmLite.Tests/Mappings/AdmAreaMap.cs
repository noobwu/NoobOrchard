using Orchard.Domain.Entities;
using ServiceStack.DataAnnotations;
using System;

namespace Orchard.Data.OrmLite.Tests.Mappings
{
    /// <summary>
    /// 地区
    /// </summary>
    [Serializable]
    [Alias("wt_adm_area")]
    public class AdmAreaMap : Entity<int>, ISoftDelete
    {
        public AdmAreaMap()
        {

        }

        /// <summary>
        /// 自增长ID
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        [Alias("ID")]
        [Required]
        public override int Id { get; set; }

        ///<summary>
        /// 地区ID
        /// </summary>
        [StringLength(50)]
        [Required]
        public virtual string AreaID { get; set; }

        ///<summary>
        /// 地区名称
        /// </summary>
        [StringLength(50)]
        [Required]
        [Alias("AreaName")]
        public virtual string AreaName { get; set; }

        ///<summary>
        /// 父级ID
        /// </summary>
        [StringLength(50)]
        [Required]
        public virtual string ParentId { get; set; }

        ///<summary>
        /// 地区简称
        /// </summary>
        [StringLength(50)]
        [Required]
        public virtual string ShortName { get; set; }

        ///<summary>
        /// 地区类型(0:国家,1:直辖市或省份 2:市 3:区或者县)
        /// </summary>
        [Required]
        public virtual byte LevelType { get; set; }

        ///<summary>
        /// 区号
        /// </summary>
        [StringLength(50)]
        public virtual string CityCode { get; set; }

        ///<summary>
        /// 邮编
        /// </summary>
        [StringLength(50)]
        public virtual string ZipCode { get; set; }

        ///<summary>
        /// 完整地区名称
        /// </summary>
        [StringLength(500)]
        [Required]
        public virtual string AreaNamePath { get; set; }

        ///<summary>
        /// 完整地区名称
        /// </summary>
        [StringLength(500)]
        [Required]
        public virtual string AreaIDPath { get; set; }

        ///<summary>
        /// 经度
        /// </summary>
        [Alias("lng")]
        [Required]
        public virtual decimal Lng { get; set; }

        ///<summary>
        /// 纬度
        /// </summary>
        [Required]
        public virtual decimal Lat { get; set; }

        ///<summary>
        /// 拼音
        /// </summary>
        [StringLength(50)]
        public virtual string PinYin { get; set; }

        ///<summary>
        /// 拼音缩写
        /// </summary>
        [StringLength(20)]
        public virtual string ShortPinYin { get; set; }

        ///<summary>
        /// 拼音第一个字母
        /// </summary>
        [StringLength(10)]
        public virtual string PYFirstLetter { get; set; }

        ///<summary>
        /// 排序值
        /// </summary>
        [Required]
        public virtual int SortOrder { get; set; }

        ///<summary>
        /// 状态(1:启用,0:禁用)
        /// </summary>
        [Required]
        public virtual byte Status { get; set; }

        ///<summary>
        /// 创建时间
        /// </summary>
        [Required]
        public virtual DateTime CreateTime { get; set; }

        ///<summary>
        /// 创建人
        /// </summary>
        [Required]
        public virtual int CreateUser { get; set; }

        ///<summary>
        /// 最后更新时间
        /// </summary>
        [Required]
        public virtual DateTime UpdateTime { get; set; }

        ///<summary>
        /// 最后更新人
        /// </summary>
        [Required]
        public virtual int UpdateUser { get; set; }

        ///<summary>
        /// 删除状态
        /// </summary>
        [Required]
        public virtual bool DeleteFlag { get; set; }

        ///<summary>
        /// 删除时间
        /// </summary>
        [Required]
        public virtual DateTime DeleteTime { get; set; }

        ///<summary>
        /// 删除人
        /// </summary>
        [Required]
        public virtual int DeleteUser { get; set; }

    }
}
