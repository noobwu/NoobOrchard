using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;


using Noob.Web.Admin.Models;
using Noob.IServices;
using Noob.Web.Admin.Security;
using Orchard.Logging;
using Orchard.Caching;
using Orchard.Data;
using Noob.Domain.Entities;
using System.Threading.Tasks;
using Orchard.Web.Models;

namespace Noob.Web.Admin.EasyUI.Controllers.Cms
{
    /// <summary>
    /// (广告位) 控制器
    /// </summary>
    [AdminAuthorize(Permission = "CMSAdvPosition")]
    public class AdvPositionController : BaseAdminController
    {
        #region Members

        private readonly ICmsAdvPositionService _service;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public AdvPositionController(ICmsAdvPositionService service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
            : base(cacheManager,loggerFactory)
        {
            _service = service;
        }

        // GET: Rights
        public ActionResult Index()
        {
            ViewBag.Create = true;
            ViewBag.Edit = true;
            ViewBag.Delete = true;
            return View();
        }
        public async Task<string> GetList(string advPositionCode, string advPositionName, int? status, int page = 1, int rows = 20)
        {
            //return "{\"rows\":[]";

            var predicate = PredicateBuilder.True<CmsAdvPosition>();
            if (!string.IsNullOrEmpty(advPositionCode))
            {
                predicate = predicate.And(a => a.AdvPositionCode.Contains(advPositionCode));
            }
            if (!string.IsNullOrEmpty(advPositionName))
            {
                predicate = predicate.And(a => a.AdvPositionName.Contains(advPositionName));

            }
            if (status.HasValue && status > -1)
            {
                predicate = predicate.And(a => a.Status == status);
            }
            IOrderByExpression<CmsAdvPosition>[] orderByExpressions = {
                new OrderByExpression<CmsAdvPosition,int>(a=>a.AdvPositionId)
            };
            int totalCount = await _service.CountAsync(predicate);
            List<CmsAdvPosition> pageList = await _service.GetPaggingListAsync(predicate, page, rows, orderByExpressions);
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
                    jsonBuilder.Append("\"AdvPositionId\":" + "\"" + pageList[i].AdvPositionId + "\"");
                    jsonBuilder.Append(",\"AdvPositionCode\":" + "\"" + pageList[i].AdvPositionCode + "\"");
                    jsonBuilder.Append(",\"AdvPositionName\":" + "\"" + pageList[i].AdvPositionName + "\"");
                    jsonBuilder.Append(",\"Status\":" + "\"" + pageList[i].Status + "\"");
                    jsonBuilder.Append(",\"Width\":" + "\"" + pageList[i].Width + "\"");
                    jsonBuilder.Append(",\"Height\":" + "\"" + pageList[i].Height + "\"");
                    jsonBuilder.Append(",\"SortOrder\":" + "\"" + pageList[i].SortOrder + "\"");
                    jsonBuilder.Append(",\"Remark\":" + "\"" + pageList[i].Remark + "\"");
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
        public async Task<JsonResult> Create(CmsAdvPositionModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<CmsAdvPositionModel, CmsAdvPosition>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<CmsAdvPositionModel, CmsAdvPosition>(model);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;

                entity = await _service.InsertAsync(entity);
                if (entity.AdvPositionId > 0)
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
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<CmsAdvPosition, CmsAdvPositionModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<CmsAdvPosition, CmsAdvPositionModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(CmsAdvPositionModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                Expression<Func<CmsAdvPosition,bool>> predicate = a => a.AdvPositionId == model.AdvPositionId;
                Expression<Func<CmsAdvPosition, CmsAdvPosition>> updateExpression =x=> new CmsAdvPosition
                {
                    UpdateUser = LoginUserID,
                    UpdateTime = DateTime.Now,
                    AdvPositionCode = model.AdvPositionCode,
                    AdvPositionName = model.AdvPositionName,
                    Status = model.Status,
                    Width = model.Width,
                    Height = model.Height,
                    SortOrder = model.SortOrder,
                    Remark = model.Remark
                };
                int updateResult =await _service.UpdateAsync(updateExpression, predicate);
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
                Expression<Func<CmsAdvPosition, bool>> predicate = x => idList.Contains(x.AdvPositionId);
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
                Expression<Func<CmsAdvPosition, bool>> predicate = x => idList.Contains(x.AdvPositionId);
                Expression<Func<CmsAdvPosition, CmsAdvPosition>> updateExpression=x=>new CmsAdvPosition
                {
                    Status = status,
                    UpdateTime=DateTime.Now,
                    UpdateUser=LoginUserID
                };
                int updateResult =await _service.UpdateAsync(updateExpression, predicate);
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


        #region 获取树型结构

        public async Task<string> GetSubList(int? id)
        {
            var predicate = PredicateBuilder.True<CmsAdvPosition>();
            IOrderByExpression<CmsAdvPosition>[] orderByExpressions = {
                new OrderByExpression<CmsAdvPosition,int>(a=>a.SortOrder)
            };
            List<CmsAdvPosition> list =await _service.GetListAsync(predicate, orderByExpressions);
            return GetComboboxData(list);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        [NonAction]
        public string GetComboboxData(List<CmsAdvPosition> list)
        {
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < list.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("" + list[i].AdvPositionId + ",");
                jsonBuilder.Append("\"text\":");
                jsonBuilder.Append("\"" + list[i].AdvPositionName + "\",");
                jsonBuilder.Append("\"desc\":\"" + list[i].AdvPositionName + "\"");
                if (i == list.Count - 1)
                {
                    jsonBuilder.Append("}");
                }
                else
                {
                    jsonBuilder.Append("},");
                }
            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }
        #endregion
    }

}
