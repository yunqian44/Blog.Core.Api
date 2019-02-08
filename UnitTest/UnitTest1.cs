using Demo.Core.Api.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string key = "User";
            for (int i = 0; i < 500; i++)
            {
                var msg = new { Name = "你好" + i, Status = 1, Gender = 0, Image = "http://localhost:7779/Image/driver.png", Remark = "我是测试的redis", IdCard = 610124199303083650, Title = "测试" };

                string json = JsonConvert.SerializeObject(msg);
                var redis= RedisFactory.GetRedisClient(key);
                redis.HashSet(key,i,json);
            }
        }
    }
}
