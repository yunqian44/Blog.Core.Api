using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.ViewModel
{
    /// <summary>
    /// 博客信息展示类
    /// </summary>
    public class BlogViewModels
    {
        /// <summary>
        /// 博客Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Submitter { get; set; }

        /// <summary>
        /// 博客标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>摘要
        /// 
        /// </summary>
        public string Digest { get; set; }

        /// <summary>
        /// 上一篇
        /// </summary>
        public string Previous { get; set; }

        /// <summary>
        /// 上一篇id
        /// </summary>
        public int PreviousId { get; set; }

        /// <summary>
        /// 下一篇
        /// </summary>
        public string Next { get; set; }

        /// <summary>
        /// 下一篇id
        /// </summary>
        public int NextId { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 访问量
        /// </summary>
        public int Traffic { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentNum { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime CreateTime { get; set; }

        /// <summary> 
        /// 修改时间
        /// </summary>
        public DateTime MotifyTime { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
