using Massoterapia.Infra.Data.Mongo.Context;
using Massoterapia.Domain;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Infra.Data.Mongo.Repositories;
using Massoterapia.Application.user.models;
using AutoMapper;
using Massoterapia.Application.user;
using Xunit;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization;
using Massoterapia.Domain.Entities;
using System;
using Massoterapia.Application.user.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using Massoterapia.Infra.IoC.Settings;

namespace Massoterapia.Application.Tests
{
    public class userRepositoryTests
    {

       
        IUserRepository userRepository;

        UserInputModel userInputModel;

        public userRepositoryTests()
        {

            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(@"J:\Desenvolvimento\Projetos\Massoterapia\MassoterapiaAPI\Massoterapia.API\appsettings.Development.json")
            .Build();

            var mongoDbSettings = config.GetSection("MongoDBSetting").Get<MongoDBSetting>();            

            Massoterapia.Infra.Data.Mongo.Configurations.GlobalConfigurations.ConventionPack_IgnoreExtraElements();

            Massoterapia.Infra.Data.Mongo.Configurations.UserConfiguration.UserMapping();

            var connectionFactory = new ConnectionFactory(mongoDbSettings.ConnectionString);     
            userRepository = new  UserRepository(connectionFactory, mongoDbSettings.DatabaseName, mongoDbSettings.CollectionName);

            userInputModel = new UserInputModel{
                Name="nome usuario",
                Password_Text = "password",
                NumberSalt = 70,
                Interation = 10101,
                Nhash = 70
                };            

        }


        [Fact]
        public void RepositoryReal_ceate_user_corrected()
        {

             var userService = new UserService(configurations.FakeMapper(),userRepository);

            userService.CreateUser(userInputModel);

        }


        [Fact]
        public void RepositoryReal_ceate_user_error()
        {

            var userInputModel_no_password = new UserInputModel{
                Name="nome usuario",
                Password_Text = "",
                NumberSalt = 70,
                Interation = 10101,
                Nhash = 70
                };

            var userService = new UserService(configurations.FakeMapper(),userRepository);

            Assert.Throws<Exception> (()=> userService.CreateUser(userInputModel_no_password));

        }        
    

        [Fact]
        public void RepositoryReal_authenticate_user_corrected()
        {

            var userService = new UserService(configurations.FakeMapper(),userRepository);

            var result = userService.Authenticate(userInputModel).Result;

            Assert.True(result);

        }



        [Fact]
        public void RepositoryReal_authenticate_user_fail()
        {

            var userInputModel_password_wrong = new UserInputModel{
                Name="nome usuario",
                Password_Text = "passworD",
                NumberSalt = 70,
                Interation = 10101,
                Nhash = 70
                };

            var userService = new UserService(configurations.FakeMapper(),userRepository);

            var result = userService.Authenticate(userInputModel_password_wrong).Result;

            Assert.False(result);
        }
    }
}