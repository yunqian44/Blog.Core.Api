using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Autofac.Extensions.DependencyInjection;
using Demo.Core.Api.Core.Extension;
using Demo.Core.Api.Model.Seed;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demo.Core.Api.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("Log4net.config"));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            // 生成承载 web 应用程序的 Microsoft.AspNetCore.Hosting.IWebHost。Build是WebHostBuilder最终的目的，将返回一个构造的WebHost，最终生成宿主。
            var host = CreateHostBuilder(args).Build();

            // 创建可用于解析作用域服务的新 Microsoft.Extensions.DependencyInjection.IServiceScope。
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var env = services.GetRequiredService<IWebHostEnvironment>();
                if (env.IsDevelopment())
                {
                    try
                    {
                        // 从 system.IServicec提供程序获取 T 类型的服务。
                        var configuration = services.GetRequiredService<IConfiguration>();
                        if (configuration.GetSection("AppSettings")["SeedDBEnabled"].ObjToBool() || configuration.GetSection("AppSettings")["SeedDBDataEnabled"].ObjToBool())
                        {
                            var myContext = services.GetRequiredService<MyContext>();
                            DBSeed.SeedAsync(myContext).Wait();
                        }
                    }
                    catch (Exception e)
                    {
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogError(e, "Error occured seeding the Database.");
                    }
                }
            }

            // 运行 web 应用程序并阻止调用线程, 直到主机关闭。
            // 创建完 WebHost 之后，便调用它的 Run 方法，而 Run 方法会去调用 WebHost 的 StartAsync 方法
            // 将Initialize方法创建的Application管道传入以供处理消息
            // 执行HostedServiceExecutor.StartAsync方法
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           //使用预配置的默认值初始化 Microsoft.AspNetCore.Hosting.WebHostBuilder 类的新实例。aa
           Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory()) //<--NOTE THIS
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .ConfigureKestrel(serverOptions =>
                {
                    serverOptions.AllowSynchronousIO = true;//启用同步 IO
                })
                .UseStartup<Startup>()
                .UseUrls("http://*:7001")
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                });
            });
    }
}
