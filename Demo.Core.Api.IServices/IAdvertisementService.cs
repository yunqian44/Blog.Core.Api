using Demo.Core.Api.Model.Entity;
using System;

namespace Demo.Core.Api.IServices
{
    /// <summary>
    /// 广告服务接口
    /// </summary>
    public interface IAdvertisementService:IBaseService<Advertisement>
    {
        int Sum(int i, int j);
    }
}
