using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;

namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 广告
    /// </summary>
    [Serializable]
    [Validator(typeof(CmsAdvModelValidator))]
    public class CmsAdvModel : BaseNopModel
    {
        /// <summary>
        /// 广告ID
        /// </summary>
        public virtual int AdvID { get; set; }
        /// <summary>
        /// 广告名称
        /// </summary>
        public virtual string AdvName { get; set; }
        /// <summary>
        /// 广告标识
        /// </summary>
        public virtual string AdvCode { get; set; }
        /// <summary>
        /// 广告位ID
        /// </summary>
        public virtual int AdvPositionId { get; set; }
        /// <summary>
        /// 广告标题
        /// </summary>
        public virtual string AdvTitle { get; set; }
        /// <summary>
        /// 广告Html代码
        /// </summary>
        public virtual string AdvHtmlCode { get; set; }
        /// <summary>
        /// 广告图片地址
        /// </summary>
        public virtual string ImageUrl { get; set; }
        /// <summary>
        /// 广告链接地址
        /// </summary>
        public virtual string Url { get; set; }
        /// <summary>
        /// 广告类型 (1:外部链接广告 2 :内部链接广告 3:Html广告)
        /// </summary>
        public virtual byte AdvType { get; set; }
        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }
        /// <summary>
        /// 状态(1:启用  0:禁用)
        /// </summary>
        public virtual byte Status { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Remark { get; set; }

        /// <summary>
        /// 关联的资讯Id列表
        /// </summary>
        public virtual string NewsIds { get; set; }
        /// <summary>
        /// 关联的资讯标题
        /// </summary>
        public virtual string Titles { get; set; }

    }

}
