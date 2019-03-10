using System;
using StackExchange.Redis;

namespace Demo.Core.Api.Data
{
    public class RedisManager
    {
        public IDatabase GetRedisDB(string sectionName)
        {
             return RedisFactory.GetRedisClient(sectionName);
        }
    }
}
