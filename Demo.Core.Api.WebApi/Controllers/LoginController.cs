using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.Core.Api.Core.Extension;
using Demo.Core.Api.Core.Helper;
using Demo.Core.Api.IServices;
using Demo.Core.Api.Model;
using Demo.Core.Api.WebApi.AuthHelper.OverWrite;
using Demo.Core.Api.WebApi.AuthHelper.Policys;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Core.Api.WebApi.Controllers
{
    /// <summary>
    /// 登陆管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Login")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        readonly ISysUserInfoService _sysUserInfoService;
        readonly IUserRoleService _userRoleService;
        readonly IRoleService _roleService;
        readonly PermissionRequirement _requirement;

        public LoginController(ISysUserInfoService sysUserInfoServices,
            IUserRoleService userRoleServices,
            IRoleService roleServices,
            PermissionRequirement requirement)
        {
            this._sysUserInfoService = sysUserInfoServices;
            this._userRoleService = userRoleServices;
            this._roleService = roleServices;
            _requirement = requirement;
        }

        #region 01,获取令牌+async Task<object> GetJwtStr(string name, string pass)
        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pass">密码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Token")]
        public async Task<object> GetJwtStr(string name, string pass)
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
            //var result = new
            //{
            //    data = new { success = suc, token = jwtStr }
            //};

            return new HttpResult(jwtStr);
        }
        #endregion

        #region 02，获取JWT(给Nuxt提供)的方法+object GetJwtStrForNuxt(string name, string pass)
        /// <summary>
        /// 获取JWT的方法2：给Nuxt提供
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTokenNuxt")]
        public object GetJwtStrForNuxt(string name, string pass)
        {
            string jwtStr = string.Empty;
            bool suc = false;
            //这里就是用户登陆以后，通过数据库去调取数据，分配权限的操作
            //这里直接写死了
            if (name == "admins" && pass == "123456")
            {
                TokenModelJwt tokenModel = new TokenModelJwt
                {
                    Uid = 1,
                    Role = "Admin"
                };

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

            return Ok(new
            {
                success = suc,
                data = new { success = suc, token = jwtStr }
            });
        } 
        #endregion

        #region 03，获取JWT的方法3
        /// <summary>
        /// 获取JWT的方法3：整个系统主要方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("JWTToken3.0")]
        public async Task<object> GetJwtToken3(string name = "", string pass = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                return new JsonResult(new
                {
                    Status = false,
                    message = "用户名或密码不能为空"
                });
            }

            pass = MD5Helper.MD5Encrypt32(pass);

            var user = await _sysUserInfoService.Query(d => d.LoginName == name && d.LoginPwd == pass);
            if (user.Count > 0)
            {
                var userRoles = await _sysUserInfoService.GetUserRoleNameStr(name, pass);
                //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(JwtRegisteredClaimNames.Jti, user.FirstOrDefault().Id.ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                claims.AddRange(userRoles.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

                //用户标识
                var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                identity.AddClaims(claims);

                var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
                return new HttpResult(token);
            }
            else
            {
                return new HttpResult(0, "认证失败");
            }
        }
        #endregion

        #region 04，请求刷新Token（以旧换新）+async Task<object> RefreshToken(string token = "")
        /// <summary>
        /// 请求刷新Token（以旧换新）
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefreshToken")]
        public async Task<object> RefreshToken(string token = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                return new HttpResult(0, "token无效，请重新登录！");
            }
            var tokenModel = JwtHelper.SerializeJwt(token);
            if (tokenModel != null && tokenModel.Uid > 0)
            {
                var user = await _sysUserInfoService.QueryById(tokenModel.Uid);
                if (user != null)
                {
                    var userRoles = await _sysUserInfoService.GetUserRoleNameStr(user.LoginName, user.LoginPwd);
                    //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.LoginName),
                    new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ObjToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                    claims.AddRange(userRoles.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

                    //用户标识
                    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                    identity.AddClaims(claims);

                    var refreshToken = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
                    return new HttpResult(refreshToken);
                }
            }

            return new JsonResult(0, "认证失败");
        }
        #endregion

        #region 05，获取JWT的方法4：给 JSONP 测试+void Getjsonp(string callBack, long id = 1, string sub = "Admin", int expiresSliding = 30, int expiresAbsoulute = 30)
        /// <summary>
        /// 获取JWT的方法4：给 JSONP 测试
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="id"></param>
        /// <param name="sub"></param>
        /// <param name="expiresSliding"></param>
        /// <param name="expiresAbsoulute"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("jsonp")]
        public void Getjsonp(string callBack, long id = 1, string sub = "Admin", int expiresSliding = 30, int expiresAbsoulute = 30)
        {
            TokenModelJwt tokenModel = new TokenModelJwt
            {
                Uid = id,
                Role = sub
            };

            string jwtStr = JwtHelper.IssueJwt(tokenModel);

            string response = string.Format("\"value\":\"{0}\"", jwtStr);
            string call = callBack + "({" + response + "})";
            Response.WriteAsync(call);
        }
        #endregion

        #region 06，测试 MD5 加密字符串+string Md5Password(string password = "")
        /// <summary>
        /// 测试 MD5 加密字符串
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Md5Password")]
        public string Md5Password(string password = "")
        {
            return MD5Helper.MD5Encrypt32(password);
        } 
        #endregion
    }
}