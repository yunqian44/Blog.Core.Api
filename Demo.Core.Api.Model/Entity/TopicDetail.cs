using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Core.Api.Model.Entity
{
    /// <summary>
    /// Tibug 博文
    /// </summary>
    public class TopicDetail 
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int TopicId { get; set; }

        public string Logo { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public string Detail { get; set; }

        public string SectendDetail { get; set; }

        public bool? IsDeleted { get; set; }

        public int Read { get; set; }

        public int Commend { get; set; }

        public int Good { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime Modifytime { get; set; } = DateTime.Now;

        public int Top { get; set; }

        public string Author { get; set; }

        [SugarColumn(IsIgnore = true)]
        public virtual Topic Topic { get; set; }
    }
}
