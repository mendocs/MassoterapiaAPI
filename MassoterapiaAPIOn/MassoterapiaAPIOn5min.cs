using System.Net.Http;
using System;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace MassoterapiaAPI.Function
{
    public class MassoterapiaAPIOn5min
    {
        [FunctionName("MassoterapiaAPIOn5min")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {

            var json = new WebClient().DownloadString("https://massoterapiaapi.azurewebsites.net/WeatherForecast");
            //var json = new HttpClient().GetStringAsync("https://massoterapiaapi.azurewebsites.net/WeatherForecast");
            //Microsoft.AspNetCore.Mvc.OkObjectResult ook = new(json);

            //return new Microsoft.AspNetCore.Mvc.OkObjectResult(json);
        }
    }
}
