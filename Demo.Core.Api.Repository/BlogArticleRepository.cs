using Demo.Core.Api.IRepository;
using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Repository
{
    public class BlogArticleRepository : BaseRepository<BlogArticle>, IBlogArticleRepository
    {

    }
}
