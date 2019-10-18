using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Demo.Core.Api.Core.Helper;
using Demo.Core.Api.Data;
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

namespace Demo.Core.Api.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Env { get; }


        private const string ApiName = "Blog.Core";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(options =>
            {
                options.Filters.Add<ApiErrorHandleFilter>();

                //返回xml格式 asp.net core 默认提供的是json格式
                //options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            // Register the Swagger generator, defining 1 or more Swagger documents
            #region Swagger
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

                   #region 获取xml
                   // Set the comments path for the Swagger JSON and UI.
                   var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                   var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                   c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改 
                   #endregion



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

            #region Authorize 权限认证

            #region 参数
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
            services.AddSingleton(new RedisConfigInfo(Env.ContentRootPath));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                #region Swagger
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                // specifying the Swagger JSON endpoint.
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
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region CORS(跨越资源共享)
            //跨域第一种版本，启用跨域策略  不推荐使用
            //app.UseCors("AllowAllOrigin"); 

            //跨域第一种版本，请要ConfigureService中配置服务 services.AddCors();
            //    app.UseCors(options => options.WithOrigins("http://localhost:8021").AllowAnyHeader()
            //.AllowAnyMethod()); 

            //跨域第二种方法，使用策略，详细策略信息在ConfigureService中
            app.UseCors("LimitRequests");//将 CORS 中间件添加到 web 应用程序管线中, 以允许跨域请求。
            #endregion

            #region UserDefine

            // 使用静态文件
            app.UseStaticFiles();

            //跳转https
            app.UseHttpsRedirection();

            //返回错误码
            app.UseStatusCodePages();//将错误码返回给前台，比如404 
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
