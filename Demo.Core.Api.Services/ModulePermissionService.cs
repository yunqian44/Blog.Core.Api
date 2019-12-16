using Demo.Core.Api.IRepository;
using Demo.Core.Api.IServices;
using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Services
{
    /// <summary>
    /// ModulePermissionServices
    /// </summary>	
    public class ModulePermissionService : BaseService<ModulePermission>, IModulePermissionService
    {

        IModulePermissionRepository _dal;
        public ModulePermissionService(IModulePermissionRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }

    }
}
