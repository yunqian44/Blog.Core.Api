using System;

namespace Demo.Core.Api.Model
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        /// <value></value>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        /// <value></value>
        public string UserName { get; set; }

        /// <summary>
        /// 出生年月
        /// </summary>
        /// <value></value>
        public DateTime Brithday { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        /// <value></value>
        public string Address { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        /// <value></value>
        public int Status { get; set; }
    }
}
