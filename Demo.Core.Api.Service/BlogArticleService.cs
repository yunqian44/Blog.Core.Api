using AutoMapper;
using Demo.Core.Api.IRepository;
using Demo.Core.Api.IService;
using Demo.Core.Api.Model.Entity;
using Demo.Core.Api.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.Service
{
    public class BlogArticleServices : BaseServices<BlogArticle>, IBlogArticleService
    {
        IBlogArticleRepository dal;
        IMapper IMapper;
        public BlogArticleServices(IBlogArticleRepository dal, IMapper iMapper)
        {
            this.dal = dal;
            base.BaseDal = dal;
            IMapper = iMapper;
        }

        /// <summary>
        /// 获取视图博客详情信息(一般版本)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BlogViewModels> getBlogDetails(int id)
        {
            var bloglist = await dal.Query(a => a.Id > 0, a => a.Id);
            var blogArticle = (await dal.Query(a => a.Id == id)).FirstOrDefault();
            BlogViewModels models = null;

            if (blogArticle != null)
            {
                BlogArticle prevblog;
                BlogArticle nextblog;
                int blogIndex = bloglist.FindIndex(item => item.Id == id);
                if (blogIndex >= 0)
                {
                    try
                    {
                        // 上一篇
                        prevblog = blogIndex > 0 ? (((BlogArticle)(bloglist[blogIndex - 1]))) : null;
                        // 下一篇
                        nextblog = blogIndex + 1 < bloglist.Count() ? (BlogArticle)(bloglist[blogIndex + 1]) : null;

                        // 注意就是这里,mapper
                        models = IMapper.Map<BlogViewModels>(blogArticle);

                        #region 旧代码
                        //models = new BlogViewModels()
                        //{
                        //    Submitter = blogArticle.Submitter,
                        //    Title = blogArticle.Title,
                        //    Category = blogArticle.Category,
                        //    Content = blogArticle.Content,
                        //    Traffic = blogArticle.Traffic,
                        //    CommentNum = blogArticle.CommentNum,
                        //    ModifyTime = blogArticle.ModifyTime,
                        //    CreateTime = blogArticle.CreateTime,
                        //    Remark = blogArticle.Remark,
                        //}; 
                        #endregion

                        if (nextblog != null)
                        {
                            models.Next = nextblog.Title;
                            models.NextId = nextblog.Id;
                        }
                        if (prevblog != null)
                        {
                            models.Previous = prevblog.Title;
                            models.PreviousId = prevblog.Id;
                        }
                    }
                    catch (Exception) { }
                }
                blogArticle.Traffic += 1;
                await dal.Update(blogArticle, new List<string> { "Traffic" });
            }

            return models;

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
