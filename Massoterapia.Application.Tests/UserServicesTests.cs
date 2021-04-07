using System.Threading.Tasks;
using System;
using AutoMapper;
using Xunit;
using Massoterapia.Application.user.models;
using Massoterapia.Application.user;
using Moq;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Domain.Entities;
using Massoterapia.Application.user.Services;

namespace Massoterapia.Application.Tests
{
    public class UserServicestests
    {
 

        [Fact]
        public void Authenticate_corrected()
        {


            var userInputModel = new UserInputModel{
                Name="nome usuario",
                Password_Text = "password",
                NumberSalt = 70,
                Interation = 10101,
                Nhash = 70
                };

            var userService = new UserService(configurations.FakeMapper(),configurations.FakeUserRepository().Object);

            var result = userService.Authenticate(userInputModel).Result;

            Assert.True(result);

        }
    
        [Fact]
        public void Authenticate_passord_incorrected()
        {

            var userInputModel = new UserInputModel{
                Name="nome usuario",
                Password_Text = "password1",
                NumberSalt = 70,
                Interation = 10101,
                Nhash = 70
                };

            var userService = new UserService(configurations.FakeMapper(), configurations.FakeUserRepository().Object);

            var result = userService.Authenticate(userInputModel).Result;

            Assert.False(result);

        }      


        [Fact]
        public void Create_corrected()
        {


            var userInputModel = new UserInputModel{
                Name="nome usuario",
                Password_Text = "password1",
                NumberSalt = 70,
                Interation = 10101,
                Nhash = 70
                };

             configurations.mockUserRepository.Setup(repo => repo.Insert(configurations.UserReturns));
            
            var userService = new UserService(configurations.FakeMapper(),configurations.mockUserRepository.Object);

            userService.CreateUser(userInputModel);
        } 


        [Fact]
        public void Create_fail()
        {


            var userInputModel = new UserInputModel{
                Name="nome usuario",
                Password_Text = "",
                NumberSalt = 70,
                Interation = 10101,
                Nhash = 70
                };

            //mockRepository.Setup(repo => repo.Insert(UserReturns));            
            var userService = new UserService(configurations.FakeMapper(),configurations.FakeUserRepository().Object);

            Assert.Throws<Exception> (()=> userService.CreateUser(userInputModel));
        }         
    }
}
