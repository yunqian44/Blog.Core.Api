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
using Demo.Core.Api.Model.ViewModel;
using System.Linq.Expressions;

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

        #region 01，获取用户列表+HttpResult Get([FromQuery]UserReqQuery reqQuery)
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="reqQuery">筛选条件</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResult Get([FromQuery]UserReqQuery reqQuery)
        {
            var redis = RedisFactory.GetRedisClient(userkey);
            var redisValue = redis.HashGetAll(userkey);
            var userList = new List<UserModel>();
            if (redisValue != null && redisValue.Count() > 0)
            {
                for (int i = 0; i < redisValue.Count(); i++)
                {
                    userList.Add(JsonConvert.DeserializeObject<UserModel>(redisValue[i].Value));
                }

            }
            Expression<Func<UserModel, bool>> where = c1 => true;
            if (!string.IsNullOrWhiteSpace(reqQuery.Name))
            {
                Expression<Func<UserModel, bool>> query = c1 => c1.UserName.Contains(reqQuery.Name);
                where = where.AndJoin(query);
            }
            if (reqQuery.Date != null)
            {
                var date = reqQuery.Date.GetValueOrDefault().Date.AddDays(1);
                Expression<Func<UserModel, bool>> query = c1 => c1.Brithday <= date;
                where = where.AndJoin(query);
            }
            var modelList = userList.AsQueryable().Where(where)
                                .Skip(reqQuery.PageSize * (reqQuery.PageIndex - 1))
                                .Take(reqQuery.PageSize).ToList();
            var result=new UserListViewModel().ToEntities(modelList);                  
            return new HttpResult(new
            {
                userList = result,
                total=userList.Count                
            });
        }
        #endregion


        #region 02，获取单个用户根据主键Id+HttpResult Get(long id)
        /// <summary>
        /// 获取单个用户根据主键Id
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public HttpResult Get(long id)
        {
            var redis = RedisFactory.GetRedisClient(userkey);
            var redisValue = redis.HashGet(userkey, id);
            if (!redisValue.IsNull)
            {
                return new HttpResult(JsonConvert.DeserializeObject<UserModel>(redisValue));
            }
            else
            {
                return new HttpResult();
            }
        }
        #endregion


        #region 03，新增用户+HttpResult Post([FromBody]UserModel userModel)
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
            return new HttpResult(exist ? 1 : 0, exist ? string.Empty : "添加失败");
            //return new HttpResult(0, "添加失败");
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
