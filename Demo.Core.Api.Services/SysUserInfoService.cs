using Demo.Core.Api.Core.Extension;
using Demo.Core.Api.IRepository;
using Demo.Core.Api.IServices;
using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.Services
{
    public class SysUserInfoService : BaseService<UserModel>, ISysUserInfoService
    {
        ISysUserInfoRepository _dal;
        IUserRoleService _userRoleService;
        IRoleRepository _roleRepository;
        public SysUserInfoService(ISysUserInfoRepository dal, IUserRoleService userRoleServices, IRoleRepository roleRepository)
        {
            this._dal = dal;
            this._userRoleService = userRoleServices;
            this._roleRepository = roleRepository;
            base.BaseDal = dal;
        }
        
        #region 01，保存用户信息+async Task<UserModel> SaveUserInfo(string loginName, string loginPwd)
        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="loginName">登陆名</param>
        /// <param name="loginPwd">密码</param>
        /// <returns></returns>
        public async Task<UserModel> SaveUserInfo(string loginName, string loginPwd)
        {
            UserModel sysUserInfo = new UserModel(loginName, loginPwd);
            UserModel model = new UserModel();
            var userList = await base.Query(a => a.LoginName == sysUserInfo.LoginName && a.LoginPwd == sysUserInfo.LoginPwd);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(sysUserInfo);
                model = await base.QueryById(id);
            }

            return model;

        }
        #endregion

        #region 02，获取角色名称字符串根据登录名和密码+async Task<string> GetUserRoleNameStr(string loginName, string loginPwd)
        /// <summary>
        /// 获取角色名称字符串根据登录名和密码
        /// </summary>
        /// <param name="loginName">登陆名</param>
        /// <param name="loginPwd">密码</param>
        /// <returns></returns>
        public async Task<string> GetUserRoleNameStr(string loginName, string loginPwd)
        {
            string roleName = "";
            var user = (await base.Query(a => a.LoginName == loginName && a.LoginPwd == loginPwd)).FirstOrDefault();
            if (user != null)
            {
                var userRoles = await _userRoleService.Query(ur => ur.UserId == user.Id);
                if (userRoles.Count > 0)
                {
                    var roles = await _roleRepository.QueryByIds(userRoles.Select(ur => ur.RoleId.ObjToString()).ToArray());

                    roleName = string.Join(',', roles.Select(r => r.Name).ToArray());
                }
            }
            return roleName;
        } 
        #endregion
    }
}
