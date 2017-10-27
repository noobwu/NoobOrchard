using Orchard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFramework.Tests.Entities
{

    /// <summary>
    /// 系统角色权限扩展信息
    /// </summary>
    public class AdmUserRightsExt : AdmUserRights
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual AdmRightsExt AdmRights { get; set; }

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

        public bool Equals(AdmUserRights other)
        {

            //Check whether the compared object is null.
            if (object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return UserID.Equals(other.UserID) && RightsID.Equals(other.RightsID) && DeleteFlag.Equals(other.DeleteFlag);
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 
        public override int GetHashCode()
        {

            //Get hash code for the Code field. 
            int hashRightsID = RightsID.GetHashCode();

            //Get hash code for the Code field. 
            int hashUserID = UserID.GetHashCode();

            //Get hash code for the Code field. 
            int hashDeleteFlag = DeleteFlag.GetHashCode();

            //Calculate the hash code for the product. 
            return hashRightsID ^ hashUserID ^ hashDeleteFlag;
        }
    }
}
