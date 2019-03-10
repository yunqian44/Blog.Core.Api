using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace Demo.Core.Api.Data
{
    public interface IRedisDB:IDisposable
    {
       void InsertItemOnList(string key,string value);
    }
}
