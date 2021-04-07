using System.Linq;
using System.Threading.Tasks;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Infra.Data.Mongo.Base;
using Massoterapia.Infra.Data.Mongo.Context;
using MongoDB.Driver;

namespace Massoterapia.Infra.Data.Mongo.Repositories
{
    public class UserRepository: Repository<User>, IUserRepository
    {
        public UserRepository(IMongoCollection<User> collectionName) : base(collectionName)
        {
        }

        public UserRepository(IConnectionFactory connectionFactory, string databaseName, string collectionName)
            : base(connectionFactory, databaseName, collectionName)
        {
        }

        public void delete(User obj)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> QueryByNamePasswordHash(string name)
        {
            User user = _collectionName.AsQueryable<User>().FirstOrDefault(w => w.Name == name);
            return Task.FromResult(user);
            //return _collectionName.AsQueryable<User>().Where(p => p.ProductChilds.Any(c=> c.confirmed == confirmed )) ; // Select(w => w.ProductChilds. == key);
        }

        public void update(User obj)
        {
            throw new System.NotImplementedException();
        }
    }        
    
}