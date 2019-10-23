using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Data.MemoryCache
{
    /// <summary>
    /// 简单的缓存接口，只有查询和添加，以后会进行扩展
    /// </summary>
    public interface ICaching
    {
        object Get(string cacheKey);

        void Set(string cacheKey, object cacheValue);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <param name="cacheValue">值</param>
        /// <param name="overTime">过期时间（毫秒）/param>
        void Set(string cacheKey, object cacheValue, TimeSpan cacheTime);
    }
}
