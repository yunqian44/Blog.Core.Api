using System;

namespace Demo.Core.Api.Core
{
    public static class TimeDateExtension
    {
        /// <summary>
        /// 转化时间戳
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>时间戳</returns>
        public static long TimeStamp(this DateTime time)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);   
        }
    }
}