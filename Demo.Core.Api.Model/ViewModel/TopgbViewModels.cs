using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.ViewModel
{
    /// <summary>
    /// 留言排名展示类
    /// </summary>
    public class TopgbViewModels
    {
        /// <summary>
        /// 博客Id
        /// </summary>
        public int? BlogId { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int Counts { get; set; }

        /// <summary>
        /// 博客标题
        /// </summary>
        public string Title { get; set; }
    }
}
