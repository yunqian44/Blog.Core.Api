using System;
using Demo.Core.Api.Model.Entity;

namespace Demo.Core.Api.Model.ReqModel
{
    public class UserReqModel
    {
        public string name { get; set; } 

        public string address { get; set; }

        public DateTime date{get;set;}

        public UserModel ToEntity()
        {
            var model=new UserModel();
            model.Address=this.address;
            model.Brithday=this.date;
            model.UserName=this.name;
            return model;
        }
    }
}