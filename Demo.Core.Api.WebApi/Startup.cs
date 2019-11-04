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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using Demo.Core.Api.Core.Extension;
using AutoMapper;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Demo.Core.Api.WebApi
{
    public class Startup
    {
        /// <summary>
        /// log4net 仓储库
        /// </summary>
        public static ILoggerRepository Repository { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
            //log4net
            Repository = LogManager.CreateRepository(Configuration["Logging:Log4Net:Name"]);
            //指定配置文件，如果这里你遇到问题，应该是使用了InProcess模式，请查看Blog.Core.csproj,并删之
            var contentPath = env.ContentRootPath;
            var log4Config = Path.Combine(contentPath, "log4net.config");
            XmlConfigurator.Configure(Repository, new FileInfo(log4Config));
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Env { get; }


        private const string ApiName = "Blog.Core";

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(options =>
            {
                options.Filters.Add<ApiErrorHandleFilter>();

                //返回xml格式 asp.net core 默认提供的是json格式
                //options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // Redis注入
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();

            // log日志注入
            services.AddSingleton<ILoggerHelper, LogHelper>();

            // 缓存注入
            services.AddScoped<ICaching, MemoryCaching>();

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
                //↓↓↓↓↓↓↓注意正式环境不要使用这种全开放的处理↓↓↓↓↓↓↓↓↓↓
                //c.AddPolicy("AllRequests", policy =>
                //{
                //    policy
                //    .AllowAnyOrigin()//允许任何源
                //    .AllowAnyMethod()//允许任何方式
                //    .AllowAnyHeader()//允许任何头
                //    .AllowCredentials();//允许cookie
                //});
                //↑↑↑↑↑↑↑注意正式环境不要使用这种全开放的处理↑↑↑↑↑↑↑↑↑↑


                //一般采用这种方法
                c.AddPolicy("LimitRequests", policy =>
                {
                    policy
                    .WithOrigins("http://127.0.0.1:1818", "http://localhost:8080", "http://localhost:8021", "http://localhost:8081", "http://localhost:1818", "http://localhost:8082")//支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
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
                   c.SwaggerDoc($"{ApiName}",
                       new Info
                       {
                           Title = "Blog.Core API",
                           Version = "v1.0",
                           Description = "框架说明文档",
                           TermsOfService = "None",
                           Contact = new Contact()
                           {
                               Name = "刺客",
                               Email = "yunqian8@live.com",
                               Url = "暂时没有"
                           }
                       });

                   //就是这里
                   var xmlPath = Path.Combine(basePath, "Demo.Core.Api.WebApi.xml");//这个就是刚刚配置的xml文件名
                   c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                   var xmlModelPath = Path.Combine(basePath, "Demo.Core.Api.Model.xml");//这个就是Model层的xml文件名
                   c.IncludeXmlComments(xmlModelPath,false);

                   #region Token绑定到ConfigureServices
                   //添加header验证信息
                   //c.OperationFilter<SwaggerHeader>();
                   var security = new Dictionary<string, IEnumerable<string>> { { "Blog.Core", new string[] { } }, };
                   c.AddSecurityRequirement(security);
                   //方案名称“Blog.Core”可自定义，上下一致即可
                   c.AddSecurityDefinition("Blog.Core", new ApiKeyScheme
                   {
                       Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                       Name = "Authorization",//jwt默认的参数名称
                       In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                       Type = "apiKey"
                   });
                   #endregion
               });
            #endregion

            #region SignalR 通讯
            services.AddSignalR();
            #endregion

            #region Authorize 权限认证

            #region 参数
            services.AddAuthorization(option =>
            {
                option.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
            });


            //读取配置文件
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            #endregion

            #region 认证
            services.AddAuthentication(x =>
                {
                    //看这个单词熟悉么？没错，就是上边错误里的那个。
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                 .AddJwtBearer(o =>
                 {
                     o.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,//是否验证Issuer
                         IssuerSigningKey = signingKey,//参数配置在下边
                         ValidateIssuer = true,//是否验证Issuer
                         ValidIssuer = audienceConfig["Issuer"],//发行人
                         ValidateAudience = true,
                         ValidAudience = audienceConfig["Audience"],//订阅人
                         ValidateLifetime = true,//是否验证超时
                         //注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
                         ClockSkew = TimeSpan.FromSeconds(30),
                         RequireExpirationTime = true,
                     };

                 });
            #endregion
            #endregion

            services.AddSingleton(new Appsettings(Env.ContentRootPath));
            services.AddSingleton(new LogLock(Env.ContentRootPath));

            #region AutoFac DI
            //实例化 AutoFac  容器   
            var builder = new ContainerBuilder();

            //注册要通过反射创建的组件

            //builder.RegisterType<AdvertisementService>().As<IAdvertisementService>();
            builder.RegisterType<BlogCacheAOP>();//可以直接替换其他拦截器
            //builder.RegisterType<BlogRedisCacheAOP>();//可以直接替换其他拦截器
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
                //if (Appsettings.app(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).ObjToBool())
                //{
                //    cacheType.Add(typeof(BlogRedisCacheAOP));
                //}
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
                          .InterceptedBy(cacheType.ToArray());
                //引用Autofac.Extras.DynamicProxy;
                // 如果你想注入两个，就这么写  InterceptedBy(typeof(BlogCacheAOP), typeof(BlogLogAOP));
                // 如果想使用Redis缓存，请必须开启 redis 服务，端口号我的是6319，如果不一样还是无效，否则请使用memory缓存 BlogCacheAOP
                //.InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。 
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
            builder.Populate(services);

            //使用已进行的组件登记创建新容器
            var ApplicationContainer = builder.Build();

            #endregion

            return new AutofacServiceProvider(ApplicationContainer);//第三方IOC接管 core内置DI容器

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
                c.SwaggerEndpoint($"/swagger/{ApiName}/swagger.json", $"{ApiName}");

                // 将swagger设置成首页
                c.RoutePrefix = "";
                //路径配置，设置为空，表示直接访问该文件，
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


            #region 开启认证中间件
            //自定义认证中间件
            //app.UseJwtTokenAuth(); //也可以app.UseMiddleware<JwtTokenAuth>(); 

            //如果你想使用官方认证，必须在上边ConfigureService 中，配置JWT的认证服务 (.AddAuthentication 和 .AddJwtBearer 二者缺一不可)
            app.UseAuthentication();
            #endregion

            app.UseMvc();
        }
    }
}
