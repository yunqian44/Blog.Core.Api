using System;

namespace Demo.Core.Api.Model.ReqModel
{
    public class UserReqQuery
    {
        public UserReqQuery()
        {
        this.Name=string.Empty;
        this.PageIndex=1;
        this.PageSize=20;
        }

        /// <summary>
        /// 姓名
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        /// <value></value>
        public DateTime? Date { get; set; }
        
        /// <summary>
        /// 页码
        /// </summary>
        /// <value></value>
        public int PageIndex{get;set;}

        /// <summary>
        /// 页大小
        /// </summary>
        /// <value></value>
        public int PageSize{get;set;}
    }
}