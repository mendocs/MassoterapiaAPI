using System.Threading.Tasks;
using Massoterapia.Infra.Data.Mongo.Context;
using MongoDB.Driver;
using SharedCore.Entities;
using SharedCore.Repositories;
using System.Linq;


namespace Massoterapia.Infra.Data.Mongo.Base
{
    public abstract class RepositoryWrite<T> : RepositoryRead<T>,  IRepositoryWrite<T> where T : IEntity
    {
        protected RepositoryWrite(IMongoCollection<T> collectionName): base(collectionName){}

        protected RepositoryWrite(IConnectionFactory connectionFactory, string databaseName, string collectionName)
        : base (connectionFactory, databaseName,  collectionName){}

        public Task<long> Update(T obj)
        {

            obj.SetUpdate();

            var filter = Builders<T>.Filter.Eq(s => s.Key, obj.Key);

            var result = _collectionName.ReplaceOne(filter, obj);

            return Task.FromResult(result.ModifiedCount);
                
        }

        public Task<T> Insert(T obj)
        {
            _collectionName.InsertOne(obj);

            T entity = this.Query(obj.Key).Result; 

            return Task.FromResult(entity);
        }

        public Task Delete(T obj)
        {
            throw new System.NotImplementedException();
        }
    }
}