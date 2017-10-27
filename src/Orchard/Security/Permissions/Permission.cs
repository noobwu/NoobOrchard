using System.Collections.Generic;

namespace Orchard.Security.Permissions {
    /// <summary>
    /// 
    /// </summary>
    public class Permission {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Permission> ImpliedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool RequiresOwnership { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Permission Named(string name) {
            return new Permission { Name = name };
        }
    }
}