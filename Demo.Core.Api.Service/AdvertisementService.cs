using Demo.Core.Api.IRepository;
using Demo.Core.Api.IService;
using Demo.Core.Api.Model.Entity;
using Demo.Core.Api.Repository;
using System;

namespace Demo.Core.Api.Service
{
    /// <summary>
    /// 广告接口实现
    /// </summary>
    public class AdvertisementService :BaseServices<Advertisement>, IAdvertisementService
    {
        IAdvertisementRepository dal;

        public AdvertisementService(IAdvertisementRepository dal)
        {
            this.dal = dal;
            base.BaseDal = dal;
        }

        public int Sum(int i, int j)
        {
           return dal.Sum(i, j);
        }
    }
}
