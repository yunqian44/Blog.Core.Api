using Demo.Core.Api.Model.Entity;
using System;

namespace Demo.Core.Api.IRepository
{
    public interface IAdvertisementRepository:
        IBaseRepository<Advertisement>
    {
        int Sum(int i, int j);
    }
}
