using Demo.Core.Api.Model.Entity;
using Demo.Core.Api.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.IServices
{
    public interface IBlogArticleService:IBaseService<BlogArticle>
    {
        Task<List<BlogArticle>> getBlogs();

        Task<BlogViewModels> getBlogDetails(int id);
    }
}
