using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;

namespace Noob.Web.Admin.Models.Account
{
    [Serializable]
    [Validator(typeof(LoginModelValidator))]
    public class LoginModel : BaseNopModel
    {
        /// <summary>
        /// 账户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}