using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Demo.Core.Api.Core.Helper;
using Demo.Core.Api.Data;
using Demo.Core.Api.Data.Hubs;
using Demo.Core.Api.Data.LogHelper;
using Demo.Core.Api.Data.MemoryCache;
using Demo.Core.Api.IServices;
using Demo.Core.Api.Model.Seed;
using Demo.Core.Api.WebApi.AOP;
using Demo.Core.Api.WebApi.App_Start;
using Demo.Core.Api.WebApi.AuthHelper.OverWrite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Demo.Core.Api.Core.Extension;
using AutoMapper;
using log4net;
using log4net.Config;
using log4net.Repository;
using static Demo.Core.Api.WebApi.SwaggerHelper.CustomApiVersion;
using Newtonsoft.Json.Serialization;
using Demo.Core.Api.WebApi.AuthHelper.Policys;
using System.Security.Claims;
using Demo.Core.Api.Core.GlobalVar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Demo.Core.Api.WebApi.Middlewares;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Http;

namespace Demo.Core.Api.WebApi
{
    public class Startup
    {
        /// <summary>
        /// log4net 仓储库
        /// </summary>
        public static ILoggerRepository Repository { get; set; }
        private static readonly ILog log = LogManager.GetLogger(typeof(ApiErrorHandleFilter));
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
            #region 迁移到program.cs 文件中了
            //log4net
            //Repository = LogManager.CreateRepository(Configuration["Logging:Log4Net:Name"]);
            ////指定配置文件，如果这里你遇到问题，应该是使用了InProcess模式，请查看Blog.Core.csproj,并删之
            //var contentPath = env.ContentRootPath;
            //var log4Config = Path.Combine(contentPath, "log4net.config");
            //XmlConfigurator.Configure(Repository, new FileInfo(log4Config)); 
            #endregion
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; }

        private const string ApiName = "Blog.Core";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region MVC + GlobalExceptions

            //注入全局异常捕获
            services.AddControllers(o =>
            {
                // 全局异常过滤
                o.Filters.Add(typeof(ApiErrorHandleFilter));

                // 全局路由权限公约
                //o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
                // 全局路由前缀，统一修改路由
                //o.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(RoutePrefix.Name)));
            })
            //全局配置Json序列化处理
            .AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            #endregion

            // Redis注入
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();

            // 缓存注入
            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            services.AddSingleton(new Appsettings(Env.ContentRootPath));
            //log日志注入
            services.AddSingleton(new LogLock(Env.ContentRootPath));
            services.AddSingleton(new GetTableData(Env.ContentRootPath));

            #region 初始化DB
            services.AddScoped<DBSeed>();
            services.AddScoped<MyContext>();

            #endregion

            #region Automapper
            services.AddAutoMapper(typeof(Startup));//这是AutoMapper的2.0新特性
            #endregion

            #region MiniProfiler

            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";
                //(options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(10);
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.Left;
                options.PopupShowTimeWithChildren = true;

                // 可以增加权限
                //options.ResultsAuthorize = request => request.HttpContext.User.IsInRole("Admin");
                //options.UserIdProvider = request => request.HttpContext.User.Identity.Name;
            }
            );

            #endregion

            #region CORS
            //跨域第二种方法，声明策略，记得下边app中配置
            services.AddCors(c =>
            {
                //一般采用这种方法
                c.AddPolicy("LimitRequests", policy =>
                {
                    policy
                    .WithOrigins("http://127.0.0.1:1818", "http://localhost:8080", "http://localhost:8021", "http://localhost:8081", "http://localhost:8400", "http://localhost:8500")//支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
                    .AllowAnyHeader()//Ensures that the policy allows any header.
                    .AllowAnyMethod();
                });
            });

            //跨域第一种办法，注意下边 Configure 中进行配置 
            //services.AddCors();
            #endregion

            #region Swagger

            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

            services.AddSwaggerGen(c =>
               {
                   //遍历出全部的版本，做文档信息展示
                   typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                   {
                       c.SwaggerDoc(version, new OpenApiInfo
                       {
                           Title = $"{ApiName}接口文档——Netcore 3.0",
                           Version = version,
                           Description = $"{ApiName}框架说明文档" + version,
                           Contact = new OpenApiContact()
                           {
                               Name = "刺客",
                               Email = "yunqian8@live.com",
                               Url = new Uri("https://www.xxx.com")
                           },
                           License = new OpenApiLicense 
                           { Name = ApiName, 
                               Url = new Uri("https://www.xxx.com") }
                       });
                       // 按相对路径排序
                       c.OrderActionsBy(o => o.RelativePath);
                   });

                   try
                   {
                       //就是这里
                       var xmlPath = Path.Combine(basePath, "Demo.Core.Api.WebApi.xml");//这个就是刚刚配置的xml文件名
                       c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                       var xmlModelPath = Path.Combine(basePath, "Demo.Core.Api.Model.xml");//这个就是Model层的xml文件名
                       c.IncludeXmlComments(xmlModelPath);
                   }
                   catch (Exception ex)
                   {
                       log.Error("Demo.Core.Api.WebApi.xml和Demo.Core.Api.Model.xml 丢失，请检查并拷贝。\n" + ex.Message);
                   }

                   c.OperationFilter<AddResponseHeadersFilter>();
                   c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                   c.OperationFilter<SecurityRequirementsOperationFilter>();


                   #region Token绑定到ConfigureServices
                   //添加header验证信息
                   c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                   {
                       Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                       Name = "Authorization",//jwt默认的参数名称
                       In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                       Type = SecuritySchemeType.ApiKey
                   });
                   #endregion
               });
            #endregion

            #region SignalR 通讯
            services.AddSignalR();
            #endregion

            #region Httpcontext

            // Httpcontext 注入
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #endregion

            #region Authorize 权限认证

            #region 基于策略的简单授权
            services.AddAuthorization(option =>
            {
                option.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
            });
            #endregion

            #region 复杂策略授权

            #region 参数
            //读取配置文件
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);


            // 如果要数据库动态绑定，这里先留个空，后边处理器里动态赋值
            var permission = new List<PermissionItem>();

            // 角色与接口的权限要求参数
            var permissionRequirement = new PermissionRequirement(
                "/api/denied",// 拒绝授权的跳转地址（目前无用）
                permission,
                ClaimTypes.Role,//基于角色的授权
                audienceConfig["Issuer"],//发行人
                audienceConfig["Audience"],//听众
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
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };

            services.AddAuthentication(x =>
                {
                    //看这个单词熟悉么？没错，就是上边错误里的那个。
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
                 });
            #endregion

            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);
            #endregion
            //return new AutofacServiceProvider(ApplicationContainer);//第三方IOC接管 core内置DI容器

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            #region AutoFac DI
            //注册要通过反射创建的组件

            //builder.RegisterType<AdvertisementService>().As<IAdvertisementService>();
            builder.RegisterType<BlogCacheAOP>();//可以直接替换其他拦截器
            builder.RegisterType<BlogRedisCacheAOP>();//可以直接替换其他拦截器
            builder.RegisterType<BlogLogAOP>();//这样可以注入第二个

            // ※※★※※ 如果你是第一次下载项目，请先F6编译，然后再F5执行，※※★※※

            #region 带有接口层的服务注入

            #region Service.dll 注入，有对应接口
            //获取项目绝对路径，请注意，这个是实现类的dll文件，不是接口 IService.dll ，注入容器当然是Activatore
            try
            {
                var servicesDllFile = Path.Combine(basePath, "Demo.Core.Api.Services.dll");
                var assemblysServices = Assembly.LoadFrom(servicesDllFile);//直接采用加载文件的方法  ※※★※※ 如果你是第一次下载项目，请先F6编译，然后再F5执行，※※★※※

                //var assemblysServices = Assembly.Load("Demo.Core.Api.Service");//要记得!!!这个注入的是实现类层，不是接口层！不是 
                //builder.RegisterAssemblyTypes(assemblysServices).AsImplementedInterfaces();//指定已扫描程序集中的类型注册为提供所有其实现的接口。


                // AOP 开关，如果想要打开指定的功能，只需要在 appsettigns.json 对应对应 true 就行。
                var cacheType = new List<Type>();
                if (Appsettings.app(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).ObjToBool())
                {
                    cacheType.Add(typeof(BlogRedisCacheAOP));
                }
                if (Appsettings.app(new string[] { "AppSettings", "MemoryCachingAOP", "Enabled" }).ObjToBool())
                {
                    cacheType.Add(typeof(BlogCacheAOP));
                }
                if (Appsettings.app(new string[] { "AppSettings", "LogAOP", "Enabled" }).ObjToBool())
                {
                    cacheType.Add(typeof(BlogLogAOP));
                }

                builder.RegisterAssemblyTypes(assemblysServices)
                          .AsImplementedInterfaces()
                          .InstancePerLifetimeScope()
                          .EnableInterfaceInterceptors()
                //引用Autofac.Extras.DynamicProxy;
                // 如果你想注入两个，就这么写  InterceptedBy(typeof(BlogCacheAOP), typeof(BlogLogAOP));
                // 如果想使用Redis缓存，请必须开启 redis 服务，端口号我的是6319，如果不一样还是无效，否则请使用memory缓存 BlogCacheAOP
                .InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。 
                #endregion

                #region Repository.dll 注入，有对应接口
                var repositoryDllFile = Path.Combine(basePath, "Demo.Core.Api.Repository.dll");
                var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
                builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("※※★※※ 如果你是第一次下载项目，请先对整个解决方案dotnet build（F6编译），然后再对api层 dotnet run（F5执行），\n因为解耦了，如果你是发布的模式，请检查bin文件夹是否存在Repository.dll和service.dll ※※★※※" + ex.Message + "\n" + ex.InnerException);
            }

            #endregion


            #region 没有接口层的服务层注入

            ////因为没有接口层，所以不能实现解耦，只能用 Load 方法。
            ////注意如果使用没有接口的服务，并想对其使用 AOP 拦截，就必须设置为虚方法
            ////var assemblysServicesNoInterfaces = Assembly.Load("Blog.Core.Services");
            ////builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);

            #endregion

            //将services填充到Autofac容器生成器中
            //builder.Populate(services);

            //使用已进行的组件登记创建新容器
            //var ApplicationContainer = builder.Build();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            #region RecordAllLogs

            if (Appsettings.app("AppSettings", "Middleware_RecordAllLogs", "Enabled").ObjToBool())
            {
                loggerFactory.AddLog4Net();//记录所有的访问记录
            }

            #endregion

            #region ReuestResponseLog

            if (Appsettings.app("AppSettings", "Middleware_RequestResponse", "Enabled").ObjToBool())
            {
                app.UseReuestResponseLog();//记录请求与返回数据 
            }

            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }


            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //根据版本名称倒序 遍历展示
                typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName}{version}");
                });

                // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：解决方案名.index.html
                c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Demo.Core.Api.WebApi.index.html");
                //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
                c.RoutePrefix = "";
                //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，
                //这个时候去launchSettings.json中把"launchUrl": "swagger/index.html"去掉， 然后直接访问localhost:8001/index.html即可

            });
            #endregion

            #region CORS(跨越资源共享)
            //跨域第一种版本，启用跨域策略  不推荐使用
            //app.UseCors("AllowAllOrigin"); 

            //跨域第二种版本，请要ConfigureService中配置服务 services.AddCors();
            //    app.UseCors(options => options.WithOrigins("http://localhost:8021").AllowAnyHeader()
            //.AllowAnyMethod()); 

            //跨域第三种方法，使用策略，详细策略信息在ConfigureService中
            app.UseCors("LimitRequests");//将 CORS 中间件添加到 web 应用程序管线中, 以允许跨域请求。
            #endregion

            #region UserDefine

            // 使用静态文件
            app.UseStaticFiles();

            //跳转https
            //app.UseHttpsRedirection();

            //使用cookie
            app.UseCookiePolicy();

            //返回错误码
            app.UseStatusCodePages();//将错误码返回给前台，比如404 
            #endregion

            #region MiniProfiler
            app.UseMiniProfiler();
            #endregion

            app.UseRouting();//路由中间件

            #region 开启认证中间件
            //自定义认证中间件
            //app.UseJwtTokenAuth(); //也可以app.UseMiddleware<JwtTokenAuth>();//注意此授权方法已经放弃 

            //如果你想使用官方认证，必须在上边ConfigureService 中，配置JWT的认证服务 (.AddAuthentication 和 .AddJwtBearer 二者缺一不可)
            app.UseAuthentication();
            #endregion
            app.UseAuthorization();

            // 短路中间件，配置Controller路由
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
