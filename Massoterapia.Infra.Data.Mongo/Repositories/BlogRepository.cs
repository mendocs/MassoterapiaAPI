using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Infra.Data.Mongo.Base;
using Massoterapia.Infra.Data.Mongo.Context;
using MongoDB.Driver;

namespace Massoterapia.Infra.Data.Mongo.Repositories
{
    public class BlogRepository : RepositoryWrite<Blog>, IBlogRepository
    {

        public BlogRepository(IMongoCollection<Blog> collectionName) : base(collectionName)
        {
        }

        public BlogRepository(IConnectionFactory connectionFactory, string databaseName, string collectionName)
            : base(connectionFactory, databaseName, collectionName)
        {
        }

        public Task<IQueryable<Blog>> QueryAll(bool active)
        {
            var blogsFromDB = _collectionName.AsQueryable<Blog>().Where(w => (w.Active == active));
            
            return Task.FromResult(blogsFromDB);
        }

        public Task<Blog> QueryByTitleNFD(string titleNFD)
        {
            var blogFromDB = _collectionName.AsQueryable<Blog>().First(w => (w.TitleNFD == titleNFD));
            
            return Task.FromResult(blogFromDB);
        }
    }
}