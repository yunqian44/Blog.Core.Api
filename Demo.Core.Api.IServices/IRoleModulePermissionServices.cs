using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.IServices
{
    public interface IRoleModulePermissionServices : IBaseServices<RoleModulePermission>
    {

        Task<List<RoleModulePermission>> GetRoleModule();

        Task<List<RoleModulePermission>> TestModelWithChildren();

        //Task<List<TestMuchTableResult>> QueryMuchTable();
    }
}
