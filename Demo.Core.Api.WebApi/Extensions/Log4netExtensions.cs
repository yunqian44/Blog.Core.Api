using Demo.Core.Api.Core.Extension;
using Demo.Core.Api.Core.Helper;
using Demo.Core.Api.Data.LogHelper.L4n;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Core.Api.WebApi.Extensions
{
    /// <summary>
    /// lo4net日志 启动服务
    /// </summary>
    public static class Log4NetExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string log4NetConfigFile)
        {
            factory.AddProvider(new Log4NetProvider(log4NetConfigFile));
            return factory;
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory)
        {
            if (Appsettings.app("Middleware", "RecordAllLogs", "Enabled").ObjToBool())
            {
                factory.AddProvider(new Log4NetProvider("Log4net.config"));
            }
            return factory;
        }
    }
}
