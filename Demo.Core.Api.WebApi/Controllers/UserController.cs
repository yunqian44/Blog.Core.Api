using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Core.Api.Data;
using Demo.Core.Api.Core;
using Demo.Core.Api.Model;
using Demo.Core.Api.Model.Entity;
using Demo.Core.Api.Model.ReqModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Demo.Core.Api.Model.Enum;

namespace Demo.Core.Api.WebApi.Controllers
{
    /// <summary>
    /// 用户管理Api
    /// </summary>
    [Route("api/[controller]")]
    [EnableCors("AllowAllOrigin")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly static string userkey = "userList";

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 获取单个用户根据主键Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        #region 02，新增用户+void Post([FromBody]UserModel userModel)
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="req">用户对象</param>
        [HttpPost]
        public HttpResult Post([FromBody]UserReqModel req)
        {
            var userModel = req.ToEntity();
            userModel.Id = IdGenerator.GetTimeStamp();
            userModel.Status = UserStatusEnum.Enable;
            var json = JsonConvert.SerializeObject(userModel);

            var redis = RedisFactory.GetRedisClient(userkey);
            var exist = redis.HashSet(userkey, userModel.Id, json);
            return new HttpResult(exist?1:0, exist?string.Empty:"添加失败");
        }
        #endregion

        /// <summary>
        /// 根据Id和用户实体修改用户对象
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]UserModel value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
