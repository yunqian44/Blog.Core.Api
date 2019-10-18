using Demo.Core.Api.IRepository;
using Demo.Core.Api.IService;
using Demo.Core.Api.Repository;
using System;

namespace Demo.Core.Api.Service
{
    /// <summary>
    /// 广告接口实现
    /// </summary>
    public class AdvertisementService : IAdvertisementService
    {
        IAdvertisementRepository dal = new AdvertisementRepository();

        public int Sum(int i, int j)
        {
           return dal.Sum(i, j);
        }
    }
}
