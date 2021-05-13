using System;
using System.Linq;
using Massoterapia.Domain.Entities;

namespace Massoterapia.Infra.Data.Mongo.Interfaces
{
    public interface IRepository_old<T>
    {
        IQueryable<T> QueryAll();

        T Query(Guid key);
        void Insert(T obj);

        long Update (T obj);

    }
}