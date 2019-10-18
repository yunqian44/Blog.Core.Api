using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.Entity
{
    /// <summary>
    /// 博客文章
    /// </summary>
    public class BlogArticle
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Submitter { get; set; }

        /// <summary>
        /// 标题blog
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Bcontent { get; set; }

        /// <summary>
        /// 访问量
        /// </summary>
        public int Traffic { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentNum { get; set; }

        /// <summary> 
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool? IsDeleted { get; set; }

    }
}
