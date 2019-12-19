using Demo.Core.Api.Core.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Core.Api.WebApi.Extensions
{
    /// <summary>
    /// Authorize 权限认证
    /// </summary>
    public static class CorsSetup
    {
        public static void AddCorsSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCors(c =>
            {
                c.AddPolicy("LimitRequests", policy =>
                {
                    // 支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
                    // 注意，http://127.0.0.1:8700 和 http://localhost:8800 是不一样的，尽量写两个
                    policy
                    .WithOrigins(Appsettings.app(new string[] { "Startup", "Cors", "IPs" }).Split(','))
                    .AllowAnyHeader()//Ensures that the policy allows any header.
                    .AllowAnyMethod();
                });
            });
        }
    }
}
