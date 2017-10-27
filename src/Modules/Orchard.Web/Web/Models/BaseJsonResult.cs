using System;
using System.Runtime.Serialization;
namespace Orchard.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
   // [Serializable]
    public class BaseJsonResult
    {
        /// <summary>
        /// 状态码，0：失败1：成功
        /// </summary>
        [DataMember(Name = "code", Order = 1)]
       // [JsonProperty(PropertyName = "code",Order=1)]
        public int Code { set; get; }

        /// <summary>
        /// 提示信息
        /// </summary>
        [DataMember(Name = "msg", Order = 2)]
        //[JsonProperty(PropertyName = "msg", Order = 2)]
        public string Msg { set; get; }
    }
}
