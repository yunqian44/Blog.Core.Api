using Demo.Core.Api.IRepository;
using Demo.Core.Api.IServices;
using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Services
{
    public partial class PasswordLibService : BaseService<PasswordLib>, IPasswordLibService
    {
        IPasswordLibRepository _dal;
        public PasswordLibService(IPasswordLibRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }

    }
}
