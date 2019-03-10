using System;
using StackExchange.Redis;

namespace Demo.Core.Api.Data
{
    public class RedisFactory
    {
        public static IDatabase  GetRedisClient (string sectionName)
        {
            string name = "redis_" + sectionName;
            IDatabase redisBD = CallContext.GetData(name) as IDatabase;
            if (redisBD == null)
            {
                redisBD = RedisPoolManager.GetRedisDataBase();
                CallContext.SetData(name, redisBD);
            }
            return redisBD;
        }
    }
}
