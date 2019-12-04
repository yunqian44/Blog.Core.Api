using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Core.Api.Core.CachingAttribute;
using Demo.Core.Api.Core.Helper;
using Demo.Core.Api.Data;
using Demo.Core.Api.IServices;
using Demo.Core.Api.Model.Entity;
using Demo.Core.Api.WebApi.SwaggerHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;
using static Demo.Core.Api.WebApi.SwaggerHelper.CustomApiVersion;

namespace Demo.Core.Api.WebApi.Controllers
{
    [Route("api/Blog")]
    [ApiController]
    [Authorize]
    public class BlogController : ControllerBase
    {
        private readonly IHostingEnvironment _env;
        readonly IRedisCacheManager _redisCacheManager;
        readonly IBlogArticleService _blogArticleService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="blogArticleService"></param>
        /// <param name="redisCacheManager"></param>
        public BlogController(IBlogArticleService blogArticleService,
            IRedisCacheManager redisCacheManager,
            IHostingEnvironment env)
        {
            _env = env;
            _blogArticleService = blogArticleService;
            _redisCacheManager = redisCacheManager;
        }

        /// <summary>
        /// 获取博客列表【无权限】
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="category"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<object> Get(int id, int page = 1, string category = "技术博文", string key = "")
        {
            int intTotalCount = 6;
            int total;
            int totalCount = 1;
            List<BlogArticle> blogArticleList = new List<BlogArticle>();
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            using (MiniProfiler.Current.Step("开始加载数据："))
            {
                try
                {
                    if (_redisCacheManager.Get<object>("Redis.Blog") != null)
                    {
                        MiniProfiler.Current.Step("从Redis服务器中加载数据：");
                        blogArticleList = _redisCacheManager.Get<List<BlogArticle>>("Redis.Blog");
                    }
                    else
                    {
                        MiniProfiler.Current.Step("从MySql服务器中加载数据：");
                        blogArticleList = await _blogArticleService.Query(a => a.Category == category && a.IsDeleted == false);
                        _redisCacheManager.Set("Redis.Blog", blogArticleList, TimeSpan.FromHours(2));
                    }

                }
                catch (Exception e)
                {
                    MiniProfiler.Current.CustomTiming("Errors：", "Redis服务未启用" + e.Message);
                    blogArticleList = await _blogArticleService.Query(a => a.Category == category && a.IsDeleted == false);
                }
            }

            blogArticleList = blogArticleList.Where(d => (d.Title != null && d.Title.Contains(key)) || (d.Content != null && d.Content.Contains(key))).ToList();
            total = blogArticleList.Count();
            totalCount = blogArticleList.Count() / intTotalCount;

            using (MiniProfiler.Current.Step("获取成功后，开始处理最终数据"))
            {
                blogArticleList = blogArticleList.OrderByDescending(d => d.Id).Skip((page - 1) * intTotalCount).Take(intTotalCount).ToList();

                foreach (var item in blogArticleList)
                {
                    if (!string.IsNullOrEmpty(item.Content))
                    {
                        item.Remark = (HtmlHelper.ReplaceHtmlTag(item.Content)).Length >= 200 ? (HtmlHelper.ReplaceHtmlTag(item.Content)).Substring(0, 200) : (HtmlHelper.ReplaceHtmlTag(item.Content));
                        int totalLength = 500;
                        if (item.Content.Length > totalLength)
                        {
                            item.Content = item.Content.Substring(0, totalLength);
                        }
                    }
                }
            }

            return Ok(new
            {
                success = true,
                page,
                total,
                pageCount = totalCount,
                data = blogArticleList
            });
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


        // GET: api/Blog/Environment
        /// <summary>
        /// Environment接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Environment")]
        [AllowAnonymous]
        public string Get()
        {
            //return _advertisementService.Sum(i, j);
            return $"当前系统处于什么环境，开发:{_env.IsDevelopment()}-----生成环境:{_env.IsProduction()}";
        }

        // GET: api/Blog/5
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Get")]
        public async Task<object> Get(int id)
        {
            var model = await _blogArticleService.getBlogDetails(id);//调用该方法
            var data = new { success = true, data = model };
            return data;
        }

        /// <summary>
        /// 获取详情【无权限】
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DetailNuxtNoPer")]
        [AllowAnonymous]
        public async Task<object> DetailNuxtNoPer(int id)
        {
            var model = await _blogArticleService.getBlogDetails(id);
            return Ok(new
            {
                success = true,
                data = model
            });
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


        /// <summary>
        /// 获取博客测试信息 v2版本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        ////MVC自带特性 对 api 进行组管理
        //[ApiExplorerSettings(GroupName = "v2")]
        ////路径 如果以 / 开头，表示绝对路径，反之相对 controller 的想u地路径
        //[Route("/api/v2/blog/Blogtest")]
        //和上边的版本控制以及路由地址都是一样的l
        [CustomRoute(ApiVersions.v2, "Blogtest")]
        public async Task<object> V2_Blogtest()
        {
            return  Ok(new { status = 220, data = "我是第二版的博客信息" });
        }
    }
}
