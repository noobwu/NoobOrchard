using System.Web.Security;

namespace Orchard.Security.Providers {
    /// <summary>
    /// 
    /// </summary>
    public class DefaultSslSettingsProvider : ISslSettingsProvider {
        /// <summary>
        /// 
        /// </summary>
        public bool RequireSSL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DefaultSslSettingsProvider() {
            RequireSSL = FormsAuthentication.RequireSSL;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetRequiresSSL() {
            return RequireSSL;
        }
    }
}
