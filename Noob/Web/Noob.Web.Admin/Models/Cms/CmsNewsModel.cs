using System;
using System.Web;
using System.Data;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;

namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 资讯
    /// </summary>
    [Serializable]
    [Validator(typeof(CmsNewsModelValidator))]
    public class CmsNewsModel : BaseNopModel
    {
        /// <summary>
        /// 新闻Id
        /// </summary>
        public virtual int NewsId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public virtual string ImageUrl { get; set; }
        /// <summary>
        /// 类别Id
        /// </summary>
        public virtual int CategoryId { get; set; }
        /// <summary>
        /// 导读内容
        /// </summary>
        public virtual string NaviContent { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public virtual DateTime ReleaseTime { get; set; }
        /// <summary>
        /// 内容来源
        /// </summary>
        public virtual string ContentSource { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public virtual string Author { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public virtual string Tag { get; set; }
        /// <summary>
        /// 状态(1:启用  0:禁用)
        /// </summary>
        public virtual byte Status { get; set; }
        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }
        /// <summary>
        /// 资讯类型
        /// </summary>
        public virtual byte NewsType { get; set; }
        /// <summary>
        /// 新闻内容
        /// </summary>
        public virtual string NewsContent { get; set; }
     
    }

}
