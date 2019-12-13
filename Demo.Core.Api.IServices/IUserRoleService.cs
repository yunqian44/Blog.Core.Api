using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.IServices
{
    public interface IUserRoleService : IBaseService<UserRole>
    {
        Task<UserRole> SaveUserRole(int uId, int rId);

        Task<int> GetRoleIdByUId(int uId);
    }
}
