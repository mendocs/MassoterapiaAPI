using System.Linq;

namespace Massoterapia.Infra.Data.Mongo.Interfaces
{
    public interface IProductRepository<T> : IRepository<T>
    {
        IQueryable<T> QuerybydateSchedule(bool confirmed);
        
         
    }
}