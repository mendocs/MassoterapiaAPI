using System;
using System.Collections.Generic;
using Massoterapia.Application.Patient.Interfaces;
using Massoterapia.Application.Patient.Models;
using Massoterapia.Application.Patient.Services;
using Xunit;

namespace Massoterapia.Application.Tests
{
    public class PatientServicesTests
    {

        IPatientService patientService;
        public PatientServicesTests()
        {
            this.patientService = new PatientService(configurations.FakeMapper(),configurations.FakePatientRepository().Object);
        }

        [Fact]
        public void create_patient_corrected()
        {

            var patienteInputModel = configurations.GetUserInputModelCriation();

            var result = patientService.CreatePatient(patienteInputModel);

            //retorna array de pacientess. na criação tem que retornar apenas 1
            Assert.Equal(1,result.Result.Count);

        }   

        [Fact]
        public void create_patient_incorrected_schedule_date_1_day_before()
        {

            var patienteInputModel = configurations.GetUserInputModelCriation();
            patienteInputModel.Scheduletime = patienteInputModel.Scheduletime.AddDays(-2);

                       
            var ex = Assert.ThrowsAsync<ArgumentException> (()=> patientService.CreatePatient(patienteInputModel));

            Assert.Contains("não pode ser menor", ex.Result.Message );

        }


        [Fact]
        public void create_patient_incorrected_schedule_existent()
        {

            var patienteInputModel = new PatientInputModel();
            patienteInputModel.Name = "name teste 14";
            patienteInputModel.Phone = "11909998888";
            patienteInputModel.Scheduletime = new DateTime(2021,05,11,10,0,0);
                       
            var ex = Assert.ThrowsAsync<ArgumentException> (()=> patientService.CreatePatient(patienteInputModel));

            Assert.Contains("existe atendimento neste", ex.Result.Message );

        }        



        [Fact]
        public void create_patient_existent_returns_patient_Existent()
        {
            var patienteInputModel = configurations.GetUserInputModelCriation();

            patienteInputModel.Name = "usuario";
            patienteInputModel.Phone = "11998877666";

            IList<PatientViewModelList> PatientList = patientService.CreatePatient(patienteInputModel).Result;

            Assert.Equal("11998877666", PatientList[0].Phone );
        }

        [Fact]
        public void create_patient_inexistent_returns_new_patient()
        {
            var patienteInputModel = configurations.GetUserInputModelCriation();

            patienteInputModel.Name = "usuario1";
            patienteInputModel.Phone = "11998877665";

            IList<PatientViewModelList> PatientList = patientService.CreatePatient(patienteInputModel).Result;

            Assert.Equal("11998887777", PatientList[0].Phone );
        }

        [Fact]
        public void query_patient_corrected()
        {
            Guid blogGuid = new Guid("ca3aa909-c935-4c9f-b304-7d744dbe050e");

            Domain.Entities.Patient PatientList = patientService.SearchByKey(blogGuid).Result;

            Assert.Equal("nome teste criar", PatientList.Name );
        }

        [Fact]
        public void query_patient_fail()
        {
            Guid blogGuid = new Guid();

            Domain.Entities.Patient PatientList = patientService.SearchByKey(blogGuid).Result;

            Assert.Null(PatientList );
        }        


    }
}