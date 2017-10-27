using Orchard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFramework.Tests.Entities
{
    /// <summary>
    /// 系统权限分类表
    /// </summary>
    [Serializable]
    public class AdmRightsType : Entity<int>, ISoftDelete
    {
        /// <summary>
        /// 权限类别编号
        /// </summary>
    	public virtual int RightsTypeID { get { return Id; } set { } }
        /// <summary>
        /// 权限类别名称
        /// </summary>
        public virtual string TypeName { get; set; }
        /// <summary>
        /// 父类编号
        /// </summary>
        public virtual int ParentID { get; set; }
        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }
        /// <summary>
        /// ID路径
        /// </summary>
        public virtual string IDPath { get; set; }
        /// <summary>
        /// 名称路径
        /// </summary>
        public virtual string NamePath { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public virtual int CreateUser { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        public virtual int UpdateUser { get; set; }
        /// <summary>
        /// 删除标志 1删除
        /// </summary>
        public virtual bool DeleteFlag { get; set; }
        /// <summary>
        /// 删除用户
        /// </summary>
        public virtual int DeleteUser { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public virtual DateTime DeleteTime { get; set; }


    }

}
