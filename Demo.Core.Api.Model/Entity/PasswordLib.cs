using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.Entity
{
    /// <summary>
    /// 密码库表
    /// </summary>
    public class PasswordLib
    {
        public int Id { get; set; }

        /// <summary>
        ///获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 连接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 账号名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 错误次数
        /// </summary>
        public int? ErrorCount { get; set; }

        /// <summary>
        /// 提示密码
        /// </summary>
        public string HintPwd { get; set; }

        /// <summary>
        /// 密码提示问题
        /// </summary>
        public string HintQuestion { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 最后一次输入错误时间
        /// </summary>
        public DateTime? LastErrTime { get; set; }

        public string test { get; set; }
    }
}
