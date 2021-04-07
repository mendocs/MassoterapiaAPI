using System.Threading.Tasks;
using Massoterapia.Application.user.models;

namespace Massoterapia.Application.user.Interfaces
{
    public interface IUserService
    {
         void CreateUser(UserInputModel userInputModel);

        Task<bool> Authenticate(UserInputModel userInputModel);


    }
}