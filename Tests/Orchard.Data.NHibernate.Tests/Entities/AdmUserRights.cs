using Orchard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.NHibernate.Tests.Entities
{

    /// <summary>
    /// 系统角色权限扩展信息
    /// </summary>
    public class AdmUserRightsExt : AdmUserRights
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual AdmRights AdmRights { get; set; }

    }
    /// <summary>
    /// 系统用户权限
    /// </summary>
    [Serializable]
    public class AdmUserRights : Entity<int>, ISoftDelete
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual int UserRightID { get { return Id; } set { } }
        /// <summary>
        /// 管理员ID
        /// </summary>
        public virtual int UserID { get; set; }
        /// <summary>
        /// 权限编号
        /// </summary>
        public virtual int RightsID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public virtual int CreateUser { get; set; }
        /// <summary>
        /// 删除标志
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
