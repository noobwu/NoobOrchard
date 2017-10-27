using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Orchard.Data.MongoDb.Tests.Entities;
using Orchard.Utility;

namespace Orchard.Data.MongoDb.Tests.Repositories
{
    [TestFixture]
    public class Post_Tests: MongoDbTestsBase
    {
        public override void Register(ContainerBuilder builder)
        {
           
        }

        [Test]
        public void MongoCollection_Insert()
        {
            // TODO: Add your test code here
            Collection.Insert(CreatePost());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Post CreatePost()
        {
            Post postEntity = new Post();
            postEntity.Author = "Author "+RandomHelper.GetRandom(100,999);
            postEntity.Comments = new List<AdmArea>() { CreateAdmArea(),CreateAdmArea()};
            postEntity.Date = DateTime.Now;
            postEntity.Details= "Details " + RandomHelper.GetRandom(100, 999);
            postEntity.Summary= "Summary " + RandomHelper.GetRandom(100, 999);
            postEntity.Title = "Title " + RandomHelper.GetRandom(100, 999);
            postEntity.TotalComments = postEntity.Comments.Count();
            postEntity.Url = "https://docs.mongodb.com/ecosystem/drivers/csharp/";
            return postEntity;
        }
        public virtual MongoCollection<Post> Collection
        {
            get
            {
                return DataBase.GetCollection<Post>(typeof(Post).Name);
            }
        }
    }
}
