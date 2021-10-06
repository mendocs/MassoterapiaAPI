using System.Threading.Tasks;
using MongoDB.Driver;
using SharedCore.Entities;
using Massoterapia.Infra.Data.Mongo.Context;
using Massoterapia.Infra.Data.Mongo.Interfaces;
using System;
using System.Linq;
using SharedCore.Repositories;

namespace Massoterapia.Infra.Data.Mongo.Base
{
    public abstract class RepositoryRead<T> : IRepositoryRead<T> where T : IEntity
    {
        public readonly IMongoCollection<T> _collectionName;

        protected RepositoryRead(IMongoCollection<T> collectionName)
        {
            _collectionName = collectionName;
        }

        protected RepositoryRead(IConnectionFactory connectionFactory, string databaseName, string collectionName)
        {
            _collectionName = connectionFactory.GetDatabase(databaseName).GetCollection<T>(collectionName);
        }

        public Task<IQueryable<T>> QueryAll()
        {
            return Task.FromResult(_collectionName.AsQueryable<T>().AsQueryable());
        }

        public Task<T> Query(Guid key)
        {
            return Task.FromResult(_collectionName.AsQueryable<T>().FirstOrDefault(w => w.Key == key));
        }



    }
}