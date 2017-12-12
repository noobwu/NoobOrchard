using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Orchard.Web.Models;

namespace Noob.Web.Admin.Models.Api
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class UploaderResult : BaseJsonResult
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "rs")]
        public FileInfo Result { set; get; }

        [DataContract]
        public class FileInfo
        {
            /// <summary>
            /// 文件Id
            /// </summary>
            [DataMember(Name = "id", Order = 3)]
            public string Id { set; get; }
            /// <summary>
            /// 文件路径
            /// </summary>
            [DataMember(Name = "filePath", Order = 4)]
            public string FilePath { set; get; }
        }
    }
}