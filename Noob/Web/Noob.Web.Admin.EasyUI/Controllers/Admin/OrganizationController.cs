using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using AutoMapper;

using Noob.Domain.Entities;
using Noob.IServices;
using Noob.Web.Admin.Models;
using Noob.Web.Admin.Security;
using Orchard.Logging;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Web.Models;
using Noob.Domain;

namespace Noob.Web.Admin.EasyUI.Controllers.Admin
{
    [AdminAuthorize(Permission = "AdmOrganization")]
    public class OrganizationController : BaseAdminController
    {
        #region Members
        private readonly IAdmOrganizationService _service;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="_repository"></param>
        /// <param name="_service"></param>
        public OrganizationController(IAdmOrganizationService service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
            : base(cacheManager,loggerFactory)
        {
            _service = service;
        }
        // GET: Organization
        public ActionResult Index()
        {
            ViewBag.Create = true;
            ViewBag.Edit = true;
            ViewBag.Delete = true;
            return View();
        }

        public async Task<string> GetList(int? id)
        {
            //return "{\"rows\":[]";
            var expression = PredicateBuilder.True<AdmOrganization>();
            if (id.HasValue)
            {
                expression = expression.And(a => a.ParentID == id);
            }
            else
            {
                expression = expression.And(a => a.ParentID == 0);
            }
            IOrderByExpression<AdmOrganization>[] orderByExpressions ={
                new OrderByExpression<AdmOrganization,int>(a=>a.SortOrder)
            };
            List<AdmOrganization> list = await _service.GetListAsync(expression, orderByExpressions);
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
                    jsonBuilder.Append(list[i].OrgID + ",");
                    jsonBuilder.Append("\"name\":");
                    jsonBuilder.Append("\"" + list[i].OrgName + "\",");
                    jsonBuilder.Append("\"SortOrder\":");
                    jsonBuilder.Append(list[i].SortOrder + ",");
                    jsonBuilder.Append("\"CreateTime\":");
                    jsonBuilder.Append("\"" + list[i].CreateTime + "\",");
                    jsonBuilder.Append("\"StatusFlag\":");
                    jsonBuilder.Append(list[i].StatusFlag + ",");
                    jsonBuilder.Append("\"_parentId\":");//显示树状固定字段
                    jsonBuilder.Append(list[i].ParentID + ",");
                    jsonBuilder.Append("\"Address\":");
                    jsonBuilder.Append("\"" + list[i].Address + "\",");
                    jsonBuilder.Append("\"OfficePhone\":");
                    jsonBuilder.Append("\"" + list[i].OfficePhone + "\",");
                    jsonBuilder.Append("\"state\":");
                    int orgID = list[i].OrgID;
                    bool existsChildCount = await _service.ExistsAsync(a => a.ParentID == orgID);
                    if (existsChildCount)
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
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        public ActionResult Create(string pid)
        {
            ViewBag.pid = string.IsNullOrEmpty(pid) ? "0" : pid;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(AdmOrganizationModel model)
        {
            JsonResult result = new JsonResult();
           BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {

                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmOrganizationModel, AdmOrganization>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<AdmOrganizationModel, AdmOrganization>(model);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.DeleteFlag = false;
                entity.DeleteTime =StaticConst.DATE_BEGIN;
                entity.DeleteUser = 0;
                entity.UpdateTime =StaticConst.DATE_BEGIN;
                entity.UpdateUser = 0;
                entity.DefaultUserId = 0;
                entity.StatusFlag = 1;
                entity.IDPath = string.Empty;
                entity.NamePath = string.Empty;
                //if (_service.GetAdmOrganizationCount(a =>  a.ParentID == model.ParentID && a.OrgName == model.OrgName) > 0)
                //{
                //    data.Msg = "该名称已存在";
                //}
                //else
                //{
                entity= await _service.InsertAsync(entity);
                if (entity.OrgID > 0)
                {
                    data.Code = 1;
                    data.Msg = "添加成功";
                }
                else
                {
                    data.Msg = "添加失败";
                }
                result.Data = data;
                //}
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

            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmOrganization, AdmOrganizationModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<AdmOrganization, AdmOrganizationModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(AdmOrganizationModel model)
        {
            JsonResult result = new JsonResult();
           BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                //if (_service.GetAdmOrganizationCount(a =>  a.ParentID == model.ParentID && a.OrgName == model.OrgName && a.OrgID != model.OrgID) > 0)
                //{
                //    data.Msg = "该名称已存在";
                //    result.Data = data;
                //    return result;
                //}

                Expression<Func<AdmOrganization, bool>> predicate = a => a.OrgID == model.OrgID;
                Expression<Func<AdmOrganization, AdmOrganization>> updateExpression=a=>
                new AdmOrganization()
                {
                    OrgName = model.OrgName,
                    StatusFlag = model.StatusFlag,
                    SortOrder = model.SortOrder,
                    UpdateUser = LoginUserID,
                    UpdateTime = DateTime.Now,
                    Address = model.Address,
                    OfficePhone = model.OfficePhone
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
        public async Task<JsonResult> Delete(int id)
        {
            JsonResult result = new JsonResult();
           BaseJsonResult data = new BaseJsonResult();
            AdmOrganization model =await _service.SingleAsync(id);
            if (model == null)
            {
                data.Msg = "无效id";
            }
            else
            {
                if (await _service.ExistsAsync(a => a.ParentID == id))
                {
                    data.Msg = "请先删除子机构";
                }
                else
                {
                    Expression<Func<AdmOrganization, bool>> predicate = a => a.OrgID == model.OrgID;
                    Expression<Func<AdmOrganization, AdmOrganization>> updateExpression = a =>
                      new AdmOrganization()
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
            }
            result.Data = data;
            return result;
        }


        #region 获取树型结构

        public async Task<string> GetSubList(int? id, int? selfId)
        {
            var expression = PredicateBuilder.True<AdmOrganization>();

            if (selfId != null && selfId.HasValue && selfId.Value > 0)
            {
                expression = expression.And(a => a.OrgID != selfId.Value);
            }
            IOrderByExpression<AdmOrganization>[] orderByExpressions = {
                new OrderByExpression<AdmOrganization,int>(a=>a.SortOrder)
            };
            List<AdmOrganization> list =await _service.GetListAsync(expression, orderByExpressions);
            return ListToJson(list, 0, "10");
        }
        public async Task<string> GetTreeList(int? id, int? selfId)
        {
            var expression = PredicateBuilder.True<AdmOrganization>();

            if (selfId != null && selfId.HasValue && selfId.Value > 0)
            {
                expression = expression.And(a => a.OrgID != selfId.Value);
            }
            IOrderByExpression<AdmOrganization>[] orderByExpressions = {
                new OrderByExpression<AdmOrganization,int>(a=>a.SortOrder)
            };
            List<AdmOrganization> list =await _service.GetListAsync(expression, orderByExpressions);
            return ListToJson(list, 0, "11");
        }

        /// <summary>
        /// 返回Tree数据，默认type:00 基础数据不展开 01:全展开 10:所有数据不展开 11:所有数据展开
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [NonAction]
        public string ListToJson(List<AdmOrganization> list, int parentId, string type)
        {
            if (list != null && list.Count > 0)
            {
                bool allExpand = false;
                //bool allData = false;
                switch (type)
                {
                    case "01":
                        //allData = false;
                        allExpand = true;
                        break;
                    case "10":
                        //allData = true;
                        allExpand = false;
                        break;
                    case "11":
                        //allData = true;
                        allExpand = true;
                        break;
                    default:
                        //allData = false;
                        allExpand = false;
                        break;
                }
                //清空数据
                return GetTreeJson(list, parentId, allExpand);
            }
            else
            {
                return "[]";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parentId"></param>
        /// <param name="allExpand"></param>
        /// <returns></returns>
        [NonAction]
        public string GetTreeJson(List<AdmOrganization> list, int parentId = 0, bool allExpand = false)
        {
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("[");
            List<AdmOrganization> tmpList = list.Where(a => a.ParentID == parentId).ToList();
            for (int i = 0; i < tmpList.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("" + tmpList[i].OrgID + ",");
                jsonBuilder.Append("\"text\":");
                jsonBuilder.Append("\"" + tmpList[i].OrgName + "\",");
                jsonBuilder.Append("\"state\":");
                if (list.Exists(a => a.ParentID == tmpList[i].OrgID))
                {
                    if (allExpand)
                    {
                        jsonBuilder.Append("\"open\"");
                    }
                    else
                    {
                        jsonBuilder.Append("\"closed\"");
                    }
                    jsonBuilder.Append(",\"children\":");
                    jsonBuilder.Append(GetTreeJson(list, tmpList[i].OrgID, allExpand));
                }
                else
                {
                    jsonBuilder.Append("\"open\"");
                }
                if (i == tmpList.Count - 1)
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