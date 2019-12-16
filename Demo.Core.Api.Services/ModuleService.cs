using Demo.Core.Api.IRepository;
using Demo.Core.Api.IServices;
using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Services
{
    /// <summary>
    /// ModuleServices
    /// </summary>	
    public class ModuleService : BaseService<Module>, IModuleService
    {

        IModuleRepository _dal;
        public ModuleService(IModuleRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }

    }
}
