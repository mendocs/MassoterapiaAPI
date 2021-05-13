using System;
using System.Collections;
using System.Collections.Generic;
using Massoterapia.Application.Patient.Interfaces;
using Massoterapia.Application.Patient.Models;
using Massoterapia.Application.Patient.Services;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Domain.Tests;
using Massoterapia.Infra.Data.Mongo.Repositories;
using Xunit;

namespace Massoterapia.Application.Tests
{

    

    public class PatientRepositoryTests
    {
        IPatientRepository patientRepository;
        IPatientService patientService;

        public PatientRepositoryTests()
        {
            this.patientRepository = new  PatientRepository(RepositoryConfiguration.ConnFactory(), RepositoryConfiguration.DatabaseName, RepositoryConfiguration.CollectionPatient);

            this.patientService = new PatientService(configurations.FakeMapper(),this.patientRepository);
            
        }


        [Fact]
        public void RepositoryReal_ceate_patient_corrected()
        {
            PatientInputModel patientImputModel = configurations.GetUserInputModelCriation();
            patientImputModel.Name = "name create 3";
            patientImputModel.Phone = "11998007766";

            IList<PatientViewModelList> patientSaved = this.patientService.CreatePatient(patientImputModel).Result;

            Assert.Equal(1,patientSaved.Count);
        }


        [Fact]
        public void RepositoryReal_serch_patient_by_nome_phone_corrected()
        {
            PatientInputModel patientImputModel = configurations.GetUserInputModelCriation();
            IList<PatientViewModelList> patientSaved = this.patientService.SearchForCreate(patientImputModel).Result;

            Assert.Equal(1,patientSaved.Count);
        }


        [Fact]
        public void RepositoryReal_serch_patient_by_nome_phone_not_found()
        {
            PatientInputModel patientImputModel = configurations.GetUserInputModelCriation();

            patientImputModel.Name = "nome não existente";
            patientImputModel.Phone = "11999994444";

            IList<PatientViewModelList> patientSaved = this.patientService.SearchForCreate(patientImputModel).Result;

            Assert.Equal(0,patientSaved.Count);
        }
        

        [Fact]
        public void RepositoryReal_serchForCreate_patient_only_nome_equal_found()
        {
            PatientInputModel patientImputModel = configurations.GetUserInputModelCriation();

            //nome se mantem e telefone mudado
            patientImputModel.Phone = "11999994444";

            IList<PatientViewModelList> patientSaved = this.patientService.SearchForCreate(patientImputModel).Result;

            Assert.Equal(1,patientSaved.Count);
        }

        [Fact]
        public void RepositoryReal_serchForCreate_patient_only_phone_equal_found()
        {
            PatientInputModel patientImputModel = configurations.GetUserInputModelCriation();

            //nome se altera e telefone se manter
            patientImputModel.Name = "nome não existente";

            IList<PatientViewModelList> patientSaved = this.patientService.SearchForCreate(patientImputModel).Result;

            Assert.Equal(1,patientSaved.Count);
        }



        [Fact]
        public void RepositoryReal_query_patient_only_name_parcial_found()
        {
            PatientInputModel patientImputModel = new PatientInputModel {
                Name = "nome test"
            };

            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.Equal(3,patientSearched.Count);
        }

        [Fact]
        public void RepositoryReal_query_patient_only_phone_parcial_found()
        {
            PatientInputModel patientImputModel = new PatientInputModel {
                Phone = "11999"
            };

            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.True(patientSearched?[0].Phone.Contains("11999"));
        }        


        [Fact]
        public void RepositoryReal_query_patient_date_range_found_1()
        {
            PatientInputModel patientImputModel = new PatientInputModel () ;
                patientImputModel.ScheduledateRange.Add(new System.DateTime(2021,04,27));
                patientImputModel.ScheduledateRange.Add(new System.DateTime(2021,04,28));
            
            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.Equal(1,patientSearched?.Count);
        }


        [Fact]
        public void RepositoryReal_query_patient_date_range_found_2()
        {
            PatientInputModel patientImputModel = new PatientInputModel () ;
                patientImputModel.ScheduledateRange.Add(new System.DateTime(2021,04,28));
                patientImputModel.ScheduledateRange.Add(new System.DateTime(2021,04,29,0,2,0));

            var strrr = patientImputModel.ScheduledateRange[1].ToUniversalTime();

           // DateTime ttt = DateTime.Parse(strrr);

            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.Equal(3,patientSearched?.Count);
        }


        [Theory]
        [InlineData("nome teste criar 2    ","",1)]
        [InlineData("nome teste ","",2)]
        [InlineData("nome teste criar 2    ","11999887775",1)]
        [InlineData("nome teste ","11999887776",1)]     
        [InlineData("nome teste ","1199988",2)]
        [InlineData("","11999887775",1)]
        [InlineData("","11999887776",1)]
        [InlineData("","11999886666",0)]
        [InlineData("nome sem registro","",0)]
        public void RepositoryReal_query_patient_name_and_date_range(string name, string phone, int records)
        {
            PatientInputModel patientImputModel = new PatientInputModel () 
            {
                Name= name,
                Phone = phone
            } ;
                patientImputModel.ScheduledateRange.Add(new System.DateTime(2021,04,27));
                patientImputModel.ScheduledateRange.Add(new System.DateTime(2021,04,30,0,2,0));

            var strrr = patientImputModel.ScheduledateRange[1].ToUniversalTime();

           // DateTime ttt = DateTime.Parse(strrr);

            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.Equal(patientSearched?.Count,records);
        }        


        [Fact]
        public void RepositoryReal_query_patient_only_name_parcial_not_found()
        {
            PatientInputModel patientImputModel = new PatientInputModel {
                Name = "nome _texto não deve ser encontrado"
            };

            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.Equal(0,patientSearched.Count);
        }


        [Fact]
        public void RepositoryReal_query_patient_only_phone_parcial_not_found()
        {
            PatientInputModel patientImputModel = new PatientInputModel {
                Phone = "1199448875"
            };

            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.Equal(0,patientSearched.Count);
        }


        [Fact]
        public void RepositoryReal_query_patient_date_range_not_found()
        {
            PatientInputModel patientImputModel = new PatientInputModel () ;
                patientImputModel.ScheduledateRange.Add(new System.DateTime(2021,04,20));
                patientImputModel.ScheduledateRange.Add(new System.DateTime(2021,04,21));

            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.Equal(0,patientSearched.Count);
        }

        [Fact]
        public void RepositoryReal_update_patient_corrected()
        {

            PatientTests patientTests = new PatientTests();

            //Domain.Entities.Patient patientToBeUpdate = patientTests.loadPatientFromFile("patientFullCreate.json","","");
            Domain.Entities.Patient patientToBeUpdate = patientTests.loadPatientFromFile("PatientFull_real_updated_fromDB.json","","");

            var patientUpdated = this.patientService.UpdatePatient(patientToBeUpdate).Result;

            Assert.Equal(1,patientUpdated);
        }

        [Fact]
        public void RepositoryReal_Search_by_key_patient_corrected()
        {

            PatientTests patientTests = new PatientTests();

            Domain.Entities.Patient patientToBeSearched = patientTests.loadPatientFromFile("PatientFull_real_updated_fromDB.json","","");

            Domain.Entities.Patient patientFound = this.patientService.SearchByKey(patientToBeSearched.Key).Result;

            var patientSerialized = patientTests.SerializePatient(patientFound);

            Assert.Equal(patientToBeSearched.Key , patientFound.Key);
        }



    }
}