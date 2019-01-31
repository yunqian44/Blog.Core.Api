using System;
using StackExchange.Redis;

namespace Demo.Core.Api.Data
{
    internal class RedisPoolManager
    {
        //private static string _conncetion;
        public RedisPoolManager()
        {

        }
        public static IDatabase GetRedisDataBase()
        {
            var redisSection = RedisConfigInfo.GetConfig();
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisSection.Value);
            return redis.GetDatabase();
        }
    }
}