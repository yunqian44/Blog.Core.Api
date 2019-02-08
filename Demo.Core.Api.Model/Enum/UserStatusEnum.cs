using System;
using System.ComponentModel;

namespace Demo.Core.Api.Model.Enum
{
    /// <summary>
    /// 用户状态枚举
    /// </summary>
    public enum UserStatusEnum
    {
        
        [Description("启用")]
        Unable = 0,

        [Description("未启用")]
        Enable = 1
    }
}
