using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.Entity
{
    /// <summary>
    /// 路由菜单表
    /// </summary>
    public class Permission
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 菜单执行Action名
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 菜单显示名（如用户页、编辑(按钮)、删除(按钮)）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否是按钮
        /// </summary>
        public bool IsButton { get; set; } = false;

        /// <summary>
        /// 是否是隐藏菜单
        /// </summary>
        public bool? IsHide { get; set; } = false;

        /// <summary>
        /// 上一级菜单（0表示上一级无菜单）
        /// </summary>
        public int PId { get; set; }

        /// <summary>
        /// 按钮事件
        /// </summary>
        public string Func { get; set; }

        /// <summary>
        /// 接口api
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderSort { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 菜单描述    
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 激活状态
        /// </summary>
        public bool Enabled { get; set; }

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
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改ID
        /// </summary>
        public int? ModifyId { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public string ModifyBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; } = DateTime.Now;

        /// <summary>
        ///获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool? IsDeleted { get; set; }
        
        [SugarColumn(IsIgnore = true)]
        public virtual List<int> PidArr { get; set; } = new List<int>();
        
        [SugarColumn(IsIgnore = true)]
        public virtual List<string> PnameArr { get; set; } = new List<string>();
        
        [SugarColumn(IsIgnore = true)]
        public virtual List<string> PCodeArr { get; set; } = new List<string>();
    }
}
