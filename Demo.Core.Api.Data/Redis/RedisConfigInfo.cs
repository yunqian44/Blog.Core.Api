using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using StackExchange.Redis;

namespace Demo.Core.Api.Data
{
    public class RedisConfigInfo
    {
        static IConfiguration Configuration { get; set; }
        static string contentPath { get; set; }

        static string DefaultSection = "RedisConfig";

        public RedisConfigInfo(string contentPath)
        {
            string Path = "appsettings.json";
            //如果你把配置文件 是 根据环境变量来分开了，可以这样写
            //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

            Configuration = new ConfigurationBuilder()
               .SetBasePath(contentPath)
               .Add(new JsonConfigurationSource { Path = Path, Optional = false, ReloadOnChange = true })//这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
               .Build();
        }


        public static IConfigurationSection GetConfig()
        {
            //var conf = new ConfigurationBuilder()
            //    //.AddInMemoryCollection()    //将配置文件的数据加载到内存中
            //    .SetBasePath(Directory.GetCurrentDirectory())   //指定配置文件所在的目录
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            return Configuration.GetSection(DefaultSection);
        }

        public static IConfigurationSection GetConfig(string sectionName)
        {
            //var conf = new ConfigurationBuilder()
            //    //.AddInMemoryCollection()    //将配置文件的数据加载到内存中
            //    .SetBasePath(Directory.GetCurrentDirectory())   //指定配置文件所在的目录
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            return Configuration.GetSection(sectionName);
        }
    }
}
