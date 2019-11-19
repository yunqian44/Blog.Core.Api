using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Demo.Core.Api.Core.Helper
{
    public class GetTableData
    {
        static string contentRoot = string.Empty;

        public GetTableData(string ContentRootPath)
        {
            contentRoot = ContentRootPath;
        }

        public static string GetData(string tableName)
        {
            string data= ReadData(Path.Combine(contentRoot, "TableData", $"{tableName}.json"), Encoding.UTF8);
            return data;
        }

        private static string ReadData(string Path, Encoding encode)
        {
            string s = "";
            try
            {
                if (!System.IO.File.Exists(Path))
                {
                    s = null;
                }
                else
                {
                    StreamReader f2 = new StreamReader(Path, encode);
                    s = f2.ReadToEnd();
                    f2.Close();
                    f2.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //LogLock.OutSql2Log("AOPLog", new string[] { dataIntercept });
            }
            return s;
        }
    }
}
