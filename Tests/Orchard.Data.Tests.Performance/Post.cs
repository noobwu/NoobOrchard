using System;
using Soma.Core;

namespace Orchard.Data.Tests.Performance
{
    [ServiceStack.DataAnnotations.Alias("Posts")]
    [Table(Name = "Posts")]
    public partial class Post
    {
        [Id(IdKind.Identity)]
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
