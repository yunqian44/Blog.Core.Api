using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Core.Api.Model.Entity;
using Demo.Core.Api.Model.Enum;

namespace Demo.Core.Api.Model.ViewModel
{
    public class UserListViewModel
    {
        /// <summary>
        /// 主键id
        /// </summary>
        /// <value></value>
        public long id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        /// <value></value>
        public string name { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        /// <value></value>
        public string date { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        /// <value></value>
        public string address { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        /// <value></value>
        public UserStatusEnum status { get; set; }


        public List<UserListViewModel> ToEntities(List<UserModel> userList)
        {
            var modelList = from c in userList
                            select new UserListViewModel()
                            {
                                id=c.Id,
                                name=c.UserName,
                                date=c.Brithday.ToString("yyyy-MM-dd"),
                                address=c.Address,
                                status=c.Status
                            };
            return modelList.ToList();
        }
    }
}