using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Orchard.Logging;
using Orchard.Caching;
using Noob.Domain.Entities;
using Noob.IServices;
using Noob.Web.Admin.Models;
using Noob.Web.Admin.Security;
using Orchard.Data;
using Orchard.Web.Models;
using Orchard.Web.Mvc.Infrastructure;
using Autofac;
using FluentValidation.Results;

namespace Noob.Web.Admin.EasyUI.Controllers.Admin
{
    /// <summary>
    /// 地区 控制器
    /// </summary>
    [AdminAuthorize(Permission = "AdmArea")]
    public class AreaController : BaseAdminController
    {
        #region Members

        private readonly IAdmAreaService _service;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public AreaController(IAdmAreaService service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
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
        public async Task<string> GetList(string id)
        {
            //return "{\"rows\":[]";
            var predicate = PredicateBuilder.True<AdmArea>();
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                predicate = predicate.And(a => a.ParentId == id);
            }
            else
            {
                predicate = predicate.And(a => a.ParentId == "100000");
            }
            IOrderByExpression<AdmArea>[] orderByExpressions = {
                new OrderByExpression<AdmArea,string>(a=>a.AreaID)
            };
            List<AdmArea> list = await _service.GetListAsync(predicate, orderByExpressions);
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
                    jsonBuilder.Append("\"" + list[i].AreaID + "\"");
                    jsonBuilder.Append(",\"name\":");
                    jsonBuilder.Append("\"" + list[i].AreaName + "\"");
                    jsonBuilder.Append(",\"ShortName\":");
                    jsonBuilder.Append("\"" + list[i].ShortName + "\"");
                    jsonBuilder.Append(",\"PinYin\":");
                    jsonBuilder.Append("\"" + list[i].PinYin + "\"");
                    jsonBuilder.Append(",\"ShortPinYin\":");
                    jsonBuilder.Append("\"" + list[i].ShortPinYin + "\"");
                    jsonBuilder.Append(",\"PYFirstLetter\":");
                    jsonBuilder.Append("\"" + list[i].PYFirstLetter + "\"");
                    jsonBuilder.Append(",\"LevelType\":" + list[i].LevelType);
                    jsonBuilder.Append(",\"Status\":" + list[i].Status);
                    if (list[i].ParentId != "100000")
                    {
                        jsonBuilder.Append(",\"_parentId\":");//显示树状固定字段
                        jsonBuilder.Append("\"" + list[i].ParentId + "\"");
                        jsonBuilder.Append(",\"state\":");
                        string areaId = list[i].AreaID;
                        bool existChild = await _service.ExistsAsync(a => a.ParentId == areaId);
                        if (existChild)
                        {
                            jsonBuilder.Append("\"closed\"");
                        }
                        else
                        {
                            jsonBuilder.Append("\"open\"");
                        }
                    }
                    else
                    {
                        jsonBuilder.Append(",\"state\":\"closed\"");
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
        public async Task<JsonResult> Create(AdmAreaModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (ModelState.IsValid)
            {
                var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AdmAreaModel, AdmArea>());
                //Mapper.AssertConfigurationIsValid();
                var entity = mapConfig.CreateMapper().Map<AdmAreaModel, AdmArea>(model);
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = LoginUserID;
                entity.UpdateTime = new DateTime(1900, 01, 01);
                entity.UpdateUser = 0;
                if (!string.IsNullOrEmpty(model.ParentId) && model.ParentId != "0")
                {
                    AdmArea tmpModel = await _service.SingleAsync(a => a.AreaID == model.ParentId);
                    if (model == null)
                    {
                        data.Msg = "无效id";
                        result.Data = data;
                        return result;
                    }
                    entity.AreaIDPath = tmpModel.AreaIDPath + entity.AreaID + ",";
                    entity.AreaNamePath = tmpModel.AreaNamePath + entity.AreaName + ",";
                    entity.LevelType = (byte)(tmpModel.LevelType + 1);
                }
                else
                {
                    entity.LevelType = 1;
                    entity.AreaIDPath = "," + model.AreaID + ",";
                    entity.AreaNamePath = "," + model.AreaName + ",";
                }
                entity = await _service.InsertAsync(entity);
                if (entity.Id > 0)
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

        public async Task<ActionResult> Edit(string id)
        {
            var entity = await _service.SingleAsync(a => a.AreaID == id);
            CheckData(entity);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<AdmArea, AdmAreaModel>());
            var model = config.CreateMapper().Map<AdmArea, AdmAreaModel>(entity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdmAreaModel model)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (model == null)
            {
                data.Msg = "数据不能为空";
                result.Data = data;
                return result;
            }
            string areaIDPath = string.Empty;
            string areaNamePath = string.Empty;
            //Validators.AdmAreaModelValidator validator = new Validators.AdmAreaModelValidator(ContainerContext.Current.Container.Resolve<IAdmAreaService>());
            //ValidationResult validationResult =await validator.ValidateAsync(model);

            //if (!validationResult.IsValid)
            //{
            //    validationResult.Errors.ToList().ForEach(error =>
            //    {
            //        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            //    });
            //    data.Msg = GetErrorMsgFromModelState();
            //    result.Data = data;
            //    return result;
            //}
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.ParentId) && model.ParentId != "0")
                {
                    AdmArea tmpModel = await _service.SingleAsync(a => a.AreaID == model.ParentId);
                    if (tmpModel == null)
                    {
                        data.Msg = "无效id";
                        result.Data = data;
                        return result;
                    }
                    areaIDPath = tmpModel.AreaIDPath + model.AreaID + ",";
                    areaNamePath = tmpModel.AreaNamePath + model.AreaName + ",";
                    model.LevelType = (byte)(tmpModel.LevelType + 1);
                }
                else
                {
                    areaIDPath = "," + model.AreaID + ",";
                    areaNamePath = "," + model.AreaName + ",";
                }
                Expression<Func<AdmArea, AdmArea>> updateExpression = x => new AdmArea
                {
                    UpdateUser = LoginUserID,
                    UpdateTime = DateTime.Now,
                    AreaID = model.AreaID,
                    AreaName = model.AreaName,
                    ParentId = model.ParentId,
                    ShortName = model.ShortName,
                    LevelType = model.LevelType,
                    CityCode = model.CityCode,
                    ZipCode = model.ZipCode,
                    Lng = model.Lng,
                    Lat = model.Lat,
                    PinYin = model.PinYin,
                    ShortPinYin = model.ShortPinYin,
                    PYFirstLetter = model.PYFirstLetter,
                    SortOrder = model.SortOrder,
                    Status = model.Status,
                };
                Expression<Func<AdmArea, bool>> predicate = a => a.AreaID == model.AreaID;
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
            }
            else
            {
                data.Msg = GetErrorMsgFromModelState();
            }
            result.Data = data;
            return result;

        }

        public async Task<ActionResult> Details(string id)
        {
            var entity = await _service.SingleAsync(a => a.AreaID == id);
            CheckData(entity);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<AdmArea, AdmAreaModel>());
            var model = config.CreateMapper().Map<AdmArea, AdmAreaModel>(entity);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (string.IsNullOrEmpty(id))
            {
                data.Msg = "无效ID";
            }
            else
            {
                if (await _service.CountAsync(a => a.ParentId == id) > 0)
                {
                    data.Msg = "请先删除子地区";
                }
                else
                {
                    var expression = PredicateBuilder.True<AdmArea>();
                    expression = expression.And(a => ExpressionExtensions.In(a.AreaID, id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
                    int deleteResult = await _service.DeleteAsync(expression);
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
            }
            result.Data = data;
            return result;
        }

        /// <summary>
        /// 更新启用状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdateStatus(string id, byte status)
        {
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult();
            if (string.IsNullOrEmpty(id) || (status != 1 && status != 0))
            {
                data.Msg = "无效ID";
            }
            else
            {
                //var expression = PredicateBuilder.True<AdmArea>();
                //expression = expression.And(a => ExpressionExtensions.In(a.AreaID, id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
                var areaIdList= id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                Expression <Func<AdmArea, bool>> predicate=x=> areaIdList.Contains(x.AreaID);

                Expression<Func<AdmArea, AdmArea>> updateExpression = x => new AdmArea
                {
                    Status = status,
                };
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
                await Task.FromResult(0);
            }
            result.Data = data;
            return result;
        }

    }

}
