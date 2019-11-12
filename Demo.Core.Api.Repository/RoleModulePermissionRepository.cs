using Demo.Core.Api.IRepository;
using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.Repository
{
    public class RoleModulePermissionRepository : BaseRepository<RoleModulePermission>, IRoleModulePermissionRepository
    {

        public async Task<List<RoleModulePermission>> WithChildrenModel()
        {
            var list = await Task.Run(() => Db.Queryable<RoleModulePermission>()
                    .Mapper(it => it.Role, it => it.RoleId)
                    .Mapper(it => it.Permission, it => it.PermissionId)
                    .Mapper(it => it.Module, it => it.ModuleId).ToList());

            return null;
        }

        //public async Task<List<TestMuchTableResult>> QueryMuchTable()
        //{
        //    return await QueryMuch<RoleModulePermission, Module, Permission, TestMuchTableResult>(
        //        (rmp, m, p) => new object[] {
        //            JoinType.Left, rmp.ModuleId == m.Id,
        //            JoinType.Left,  rmp.PermissionId == p.Id
        //        },

        //        (rmp, m, p) => new TestMuchTableResult()
        //        {
        //            moduleName = m.Name,
        //            permName = p.Name,
        //            rid = rmp.RoleId,
        //            mid = rmp.ModuleId,
        //            pid = rmp.PermissionId
        //        },

        //        (rmp, m, p) => rmp.IsDeleted == false
        //        );
        //}

    }
}
