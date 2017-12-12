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
using Noob.Domain;
using Orchard.Web.Models;

namespace Noob.Web.Admin.Bootstrap.Controllers.Admin
{
    //[AdminAuthorize(Permission = "AdmRightsType")]
    public class RightsTypeController : BaseAdminController
    {
        #region Members

        private readonly IAdmRightsTypeService _service;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logFactory"></param>
        public RightsTypeController(IAdmRightsTypeService service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
            : base(cacheManager,loggerFactory)
        {
            _service = service;
        }
        // GET: RightsType
        public ActionResult Index()
        {
            ViewBag.Edit = true;
            ViewBag.Delete = true;
            ViewBag.Create = true;
            return View();
        }
        public async Task<string> GetList(int? id)
        {
            //return "{\"rows\":[]";
            var expression = PredicateBuilder.True<AdmRightsType>();

            //if (id.HasValue)
            //{
            //    expression = expression.And(a => a.ParentID == id);
            //}
            //else
            //{
            //    expression = expression.And(a => a.ParentID == 0);
            //}
            IOrderByExpression<AdmRightsType>[] orderByExpressions ={
                new OrderByExpression<AdmRightsType,int>(a=>a.SortOrder)
            };
            List<AdmRightsType> list = await _service.GetListAsync(expression, orderByExpressions);
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
                    jsonBuilder.Append("" + list[i].RightsTypeID + ",");
                    jsonBuilder.Append("\"name\":");
                    jsonBuilder.Append("\"" + list[i].TypeName + "\",");
                    jsonBuilder.Append("\"SortOrder\":");
                    jsonBuilder.Append("" + list[i].SortOrder + ",");
                    jsonBuilder.Append("\"CreateTime\":");
                    jsonBuilder.Append("\"" + list[i].CreateTime + "\",");
                    jsonBuilder.Append("\"_parentId\":");
                    jsonBuilder.Append("" + list[i].ParentID + ",");
                    jsonBuilder.Append("\"state\":");
                    //bool existChild = allList.Exists(a => a.ParentID == list[i].RightsTypeID);
                    int rightsTypeID = list[i].RightsTypeID;
                    //bool existChild = await _service.ExistsAsync(a => a.ParentID == rightsTypeID);
                    bool existChild = list.Exists(a => a.ParentID == rightsTypeID);
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
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        public ActionResult Create(string pid)
        {
            ViewBag.pid = string.IsNullOrEmpty(pid) ? "" : pid;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(AdmRightsTypeModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmRightsTypeModel, AdmRightsType>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<AdmRightsTypeModel, AdmRightsType>(model);
                entity = new AdmRightsType()
                {
                    TypeName = model.TypeName,
                    ParentID = model.ParentID,
                    SortOrder = model.SortOrder,
                    CreateTime = DateTime.Now,
                    CreateUser = LoginUserID,
                    DeleteFlag = false,
                    DeleteTime = StaticConst.DATE_BEGIN,
                    DeleteUser = 0,
                    UpdateTime = StaticConst.DATE_BEGIN,
                    UpdateUser = 0,
                    IDPath = string.Empty,
                    NamePath = string.Empty,
                };
                entity = await _service.InsertAsync(entity);
                if (entity.RightsTypeID > 0)
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

        //[AdminAuthorize(Permission = "AdmRightsTypeEdit")]
        public async Task<ActionResult> Edit(int id)
        {
            var entity = await _service.SingleAsync(id);
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmRightsType, AdmRightsTypeModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<AdmRightsType, AdmRightsTypeModel>(entity);
            return View(model);
        }

        //[AdminAuthorize(Permission = "AdmRightsTypeEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(AdmRightsTypeModel model)
        {

            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();

            if (ModelState.IsValid)
            {
                Expression<Func<AdmRightsType, bool>> predicate = a => a.RightsTypeID == model.RightsTypeID;
                Expression<Func<AdmRightsType, AdmRightsType>> updateExpression = null;

                if (model.ParentID > 0)
                {
                    AdmRightsType tmpModel = await _service.SingleAsync(model.ParentID);
                    if (model == null)
                    {
                        data.Msg = "无效id";
                        result.Data = data;
                        return result;
                    }
                    updateExpression = a => new AdmRightsType
                    {
                        TypeName = model.TypeName,
                        SortOrder = model.SortOrder,
                        UpdateUser = LoginUserID,
                        UpdateTime = DateTime.Now,
                        ParentID = model.ParentID,
                        IDPath = tmpModel.IDPath + model.RightsTypeID + "^",
                        NamePath = tmpModel.NamePath + model.TypeName + "^",
                    };
                }
                else
                {
                    updateExpression = a => new AdmRightsType
                    {
                        TypeName = model.TypeName,
                        SortOrder = model.SortOrder,
                        UpdateUser = LoginUserID,
                        UpdateTime = DateTime.Now,
                        ParentID = model.ParentID,
                        IDPath = "^" + model.RightsTypeID + "^",
                        NamePath = "^" + model.TypeName + "^",
                    };
                }
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
                // }
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
            AdmRightsType model = await _service.SingleAsync(id);
            if (model == null)
            {
                data.Msg = "无效id";
            }
            else
            {
                if (await _service.ExistsAsync(a => a.ParentID == id))
                {
                    data.Msg = "请先删除子权限类别";
                }
                else
                {
                    Expression<Func<AdmRightsType, bool>> predicate = a => a.RightsTypeID == model.RightsTypeID;
                    Expression<Func<AdmRightsType, AdmRightsType>> updateExpression =
                        a => new AdmRightsType
                        {
                            DeleteUser = LoginUserID,
                            DeleteFlag = true,
                            DeleteTime = DateTime.Now
                        };
                    int updateResult = await _service.UpdateAsync(updateExpression, predicate);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selfId"></param>
        /// <returns></returns>
        public async Task<string> GetSubList(int? id, int? selfId)
        {
            var expression = PredicateBuilder.True<AdmRightsType>();

            if (selfId != null && selfId.HasValue && selfId.Value > 0)
            {
                expression = expression.And(a => a.RightsTypeID != selfId.Value);
            }
            IOrderByExpression<AdmRightsType>[] orderByExpressions ={
                new OrderByExpression<AdmRightsType,int>(a=>a.SortOrder)
            };
            List<AdmRightsType> list = await _service.GetListAsync(expression, orderByExpressions);
            return ListToJson(list,id, 0, "10");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selfId"></param>
        /// <returns></returns>
        public async Task<string> GetTreeList(int? id, int? selfId)
        {
            var expression = PredicateBuilder.True<AdmRightsType>();

            if (selfId != null && selfId.HasValue && selfId.Value > 0)
            {
                expression = expression.And(a => a.RightsTypeID != selfId.Value);
            }
            IOrderByExpression<AdmRightsType>[] orderByExpressions ={
                new OrderByExpression<AdmRightsType,int>(a=>a.SortOrder)
            };
            List<AdmRightsType> list = await _service.GetListAsync(expression, orderByExpressions);
            return ListToJson(list, id, 0, "11");
        }

        /// <summary>
        /// 返回Tree数据，默认type:00 基础数据不展开 01:全展开 10:所有数据不展开 11:所有数据展开
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [NonAction]
        public string ListToJson(List<AdmRightsType> list, int? curParentId, int parentId, string type)
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
                return GetTreeJson(list,curParentId, parentId, allExpand);
            }
            else
            {
                return "[]";
            }
        }
        /// <summary>
        /// https://www.jstree.com/docs/json/
        /// https://github.com/vakata/jstree
        /// </summary>
        /// <param name="list"></param>
        /// <param name="curParentId"></param>
        /// <param name="parentId"></param>
        /// <param name="allExpand"></param>
        /// <returns></returns>
        [NonAction]
        public string GetTreeJson(List<AdmRightsType> list, int? curParentId, int parentId = 0, bool allExpand = false)
        {
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("[");
            List<AdmRightsType> tmpList = list.Where(a => a.ParentID == parentId).ToList();
            for (int i = 0; i < tmpList.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("" + tmpList[i].RightsTypeID + ",");
                jsonBuilder.Append("\"text\":");
                jsonBuilder.Append("\"" + tmpList[i].TypeName + "\",");
                if (list.Exists(a => a.ParentID == tmpList[i].RightsTypeID))
                {
                    jsonBuilder.Append("\"icon\":\"jstree-folder\",\"state\":{\"opened\":");
                    if (allExpand)
                    {
                        jsonBuilder.Append("true");
                    }
                    else
                    {
                        jsonBuilder.Append("false");
                    }
                    if (curParentId.HasValue && tmpList[i].RightsTypeID == curParentId.Value)
                    {
                        jsonBuilder.Append(",\"selected\":true");
                    }
                    jsonBuilder.Append("},\"children\":");
                    jsonBuilder.Append(GetTreeJson(list,curParentId, tmpList[i].RightsTypeID, allExpand));
                }
                else
                {
                    jsonBuilder.Append("\"icon\":\"jstree-file\",\"state\":{\"opened\":true");
                    if (curParentId.HasValue && tmpList[i].RightsTypeID == curParentId.Value)
                    {
                        jsonBuilder.Append(",\"selected\":true");
                    }
                    jsonBuilder.Append("}");
                }
                jsonBuilder.Append("},");
            }
            return jsonBuilder.ToString().Trim(',') + "]";
        }
        #endregion
    }
}