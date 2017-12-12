using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

using Noob.Domain.Entities;
using Noob.IServices;
using Noob.Web.Admin.Security;
using Orchard.Logging;
using Orchard.Caching;
using Orchard.Web.Models;
using System.Threading.Tasks;
using Noob.Domain;
using Orchard.Data;

namespace Noob.Web.Admin.EasyUI.Controllers.Admin
{
    /// <summary>
    /// 
    /// </summary>
    [AdminAuthorize(Permission = "AdmGroupAuthorize,AdmUserAuthorize")]
    public class AuthorizeController : BaseAdminController
    {
        #region Members

        private readonly IAdmRightsService _service;
        private readonly IAdmRightsTypeService _rightsTypeService;
        private readonly IAdmGroupRightsService _groupRightsService;
        private readonly IAdmUserRightsService _userRightsService;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="rightsTypeService"></param>
        /// <param name="groupRightsService"></param>
        /// <param name="userRightsService"></param>
        /// <param name="logFactory"></param>
        public AuthorizeController(IAdmRightsService service, IAdmRightsTypeService rightsTypeService, IAdmGroupRightsService groupRightsService, IAdmUserRightsService userRightsService, ICacheManager cacheManager,ILoggerFactory loggerFactory)
            : base(cacheManager,loggerFactory)
        {
            _service = service;
            _rightsTypeService = rightsTypeService;
            _groupRightsService = groupRightsService;
            _userRightsService = userRightsService;
        }

        // GET: Rights
        public ActionResult Index(int id, int type)
        {
            ViewBag.Create = true;
            ViewBag.Edit = true;
            ViewBag.Delete = true;
            ViewBag.id = id;
            ViewBag.type = type;
            return View();
        }

        // GET: Authorize
        [HttpPost]
        public async Task<ActionResult> AuthorizeRights(int id, int type, string rightsId)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (id == 0 || (type != 0 && type != 1) || string.IsNullOrEmpty(rightsId))
            {
                data.Msg = "无效的参数";
            }
            else
            {
                string[] tmpRightsList = rightsId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> rightsList = tmpRightsList.ToList<int>();
                if (rightsList == null || rightsList.Count == 0)
                {
                    data.Msg = "无效的参数";
                }
                else
                {
                    DateTime nowTime = DateTime.Now;
                    if (type == 0)
                    {
                        List<AdmGroupRights> list = (from i in rightsList
                                                     select new AdmGroupRights {
                                                         CreateTime = nowTime,
                                                         DeleteFlag = false,
                                                         CreateUser = LoginUserID,
                                                         DeleteTime = StaticConst.DATE_BEGIN,
                                                         DeleteUser = 0, GroupID = id, RightsID = i }).ToList();
                        int authorizeResult =await _groupRightsService.AuthorizeAdmGroupRightsAsync(list, id);
                        switch (authorizeResult)
                        {
                            case 0:
                                data.Msg = "授权失败";
                                break;
                            case 1:
                                data.Code = 1;
                                data.Msg = "授权成功";
                                break;
                            default:
                                data.Msg = "无效的参数";
                                break;
                        }

                    }
                    else
                    {
                        List<AdmUserRights> list = (from i in rightsList select new AdmUserRights { CreateTime = nowTime, DeleteFlag = false, CreateUser = LoginUserID, DeleteTime = StaticConst.DATE_BEGIN, DeleteUser = 0, UserID = id, RightsID = i }).ToList();
                        int authorizeResult = _userRightsService.AuthorizeAdmUserRights(list, id);
                        switch (authorizeResult)
                        {
                            case 0:
                                data.Msg = "授权失败";
                                break;
                            case 1:
                                data.Code = 1;
                                data.Msg = "授权成功";
                                break;
                            default:
                                data.Msg = "无效的参数";
                                break;
                        }
                    }
                }
            }
            result.Data = data;
            return result;
        }
        public async Task<string> GetList(int id, int type)
        {
            if (id == 0 || (type != 0 && type != 1))
            {
                return "{\"rows\":[]}";
            }
            if (type == 0)
            {
                return await GetGroupRights(id);
            }
            else
            {
                return await GetUserRights(id);
            }

        }

        #region Methods
        [NonAction]
        public async Task<string> GetGroupRights(int groupId)
        {
            List<AdmGroupRightsExt> groupRights = await _groupRightsService.GetAdmGroupRightsExtListAsync(
                a => a.GroupID == groupId);
            Expression<Func<AdmRightsType, bool>> rightsTypePredicate = PredicateBuilder.True<AdmRightsType>();
            IOrderByExpression<AdmRightsType>[] rightsTypeOrderByExpressions = {
                new OrderByExpression<AdmRightsType,int>(a=>a.SortOrder)
            };
            List<AdmRightsType> rightsTypeList = await _rightsTypeService.GetListAsync(rightsTypePredicate, rightsTypeOrderByExpressions);
            if (rightsTypeList == null || rightsTypeList.Count == 0)
            {
                return "{\"rows\":[]}";
            }

            Expression<Func<AdmRightsExt, bool>> predicate = PredicateBuilder.True<AdmRightsExt>();
            IOrderByExpression<AdmRightsExt>[] orderByExpressions ={
                new OrderByExpression<AdmRightsExt,int>(a=>a.SortOrder)
            };
            List<AdmRightsExt> list = await _service.GetAdmRightsExtListAsync(predicate, orderByExpressions);
            if (list == null || list.Count == 0)
            {
                return "{\"rows\":[]}";
            }
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"rows\":");
            jsonBuilder.Append("[");
            for (int i = 0; i < rightsTypeList.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("\"" + rightsTypeList[i].RightsTypeID + "\",");
                jsonBuilder.Append("\"name\":");
                jsonBuilder.Append("\"" + rightsTypeList[i].TypeName + "\",");
                jsonBuilder.Append("\"code\":");
                jsonBuilder.Append("\"\",");
                jsonBuilder.Append("\"type\":");
                jsonBuilder.Append("0,");
                jsonBuilder.Append("\"rightsType\":");
                jsonBuilder.Append("-1,");
                jsonBuilder.Append("\"check\":");
                jsonBuilder.Append("" + GetGroupRightsTypeChecked(list, groupRights, rightsTypeList[i].RightsTypeID) + ",");
                jsonBuilder.Append("\"_parentId\":");//显示树状固定字段
                jsonBuilder.Append(rightsTypeList[i].ParentID + ",");
                jsonBuilder.Append("\"state\":");
                //if (rightsTypeList.Exists(a => a.ParentID == rightsTypeList[i].RightsTypeID))
                //{
                //    jsonBuilder.Append("\"open\"");
                //}
                //else
                //{
                //    jsonBuilder.Append("\"closed\"");
                //}
                jsonBuilder.Append("\"closed\"");
                jsonBuilder.Append("},");
            }
            for (int i = 0; i < list.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("\"" + list[i].RightsTypeID + "_" + list[i].RightsID + "\",");
                jsonBuilder.Append("\"name\":");
                jsonBuilder.Append("\"" + list[i].RightsName + "\",");
                jsonBuilder.Append("\"code\":");
                jsonBuilder.Append("\"" + list[i].RightsCode + "\",");
                jsonBuilder.Append("\"type\":");
                jsonBuilder.Append("1,");
                jsonBuilder.Append("\"check\":");
                jsonBuilder.Append("" + GetGroupRightsChecked(groupRights, list[i].RightsID) + ",");
                jsonBuilder.Append("\"rightsType\":");
                jsonBuilder.Append("" + list[i].RightsType + ",");
                jsonBuilder.Append("\"_parentId\":");//显示树状固定字段
                jsonBuilder.Append(list[i].RightsTypeID + "");
                //jsonBuilder.Append("\"state\":");
                //jsonBuilder.Append("\"closed\"");
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
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
        [NonAction]
        private int GetGroupRightsTypeChecked(List<AdmRightsExt> rights, List<AdmGroupRightsExt> groupRights, int rightsTypeID)
        {
            if (groupRights == null || groupRights.Count == 0) return 0;
            var groupRightsCount = groupRights.Count(a => a.IDPath.Contains("^" + rightsTypeID + "^"));
            if (groupRightsCount == 0) return 0;
            var rightsCount = rights.Count(a => a.IDPath.Contains("^" + rightsTypeID + "^"));

            var joinCount = (from gr in groupRights
                             join r in rights
                             on gr.RightsID equals r.RightsID
                             where gr.IDPath.Contains("^" + rightsTypeID + "^")
                             select gr).Count();

            if (groupRightsCount == joinCount && groupRightsCount == rightsCount) return 1;//如果用户组拥有的权限与当前的权限一样返回1

            return 2;//如果用户组拥有的权限小于当前的权限一样返回 1
        }
        [NonAction]
        private int GetGroupRightsChecked(List<AdmGroupRightsExt> groupRights, int rightsID)
        {
            if (groupRights == null || groupRights.Count == 0) return 0;
            return groupRights.Exists(a => a.RightsID == rightsID) ? 1 : 0;
        }



        [NonAction]
        private int GetUserRightsTypeChecked(List<AdmRightsExt> rights, List<AdmUserRightsExt> userRights, int rightsTypeID)
        {
            if (userRights == null || userRights.Count == 0) return 0;
            var userRightsCount = userRights.Count(a => a.IDPath.Contains("^" + rightsTypeID + "^"));
            if (userRightsCount == 0) return 0;
            var rightsCount = rights.Count(a => a.IDPath.Contains("^" + rightsTypeID + "^"));
            var jsoinCount = (from ur in userRights
                              join r in rights
                              on ur.RightsID equals r.RightsID
                              where ur.IDPath.Contains("^" + rightsTypeID + "^")
                              select ur).Count();
            if (userRightsCount == rightsCount && userRightsCount == jsoinCount) return 1;//如果用户拥有的权限与当前的权限一样返回1
            return 2;
        }
        [NonAction]
        private int GetUserRightsChecked(List<AdmUserRightsExt> userRights, int rightsID)
        {
            if (userRights == null || userRights.Count == 0) return 0;
            return userRights.Exists(a => a.RightsID == rightsID) ? 1 : 0;
        }

        [NonAction]
        private async Task<string> GetUserRights(int userId)
        {
            List<AdmUserRightsExt> userRights = await _userRightsService.GetAdmUserRightsExtListAsync(a => a.UserID == userId);
            Expression<Func<AdmRightsType, bool>> rightsTypePredicate = PredicateBuilder.True<AdmRightsType>();
            IOrderByExpression<AdmRightsType>[] rightsTypeOrderByExpressions = {
                new OrderByExpression<AdmRightsType,int>(a=>a.SortOrder)
            };
            List<AdmRightsType> rightsTypeList = await _rightsTypeService.GetListAsync(rightsTypePredicate, rightsTypeOrderByExpressions);
            if (rightsTypeList == null || rightsTypeList.Count == 0)
            {
                return "{\"rows\":[]}";
            }

            Expression<Func<AdmRightsExt, bool>> predicate = PredicateBuilder.True<AdmRightsExt>();
            IOrderByExpression<AdmRightsExt>[] orderByExpressions ={
                new OrderByExpression<AdmRightsExt,int>(a=>a.SortOrder)
            };
            List<AdmRightsExt> list = await _service.GetAdmRightsExtListAsync(predicate, orderByExpressions);
            if (list == null || list.Count == 0)
            {
                return "{\"rows\":[]}";
            }
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"rows\":");
            jsonBuilder.Append("[");
            for (int i = 0; i < rightsTypeList.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("\"" + rightsTypeList[i].RightsTypeID + "\",");
                jsonBuilder.Append("\"name\":");
                jsonBuilder.Append("\"" + rightsTypeList[i].TypeName + "\",");
                jsonBuilder.Append("\"code\":");
                jsonBuilder.Append("\"\",");
                jsonBuilder.Append("\"type\":");
                jsonBuilder.Append("0,");
                jsonBuilder.Append("\"rightsType\":");
                jsonBuilder.Append("-1,");
                jsonBuilder.Append("\"check\":");
                jsonBuilder.Append("" + GetUserRightsTypeChecked(list, userRights, rightsTypeList[i].RightsTypeID) + ",");
                jsonBuilder.Append("\"_parentId\":");//显示树状固定字段
                jsonBuilder.Append(rightsTypeList[i].ParentID + ",");
                jsonBuilder.Append("\"state\":");
                //if (rightsTypeList.Exists(a => a.ParentID == rightsTypeList[i].RightsTypeID))
                //{
                //    jsonBuilder.Append("\"open\"");
                //}
                //else
                //{
                //    jsonBuilder.Append("\"closed\"");
                //}
                jsonBuilder.Append("\"closed\"");
                jsonBuilder.Append("},");
            }
            for (int i = 0; i < list.Count; i++)
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"id\":");
                jsonBuilder.Append("\"" + list[i].RightsTypeID + "_" + list[i].RightsID + "\",");
                jsonBuilder.Append("\"name\":");
                jsonBuilder.Append("\"" + list[i].RightsName + "\",");
                jsonBuilder.Append("\"code\":");
                jsonBuilder.Append("\"" + list[i].RightsCode + "\",");
                jsonBuilder.Append("\"type\":");
                jsonBuilder.Append("1,");
                jsonBuilder.Append("\"check\":");
                jsonBuilder.Append("" + GetUserRightsChecked(userRights, list[i].RightsID) + ",");
                jsonBuilder.Append("\"rightsType\":");
                jsonBuilder.Append("" + list[i].RightsType + ",");
                jsonBuilder.Append("\"_parentId\":");//显示树状固定字段
                jsonBuilder.Append(list[i].RightsTypeID + "");
                //jsonBuilder.Append("\"state\":");
                //jsonBuilder.Append("\"closed\"");
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
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
        #endregion Methods
    }
}