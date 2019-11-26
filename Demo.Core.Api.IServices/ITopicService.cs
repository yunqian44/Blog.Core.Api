using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.IServices
{
    public interface ITopicService : IBaseService<Topic>
    {
        Task<List<Topic>> GetTopics();
    }
}
