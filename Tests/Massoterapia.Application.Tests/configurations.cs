using System.Globalization;
using System;
using System.Collections.Generic;
using AutoMapper;
using Massoterapia.Application.Patient.Models;
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

        public static Mock<IPatientRepository> mockPatientRepository = new Mock<IPatientRepository>();

        public static Mock<IUserRepository> FakeUserRepository()
        {
            mockUserRepository.Setup(repo => repo.QueryByNamePasswordHash("nome usuario")).ReturnsAsync(UserReturns) ;

            return mockUserRepository;
            
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


        public static IMapper FakeMapper()
        {
            var myProfile = new Massoterapia.Application.user.Mappings.UserDomainToUserTobeCreatedMappingProfile();

            var patientDomainToPatientViewModelListMappingProfile = new Massoterapia.Application.Patient.Mappings.PatientDomainToPatientViewModelListMappingProfile();

            List<Profile> profiles  = new List<Profile>();
            profiles.Add(myProfile);
            profiles.Add(patientDomainToPatientViewModelListMappingProfile);

            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(profiles));
            

            return configuration.CreateMapper();  //new Mapper(configuration);
        }        
        
    }
}