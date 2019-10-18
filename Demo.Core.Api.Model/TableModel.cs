using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model
{
    /// <summary>
    /// 表格数据，支持分页
    /// </summary>
    public class TableModel
    {
        /// <summary>
        /// 返回编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 返回的数据集
        /// </summary>
        public dynamic Data { get; set;}
    }
}
