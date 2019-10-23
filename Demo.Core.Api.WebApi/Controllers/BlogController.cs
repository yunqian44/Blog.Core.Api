using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Core.Api.Core.CachingAttribute;
using Demo.Core.Api.Data;
using Demo.Core.Api.IService;
using Demo.Core.Api.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Core.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy ="Admin")]
    public class BlogController : ControllerBase
    {
        readonly IAdvertisementService _advertisementService;
        readonly IRedisCacheManager _redisCacheManager;
        readonly IBlogArticleService _blogArticleService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="advertisementService"></param>
        /// <param name="blogArticleService"></param>
        /// <param name="redisCacheManager"></param>
        public BlogController(IAdvertisementService advertisementService,
            IBlogArticleService blogArticleService,
            IRedisCacheManager redisCacheManager)
        {
            _advertisementService = advertisementService;
            _blogArticleService = blogArticleService;
            _redisCacheManager = redisCacheManager;
        }

        /// <summary>
        /// 获取博客列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBlogs")]
        public async Task<List<BlogArticle>> GetBlogs()
        {
            var blogArticleList = new List<BlogArticle>();

            if (_redisCacheManager.Get<object>("Redis.Blog") != null)
            {
                blogArticleList = _redisCacheManager.Get<List<BlogArticle>>("Redis.Blog");
            }
            else
            {
                blogArticleList = await _blogArticleService.getBlogs();
                _redisCacheManager.Set("Redis.Blog", blogArticleList, TimeSpan.FromHours(2));//缓存2小时
            }

            return blogArticleList;
        }


        // GET: api/Blog
        /// <summary>
        /// Sum接口
        /// </summary>
        /// <param name="i">参数i</param>
        /// <param name="j">参数j</param>
        /// <returns></returns>
        [HttpGet]
        public int Get(int i,int j)
        {
            return _advertisementService.Sum(i, j);
        }

        // GET: api/Blog/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<List<Advertisement>> Get(int id)
        {
            return await _advertisementService.Query(d => d.Id == id);
        }

        // POST: api/Blog
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Blog/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
