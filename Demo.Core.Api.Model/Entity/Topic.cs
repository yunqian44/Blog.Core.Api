using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.Entity
{
    /// <summary>
    /// Tibug 类别
    /// </summary>
    public class Topic
    {
        public int Id { get; set; }

        public string Logo { get; set; }

        public string Name { get; set; }

        public string Detail { get; set; }

        public string Author { get; set; }

        public string SectendDetail { get; set; }

        public bool IsDeleted { get; set; }

        public int Read { get; set; }

        public int Commend { get; set; }

        public int Good { get; set; }

        public DateTime Createtime { get; set; } = DateTime.Now;

        public DateTime Modifytime { get; set; } = DateTime.Now;

        public virtual IEnumerable<TopicDetail> TopicDetail { get; set; } = new List<TopicDetail>();
    }
}
