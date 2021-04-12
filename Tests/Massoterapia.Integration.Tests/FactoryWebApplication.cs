using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Massoterapia.Integration.Tests
{
    public static class FactoryWebApplication
    {

       public static WebApplicationFactory<Massoterapia.API.Startup> factory = new WebApplicationFactory<Massoterapia.API.Startup>();

        public static HttpClient GetWebApplication()
        {
                        
            var client = factory.CreateClient( 
                new WebApplicationFactoryClientOptions {
                    BaseAddress = new Uri( "https://localhost:5001")
                    });

            return client;        
        }
    }
}