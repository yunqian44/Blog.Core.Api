using Demo.Core.Api.Core.CachingAttribute;
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
    /// <summary>
    /// RoleServices
    /// </summary>	
    public class RoleServices : BaseService<Role>, IRoleService
    {

        IRoleRepository _dal;
        public RoleServices(IRoleRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<Role> SaveRole(string roleName)
        {
            Role role = new Role(roleName);
            Role model = new Role();
            var userList = await base.Query(a => a.Name == role.Name && a.Enabled);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(role);
                model = await base.QueryById(id);
            }

            return model;

        }

        [Caching(AbsoluteExpiration = 30)]
        public async Task<string> GetRoleNameByRId(int rid)
        {
            return ((await base.QueryById(rid))?.Name);
        }
    }
}
