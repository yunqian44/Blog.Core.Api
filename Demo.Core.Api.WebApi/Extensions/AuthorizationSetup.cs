using Demo.Core.Api.Core.GlobalVar;
using Demo.Core.Api.Core.Helper;
using Demo.Core.Api.WebApi.AuthHelper.Policys;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.WebApi.Extensions
{
    /// <summary>
    /// Db 启动服务
    /// </summary>
    public static class AuthorizationSetup
    {
        public static void AddAuthorizationSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            #region Authorize 权限认证

            // 1【授权】、这个很简单，其他什么都不用做， 只需要在API层的controller上边，增加特性即可，注意，只能是角色的:
            // [Authorize(Roles = "Admin,System")]

            #region 基于策略的简单授权
            services.AddAuthorization(option =>
            {
                option.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
            });
            #endregion

            #region 复杂策略授权

            #region 参数
            //读取配置文件
            var symmetricKeyAsBase64 = AppSecretConfig.Audience_Secret_String;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = Appsettings.app(new string[] { "Audience", "Issuer" });
            var Audience = Appsettings.app(new string[] { "Audience", "Audience" });

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);


            // 如果要数据库动态绑定，这里先留个空，后边处理器里动态赋值
            var permission = new List<PermissionItem>();

            // 角色与接口的权限要求参数
            var permissionRequirement = new PermissionRequirement(
                "/api/denied",// 拒绝授权的跳转地址（目前无用）
                permission,
                ClaimTypes.Role,//基于角色的授权
                Issuer,//发行人
                Audience,//听众
                signingCredentials,//签名凭据
                expiration: TimeSpan.FromSeconds(60 * 60)//接口的过期时间
                );

            //【授权】
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Permissions.Name,
                         policy => policy.Requirements.Add(permissionRequirement));
            });
            #endregion

            #endregion

            #region 认证
            //令牌参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = Issuer,//发行人
                ValidateAudience = true,
                ValidAudience = Audience,//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };

            services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = nameof(ApiResponseHandler);
                x.DefaultForbidScheme = nameof(ApiResponseHandler);
            })
                 .AddJwtBearer(o =>
                 {
                     o.TokenValidationParameters = tokenValidationParameters;
                     o.Events = new JwtBearerEvents
                     {
                         OnAuthenticationFailed = context =>
                         {
                             // 如果过期，则把<是否过期>添加到，返回头信息中
                             if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                             {
                                 context.Response.Headers.Add("Token-Expired", "true");
                             }
                             return Task.CompletedTask;
                         }
                     };
                 })
                 .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { }); ;
            #endregion

            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);
            #endregion
        }
    }
}
