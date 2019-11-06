using System;
using Demo.Core.Api.Model.Enum;
using SqlSugar;

namespace Demo.Core.Api.Model.Entity
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public class UserModel
    {
        public UserModel()
        {
            
        }

        public UserModel(string loginName, string loginPwd)
        {
            
        }

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        public string LoginName { get; set; }

        public string LoginPwd { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <value></value>
        public int Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public System.DateTime ModifyTime { get; set; } = DateTime.Now;

        /// <summary>
        ///最后登录时间 
        /// </summary>
        public DateTime LastErrTime { get; set; } = DateTime.Now;

        /// <summary>
        ///错误次数 
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool IsDeleted { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        #region 用户个人信息
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
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        /// <value></value>
        public string Address { get; set; }
        #endregion
    }
}
