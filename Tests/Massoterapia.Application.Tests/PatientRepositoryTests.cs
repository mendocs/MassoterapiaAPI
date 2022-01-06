using System;
using System.Collections;
using System.Collections.Generic;
using Massoterapia.Application.Patient.Interfaces;
using Massoterapia.Application.Patient.Models;
using Massoterapia.Application.Patient.Services;
using Massoterapia.Domain.Entities;
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
            patientImputModel.Name = "name 112";
            patientImputModel.Phone = "11598487760";
            patientImputModel.ScheduleData = new Schedule(DateTime.Now.AddDays(1),false,"",false,30,DateTime.Now.AddDays(1),false,"escalda pés","Pacote Relaxar 1",100,1);

            IList<PatientViewModelList> patientSaved = this.patientService.CreatePatient(patientImputModel).Result;

            Assert.Equal(1,patientSaved.Count);
        }


        [Fact]
        public void QueryScheduledateRangeForScheduleFree_Start_ocupied()
        {
            IList<DateTime> ScheduledateRange = new List<DateTime>();

            ScheduledateRange.Add(new DateTime(2021,05,20,18,40,0,DateTimeKind.Utc));
            ScheduledateRange.Add(ScheduledateRange[0].AddMinutes(50));

            IList<Massoterapia.Domain.Entities.Patient> result = this.patientRepository.QueryScheduledateRangeForScheduleFree(ScheduledateRange).Result;

            Assert.Equal(14,result.Count);
        }

        [Fact]
        public void QueryScheduledateRangeForScheduleFree_end_ocupied()
        {
            IList<DateTime> ScheduledateRange = new List<DateTime>();

            ScheduledateRange.Add(new DateTime(2021,05,20,18,0,0,DateTimeKind.Utc));
            ScheduledateRange.Add(ScheduledateRange[0].AddMinutes(50));

            IList<Massoterapia.Domain.Entities.Patient> result = this.patientRepository.QueryScheduledateRangeForScheduleFree(ScheduledateRange).Result;

            Assert.Equal(14,result.Count);
        }


       [Fact]
        public void QueryScheduledateRangeForScheduleFree_not_ocupied()
        {
            IList<DateTime> ScheduledateRange = new List<DateTime>();

            ScheduledateRange.Add(new DateTime(2021,05,20,17,20,0,DateTimeKind.Utc));
            ScheduledateRange.Add(ScheduledateRange[0].AddMinutes(50));

            IList<Massoterapia.Domain.Entities.Patient> result = this.patientRepository.QueryScheduledateRangeForScheduleFree(ScheduledateRange).Result;

            Assert.Equal(0,result.Count);
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
                Name = "name create 09"
            };

            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.Equal(4,patientSearched.Count);
        }

        [Fact]
        public void RepositoryReal_query_all_patient_no_filter()
        {
            PatientInputModel patientImputModel = new PatientInputModel {};

            IList<PatientViewModelList> patientSearched = this.patientService.SearchByLikeNamePhoneScheduleDateRange (patientImputModel).Result;

            Assert.NotEqual(0,patientSearched.Count);
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


        [Fact]
        public void update_corrected()
        {

            PatientTests patientTests = new PatientTests();

            Domain.Entities.Patient patientFound = this.patientService.SearchByKey(new Guid("eef2220b-160a-4547-b5b5-51746477cbef")).Result;

            patientFound.Schedules[0].SetStartdDate(new System.DateTime(2021,12,10,13,00,0,DateTimeKind.Utc));
            patientFound.Schedules[0].SetDuration(40);
        

            var patientUpdated = this.patientService.UpdatePatient(patientFound).Result;

            Assert.Equal(1,patientUpdated);
        }


        [Fact]
        public void update_name_corrected()
        {
            PatientTests patientTests = new PatientTests();

            Domain.Entities.Patient patientFound = this.patientService.SearchByKey(new Guid("08c9895d-d2ff-4720-8d09-5e98a0fe928f")).Result;

            patientFound.SetNamePhone("name 110 _", patientFound.Phone);

            var patientUpdated = this.patientService.UpdatePatient(patientFound).Result;

            Assert.Equal(1,patientUpdated);
        }

        [Fact]
        public void update_name_already_used()
        {
            PatientTests patientTests = new PatientTests();

            Domain.Entities.Patient patientFound = this.patientService.SearchByKey(new Guid("08c9895d-d2ff-4720-8d09-5e98a0fe928f")).Result;

            patientFound.SetNamePhone("name 109", patientFound.Phone);

            var ex = Assert.Throws<Exception> (()=> this.patientService.UpdatePatient(patientFound).Result);

            Assert.Contains("já existe registro com este nome", ex.Message );

        }


        [Fact]
        public void update_schedule_modified_startDate_ocuppied()
        {

            PatientTests patientTests = new PatientTests();

            Domain.Entities.Patient patientFound = this.patientService.SearchByKey(new Guid("08c9895d-d2ff-4720-8d09-5e98a0fe928f")).Result;
            patientFound.Schedules[0].SetStartdDate(new DateTime(2021,12,25,22,30,00,DateTimeKind.Utc));
            //patientFound.Schedules[0].SetDuration(51);

            //var patientUpdated = this.patientService.UpdatePatient(patientFound).Result;
            var ex = Assert.Throws<Exception> (()=> this.patientService.UpdatePatient(patientFound).Result);

            Assert.Contains("existe atendimento neste horário", ex.Message );
        }


        [Fact]
        public void update_schedule_changed_corrected()
        {

            PatientTests patientTests = new PatientTests();

            Domain.Entities.Patient patientFound = this.patientService.SearchByKey(new Guid("08c9895d-d2ff-4720-8d09-5e98a0fe928f")).Result;


            DateTime startdDate = patientFound.Schedules[0].StartdDate.AddMinutes(5);
            patientFound.Schedules[0].SetStartdDate(startdDate);
            //patientFound.Schedules[0].SetDuration(patientFound.Schedules[0].Duration + 1);
        

            var patientUpdated = this.patientService.UpdatePatient(patientFound).Result;

            Assert.Equal(1,patientUpdated);
        }        


    }
}