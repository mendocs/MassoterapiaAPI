using System.Linq;
using System.Threading.Tasks;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Infra.Data.Mongo.Base;
using Massoterapia.Infra.Data.Mongo.Context;
using MongoDB.Driver;

namespace Massoterapia.Infra.Data.Mongo.Repositories
{
    public class UserRepository: RepositoryWrite<User>, IUserRepository
    {
        public UserRepository(IMongoCollection<User> collectionName) : base(collectionName)
        {
        }

        public UserRepository(IConnectionFactory connectionFactory, string databaseName, string collectionName)
            : base(connectionFactory, databaseName, collectionName)
        {
        }

        public Task<User> QueryByNamePasswordHash(string name)
        {
            User user = _collectionName.AsQueryable<User>().FirstOrDefault(w => w.Name == name);
            return Task.FromResult(user);
            //return _collectionName.AsQueryable<User>().Where(p => p.ProductChilds.Any(c=> c.confirmed == confirmed )) ; // Select(w => w.ProductChilds. == key);
        }
     
    }        
    
}