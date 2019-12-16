using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.IRepository.UnitOfWork
{
    public interface IUnitOfWork
    {
        ISqlSugarClient GetDbClient();

        void BeginTran();

        void CommitTran();
        void RollbackTran();
    }
}
