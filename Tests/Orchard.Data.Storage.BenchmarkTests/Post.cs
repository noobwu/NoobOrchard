using Dapper.Contrib.Extensions;
using System;
namespace Orchard.Data.Storage.BenchmarkTests
{
    [Table("Posts")]
    public partial class Post
    {
        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual DateTime LastChangeDate { get; set; }
        public virtual int? Counter1 { get; set; }
        public virtual int? Counter2 { get; set; }
        public virtual int? Counter3 { get; set; }
        public virtual int? Counter4 { get; set; }
        public virtual int? Counter5 { get; set; }
        public virtual int? Counter6 { get; set; }
        public virtual int? Counter7 { get; set; }
        public virtual int? Counter8 { get; set; }
        public virtual int? Counter9 { get; set; }
    }
}
