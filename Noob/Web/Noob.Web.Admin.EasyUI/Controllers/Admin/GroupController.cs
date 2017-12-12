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
using System.Threading.Tasks;
using Orchard.Data;
using Orchard.Web.Models;

namespace Noob.Web.Admin.EasyUI.Controllers.Admin
{
    [AdminAuthorize(Permission = "AdmGroup,AdmChannelUserCreate,AdmChannelUserEdit")]
    /// <summary>
    /// 系统用户角色 控制器
    /// </summary>
    public class GroupController : BaseAdminController
    {
        #region Members

        private readonly IAdmGroupService _service;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public GroupController(IAdmGroupService service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
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
        public async Task<string> GetList()
        {
            //return "{\"rows\":[]";
            List<AdmGroup> list = await _service.GetListAsync(a => a.GroupID > 0);
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
                    jsonBuilder.Append(list[i].GroupID + ",");
                    jsonBuilder.Append("\"name\":");
                    jsonBuilder.Append("\"" + list[i].GroupName + "\",");
                    jsonBuilder.Append("\"SortOrder\":");
                    jsonBuilder.Append(list[i].SortOrder + ",");
                    jsonBuilder.Append("\"GroupType\":");
                    jsonBuilder.Append(list[i].GroupType + ",");
                    jsonBuilder.Append("\"CreateTime\":");
                    jsonBuilder.Append("\"" + list[i].CreateTime + "\",");
                    jsonBuilder.Append("\"_parentId\":");//显示树状固定字段
                    jsonBuilder.Append("0,");
                    jsonBuilder.Append("\"state\":");//显示树状固定字段
                    int childCount = 0;
                    if (childCount > 0)
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

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(AdmGroupModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmGroupModel, AdmGroup>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<AdmGroupModel, AdmGroup>(model);
                entity.DeleteFlag = false;
                entity.DeleteUser = 0;
                entity.DeleteTime = new DateTime(1900, 01, 01);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;
                entity=await _service.InsertAsync(entity);
                if (entity.GroupID > 0)
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
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmGroup, AdmGroupModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<AdmGroup, AdmGroupModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(AdmGroupModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                Expression<Func<AdmGroup, bool>> expression = a => a.GroupID == model.GroupID;
                Expression<Func<AdmGroup, AdmGroup>> updateExpression = a => new AdmGroup
                {
                    UpdateUser = LoginUserID,
                    UpdateTime = DateTime.Now,
                    GroupName = model.GroupName,
                    GroupType = (byte)model.GroupType,
                    SortOrder = model.SortOrder,
                    Description = model.Description,
                };
                int updateResult = await _service.UpdateAsync(updateExpression, expression);
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
                Expression<Func<AdmGroup, bool>> predicate = x => idList.Contains(x.GroupID);
                Expression<Func<AdmGroup, AdmGroup>> updateExpression = x => new AdmGroup
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
            result.Data = data;
            return result;
        }


        #region 获取树型结构

        public async Task<string> GetTreeList(int? channelId)
        {
            Expression<Func<AdmGroup, bool>> predicate = x => x.GroupID > 0;
            IOrderByExpression<AdmGroup>[] orderByExpressions = {
                new OrderByExpression<AdmGroup,int>(a=>a.SortOrder) };
            List<AdmGroup> list =await _service.GetListAsync(predicate, orderByExpressions);
            if (list == null || list.Count == 0)
            {
                return "[]";
            }
            return ListToJson(list);
        }

        /// <summary>
        /// 返回Tree数据，默认type:00 基础数据不展开 01:全展开 10:所有数据不展开 11:所有数据展开
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [NonAction]
        public string ListToJson(List<AdmGroup> list)
        {
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < list.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("" + list[i].GroupID + ",");
                jsonBuilder.Append("\"text\":");
                jsonBuilder.Append("\"" + list[i].GroupName + "\"");
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
