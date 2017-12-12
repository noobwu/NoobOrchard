using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;

using Noob.Domain.Entities;
using Noob.IServices;
using Noob.Web.Admin.Models;
using Noob.Web.Admin.Security;
using Orchard.Logging;
using Orchard.Caching;
using Orchard.Data;
using System.Threading.Tasks;
using Orchard.Web.Models;

namespace Noob.Web.Admin.EasyUI.Controllers.Admin
{
    /// <summary>
    /// 支付方式配置 控制器
    /// </summary>

    [AdminAuthorize(Permission = "AdmPayConfigs")]
    public class PayConfigsController : BaseAdminController
    {
        #region Members

        private readonly IAdmPayConfigsService _service;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public PayConfigsController(IAdmPayConfigsService service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
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
        public async Task<string> GetList(int page = 1, int rows = 20)
        {
            //return "{\"rows\":[]";
            var expression = PredicateBuilder.True<AdmPayConfigs>();
            IOrderByExpression<AdmPayConfigs>[] orderByExpressions =
                           {
                new OrderByExpression<AdmPayConfigs,int>(a=>a.SortOrder)
            };
            List<AdmPayConfigs> pageList = await _service.GetPaggingListAsync(expression, page, rows, orderByExpressions);
            int totalCount = await _service.CountAsync(expression);
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
                    jsonBuilder.Append("\"PayConfigID\":" + "\"" + pageList[i].PayConfigID + "\"");
                    jsonBuilder.Append(",\"PayName\":" + "\"" + pageList[i].PayName + "\"");
                    jsonBuilder.Append(",\"PayCode\":" + "\"" + pageList[i].PayCode + "\"");
                    jsonBuilder.Append(",\"PayType\":" + "\"" + pageList[i].PayType + "\"");
                    jsonBuilder.Append(",\"Logo\":" + "\"" + pageList[i].Logo + "\"");
                    jsonBuilder.Append(",\"Remark\":" + "\"" + pageList[i].Remark + "\"");
                    jsonBuilder.Append(",\"Note\":" + "\"" + pageList[i].Note + "\"");
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
        public async Task<JsonResult> Create(AdmPayConfigsModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Logo))
                {
                    model.Logo = string.Empty;
                }
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmPayConfigsModel, AdmPayConfigs>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<AdmPayConfigsModel, AdmPayConfigs>(model);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;

                entity = await _service.InsertAsync(entity);
                if (entity.PayConfigID > 0)
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
            var entity = await _service.SingleAsync(id);
            CheckData(entity);
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmPayConfigs, AdmPayConfigsModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<AdmPayConfigs, AdmPayConfigsModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(AdmPayConfigsModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                Expression<Func<AdmPayConfigs,AdmPayConfigs>> updateExpression =a=> new AdmPayConfigs
                {
                    UpdateUser = LoginUserID,
                    UpdateTime = DateTime.Now,
                    PayName = model.PayName,
                    PayCode = model.PayCode,
                    PayConfig = model.PayConfig,
                    PayType = model.PayType,
                    Remark = model.Remark,
                    Note = model.Note,
                    Status = model.Status,
                    SortOrder = model.SortOrder,
                };
                int updateResult =await _service.UpdateAsync(updateExpression, a=>a.PayConfigID==model.PayConfigID);
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
                var idList = ExpressionExtensions.ToList<int>(id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                Expression<Func<AdmPayConfigs, bool>> predicate = x => idList.Contains(x.PayConfigID);
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
