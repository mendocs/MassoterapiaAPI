using AutoMapper;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Interfaces;
using Moq;

namespace Massoterapia.Application.Tests
{
    public static class configurations
    {

            //register from fatabase
        public static User UserReturns =  new Massoterapia.Domain.Entities.User("nome usuario","password",
                "kfrMv4vktPT7tE8kn8X1hOz5qyA91tlpG+YiRXPasNeod46scZV5IPJe6EffAtTCpKoYgPDFYuhwBUxYNyg1UZWiCwY/+g==",
                    "RGgUouEEMzCHs06d1jAe/X1L9M2WqJAsbI+Ad/+DLfd5/8rbAgob1oQOtxswFjJ4PfAJkdBpnPEszI+qcFvJoLRoul9Vcw==",70,10101,70);


        public static Mock<IUserRepository> mockUserRepository = new Mock<IUserRepository>();

        public static Mock<IUserRepository> FakeUserRepository()
        {
            mockUserRepository.Setup(repo => repo.QueryByNamePasswordHash("nome usuario")).ReturnsAsync(UserReturns) ;

            return mockUserRepository;
            
        }



        public static IMapper FakeMapper()
        {
            var myProfile = new Massoterapia.Application.user.Mappings.UserDomainToUserTobeCreatedMappingProfile();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

            return configuration.CreateMapper();  //new Mapper(configuration);
        }        
        
    }
}