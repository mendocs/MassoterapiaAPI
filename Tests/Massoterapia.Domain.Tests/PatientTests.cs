using System.IO;
using System.Net.Mime;
using System.Reflection;
using System;
using Xunit;
using Massoterapia.Domain.Entities;
using Newtonsoft.Json;
using JsonNet.ContractResolvers;
using Newtonsoft.Json.Linq;
using Massoterapia.Domain.Validations;
using System.Collections.Generic;

namespace Massoterapia.Domain.Tests
{
    public class PatientTests
    {

        public Schedule GetSchedule (DateTime StartDate, int duration)
        {
            return new Schedule(StartDate,false,"",false,duration,StartDate,false,"","",0,0);
        }

        [Fact]
        public void patient_confirmed_when_created()
        {
            
            var patient = new  Patient("name","phone", this.GetSchedule(DateTime.Now.AddDays(1),50));            

            Assert.True(patient.Schedules[0].Confirmed);

        }

        [Fact]
        public void patient_not_confirmed_when_created()
        {
            var patient = new  Patient("name","phone", this.GetSchedule(DateTime.Now.AddDays(3),50));            

            Assert.False(patient.Schedules[0].Confirmed);

        }

        [Fact]
        public void patient_contract_valid()
        {
            var patient = new  Patient("name","11998886681", this.GetSchedule(DateTime.Now.AddDays(3),50));            

            PatientValidationContract patientValidationContract = new PatientValidationContract(patient);

            Assert.True(patientValidationContract.IsValid);

        }

        [Fact]
        public void patient_no_properties_contract_not_valid()
        {
            var patient = new  Patient();            

            PatientValidationContract patientValidationContract = new PatientValidationContract(patient);

            Assert.False(patientValidationContract.IsValid);

        }


        [Theory]
        [InlineData("1199888668")] //10 caracteres
        [InlineData("119988866800")] //12 caracteres
        public void patient_phone_short_invalid_contract_not_valid(string phone)
        {
            var patient = new  Patient("usuario", phone,this.GetSchedule(DateTime.Now.AddDays(1),50));
                  
            PatientValidationContract patientValidationContract = new PatientValidationContract(patient);

            Assert.Contains("telefone tem que ter 11 caracteres",patientValidationContract.Notifications.AllInvalidations());

        }        

        public IEnumerable<(Guid,string)> SearchScheduleDateTimeFree(DateTime startDate, int duration) 
        { 
            return new List<(Guid,string)>();;
        }



        [Fact]
        public void patient_whit_schedule_negative_not_can_created()
        {
            var patient = new  Patient("name","phone", this.GetSchedule(DateTime.Now.AddDays(-3),50));   

             
            var retorno = patient.SchedulesIsValid(patient.Schedules, this.SearchScheduleDateTimeFree);
            var mensagns =patient.GetScheduleNotifications();

            PatientValidationContract patientValidationContract = new PatientValidationContract(patient);

            Assert.False(retorno);

        }


        [Fact]
        public void patient_whit_schedule_valid()
        {
            var patient = new  Patient("name","phone", this.GetSchedule(DateTime.Now.AddDays(3),50));   

             
            var retorno = patient.SchedulesIsValid(patient.Schedules, this.SearchScheduleDateTimeFree);
            var mensagns =patient.GetScheduleNotifications();

            PatientValidationContract patientValidationContract = new PatientValidationContract(patient);

            Assert.True(retorno);

        }

        [Fact]
        public void patient_filled_by_json_simulateDB_with_1_new_schedule_validade_corrected()
        {
            var patient1 = this.loadPatientFromFile("patientFull_fromDB_NewSchedule.json", "{data}", DateTime.Now.AddDays(1).ToString("u"));

            PatientValidationContract patientValidationContract = new PatientValidationContract(patient1);

            var schedulesIsValid = patient1.SchedulesIsValid(patient1.Schedules, this.SearchScheduleDateTimeFree);

            var mensagns = patientValidationContract.Notifications.AllInvalidations() + patient1.GetScheduleNotifications();

            Assert.True(schedulesIsValid && patientValidationContract.IsValid);

        }

        [Fact]
        public void patient_filled_by_json_simulateDB_with_1_new_schedule_date_lower_not_executed()
        {
            var patient1 = this.loadPatientFromFile("patientFull_fromDB_NewSchedule.json", "{data}", DateTime.Now.AddDays(-1).ToString("u"));
            

            PatientValidationContract patientValidationContract = new PatientValidationContract(patient1);

            var schedulesIsValid = patient1.SchedulesIsValid(patient1.Schedules, this.SearchScheduleDateTimeFree);

            var mensagns = patientValidationContract.Notifications.AllInvalidations() + patient1.GetScheduleNotifications();

            Assert.False(schedulesIsValid && patientValidationContract.IsValid);

        }

        [Fact]
        public void patient_filled_by_json_simulateDB_with_1_new_schedule_date_lower_executed()
        {
            var patient1 = this.loadPatientFromFile("patientFull_fromDB_NewSchedule.json", "{data}", DateTime.Now.AddDays(-1).ToString("u"));
            patient1.Schedules[2].SetExecuted(true);

            PatientValidationContract patientValidationContract = new PatientValidationContract(patient1);

            var schedulesIsValid = patient1.SchedulesIsValid(patient1.Schedules, this.SearchScheduleDateTimeFree);

            var mensagns = patientValidationContract.Notifications.AllInvalidations() + patient1.GetScheduleNotifications();

            Assert.True(schedulesIsValid && patientValidationContract.IsValid);

        }



        private string GetJsonFromFile(string fileName)
        {
            return File.ReadAllText( $"{AppContext.BaseDirectory}/data/{fileName}" );
        }

        public Patient loadPatientFromFile(string fileName, string dataToBereplace, string replaceFor )
        {
          // var patient = new  Patient();

           var jsonOriginal = this.GetJsonFromFile(fileName);

           if  (!string.IsNullOrEmpty(dataToBereplace))
                jsonOriginal = jsonOriginal.Replace(dataToBereplace, replaceFor);

           var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var patient1 = JsonConvert.DeserializeObject<Patient>(jsonOriginal,settings);

            return patient1;
        }

        public string SerializePatient(Patient patient)
        {
            Console.Write(JsonConvert.SerializeObject(patient));
            return  JsonConvert.SerializeObject(patient);
        }


        [Fact]
        public void patient_serialize_json_lists_empty_not_null()
        {

            var patient1 = new Patient();///"fdsf","fdfd",DateTime.Now);

            var patientSerialized = SerializePatient(patient1);

            Assert.False(patientSerialized.Contains("\"Motivation\" : null"));
        }




        [Fact]
        public void patient_filled_by_jsonCreated_idFromDatabase_false()
        {

            var patient1 = this.loadPatientFromFile("patientFullCreate.json","","");

            Assert.False(patient1.isFromDatabase());
        }


        [Fact]
        public void patient_filled_by_jsonCreated_idFromDatabase_true()
        {

            var patient1 = this.loadPatientFromFile("patientFull_fromDB_NewSchedule.json", "{data}", DateTime.Now.AddDays(1).ToString("u"));

            Assert.True(patient1.isFromDatabase());
        }



        [Theory]
        [InlineData("patientFullCreate.json")]
        [InlineData("patientFull_fromDB.json")]
        public void patient_filled_by_json(string fileJsonName)
        {

            var jsonOriginal = this.GetJsonFromFile( fileJsonName);

            var patient1 = this.loadPatientFromFile(fileJsonName,"","");

            var jsonDeserialize  = JsonConvert.SerializeObject(patient1);

            

            int propertyCountOriginalJson = 0;
            int propertyCountDeserialize = 0;

            JObject OriginalJsonObject = JObject.Parse(jsonOriginal);
            JObject DeserializedObject = JObject.Parse(jsonDeserialize);


            foreach (JToken token_expected in OriginalJsonObject.Children())
                if (token_expected is JProperty && ((JProperty)token_expected).Value.ToObject<Object>() != null)
                    propertyCountOriginalJson++;

            //retira  propriedade equilavente a schedules
            propertyCountOriginalJson--;        


            foreach (JToken token_expected in OriginalJsonObject.Children())
            {
                if (token_expected is JProperty)
                {
                    var prop_expected = token_expected as JProperty;

                    foreach (JToken token_actual in DeserializedObject.Children())
                    {
                        if (token_actual is JProperty)
                        {
                            var prop_actual = token_actual as JProperty;

                            if (prop_expected.Name.ToLower() == prop_actual.Name.ToLower() 
                                && prop_expected.Value.ToString() == prop_actual.Value.ToString()
                                )
                                propertyCountDeserialize++;

                        }
                    }
                }
            }


           //Assert.Equal(patient.Name, "ca3aa919-c935-4c9f-b304-7d744dbe050e");
           Assert.Equal(propertyCountOriginalJson, propertyCountDeserialize);

        }
        
    }
}