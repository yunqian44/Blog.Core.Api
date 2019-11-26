using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.IServices
{
    public interface ISysUserInfoService : IBaseService<UserModel>
    {
        Task<UserModel> SaveUserInfo(string loginName, string loginPwd);

        Task<string> GetUserRoleNameStr(string loginName, string loginPwd);
    }
}
