using System.Linq;
using System.Globalization;
using System;
using System.Collections.Generic;
using AutoMapper;
using Massoterapia.Application.Patient.Models;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Interfaces;
using Moq;
using Massoterapia.Application.Blog.Models;

namespace Massoterapia.Application.Tests
{
    public static class configurations
    {

            //register from fatabase
        public static User UserReturns =  new Massoterapia.Domain.Entities.User("nome usuario","password",
                "kfrMv4vktPT7tE8kn8X1hOz5qyA91tlpG+YiRXPasNeod46scZV5IPJe6EffAtTCpKoYgPDFYuhwBUxYNyg1UZWiCwY/+g==",
                    "RGgUouEEMzCHs06d1jAe/X1L9M2WqJAsbI+Ad/+DLfd5/8rbAgob1oQOtxswFjJ4PfAJkdBpnPEszI+qcFvJoLRoul9Vcw==",70,10101,70);



        public static Mock<IBlogRepository> mockBlogRepository = new Mock<IBlogRepository>();
        public static Mock<IUserRepository> mockUserRepository = new Mock<IUserRepository>();

        public static Mock<IPatientRepository> mockPatientRepository = new Mock<IPatientRepository>();

        public static Mock<IUserRepository> FakeUserRepository()
        {
            mockUserRepository.Setup(repo => repo.QueryByNamePasswordHash("nome usuario")).ReturnsAsync(UserReturns) ;

            return mockUserRepository;
            
        }

        public static Mock<IBlogRepository> FakeBlogRepository()
        {
            var blogFake = GetBlogDomainFake();

            List<Domain.Entities.Blog> ListBlog = new List<Domain.Entities.Blog>();
            ListBlog.Add(blogFake);
            ListBlog.Add(blogFake);
            ListBlog.Add(blogFake);
            
            IQueryable<Domain.Entities.Blog> IQueryableBlog_true = ListBlog.ToList().AsQueryable(); 

            ListBlog.RemoveAt(0);
            IQueryable<Domain.Entities.Blog> IQueryableBlog_false = ListBlog.AsQueryable(); 
            


            Guid blogGuid = new Guid("ca3aa909-c935-4c9f-b304-7d744dbe050e");

            mockBlogRepository.Setup(repo => repo.Insert(It.IsAny<Domain.Entities.Blog>())).ReturnsAsync(blogFake) ;
            mockBlogRepository.Setup(repo => repo.Update(It.IsAny<Domain.Entities.Blog>())).ReturnsAsync(1) ;
            mockBlogRepository.Setup(repo => repo.Query(blogGuid)).ReturnsAsync(blogFake);
            mockBlogRepository.Setup(repo => repo.QueryByTitleNFD("titlenfd")).ReturnsAsync(blogFake);

            mockBlogRepository.Setup(repo => repo.QueryAll(true)).ReturnsAsync(IQueryableBlog_true);
            mockBlogRepository.Setup(repo => repo.QueryAll(false)).ReturnsAsync(IQueryableBlog_false);

            

            return mockBlogRepository;
            
        }


        public static Mock<IPatientRepository> FakePatientRepository()
        {

            Massoterapia.Domain.Tests.PatientTests patientTests = new Domain.Tests.PatientTests();


            Domain.Entities.Patient patientCreatedFakefromDB = patientTests.loadPatientFromFile("patientFull_Create_fromDB.json","","") ;
            Domain.Entities.Patient patientFakefromDB = patientTests.loadPatientFromFile("patientFull_fromDB.json","","") ;

            var patientViewModel = configurations.GetUserInputModelCriation();

            Domain.Entities.Patient patientTobeSaved = new Domain.Entities.Patient(
                patientViewModel.Name,
                patientViewModel.Phone,
                patientViewModel.Scheduletime
            );            

            mockPatientRepository.Setup(repo => repo.Insert( It.IsAny<Domain.Entities.Patient>() )).ReturnsAsync(patientCreatedFakefromDB) ;

            IList<Domain.Entities.Patient> PatientList = new List<Domain.Entities.Patient>();
            
            patientFakefromDB.SetNamePhone("usuario","11998877666");
            PatientList.Add(patientFakefromDB);

            mockPatientRepository.Setup(repo => repo.QueryByNameOrPhone("usuario","11998877666")).ReturnsAsync(PatientList) ;

            List<DateTime> dateRange = new List<DateTime>();
            dateRange.Add(new DateTime(2021,05,11,09,10,0) );
            dateRange.Add(new DateTime(2021,05,11,10,50,0) );

            mockPatientRepository.Setup(repo => repo.QueryLikeNamePhoneScheduledateRange(null,null,dateRange)).ReturnsAsync(PatientList) ;

            Guid blogGuid = new Guid("ca3aa909-c935-4c9f-b304-7d744dbe050e");
            mockPatientRepository.Setup(repo => repo.Query(blogGuid)).ReturnsAsync(patientTobeSaved) ;
            
            
            return mockPatientRepository;
            
        }


        public static PatientInputModel GetUserInputModelCriation()
        {
            var patientInputModel = new PatientInputModel();

            patientInputModel.Name = "nome teste criar";
            patientInputModel.Phone = "11999887776";
            patientInputModel.Scheduletime = DateTime.Now.AddDays(1);

            return patientInputModel;

        }


        public static Domain.Entities.Blog GetBlogDomainFake()
        {
            return new Domain.Entities.Blog("title","titlenfd","imagecard","tags","text");
        }

        public static BlogInputModel GetBlogInputModel()
        {
            var blogInputModel = new BlogInputModel
            {
                Title = "title",
                Text = "text",
                ImageCard = "imagecard",
                Active = true,
                Tags = "tags",
                TitleNFD = "titlenfd" 
            };            

            return blogInputModel;
        }

        public static IMapper FakeMapper()
        {
            var myProfile = new Massoterapia.Application.user.Mappings.UserDomainToUserTobeCreatedMappingProfile();

            var patientDomainToPatientViewModelListMappingProfile = new Massoterapia.Application.Patient.Mappings.PatientDomainToPatientViewModelListMappingProfile();

            var BlogDomainToBlogViewModelMappingProfile = new Massoterapia.Application.Blog.Mappings.BlogDomainToBlogViewModelMappingProfile();

            var blogInputModelToBlogDomainMappingProfile = new Massoterapia.Application.Blog.Mappings.BlogInputModelToBlogDomainMappingProfile();

            List<Profile> profiles  = new List<Profile>();
            profiles.Add(myProfile);
            profiles.Add(patientDomainToPatientViewModelListMappingProfile);
            profiles.Add(BlogDomainToBlogViewModelMappingProfile);
            profiles.Add(blogInputModelToBlogDomainMappingProfile);
            

            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(profiles));
            

            return configuration.CreateMapper();  //new Mapper(configuration);
        }        
        
    }
}