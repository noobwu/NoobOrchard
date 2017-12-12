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
    /// 系统配置 控制器
    /// </summary>
    [AdminAuthorize(Permission = "AdmConfigs")]
    public class ConfigsController : BaseAdminController
    {
        #region Members

        private readonly IAdmConfigsService _service;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public ConfigsController(IAdmConfigsService service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
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
        public async Task<string> GetList(string configGroupName, string configCode, string configName, int page = 1, int rows = 20)
        {
            //return "{\"rows\":[]";
            var expression = PredicateBuilder.True<AdmConfigs>();

            if (!string.IsNullOrEmpty(configGroupName))
            {
                expression = expression.And(a => a.ConfigGroupName == configGroupName);
            }
            if (!string.IsNullOrEmpty(configCode))
            {
                expression = expression.And(a => a.ConfigCode == configCode);
            }
            if (!string.IsNullOrEmpty(configName))
            {
                expression = expression.And(a => a.ConfigName == configName);
            }
            IOrderByExpression<AdmConfigs>[] orderByExpressions = {
                new OrderByExpression<AdmConfigs,int>(a=>a.SortOrder)
            };
            List<AdmConfigs> pageList = await _service.GetPaggingListAsync(expression, page, rows, orderByExpressions);
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
                    jsonBuilder.Append("\"ConfigID\":");
                    jsonBuilder.Append(pageList[i].ConfigID + ",");
                    jsonBuilder.Append("\"ConfigCode\":");
                    jsonBuilder.Append("\"" + pageList[i].ConfigCode + "\",");
                    jsonBuilder.Append("\"ConfigName\":");
                    jsonBuilder.Append("\"" + pageList[i].ConfigName + "\",");
                    jsonBuilder.Append("\"ConfigValue\":");
                    jsonBuilder.Append("\"" + HttpUtility.UrlEncode(pageList[i].ConfigValue) + "\",");
                    jsonBuilder.Append("\"ConfigGroupName\":");
                    jsonBuilder.Append("\"" + pageList[i].ConfigGroupName + "\",");
                    jsonBuilder.Append("\"Remark\":");
                    jsonBuilder.Append("\"" + pageList[i].Remark + "\",");
                    jsonBuilder.Append("\"Status\":");
                    jsonBuilder.Append(pageList[i].SortOrder + ",");
                    jsonBuilder.Append("\"CreateTime\":");
                    jsonBuilder.Append("\"" + pageList[i].CreateTime + "\"");
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
        public async Task<JsonResult> Create(AdmConfigsModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmConfigsModel, AdmConfigs>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<AdmConfigsModel, AdmConfigs>(model);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;
                entity.DeleteFlag = false;
                entity.DeleteTime = new DateTime(1900, 01, 01);
                entity.DeleteUser = 0;

                entity = await _service.InsertAsync(entity);
                if (entity.ConfigID > 0)
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
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmConfigs, AdmConfigsModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<AdmConfigs, AdmConfigsModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(AdmConfigsModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                Expression<Func<AdmConfigs, bool>> predicate = a => a.ConfigID == model.ConfigID;
                Expression<Func<AdmConfigs, AdmConfigs>> updateExpression = a =>
               new AdmConfigs
               {
                   UpdateUser = LoginUserID,
                   UpdateTime = DateTime.Now,
                   ConfigCode = model.ConfigCode,
                   ConfigName = model.ConfigName,
                   ConfigValue = model.ConfigValue,
                   ConfigGroupName = model.ConfigGroupName,
                   Remark = model.Remark,
                   Status = model.Status
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
                var idList = ExpressionExtensions.ToList<int>(id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                Expression<Func<AdmConfigs, bool>> predicate = x => idList.Contains(x.ConfigID);

                Expression<Func<AdmConfigs, AdmConfigs>> updateExpression =a=> new
                AdmConfigs
                {
                    DeleteUser = LoginUserID,
                    DeleteFlag = true,
                    DeleteTime = DateTime.Now
                };
                int updateResult =await _service.UpdateAsync(updateExpression, predicate);
                if (updateResult > 0)
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
