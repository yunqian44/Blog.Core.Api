using Demo.Core.Api.IService;
using Demo.Core.Api.Model.Entity;
using System;

namespace Demo.Core.Api.IRepository
{
    public interface IAdvertisementRepository:
        IBaseServices<Advertisement>
    {
        int Sum(int i, int j);
    }
}
