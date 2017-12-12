using AutoMapper;
using Noob.Domain;
using Noob.Domain.Entities;
using Noob.Services.GrainInterfaces;
using Noob.Web.Admin.Models;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Logging;
using Orchard.Orleans;
using Orchard.Utility;
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
    //[AdminAuthorize(Permission = "AdmUser,AdmChannelUserCreate,AdmChannelUserEdit")]
    /// <summary>
    /// 用户 控制器
    /// </summary>
    public class UserController : BaseAdminController
    {
        #region Members

        private readonly IAdmUserGrain _service;
        private readonly IAdmGroupRightsGrain _groupRightsService;
        private readonly IAdmUserGroupGrain _userGroupService;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="groupRightsService"></param>
        /// <param name="userGroupService"></param>
        /// <param name="logFactory"></param>
        public UserController(ICacheManager cacheManager, ILoggerFactory loggerFactory)
            : base(cacheManager, loggerFactory)
        {
            _service = GrainClient.GrainFactory.GetGrain<IAdmUserGrain>(GetRandomGrainId());
            _groupRightsService = GrainClient.GrainFactory.GetGrain<IAdmGroupRightsGrain>(GetRandomGrainId());
            _userGroupService = GrainClient.GrainFactory.GetGrain<IAdmUserGroupGrain>(GetRandomGrainId());
        }

        // GET: User
        public ActionResult Index()
        {
            ViewBag.Create = true;
            ViewBag.Edit = true;
            ViewBag.Delete = true;
            return View();
        }
        public async Task<string> GetList(string userName, string trueName, int? orgId, int? status, int start = 1, int length = 20)
        {
            //return "{\"rows\":[]";
            if (start < 1) start = 1;
            if (length < 1) length = 10;
            var predicate = PredicateBuilder.True<AdmUserExt>();
            if (!string.IsNullOrEmpty(userName))
            {
                predicate = predicate.And(a => a.UserName.Contains(userName));
            }
            if (!string.IsNullOrEmpty(trueName))
            {
                predicate = predicate.And(a => a.TrueName.Contains(trueName));

            }
            if (orgId.HasValue && orgId > 0)
            {
                predicate = predicate.And(a => a.OrgID == orgId);
            }
            if (status.HasValue && status > -1)
            {
                predicate = predicate.And(a => a.Status == status);
            }
            IOrderByExpression<AdmUserExt>[] orderByExpressions ={
                new OrderByExpression<AdmUserExt,int>(a=>a.UserID)
            };
            //int totalCount = await _service.GetAdmUserExtCountAsync(predicate);
            int totalCount = await _service.GetAdmUserExtCountAsync(predicate.ToJObject());
            //List<AdmUserExt> paggingList = await _service.GetAdmUserExtPaggingListAsync(predicate, start, length, orderByExpressions);
            List<AdmUserExt> paggingList = await _service.GetAdmUserExtPaggingListAsync(predicate.ToJObject(), start, length, orderByExpressions.ToJObjectArray());
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("{");
            if (paggingList != null && paggingList.Count > 0)
            {
                //jsonBuilder.Append("\"total\":" + totalCount + ",");
                jsonBuilder.Append("\"recordsTotal\":" + totalCount + ",");//没有过滤的数据集的记录数量
                jsonBuilder.Append("\"recordsFiltered\":" + totalCount + ",");//过滤后数据集的记录数量
                jsonBuilder.Append("\"rows\":");
                jsonBuilder.Append("[");
                for (int i = 0; i < paggingList.Count; i++)
                {
                    jsonBuilder.Append("{");
                    jsonBuilder.Append("\"id\":");
                    jsonBuilder.Append(paggingList[i].UserID + ",");
                    jsonBuilder.Append("\"UserName\":");
                    jsonBuilder.Append("\"" + paggingList[i].UserName + "\",");
                    jsonBuilder.Append("\"TrueName\":");
                    jsonBuilder.Append("\"" + paggingList[i].TrueName + "\",");
                    jsonBuilder.Append("\"Email\":");
                    jsonBuilder.Append("\"" + paggingList[i].Email + "\",");
                    jsonBuilder.Append("\"Mobile\":");
                    jsonBuilder.Append("\"" + paggingList[i].Mobile + "\",");
                    jsonBuilder.Append("\"Phone\":");
                    jsonBuilder.Append("\"" + paggingList[i].Phone + "\",");
                    jsonBuilder.Append("\"OrgName\":");
                    jsonBuilder.Append("\"" + paggingList[i].OrgName + "\",");
                    jsonBuilder.Append("\"Status\":");
                    jsonBuilder.Append(paggingList[i].Status + ",");
                    jsonBuilder.Append("\"CreateTime\":");
                    jsonBuilder.Append("\"" + paggingList[i].CreateTime + "\"");
                    if (i == paggingList.Count - 1)
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

        public ActionResult Create(int? pid, int? DefaultUserId)
        {
            if (pid.HasValue && pid.Value > 0)
            {
                int groupId = 3;
                ViewBag.AdminID = pid.Value > 0 ? pid.ToString() : "";
                ViewBag.GroupID = groupId > 0 ? groupId.ToString() : "";
            }
            else
            {
                ViewBag.AdminID = string.Empty;
                ViewBag.GroupID = string.Empty;
            }
            if (DefaultUserId.HasValue)
            {
                ViewBag.DefaultUserId = 1;
            }
            else
            {
                ViewBag.DefaultUserId = 0;
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(AdmUserModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            // AdmUserModelValidator validator = new AdmUserModelValidator(_service);
            //ValidationResult test = validator.Validate(model);

            if (ModelState.IsValid)
            {
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmUserModel, AdmUser>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<AdmUserModel, AdmUser>(model);
                entity.Password = Utils.MD5(entity.Password);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;
                entity.DeleteFlag = false;
                entity.DeleteUser = 0;
                entity.DeleteTime = new DateTime(1900, 01, 01);
                entity.RegIP = RequestHelper.GetIP();
                List<AdmUserRights> userRights = null;
                List<AdmUserGroup> userGroups = null;
                if (!string.IsNullOrEmpty(model.GroupId))
                {
                    var groupIdList = ExpressionExtensions.ToList<int>(model.GroupId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    Expression<Func<AdmGroupRights, bool>> groupRightsPredicate = x => groupIdList.Contains(x.GroupID);

                    //var groupRights = await _groupRightsService.GetListAsync(groupRightsPredicate);
                    var groupRights = await _groupRightsService.GetListAsync(groupRightsPredicate.ToJObject());
                    if (groupRights != null && groupRights.Count > 0)
                    {
                        userRights = (from gr in groupRights select new AdmUserRights { CreateTime = entity.CreateTime, CreateUser = LoginUserID, DeleteFlag = false, DeleteTime = StaticConst.DATE_BEGIN, DeleteUser = 0, RightsID = gr.RightsID }).ToList();

                    }
                    if (groupIdList.Count > 0)
                    {
                        userGroups = (from gr in groupIdList select new AdmUserGroup { CreateTime = entity.CreateTime, CreateUser = LoginUserID, DeleteFlag = false, DeleteTime = StaticConst.DATE_BEGIN, DeleteUser = 0, GroupID = gr }).ToList();
                    }
                }
                if (model.DefaultUserId == 1)
                {
                    model.IsFounder = true;
                }
                int addResult = await _service.InsertAsync(entity, userRights, userGroups);
                if (addResult > 0)
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


        public async Task<ActionResult> Details(int id)
        {
            var entity = await _service.SingleAsync(id);
            CheckData(entity);
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmUser, AdmUserModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<AdmUser, AdmUserModel>(entity);
            model.GroupId = await GetGroupIdsByUserID(entity.UserID);
            return View(model);
        }
        public async Task<ActionResult> Edit(int id, int? DefaultUserId)
        {
            var entity = await _service.SingleAsync(id);
            CheckData(entity);
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmUser, AdmUserModel>());
            //Mapper.AssertConfigurationIsValid();
            var model = mapConfig.CreateMapper().Map<AdmUser, AdmUserModel>(entity);
            model.GroupId = await GetGroupIdsByUserID(entity.UserID);
            if (DefaultUserId.HasValue)
            {
                model.DefaultUserId = DefaultUserId.Value;
            }
            return View(model);
        }
        [NonAction]
        private async Task<string> GetGroupIdsByUserID(int userId)
        {
            Expression<Func<AdmUserGroup, bool>> predicate = a => a.UserID == userId;
            //List<AdmUserGroup> userGroups = await _userGroupService.GetListAsync(predicate);
            List<AdmUserGroup> userGroups = await _userGroupService.GetListAsync(predicate.ToJObject());
            if (userGroups == null || userGroups.Count == 0)
            {
                return string.Empty;
            }
            return string.Join(",", userGroups.Select(a => a.GroupID));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(AdmUserModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                if (model.DefaultUserId == 1)
                {
                    model.IsFounder = true;
                }
                else
                {
                    model.IsFounder = false;
                }
                Expression<Func<AdmUser, bool>> predicate = a => a.UserID == model.UserID;
                Expression<Func<AdmUser, AdmUser>> updateExpression = a => new AdmUser
                {
                    UpdateUser = LoginUserID,
                    UpdateTime = DateTime.Now,
                    //AdminID = model.AdminID,
                    UserName = model.UserName,
                    Email = model.Email,
                    TrueName = model.TrueName,
                    Phone = model.Phone,
                    Mobile = model.Mobile,
                    Description = model.Description,
                    Status = (byte)model.Status,
                    IsFounder = model.IsFounder
                };
                string[] groupIdArray = model.GroupId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var tmpGroupIdArray = ExpressionExtensions.ToList<int>(groupIdArray);
                List<AdmUserGroup> userGroups = null;
                if (tmpGroupIdArray.Count() > 0)
                {
                    userGroups = (from gr in tmpGroupIdArray
                                  select
                                  new AdmUserGroup
                                  {
                                      CreateTime = DateTime.Now,
                                      CreateUser = LoginUserID,
                                      DeleteFlag = false,
                                      DeleteTime = StaticConst.DATE_BEGIN,
                                      DeleteUser = 0,
                                      GroupID = gr,
                                      UserID = model.UserID
                                  }).ToList();
                }
                //int updateResult = await _service.UpdateAsync(updateExpression, model.UserID, userGroups);
                int updateResult = await _service.UpdateAsync(updateExpression.ToJObject<AdmUser>(), model.UserID, userGroups);
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
        public ActionResult ResetPassWord(int id)
        {
            ViewBag.UserID = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPassWord(AdmUserModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (model.UserID < 1)
            {
                data.Msg = "无效用户ID";
                result.Data = data;
                return result;
            }
            Expression<Func<AdmUser, AdmUser>> updateExpression = a => new AdmUser
            {
                UpdateUser = LoginUserID,
                UpdateTime = DateTime.Now,
                // Password = Utils.MD5(model.Password),
            };
            Expression<Func<AdmUser, bool>> predicate = a => a.UserID == model.UserID;
            //int updateResult = await _service.UpdateAsync(updateExpression, predicate);
            int updateResult = await _service.UpdateAsync(updateExpression.ToJObject<AdmUser>(), predicate.ToJObject());
            if (updateResult > 0)
            {
                data.Code = 1;
                data.Msg = "更新成功";
            }
            else
            {
                data.Msg = "更新失败";
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
                Expression<Func<AdmUser, bool>> predicate = x => idList.Contains(x.UserID);
                Expression<Func<AdmUser, AdmUser>> updateExpression = x => new AdmUser
                {
                    DeleteUser = LoginUserID,
                    DeleteFlag = true,
                    DeleteTime = DateTime.Now
                };
                //int updateResult = await _service.UpdateAsync(updateExpression, predicate);
                int updateResult = await _service.UpdateAsync(updateExpression.ToJObject<AdmUser>(), predicate.ToJObject());
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
