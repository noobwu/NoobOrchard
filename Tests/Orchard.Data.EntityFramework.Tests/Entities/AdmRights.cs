using Orchard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFramework.Tests.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AdmRightsExt : AdmRights
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual AdmRightsType AdmRightsType { get; set; }
    }
    /// <summary>
    /// 系统权限
    /// </summary>
    [Serializable]
    public class AdmRights : Entity<int>, ISoftDelete
    {
        /// <summary>
        /// 权限编号
        /// </summary>
    	public virtual int RightsID { get { return Id; } set { } }
        /// <summary>
        /// 权限名称
        /// </summary>
        public virtual string RightsCode { get; set; }
        /// <summary>
        /// 权限显示名称
        /// </summary>
        public virtual string RightsName { get; set; }
        /// <summary>
        /// 权限类别编号
        /// </summary>
        public virtual int RightsTypeID { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }
        /// <summary>
        /// 权限类型(0:菜单权限 1:普通权限)
        /// </summary>
        public virtual byte RightsType { get; set; }
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
