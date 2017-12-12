using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Noob.Web.Admin.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class AdminMembership : IUser
    {
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// LastPasswordChangeTime
        /// </summary>
        public DateTime? LastPasswordChangeTime { get; set; }
    }
}