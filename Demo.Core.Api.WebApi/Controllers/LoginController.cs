using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Core.Api.WebApi.AuthHelper.OverWrite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Core.Api.WebApi.Controllers
{
    /// <summary>
    /// 登陆管理
    /// </summary>
    [Route("api/Login")]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        #region 01,获取令牌
        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pass">密码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Token")]
        public JsonResult GetJwtStr(string name, string pass)
        {
            string jwtStr = string.Empty;
            bool suc = false;
            //这里就是用户登录以后，通过数据库去调取数据，分配权限的操作
            //这里直接写死了
            if (name == "admin" && pass == "123456")
            {
                TokenModelJwt tokenModel = new TokenModelJwt();
                tokenModel.Uid = 1;
                tokenModel.Role = "Admin";

                jwtStr = JwtHelper.IssueJwt(tokenModel);
                suc = true;
            }
            else
            {
                jwtStr = "login fail!!!";
            }
            var result = new
            {
                data = new { success = suc, token = jwtStr }
            };

            return Json(result);
        }
        #endregion

    }
}