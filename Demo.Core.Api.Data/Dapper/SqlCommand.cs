using System;

namespace Demo.Core.Api.Data.Dapper
{
    /// <summary>
    /// sql命令
    /// </summary>
    public class SqlCommand
    {
        /// <summary>
        /// SQL语句名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// SQL语句或存储过程内容
        /// </summary>
        public string Sql { get; set; }
    }
}