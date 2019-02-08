using System;

namespace Demo.Core.Api.Core
{
    public static class IdGenerator
    {
        static object asyncObj = new object();
        //static volatile IdWorker _IdWorker = null;
        static uint _ServerId = 1;
        static uint _DataBaseId = 1;

        // private static IdWorker IdWorker
        // {
        //     get
        //     {
        //         if (_IdWorker == null)
        //         {
        //             lock (asyncObj)
        //             {
        //                 if (_IdWorker == null)
        //                 {
        //                     _IdWorker = new IdWorker(ServerId, DataBaseId);
        //                 }
        //             }
        //         }
        //         return _IdWorker;
        //     }
        // }

        // /// <summary>
        // /// 获取分布式全局ID。采用雪花算法
        // /// </summary>
        // /// <returns></returns>
        // public static long GetGlobalId()
        // {
        //     return IdWorker.NextId();
        // }

        /// <summary>
        /// Guid
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>s
        public static long GetTimeStamp()
        {
            return DateTime.Now.TimeStamp();
        }

    }
}