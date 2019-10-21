using Demo.Core.Api.Model.Entity;
using System;

namespace Demo.Core.Api.IService
{
    /// <summary>
    /// 广告服务接口
    /// </summary>
    public interface IAdvertisementService:IBaseServices<Advertisement>
    {
        int Sum(int i, int j);
    }
}
