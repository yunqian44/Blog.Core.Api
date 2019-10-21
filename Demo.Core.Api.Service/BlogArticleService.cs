using Demo.Core.Api.IRepository;
using Demo.Core.Api.IService;
using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.Service
{
    public class BlogArticleServices : BaseServices<BlogArticle>, IBlogArticleService
    {
        IBlogArticleRepository dal;
        public BlogArticleServices(IBlogArticleRepository dal)
        {
            this.dal = dal;
            base.BaseDal = dal;
        }
        /// <summary>
        /// 获取博客列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<BlogArticle>> getBlogs()
        {
            var bloglist = await dal.Query(a => a.Id > 0, a => a.Id);

            return bloglist;

        }
    }
}
