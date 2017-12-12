using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;

using Noob.Web.Admin.Models;
using Noob.Web.Admin.Security;
using Noob.IServices;
using Orchard.Logging;
using Orchard.Caching;
using Orchard.Data;
using Noob.Domain.Entities;
using System.Threading.Tasks;
using Orchard.Web.Models;

namespace Noob.Web.Admin.EasyUI.Controllers.Cms
{
    /// <summary>
    /// 资讯类别 控制器
    /// </summary>
    [AdminAuthorize(Permission = "CMSNewsCategory")]
    public class NewsCategoryController : BaseAdminController
    {
        #region Members

        private readonly ICmsNewsCategoryService _service;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public NewsCategoryController(ICmsNewsCategoryService service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetList(int? id)
        {
            //return "{\"rows\":[]";
            var predicate = PredicateBuilder.True<CmsNewsCategory>();
            if (id.HasValue)
            {
                predicate = predicate.And(a => a.ParentID == id);
            }
            else
            {
                predicate = predicate.And(a => a.ParentID == 0);
            }
            IOrderByExpression<CmsNewsCategory>[] orderByExpressions ={
                 new OrderByExpression<CmsNewsCategory,int>(a=>a.SortOrder)
            };
            List<CmsNewsCategory> list = await _service.GetListAsync(predicate, orderByExpressions);
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
                    jsonBuilder.Append(list[i].CategoryId + "");
                    jsonBuilder.Append(",\"name\":");
                    jsonBuilder.Append("\"" + list[i].CategoryName + "\"");
                    jsonBuilder.Append(",\"CategoryCode\":");
                    jsonBuilder.Append("\"" + list[i].CategoryCode + "\"");
                    jsonBuilder.Append(",\"ImageUrl\":" + "\"" + list[i].ImageUrl + "\"");
                    jsonBuilder.Append(",\"SortOrder\":" + "\"" + list[i].SortOrder + "\"");
                    jsonBuilder.Append(",\"CategoryType\":" + "\"" + list[i].CategoryType + "\"");
                    jsonBuilder.Append(",\"_parentId\":");//显示树状固定字段
                    jsonBuilder.Append(list[i].ParentID + "");
                    jsonBuilder.Append(",\"state\":");//显示树状固定字段
                    int categoryId = list[i].CategoryId;
                    bool existChild = _service.Exists(a => a.ParentID == categoryId);
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
        public async Task<JsonResult> Create(CmsNewsCategoryModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<CmsNewsCategoryModel, CmsNewsCategory>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<CmsNewsCategoryModel, CmsNewsCategory>(model);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;
                entity.DeleteFlag = false;
                entity.DeleteUser = 0;
                entity.DeleteTime = new DateTime(1900, 01, 01);
                entity.IDPath = string.Empty;
                entity.NamePath = string.Empty;
                entity = await _service.InsertAsync(entity);
                if (entity.CategoryId > 0)
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
            var entity = await _service.SingleAsync(id);
            CheckData(entity);
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<CmsNewsCategory, CmsNewsCategoryModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<CmsNewsCategory, CmsNewsCategoryModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(CmsNewsCategoryModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                string IDPath = string.Empty;
                string NamePath = string.Empty;
                if (model.ParentID > 0)
                {
                    CmsNewsCategory tmpModel = await _service.SingleAsync(model.ParentID);
                    if (model == null)
                    {
                        data.Msg = "无效id";
                        result.Data = data;
                        return result;
                    }

                    IDPath = tmpModel.IDPath + model.CategoryId + ",";
                    NamePath = tmpModel.NamePath + model.CategoryName + ",";
                }
                else
                {
                    IDPath = "," + model.CategoryId + ",";
                    NamePath = "," + model.CategoryName + ",";
                }
                Expression<Func<CmsNewsCategory, CmsNewsCategory>> updateExpression = x => new CmsNewsCategory
                {
                    CategoryName = model.CategoryName,
                    CategoryCode = model.CategoryCode,
                    CategoryType = model.CategoryType,
                    SortOrder = model.SortOrder,
                    UpdateUser = LoginUserID,
                    UpdateTime = DateTime.Now,
                    ParentID = model.ParentID,
                    IDPath = IDPath,
                    NamePath = NamePath,
                    ImageUrl = model.ImageUrl,
                };
                Expression<Func<CmsNewsCategory, bool>> predicate = a => a.CategoryId == model.CategoryId;
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
            if (id < 1)
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
                    Expression<Func<CmsNewsCategory,bool>> predicate = a => a.CategoryId == id;
                    Expression<Func<CmsNewsCategory, CmsNewsCategory>> updateExpression =x=> new CmsNewsCategory
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
            var predicate = PredicateBuilder.True<CmsNewsCategory>();
            if (selfId != null && selfId.HasValue && selfId.Value > 0)
            {
                predicate = predicate.And(a => a.CategoryId != selfId.Value);
            }
            IOrderByExpression<CmsNewsCategory>[] orderByExpressions ={
                 new OrderByExpression<CmsNewsCategory,int>(a=>a.SortOrder)
            };
            List<CmsNewsCategory> list =await _service.GetListAsync(predicate, orderByExpressions);
            return ListToJson(list, 0, "10");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selfId"></param>
        /// <returns></returns>
        public async Task<string> GetTreeList(int? id, int? selfId)
        {
            var predicate = PredicateBuilder.True<CmsNewsCategory>();
            if (selfId != null && selfId.HasValue && selfId.Value > 0)
            {
                predicate = predicate.And(a => a.CategoryId != selfId.Value);
            }
            IOrderByExpression<CmsNewsCategory>[] orderByExpressions ={
                 new OrderByExpression<CmsNewsCategory,int>(a=>a.SortOrder)
            };
            List<CmsNewsCategory> list =await _service.GetListAsync(predicate, orderByExpressions);
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
        public string ListToJson(List<CmsNewsCategory> list, int parentId, string type)
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
        public string GetTreeJson(List<CmsNewsCategory> list, int parentId = 0, bool allExpand = false)
        {
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("[");
            List<CmsNewsCategory> tmpList = list.Where(a => a.ParentID == parentId).ToList();

            for (int i = 0; i < tmpList.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("" + tmpList[i].CategoryId + ",");
                jsonBuilder.Append("\"text\":");
                jsonBuilder.Append("\"" + tmpList[i].CategoryName + "\",");
                jsonBuilder.Append("\"state\":");
                if (list.Exists(a => a.ParentID == tmpList[i].CategoryId))
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
                    jsonBuilder.Append(GetTreeJson(list, tmpList[i].CategoryId, allExpand));
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