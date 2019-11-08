using Demo.Core.Api.Core.DB;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.Seed
{
    public class MyDbContext
    {
        private static string _connectionString = BaseDBConfig.ConnectionString;
        private static MySqlConnection _conn;

        /// <summary>
        /// 连接字符串 
        /// Blog.Core
        /// </summary>
        public static string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public MySqlConnection Db
        {
            get { return _conn; }
            set { _conn = value; }
        }

        /// <summary>
        /// 功能描述:构造函数
        /// </summary>
        public MyDbContext()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentNullException("数据库连接字符串为空");
            _conn = new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// 功能描述:获得一个DbContext
        /// </summary>
        /// <returns></returns>
        public static MyDbContext GetDbContext()
        {
            return new MyDbContext();
        }
    }
}
