using System;
using System.Net.Http;
using System.Text;
using Massoterapia.Application.user.models;
using Massoterapia.Integration.Tests.Models;
using Newtonsoft.Json;
using Xunit;

namespace Massoterapia.Integration.Tests
{


    public class IntegrationUser
    {


        private UserInputModel userInputModel;
        private HttpRequestMessage request;

        public IntegrationUser()
        {
                userInputModel = new UserInputModel{
                    Name="nome usuario",
                    Password_Text = "password",
                    NumberSalt = 70,
                    Interation = 10101,
                    Nhash = 70
                    };        

            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://localhost:5001/api/v1/user"),
 
                
            };     
        }


        [Fact]
        public async void Integration_Authenticate_user_corrected()
        {
            var client = FactoryWebApplication.GetWebApplication();
            //var response = await client.GetAsync ("v1/products");

            var json = JsonConvert.SerializeObject(userInputModel);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            //var response = client.SendAsync(request).ConfigureAwait(false);
            var response = client.PostAsync(request.RequestUri.AbsolutePath, request.Content).ConfigureAwait(false);
 
            var responseInfo = response.GetAwaiter().GetResult();

            var forecast = JsonConvert.DeserializeObject<ResultAuthenticate>(await responseInfo.Content.ReadAsStringAsync());

            Assert.True(forecast.result);
        }

        [Fact]
        public async void Integration_Authenticate_user_incorrected_password()
        {
            var client = FactoryWebApplication.GetWebApplication();
            
            userInputModel.Password_Text = "password1";

            var json = JsonConvert.SerializeObject(userInputModel);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync(request.RequestUri.AbsolutePath, request.Content).ConfigureAwait(false);
 
            var responseInfo = response.GetAwaiter().GetResult();

            var forecast = JsonConvert.DeserializeObject<ResultAuthenticate>(await responseInfo.Content.ReadAsStringAsync());

            Assert.False(forecast.result);
        }


        [Fact]
        public async void Integration_Authenticate_user_incorrected_user()
        {
            var client = FactoryWebApplication.GetWebApplication();
            
            userInputModel.Name = "nome usuario 1";

            var json = JsonConvert.SerializeObject(userInputModel);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            //var response = client.SendAsync(request).ConfigureAwait(false);
            var response = client.PostAsync(request.RequestUri.AbsolutePath, request.Content).ConfigureAwait(false);
 
            var responseInfo = response.GetAwaiter().GetResult();

            var forecast = JsonConvert.DeserializeObject<ResultAuthenticate>(await responseInfo.Content.ReadAsStringAsync());

            Assert.False(forecast.result);
        }


    }
}
