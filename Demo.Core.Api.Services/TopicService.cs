using Demo.Core.Api.Core.CachingAttribute;
using Demo.Core.Api.IRepository;
using Demo.Core.Api.IServices;
using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.Services
{
    public class TopicServices : BaseService<Topic>, ITopicService
    {

        ITopicRepository _dal;
        public TopicServices(ITopicRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }

        /// <summary>
        /// 获取开Bug专题分类（缓存）
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 60)]
        public async Task<List<Topic>> GetTopics()
        {
            return await base.Query(a => !a.IsDeleted && a.SectendDetail == "tbug");
        }

    }
}
