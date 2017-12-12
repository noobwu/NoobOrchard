using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using AutoMapper;

using Noob.Domain.Entities;
using Noob.IServices;
using Noob.Web.Admin.Models;
using Noob.Web.Admin.Security;
using Orchard.Logging;
using Orchard.Caching;
using System.Threading.Tasks;
using Orchard.Data;
using Orchard.Web.Models;
using Noob.Domain;

namespace Noob.Web.Admin.EasyUI.Controllers.Admin
{
    [AdminAuthorize(Permission = "AdmRights")]
    public class RightsController : BaseAdminController
    {
        #region Members
        private readonly IAdmRightsService _service;
        private readonly IAdmRightsTypeService _rightsTypeService;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="rightsTypeService"></param>
        /// <param name="logFactory"></param>
        public RightsController(IAdmRightsService service, IAdmRightsTypeService rightsTypeService, ICacheManager cacheManager,ILoggerFactory loggerFactory)
            : base(cacheManager,loggerFactory)
        {
            _service = service;
            _rightsTypeService = rightsTypeService;
        }

        // GET: Rights
        public ActionResult Index()
        {
            ViewBag.Create = true;
            ViewBag.Edit = true;
            ViewBag.Delete = true;
            return View();
        }
        public async Task<string> GetList(int? tid)
        {
            //return "{\"rows\":[]";
            Expression<Func<AdmRightsExt, bool>> expression = PredicateBuilder.True<AdmRightsExt>();

            if (tid.HasValue)
            {
                expression = expression.And(a => a.RightsTypeID == tid);
            }
            IOrderByExpression<AdmRightsExt>[] orderByExpressions =
                           {
                new OrderByExpression<AdmRightsExt,int>(a=>a.SortOrder)
            };
            List<AdmRightsExt> list = await _service.GetAdmRightsExtListAsync(expression, orderByExpressions);
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"rows\":");
            jsonBuilder.Append("[");
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    jsonBuilder.Append("{");
                    jsonBuilder.Append("\"id\":");
                    jsonBuilder.Append("" + list[i].RightsID + ",");
                    jsonBuilder.Append("\"name\":");
                    jsonBuilder.Append("\"" + list[i].RightsName + "\",");
                    jsonBuilder.Append("\"TypeName\":");
                    jsonBuilder.Append("\"" + list[i].TypeName + "\",");
                    jsonBuilder.Append("\"RightsCode\":");
                    jsonBuilder.Append("\"" + list[i].RightsCode + "\",");
                    jsonBuilder.Append("\"RightsType\":");
                    jsonBuilder.Append("\"" + (list[i].RightsType == 0 ? "菜单权限" : "普通权限") + "\",");
                    jsonBuilder.Append("\"SortOrder\":");
                    jsonBuilder.Append("" + list[i].SortOrder + ",");
                    jsonBuilder.Append("\"CreateDT\":");
                    jsonBuilder.Append("\"" + list[i].CreateTime + "\",");
                    jsonBuilder.Append("\"RightsTypeID\":");
                    jsonBuilder.Append(list[i].RightsTypeID);
                    if (i == list.Count - 1)
                    {
                        jsonBuilder.Append("}");
                    }
                    else
                    {
                        jsonBuilder.Append("},");
                    }
                }
            }
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }


        /// <summary>
        /// 权限类别列表
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<string> GetRightsTypes(int? id)
        {
            var expression = PredicateBuilder.True<AdmRightsType>();

            if (id.HasValue)
            {
                expression = expression.And(a => a.ParentID == id);
            }
            else
            {
                expression = expression.And(a => a.ParentID == 0);
            }
            IOrderByExpression<AdmRightsType>[] orderByExpressions =
                                     {
                new OrderByExpression<AdmRightsType,int>(a=>a.SortOrder)
            };
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("[");
            List<AdmRightsType> list = await _rightsTypeService.GetListAsync(expression, orderByExpressions);
            List<AdmRightsType> allList = await _rightsTypeService.GetListAsync(a => 1 == 1, orderByExpressions);

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    jsonBuilder.Append("{");
                    jsonBuilder.Append("\"id\":");
                    jsonBuilder.Append("" + list[i].RightsTypeID + ",");
                    jsonBuilder.Append("\"text\":");
                    jsonBuilder.Append("\"" + list[i].TypeName + "\",");
                    jsonBuilder.Append("\"pid\":");
                    jsonBuilder.Append("" + list[i].ParentID + ",");
                    jsonBuilder.Append("\"state\":");
                    bool existChild = allList.Exists(a => a.ParentID == list[i].RightsTypeID);
                    if (existChild)
                    {
                        jsonBuilder.Append("\"closed\"");
                    }
                    else
                    {
                        jsonBuilder.Append("\"open\"");
                    }
                    if (i == list.Count - 1)
                    {
                        jsonBuilder.Append("}");
                    }
                    else
                    {
                        jsonBuilder.Append("},");
                    }
                }
            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

        public ActionResult Create(string tid)
        {
            ViewBag.tid = string.IsNullOrEmpty(tid) ? "0" : tid;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(AdmRightsModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmRightsModel, AdmRights>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<AdmRightsModel, AdmRights>(model);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.DeleteFlag = false;
                entity.DeleteTime = StaticConst.DATE_BEGIN;
                entity.DeleteUser = 0;
                model.Description = string.Empty;
                entity.UpdateTime = StaticConst.DATE_BEGIN;
                entity.UpdateUser = 0;
                entity = await _service.InsertAsync(entity);
                if (entity.RightsID > 0)
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
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmRights, AdmRightsModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<AdmRights, AdmRightsModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(AdmRightsModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                Expression<Func<AdmRights, AdmRights>> updateExpression = a => new AdmRights
                {
                    RightsName = model.RightsName,
                    RightsCode = model.RightsCode,
                    SortOrder = model.SortOrder,
                    RightsType = model.RightsType,
                    Description = model.Description,
                    UpdateUser = LoginUserID,
                    UpdateTime = DateTime.Now,
                };
                int updateResult =await _service.UpdateAsync(updateExpression, a => a.RightsID == model.RightsID);
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
                Expression<Func<AdmRights, bool>> predicate = x => idList.Contains(x.RightsID);
                Expression<Func<AdmRights, AdmRights>> updateExpression = a => new AdmRights
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


        /// <summary>
        ///根据权限类别获取菜单权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetMenuRights(int id)
        {
            var expression = PredicateBuilder.True<AdmRights>();
            expression = expression.And(a =>  a.RightsTypeID == id && a.RightsType == 0);
            IOrderByExpression<AdmRights>[] orderByExpressions =
                                             {
                new OrderByExpression<AdmRights,int>(a=>a.SortOrder)
            };
            List<AdmRights> list =await _service.GetListAsync(expression, orderByExpressions);
            if (list == null || list.Count == 0) return "[]";
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < list.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("" + list[i].RightsID + ",");
                jsonBuilder.Append("\"text\":");
                jsonBuilder.Append("\"" + list[i].RightsName + "\",");
                jsonBuilder.Append("\"state\":");
                jsonBuilder.Append("\"open\"");
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
    }
}