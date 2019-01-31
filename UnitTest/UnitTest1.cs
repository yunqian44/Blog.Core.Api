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
            string key = "123";
            for (int i = 0; i < 500; i++)
            {
                var msg = new { Name = "王维" + i, Status = 1, Gender = 0, Image = "http://localhost:7779/Image/driver.png", Remark = "我是一个特别搞笑的人，他们都叫我小白", IdCard = 610124199303083650, Title = "待办任务" };

                string json = JsonConvert.SerializeObject(msg);
                var redis= RedisFactory.GetRedisClient(key);
                redis.StringSet(key, json);
                
            }
        }
    }
}
