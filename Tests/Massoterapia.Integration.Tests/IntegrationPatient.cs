using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Massoterapia.Application.Patient.Models;
using Massoterapia.Application.user.models;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Tests;
using Massoterapia.Integration.Tests.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Massoterapia.Integration.Tests
{
    public class IntegrationPatient
    {

        private PatientInputModel patientInputModel;
        private HttpRequestMessage request;

        HttpClient client ;

        JsonSerializerSettings settings;
        public IntegrationPatient()
        {

            patientInputModel = new PatientInputModel{
                Name = "nome test"
                };

            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://localhost:5001/api/v1/patient"),
            };           

            client = FactoryWebApplication.GetWebApplication(); 

            settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            
        }

        [Fact]
        public void Integration_get_patient_corrected()
        {     
                            
            string key = "4a435118-9e38-4d5f-84cd-deadff6361ae";
            //string key = "93300e47-b4b5-4fd3-9d86-d34ef4a39ee7";
            
            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/getkey/{key}").ConfigureAwait(false);
 
            var responseInfo = response.GetAwaiter().GetResult();

            var resultado = responseInfo.Content.ReadAsStringAsync().Result;

            var patient1 = JsonConvert.DeserializeObject<Patient>(resultado,settings);

            Assert.Equal("name create 3" , patient1.Name );
        }


        [Theory]
        [InlineData("ca3aa919-c935-4c9f-b304-7d744dbe050e", System.Net.HttpStatusCode.NotFound)]
        [InlineData("ca3aa919-c935-4c9f-b304-7565656", System.Net.HttpStatusCode.BadRequest)]
        public void Integration_get_patient_statusCode(string key, System.Net.HttpStatusCode StatusResult)
        {
                        
            
            
            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/getkey/{key}").ConfigureAwait(false);
 
            var responseInfo = response.GetAwaiter().GetResult();

            var result = responseInfo.Content.ReadAsStringAsync().Result;

            Assert.Equal(responseInfo.StatusCode , StatusResult );
            
        }

        [Theory]
        [InlineData(1,"nome teste", "119998877755", "2021-04-25", "2021-04-30")]
        [InlineData(2,"nome teste", "1199", "2021-04-25", "2021-04-30")]
        [InlineData(2,"nome teste ","1199988","","")]
        [InlineData(0,"nome 5555 ","1199555","","")]
        [InlineData(2,"","1199988","","")]
        [InlineData(2,"nome teste ","","","")]
        [InlineData(2,"nome     ","","","")]
        [InlineData(1,"  create     ","","","")]
        [InlineData(2,"", "1199988", "2021-04-25", "2021-04-30")]
        [InlineData(3,"", "", "2021-04-25", "2021-04-30")]
        [InlineData(1,"criar 2", "", "2021-04-25", "2021-04-30")]
        [InlineData(1,"", "", "2021-04-26", "2021-04-28")]
        [InlineData(0,"", "", "2021-04-20", "2021-04-24")]
        [InlineData(13,"","","","")]
        public void Integration_get_patient_search(int countPatientList, string? name, string? phone, string? dateStart, string? dateEnd)
        {
            string nameParameter = !string.IsNullOrEmpty(name) ? $"name={name}&":"";
            string phoneParameter = !string.IsNullOrEmpty(phone) ? $"phone={phone}&":"";
            string dateStartParameter = !string.IsNullOrEmpty(dateStart) ? $"dateStart={DateTime.Parse(dateStart).ToString("yyyy-MM-dd")}&":"";
            string dateEndParameter = !string.IsNullOrEmpty(dateEnd) ? $"dateEnd={DateTime.Parse(dateEnd).ToString("yyyy-MM-dd")}&":"";
            
            var parameters = $"{nameParameter}{phoneParameter}{dateStartParameter}{dateEndParameter}".Replace("/","-").Replace(" ","%20");

            if (!string.IsNullOrWhiteSpace(parameters))
                parameters = parameters.Substring(0,parameters.Length-1);

            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/search?{parameters}").ConfigureAwait(false);
 
            var responseInfo = response.GetAwaiter().GetResult();

            var resultHttpRequest = responseInfo.Content.ReadAsStringAsync().Result;

            var patientListResult = JsonConvert.DeserializeObject<IList<PatientViewModelList>>(resultHttpRequest,settings);
            
            Assert.Equal(countPatientList, patientListResult.Count );
        }


        [Fact]
        public void Integration_create_patient_corrected()
        {     
            
            var patientInputModelToCreate = new PatientInputModel{
                Name = "name create 097",
                Phone = "11991877794",
                ScheduleData = new Schedule(DateTime.Now.AddDays(1).ToUniversalTime(),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0) 
                };            

            var responseInfo = this.CreationResult(patientInputModelToCreate);

            var resultado = responseInfo.Content.ReadAsStringAsync().Result;

            var patientListResult = JsonConvert.DeserializeObject<IList<PatientViewModelList>>(resultado,settings);

            Assert.Equal(1 , patientListResult.Count );
        }

        private HttpResponseMessage CreationResult(PatientInputModel patientInputModelToCreate)
        {
            var json = JsonConvert.SerializeObject(patientInputModelToCreate);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync(request.RequestUri.AbsolutePath, request.Content).ConfigureAwait(false);
 
            var responseInfo = response.GetAwaiter().GetResult();

            return responseInfo;
            
        }


        private HttpResponseMessage UpdatenResult(Patient patientInputtoUpdate)
        {
            var json = JsonConvert.SerializeObject(patientInputtoUpdate);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PutAsync(request.RequestUri.AbsolutePath, request.Content).ConfigureAwait(false);
 
            var responseInfo = response.GetAwaiter().GetResult();

            return responseInfo;
            
        }


        [Theory]
        [InlineData("","","nome não pode ser vazio", HttpStatusCode.BadRequest)]
        [InlineData("","11998877665","nome não pode ser vazio", HttpStatusCode.BadRequest)]
        [InlineData("nome test","","telefone não pode ser vazio", HttpStatusCode.BadRequest)]
        [InlineData("nome test","1199887766","telefone tem que ter 11 caracteres", HttpStatusCode.BadRequest)]
        [InlineData("nome test","119988776655","telefone tem que ter 11 caracteres", HttpStatusCode.BadRequest)]
        [InlineData("nome test","11999998888","atendimento não pode ser menor que a data atual", HttpStatusCode.BadRequest)]
        [InlineData("nome test","11999990000","existe atendimento neste", HttpStatusCode.BadRequest)]
        public void Integration_create_patient_error(string name, string phone, string messageError, HttpStatusCode errorMethod)
        {     
            var patientInputModelToCreate = new PatientInputModel{
                Name = name,
                Phone = phone,
                ScheduleData = new Schedule(DateTime.Now.AddDays(1).ToUniversalTime(),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0) 
                };

            /*
            if (phone == "11999999999")    // para incluir data vazias
                patientInputModelToCreate.Scheduletime = patientInputModelToCreate.Scheduledate;
            */

            if (phone == "11999998888")    // para incluir data menores que a data atual
                patientInputModelToCreate.ScheduleData.SetStartdDate(DateTime.Now.AddDays(-1).ToUniversalTime());


            if (phone == "11999990000")    // para incluir data já utilizada
                patientInputModelToCreate.ScheduleData.SetStartdDate(new DateTime(2021,5,10,18,30,0,DateTimeKind.Utc));

            var responseInfo = this.CreationResult(patientInputModelToCreate);

            var messageResult = responseInfo.Content.ReadAsStringAsync().Result;

            Assert.Equal(errorMethod , responseInfo.StatusCode );
            Assert.True( messageResult.ToLower().Contains(messageError));
        }

        [Fact]
        public void Integration_update_patient_corrected()
        {     
            
            PatientTests patientTests = new PatientTests();

            string dataToReplace = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            Domain.Entities.Patient patientToBeUpdate = patientTests.loadPatientFromFile("PatientFull_real_updated_fromDB.json","2021-05-01",dataToReplace);     
            patientToBeUpdate.Schedules[2].SetKeyNull();

            var responseInfo = this.UpdatenResult(patientToBeUpdate);
            var resultado = responseInfo.Content.ReadAsStringAsync().Result;

            var patientUpdateResult = JsonConvert.DeserializeObject<long>(resultado,settings);

            Assert.Equal(1 , patientUpdateResult );
        }        


        [Fact]
        public void Integration_update_patient_corrected_with_schedule_altered()
        {     
            
            PatientTests patientTests = new PatientTests();

            string dataToReplace = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            Domain.Entities.Patient patientToBeUpdate = patientTests.loadPatientFromFile("PatientFull_real_updated_fromDB.json","2021-05-01",dataToReplace);     
            patientToBeUpdate.Schedules[2].SetKeyNull();

            var responseInfo0 = this.UpdatenResult(patientToBeUpdate);


            //testa o paciente ja existente, tentando alterar o horario do schedule para 20 min depois
            string dataToReplace1 = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + ":15:20";
            Domain.Entities.Patient patientToBeUpdate1 = patientTests.loadPatientFromFile("PatientFull_real_updated_fromDB.json","2021-05-01:15:00",dataToReplace);

            var responseInfo = this.UpdatenResult(patientToBeUpdate);

            var patientUpdateResult = JsonConvert.DeserializeObject<long>(responseInfo.Content.ReadAsStringAsync().Result,settings);

            Assert.Equal(1 , patientUpdateResult );
        }  




        [Theory]
        /*
        [InlineData("","","nome não pode ser vazio", HttpStatusCode.BadRequest)]
        [InlineData("","11998877665","nome não pode ser vazio", HttpStatusCode.BadRequest)]
        [InlineData("","11999999999","do atendimento não pode ser menor que a data atual", HttpStatusCode.BadRequest)]
        [InlineData("","11888888888","existe atendimento neste", HttpStatusCode.BadRequest)]
        */
        [InlineData("","11777777777","existe atendimento neste", HttpStatusCode.BadRequest)]
        [InlineData("","11666666666","existe atendimento neste", HttpStatusCode.BadRequest)]
        public void Integration_update_patient_fail(string name, string phone, string messageError, HttpStatusCode errorMethod)
        {     
            
            PatientTests patientTests = new PatientTests();

            Domain.Entities.Patient patientToBeUpdate = patientTests.loadPatientFromFile("PatientFull_real_updated_fromDB.json","","");     

            if (phone == "11999999999")
            {
                Domain.Entities.Schedule newSchedule = new Domain.Entities.Schedule(DateTime.Now.AddDays(-1),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
                newSchedule.SetKeyNull();
                patientToBeUpdate.Schedules.Add(newSchedule);

            }
            else if (phone == "11888888888")
            {
                //schedule já existente em outro registro
                Domain.Entities.Schedule newSchedule = new Domain.Entities.Schedule(new DateTime(2021,5,10,18,30,0,DateTimeKind.Utc),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
                newSchedule.SetKeyNull();
                patientToBeUpdate.Schedules.Add(newSchedule);

            }         
            else if (phone == "11777777777")
            {
                //tenta alterar um schedule já existente em horario já utilizado por outro registro
                Domain.Entities.Schedule newSchedule = new Domain.Entities.Schedule(new DateTime(2021,5,20,18,40,0,DateTimeKind.Utc),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
                patientToBeUpdate.Schedules.Add(newSchedule);
            }
            else if (phone == "11666666666")
            {
                //tenta alterar um schedule já existente em horario já utilizado por outro registro
                Domain.Entities.Schedule newSchedule = new Domain.Entities.Schedule(new DateTime(2021,5,20,18,0,0,DateTimeKind.Utc),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
                patientToBeUpdate.Schedules.Add(newSchedule);
            }            
            else
            {
                patientToBeUpdate.SetNamePhone(name,phone);
                // .Name = name;
                //patientToBeUpdate.Phone = phone;
            }

            var responseInfo = this.UpdatenResult(patientToBeUpdate);

            var messageResult = responseInfo.Content.ReadAsStringAsync().Result;

            Assert.Equal(errorMethod , responseInfo.StatusCode );
            Assert.True( messageResult.ToLower().Contains(messageError));
        }   


    }
}