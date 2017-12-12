using AutoMapper;
using Noob.Domain.Entities;
using Noob.Services.GrainInterfaces;
using Noob.Web.Admin.Models;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Orleans;
using Orchard.Web.Models;

using Orleans;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Noob.Web.Admin.Bootstrap.Controllers.Admin
{
    //[AdminAuthorize(Permission = "AdmMenu")]
    public class MenuController : BaseAdminController
    {
        #region Members
        private readonly IAdmMenuGrain _service;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public MenuController(ICacheManager cacheManager, ILoggerFactory loggerFactory)
            : base(cacheManager, loggerFactory)
        {
            _service = GrainClient.GrainFactory.GetGrain<IAdmMenuGrain>(GetRandomGrainId());
        }
        // GET: Menu
        public ActionResult Index()
        {
            ViewBag.Create = true;
            ViewBag.Edit = true;
            ViewBag.Delete = true;
            return View();
        }

        public async Task<string> GetList()
        {
            //return "{\"rows\":[]";
            Expression<Func<AdmMenu, bool>> predicate = x => 1 == 1;
            IOrderByExpression<AdmMenu>[] orderByExpressions =
                {
                new OrderByExpression<AdmMenu,int>(a=>a.SortOrder)
            };
            //List<AdmMenu> list = await _service.GetListAsync(predicate, orderByExpressions);
            List<AdmMenu> list = await _service.GetListAsync(predicate.ToJObject(), orderByExpressions.ToJObjectArray());
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
                    jsonBuilder.Append("" + list[i].MenuID + ",");
                    jsonBuilder.Append("\"name\":");
                    jsonBuilder.Append("\"" + list[i].MenuName + "\",");
                    jsonBuilder.Append("\"MenuCode\":");
                    jsonBuilder.Append("\"" + list[i].MenuCode + "\",");
                    jsonBuilder.Append("\"MenuUrl\":");
                    jsonBuilder.Append("\"" + list[i].MenuUrl + "\",");
                    jsonBuilder.Append("\"SortOrder\":");
                    jsonBuilder.Append("" + list[i].SortOrder + ",");
                    jsonBuilder.Append("\"MenuType\":");
                    jsonBuilder.Append("" + list[i].MenuType + ",");
                    jsonBuilder.Append("\"CreateTime\":");
                    jsonBuilder.Append("\"" + list[i].CreateTime + "\",");
                    jsonBuilder.Append("\"_parentId\":");
                    jsonBuilder.Append("" + list[i].ParentID + ",");
                    jsonBuilder.Append("\"state\":");
                    bool existChild = list.Exists(a => a.ParentID == list[i].MenuID);
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
            jsonBuilder.AppendFormat(",\"draw\":{0},\"recordsTotal\":{1},\"recordsFiltered\":{1}", 1, list.Count);
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }


        public ActionResult Create(int? pid)
        {
            ViewBag.pid = pid;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(AdmMenuModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmMenuModel, AdmMenu>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<AdmMenuModel, AdmMenu>(model);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;
                entity.DeleteFlag = false;
                entity.DeleteTime = new DateTime(1900, 01, 01);
                entity.DeleteUser = 0;
                entity.IDPath = string.Empty;
                entity.NamePath = string.Empty;
                if (string.IsNullOrEmpty(model.MenuCode))
                {
                    entity.MenuCode = string.Empty;
                }
                if (string.IsNullOrEmpty(model.MenuUrl))
                {
                    entity.MenuUrl = string.Empty;
                }
                entity = await _service.InsertAsync(entity);
                if (entity.MenuID > 0)
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
            var entity = await _service.GetAdmMenuExtAsync(id);
            CheckData(entity);
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmMenuExt, AdmMenuModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<AdmMenuExt, AdmMenuModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(AdmMenuModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.MenuCode))
                {
                    model.MenuCode = "M" + model.MenuID;
                }
                if (string.IsNullOrEmpty(model.MenuUrl))
                {
                    model.MenuUrl = string.Empty;
                }

                Expression<Func<AdmMenu, AdmMenu>> updateExpression = null;
                if (model.ParentID > 0)
                {
                    AdmMenu tmpModel = await _service.SingleAsync(model.ParentID);
                    if (model == null)
                    {
                        data.Msg = "无效id";
                        result.Data = data;
                        return result;
                    }
                    updateExpression = x => new AdmMenu
                    {
                        UpdateUser = LoginUserID,
                        UpdateTime = DateTime.Now,
                        MenuCode = model.MenuCode,
                        MenuName = model.MenuName,
                        MenuUrl = model.MenuUrl,
                        RightsID = model.RightsID,
                        ParentID = model.ParentID,
                        MenuType = (byte)model.MenuType,
                        StatusFlag = (byte)model.StatusFlag,
                        SortOrder = model.SortOrder,
                        IDPath = tmpModel.IDPath + model.MenuID + "^",
                        NamePath = tmpModel.NamePath + model.MenuName + "^",
                    };
                }
                else
                {
                    updateExpression = x => new AdmMenu
                    {
                        UpdateUser = LoginUserID,
                        UpdateTime = DateTime.Now,
                        MenuCode = model.MenuCode,
                        MenuName = model.MenuName,
                        MenuUrl = model.MenuUrl,
                        RightsID = model.RightsID,
                        MenuType = (byte)model.MenuType,
                        StatusFlag = (byte)model.StatusFlag,
                        SortOrder = model.SortOrder,

                    };
                }
                Expression<Func<AdmMenu, bool>> predicate = a => a.MenuID == model.MenuID;
                //int updateResult = await _service.UpdateAsync(updateExpression, predicate);
                int updateResult = await _service.UpdateAsync(updateExpression.ToJObject<AdmMenu>(), predicate.ToJObject());
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
                Expression<Func<AdmMenu, bool>> predicate = x => idList.Contains(x.MenuID);
                Expression<Func<AdmMenu, AdmMenu>> updateExpression = x => new AdmMenu
                {
                    DeleteUser = LoginUserID,
                    DeleteFlag = true,
                    DeleteTime = DateTime.Now
                };
                //int updateResult = await _service.UpdateAsync(updateExpression, predicate);
                int updateResult = await _service.UpdateAsync(updateExpression.ToJObject<AdmMenu>(), predicate.ToJObject());
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

        #region 获取树型结构
        /// <summary>
        /// https://www.jstree.com/docs/json/
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selfId"></param>
        /// <returns></returns>
        public async Task<string> GetSubList(int? id, int? selfId)
        {
            Expression<Func<AdmMenu, bool>> predicate = a => a.MenuType == 0;
            if (id.HasValue && id > 0)
            {
                predicate = predicate.And(a => a.IDPath.Contains("^" + id + "^"));
            }
            if (selfId != null && selfId.HasValue && selfId.Value > 0)
            {
                predicate = predicate.And(a => a.MenuID != selfId.Value);
            }
            IOrderByExpression<AdmMenu>[] orderByExpressions = {
               new OrderByExpression<AdmMenu,int>(a=>a.SortOrder)
            };
            //List<AdmMenu> list = await _service.GetListAsync(predicate, orderByExpressions);
            List<AdmMenu> list = await _service.GetListAsync(predicate.ToJObject(), orderByExpressions.ToJObjectArray());
            return ListToJson(list, id, 0, "11");
        }

        public async Task<string> GetTreeList(int? id, int? selfId)
        {
            Expression<Func<AdmMenu, bool>> predicate = a => 1 == 1;
            if (selfId != null && selfId.HasValue && selfId.Value > 0)
            {
                predicate = predicate.And(a => a.MenuID != selfId.Value);
            }
            IOrderByExpression<AdmMenu>[] orderByExpressions = {
               new OrderByExpression<AdmMenu,int>(a=>a.SortOrder)
            };
            //List<AdmMenu> list = await _service.GetListAsync(predicate, orderByExpressions);
            List<AdmMenu> list = await _service.GetListAsync(predicate.ToJObject(), orderByExpressions.ToJObjectArray());
            return ListToJson(list, null, 0, "11");
        }
        /// <summary>
        /// 返回Tree数据，默认type:00 基础数据不展开 01:全展开 10:所有数据不展开 11:所有数据展开
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="curParentId"></param>
        /// <param name="parentId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [NonAction]
        public string ListToJson(List<AdmMenu> list, int? curParentId, int parentId, string type)
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
                return GetTreeJson(list, curParentId, parentId, allExpand);
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
        public string GetTreeJson(List<AdmMenu> list, int? curParentId, int parentId = 0, bool allExpand = false)
        {
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("[");
            List<AdmMenu> tmpList = list.Where(a => a.ParentID == parentId).ToList();
            for (int i = 0; i < tmpList.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("" + tmpList[i].MenuID + ",");
                jsonBuilder.Append("\"text\":");
                jsonBuilder.Append("\"" + tmpList[i].MenuName + "\",");
                if (list.Exists(a => a.ParentID == tmpList[i].MenuID))
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
                    if (curParentId.HasValue && tmpList[i].MenuID == curParentId.Value)
                    {
                        jsonBuilder.Append(",\"selected\":true");
                    }
                    jsonBuilder.Append("},\"children\":");
                    jsonBuilder.Append(GetTreeJson(list, curParentId, tmpList[i].MenuID, allExpand));
                }
                else
                {
                    jsonBuilder.Append("\"icon\":\"jstree-file\",\"state\":{\"opened\":true");
                    if (curParentId.HasValue && tmpList[i].MenuID == curParentId.Value)
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