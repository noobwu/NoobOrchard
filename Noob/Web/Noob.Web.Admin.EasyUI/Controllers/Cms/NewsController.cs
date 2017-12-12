using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;

using Noob.Web.Admin.Security;
using Noob.IServices;
using Orchard.Logging;
using Orchard.Caching;
using Orchard.Data;
using Noob.Domain.Entities;
using System.Threading.Tasks;
using Noob.Web.Admin.Models;
using Orchard.Web.Models;

namespace Noob.Web.Admin.EasyUI.Controllers.Cms
{
    /// <summary>
    /// 资讯 控制器
    /// </summary>
    [AdminAuthorize(Permission = "CMSNews")]
    public class NewsController : BaseAdminController
    {
        #region Members

        private readonly ICmsNewsService _service;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public NewsController(ICmsNewsService service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
            : base(cacheManager,loggerFactory)
        {
            _service = service;
        }

        // GET
        public ActionResult Index()
        {
            ViewBag.Create = true;
            ViewBag.Edit = true;
            ViewBag.Delete = true;
            return View();
        }
        public async Task<string> GetList(string title, int? categoryId, int? status, int page = 1, int rows = 20)
        {
            //return "{\"rows\":[]";
            var predicate = PredicateBuilder.True<CmsNews>();
            if (!string.IsNullOrEmpty(title))
            {
                predicate = predicate.And(a => a.Title.Contains(title));
            }
            if (status.HasValue && status > -1)
            {
                predicate = predicate.And(a => a.Status == status);
            }
            if (categoryId.HasValue && categoryId > -1)
            {
                predicate = predicate.And(a => a.CategoryId == categoryId);
            }
            IOrderByExpression<CmsNews>[] orderByExpressions = {
                new OrderByExpression<CmsNews,int>(a=>a.NewsId)
            };
            int totalCount = await _service.CountAsync(predicate);
            List<CmsNews> pageList = await _service.GetPaggingListAsync(predicate, page, rows, orderByExpressions);
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("{");
            if (pageList != null && pageList.Count > 0)
            {
                jsonBuilder.Append("\"total\":" + totalCount + ",");
                jsonBuilder.Append("\"rows\":");
                jsonBuilder.Append("[");
                for (int i = 0; i < pageList.Count; i++)
                {
                    jsonBuilder.Append("{");
                    jsonBuilder.Append("\"NewsId\":" + "\"" + pageList[i].NewsId + "\"");
                    jsonBuilder.Append(",\"CategoryId\":" + "\"" + pageList[i].CategoryId + "\"");
                    jsonBuilder.Append(",\"Title\":" + "\"" + pageList[i].Title + "\"");
                    jsonBuilder.Append(",\"ImageUrl\":" + "\"" + pageList[i].ImageUrl + "\"");
                    jsonBuilder.Append(",\"NaviContent\":" + "\"" + pageList[i].NaviContent + "\"");
                    jsonBuilder.Append(",\"ReleaseTime\":" + "\"" + pageList[i].ReleaseTime + "\"");
                    jsonBuilder.Append(",\"ContentSource\":" + "\"" + pageList[i].ContentSource + "\"");
                    jsonBuilder.Append(",\"Author\":" + "\"" + pageList[i].Author + "\"");
                    jsonBuilder.Append(",\"Tag\":" + "\"" + pageList[i].Tag + "\"");
                    jsonBuilder.Append(",\"Status\":" + "\"" + pageList[i].Status + "\"");
                    jsonBuilder.Append(",\"SortOrder\":" + "\"" + pageList[i].SortOrder + "\"");
                    if (i == pageList.Count - 1)
                    {
                        jsonBuilder.Append("}");
                    }
                    else
                    {
                        jsonBuilder.Append("},");
                    }
                }
            }
            else
            {
                jsonBuilder.Append("\"total\":0,");
                jsonBuilder.Append("\"rows\":");
                jsonBuilder.Append("[");
            }
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<JsonResult> Create(CmsNewsModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                DateTime nowTime = DateTime.Now;
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<CmsNewsModel, CmsNews>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<CmsNewsModel, CmsNews>(model);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;
                entity = await _service.InsertAsync(entity);
                if (entity.NewsId > 0)
                {
                    data.Code = 1;
                    data.Msg = "添加成功";
                }
                else
                {
                    data.Msg = "添加失败";
                }

            }
            else
            {
                data.Msg = GetErrorMsgFromModelState();
            }
            result.Data = data;
            return result;

        }

        public async Task<ActionResult> Edit(int id)
        {
            var entity =await _service.SingleAsync(id);
            CheckData(entity);
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<CmsNews, CmsNewsModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<CmsNews, CmsNewsModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<JsonResult> Edit(CmsNewsModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                DateTime nowTime = DateTime.Now;
                Expression<Func<CmsNews,bool>> predicate = a => a.NewsId == model.NewsId;
                Expression<Func<CmsNews, CmsNews>> updateExpression=x=> new CmsNews
                {
                    UpdateUser = LoginUserID,
                    UpdateTime = nowTime,
                    Title = model.Title,
                    ImageUrl = model.ImageUrl,
                    CategoryId = model.CategoryId,
                    NaviContent = model.NaviContent,
                    ReleaseTime = model.ReleaseTime,
                    ContentSource = model.ContentSource,
                    Author = model.Author,
                    Tag = model.Tag,
                    Status = model.Status,
                    SortOrder = model.SortOrder,
                    NewsContent = model.NewsContent,
                    NewsType = model.NewsType
                };
                int updateResult =await _service.UpdateAsync(updateExpression,predicate);
                if (updateResult>0)
                {
                    data.Code = 1;
                    data.Msg = "更新成功";
                }
                else
                {
                    data.Msg = "更新失败";
                }
            }
            else
            {
                data.Msg = GetErrorMsgFromModelState();
            }
            result.Data = data;
            return result;

        }

        [HttpPost]
        public async Task<JsonResult> Delete(string id)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (string.IsNullOrEmpty(id))
            {
                data.Msg = "无效ID";
            }
            else
            {
                List<int> idList = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<int>();
                Expression<Func<CmsNews, bool>> predicate = x => idList.Contains(x.NewsId);
                int deleteResult =await _service.DeleteAsync(predicate);
                if (deleteResult > 0)
                {
                    data.Code = 1;
                    data.Msg = "删除成功";
                }
                else
                {
                    data.Msg = "删除失败";
                }
            }
            result.Data = data;
            return result;
        }

    }
}