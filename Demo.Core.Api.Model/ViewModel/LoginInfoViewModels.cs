using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.ViewModel
{
    /// <summary>
    /// 登陆信息
    /// </summary>
    public class LoginInfoViewModels
    {
        /// <summary>
        /// 登陆名称
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 是否是会员
        /// </summary>
        public bool IsMember { get; set; }
    }
}
