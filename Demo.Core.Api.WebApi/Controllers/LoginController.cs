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
        public async Task<object> GetJwtStr(string name, string pass)
        {
            string jwtStr = string.Empty;
            bool suc = false;

            // 获取用户的角色名，请暂时忽略其内部是如何获取的，可以直接用 var userRole="Admin"; 来代替更好理解。
            var userRole = "Admin";
            //var userRole = await _sysUserInfoServices.GetUserRoleNameStr(name, pass);
            if (userRole != null)
            {
                // 将用户id和角色名，作为单独的自定义变量封装进 token 字符串中。
                TokenModelJwt tokenModel = new TokenModelJwt { Uid = 1, Role = userRole };
                jwtStr = JwtHelper.IssueJwt(tokenModel);//登录，获取到一定规则的 Token 令牌
                suc = true;
            }
            else
            {
                jwtStr = "登陆失败";
            }

            return Ok(new
            {
                success = suc,
                token = jwtStr
            });
        }
        #endregion

    }
}