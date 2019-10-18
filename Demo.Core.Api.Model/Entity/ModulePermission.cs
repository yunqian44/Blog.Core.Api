using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.Entity
{
    /// <summary>
    /// 菜单与按钮关系表
    /// </summary>
    public class ModulePermission
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        ///获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// 按钮ID
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// 创建ID
        /// </summary>
        public int? CreateId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 修改ID
        /// </summary>
        public int? ModifyId { get; set; }
        /// <summary>
        /// 修改者
        /// </summary>
        public string ModifyBy { get; set; }
        /// <summary>
        ///修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
    }
}
