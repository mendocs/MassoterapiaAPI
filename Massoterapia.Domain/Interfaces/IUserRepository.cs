using System.Threading.Tasks;
using Massoterapia.Domain.Entities;
using SharedCore.Repositories;

namespace Massoterapia.Domain.Interfaces
{
    public interface IUserRepository : IRepositoryWrite<User>
    {
         Task<User> QueryByNamePasswordHash(string name);
    }
}