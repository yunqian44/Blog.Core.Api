using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.ViewModel
{
    /// <summary>
    /// 留言信息展示类
    /// </summary>
    public class GuestbookViewModels
    {
        /// <summary>留言表
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>博客ID
        /// 
        /// </summary>
        public int? BlogId { get; set; }
        /// <summary>创建时间
        /// 
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 留言内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 是否显示在前台,0否1是
        /// </summary>
        public bool IsShow { get; set; }

        public BlogArticle BlogArticle { get; set; }
    }
}
