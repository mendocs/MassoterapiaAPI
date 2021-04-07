using System.Threading.Tasks;
using AutoMapper;
using Massoterapia.Application.user.Interfaces;
using Massoterapia.Application.user.models;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Domain.Validation;
using Massoterapia.Domain.Validations;


namespace Massoterapia.Application.user.Services
{
    public class UserService : IUserService
    {

        private IUserRepository UserRepository; 
        private readonly IMapper _mapper;

        public UserService(IMapper mapper, IUserRepository userRepository )
        {
            this.UserRepository = userRepository;
            this._mapper = mapper;
        }

        public async Task<bool> Authenticate(UserInputModel userInputModel)
        {
            User user = _mapper.Map<User> (userInputModel);
            User UserDatabase =  await this.UserRepository.QueryByNamePasswordHash(user.Name);

            if (UserDatabase == null)
                return false;

            user.SetSalt (UserDatabase.Salt);

            return (user.getHashPassword() == UserDatabase.Password_Hash);            
        }

        public void CreateUser(UserInputModel userInputModel)
        {
            User user = _mapper.Map<User> (userInputModel);

            user.SetHashes();

            UserValidationContract userValidationContract = new UserValidationContract(user);

            if ( !userValidationContract.IsValid )
                throw new System.Exception(userValidationContract.Notifications.AllInvalidations());
            
            this.UserRepository.Insert(user);
        }
    }
}