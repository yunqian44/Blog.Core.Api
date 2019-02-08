using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Demo.Core.Api.Data
{
    public class RedisConfigInfo
    {
        private const string DefaultSection = "RedisConfig";

        public static IConfigurationSection GetConfig()
        {
            var conf = new ConfigurationBuilder()
                //.AddInMemoryCollection()    //将配置文件的数据加载到内存中
                .SetBasePath(Directory.GetCurrentDirectory())   //指定配置文件所在的目录
                .AddJsonFile("appsettings.json")
                .Build();

            return conf.GetSection(DefaultSection);
        }

        public static IConfigurationSection GetConfig(string sectionName)
        {
            var conf = new ConfigurationBuilder()
                //.AddInMemoryCollection()    //将配置文件的数据加载到内存中
                .SetBasePath(Directory.GetCurrentDirectory())   //指定配置文件所在的目录
                .AddJsonFile("appsettings.json")
                .Build();

            return conf.GetSection(sectionName);
        }
    }
}
