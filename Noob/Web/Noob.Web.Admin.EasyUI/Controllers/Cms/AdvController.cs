using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;

using System.Threading.Tasks;
using Noob.Web.Admin.Security;
using Noob.IServices;
using Orchard.Logging;
using Orchard.Caching;
using Orchard.Data;
using Noob.Domain.Entities;
using Noob.Web.Admin.Models;
using Orchard.Web.Models;
using Orchard;

namespace Noob.Web.Admin.EasyUI.Controllers.Cms
{
    /// <summary>
    /// 广告管理 控制器
    /// </summary>
    [AdminAuthorize(Permission = "CMSAdv")]
    public class AdvController : BaseAdminController
    {
        #region Members
        private readonly ICmsAdvService _service;
        private readonly ICmsAdvExternalService _advExternalService;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public AdvController(ICmsAdvService service, ICmsAdvExternalService advExternalService, ICacheManager cacheManager,ILoggerFactory loggerFactory)
            : base(cacheManager,loggerFactory)
        {
            _service = service;
            _advExternalService = advExternalService;
        }

        // GET: Rights
        public ActionResult Index()
        {
            ViewBag.Create = true;
            ViewBag.Edit = true;
            ViewBag.Delete = true;
            return View();
        }
        public async Task<string> GetList(string advCode, string advName, int? advPositionId, int? status, int page = 1, int rows = 20)
        {
            //return "{\"rows\":[]";
            var predicate = PredicateBuilder.True<CmsAdvExt>();
            if (!string.IsNullOrEmpty(advCode))
            {
                predicate = predicate.And(a => a.AdvCode.Contains(advCode));
            }
            if (!string.IsNullOrEmpty(advName))
            {
                predicate = predicate.And(a => a.AdvName.Contains(advName));
            }
            if (status.HasValue && status > -1)
            {
                predicate = predicate.And(a => a.Status == status);
            }
            if (advPositionId.HasValue && advPositionId > -1)
            {
                predicate = predicate.And(a => a.AdvPositionId == advPositionId);
            }
            int totalCount = await _service.CountAsync(predicate);
            IOrderByExpression<CmsAdvExt>[] orderByExpressions = {
                new OrderByExpression<CmsAdvExt,int>(a=>a.SortOrder)
            };
            List<CmsAdvExt> pageList = await _service.GetCmsAdvExtPaggingListAsync(predicate, page, rows, orderByExpressions);
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
                    jsonBuilder.Append("\"AdvID\":" + "\"" + pageList[i].AdvID + "\"");
                    jsonBuilder.Append(",\"AdvName\":" + "\"" + pageList[i].AdvName + "\"");
                    jsonBuilder.Append(",\"AdvCode\":" + "\"" + pageList[i].AdvCode + "\"");
                    jsonBuilder.Append(",\"AdvTitle\":" + "\"" + pageList[i].AdvTitle + "\"");
                    jsonBuilder.Append(",\"ImageUrl\":" + "\"" + pageList[i].ImageUrl + "\"");
                    jsonBuilder.Append(",\"Url\":" + "\"" + pageList[i].Url + "\"");
                    jsonBuilder.Append(",\"AdvType\":" + "\"" + pageList[i].AdvType + "\"");
                    jsonBuilder.Append(",\"SortOrder\":" + "\"" + pageList[i].SortOrder + "\"");
                    jsonBuilder.Append(",\"Status\":" + "\"" + pageList[i].Status + "\"");
                    jsonBuilder.Append(",\"AdvPositionName\":" + "\"" + pageList[i].AdvPositionName + "\"");
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
        public async Task<JsonResult> Create(CmsAdvModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                DateTime nowTime = DateTime.Now;
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<CmsAdvModel, CmsAdv>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<CmsAdvModel, CmsAdv>(model);
                entity.CreateTime = nowTime;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;
                if (string.IsNullOrEmpty(entity.AdvHtmlCode))
                {
                    entity.AdvHtmlCode = string.Empty;
                }
                if (string.IsNullOrEmpty(entity.Url))
                {
                    entity.Url = string.Empty;
                }
                List<CmsAdvExternal> list = null;
                if (!string.IsNullOrEmpty(model.NewsIds))
                {
                    list = new List<CmsAdvExternal>();
                    var barberUserIdArray = model.NewsIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in barberUserIdArray)
                    {
                        list.Add(new CmsAdvExternal
                        {
                            CreateTime = nowTime,
                            CreateUser = LoginUserID,
                            AdvId = model.AdvID,
                            ExternalId = item.To<int>(),
                            ExternalIdType = 0
                        });
                    }
                }
                entity = await _service.InsertAsync(entity, list);
                if (entity.AdvID > 0)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int id)
        {
            CmsAdv entity = await _service.SingleAsync(id);
            CheckData(entity);
            List<CmsAdvExternalExt> list = await _advExternalService.GetCmsAdvExternalExtList(a => a.AdvId == id);
            CmsAdvModel model = null;
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<CmsAdv, CmsAdvModel>());
            //Mapper.AssertConfigurationIsValid();
            model = mapConfig.CreateMapper().Map<CmsAdv, CmsAdvModel>(entity);
            if (list != null && list.Count > 0)
            {
                model.NewsIds = string.Join(",", list.Select(a => a.ExternalId));
                model.Titles = string.Join(",", list.Select(a => a.Title));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<JsonResult> Edit(CmsAdvModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                Expression<Func<CmsAdv, bool>> predicate = a => a.AdvID == model.AdvID;
                if (string.IsNullOrEmpty(model.AdvHtmlCode))
                {
                    model.AdvHtmlCode = string.Empty;
                }
                if (string.IsNullOrEmpty(model.Url))
                {
                    model.Url = string.Empty;
                }
                DateTime nowTime = DateTime.Now;
                Expression<Func<CmsAdv, CmsAdv>> updateExpression = x => new CmsAdv
                {
                    AdvName = model.AdvName,
                    AdvCode = model.AdvCode,
                    AdvPositionId = model.AdvPositionId,
                    AdvTitle = model.AdvTitle,
                    AdvHtmlCode = model.AdvHtmlCode,
                    ImageUrl = model.ImageUrl,
                    Url = model.Url,
                    AdvType = model.AdvType,
                    SortOrder = model.SortOrder,
                    Status = model.Status,
                    Remark = model.Remark,
                    UpdateUser = LoginUserID,
                    UpdateTime = nowTime,
                };
                List<CmsAdvExternal> advExternalList = null;
                if (!string.IsNullOrEmpty(model.NewsIds))
                {
                    advExternalList = new List<CmsAdvExternal>();
                    var barberUserIdArray = model.NewsIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in barberUserIdArray)
                    {
                        advExternalList.Add(new CmsAdvExternal
                        {
                            CreateTime = nowTime,
                            CreateUser = LoginUserID,
                            AdvId = model.AdvID,
                            ExternalId = item.To<int>(),
                            ExternalIdType = 1
                        });
                    }
                }
                List<CmsAdvExternal> list = await _advExternalService.GetListAsync(a => a.AdvId == model.AdvID);
                int updateResult = await _service.UpdateAsync(updateExpression, advExternalList, list, model.AdvID);
                if (updateResult > 0)
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
                Expression<Func<CmsAdv, bool>> predicate = x => idList.Contains(x.AdvID);
                int deleteResult = await _service.DeleteAsync(predicate);
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


        /// <summary>
        /// 更新启用状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UpdateStatus(string id, byte status)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (string.IsNullOrEmpty(id) || (status != 1 && status != 0))
            {
                data.Msg = "无效ID";
            }
            else
            {
                List<int> idList = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<int>();
                Expression<Func<CmsAdv, bool>> predicate = x => idList.Contains(x.AdvID);
                Expression<Func<CmsAdv, CmsAdv>> updateExpression = x => new CmsAdv
                {
                    Status = status,
                    UpdateTime = DateTime.Now,
                    UpdateUser = LoginUserID
                };
                int updateResult = await _service.UpdateAsync(updateExpression, predicate);
                if (updateResult > 0)
                {
                    data.Code = 1;
                    data.Msg = "更新成功";
                }
                else
                {
                    data.Msg = "更新失败";
                }

            }
            result.Data = data;
            return result;
        }


    }

}
