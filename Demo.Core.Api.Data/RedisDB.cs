using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace Demo.Core.Api.Data
{
    public class RedisDB : IRedisDB
    {
        private IDatabase RedisClient;

        public RedisDB(IDatabase redis)
        { 
            this.RedisClient=redis;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void InsertItemOnList(string key, string value)
        {
            this.RedisClient.StringSet(key,value);
        }
    }
}
