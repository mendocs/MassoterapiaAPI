using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;

namespace Massoterapia.API.Controllers
{

    public class taskResulado
    {
        public string Result {get; set;}
    }

    public class ControllerBase_Core : ControllerBase
    {
        private readonly ILogger _logger;  

        public ControllerBase_Core(ILogger logger)
        {
            _logger = logger;
        }       


        private string ResultTask(Task taskresult)
        {

                taskresult.RunSynchronously();

                //var resultado = taskresult.Status.;

                if (taskresult.IsFaulted)
                    throw (taskresult.Exception.InnerException);
                
                var result = Task.FromResult(taskresult).Result;
                string valor = JsonConvert.SerializeObject(result);
                int fim = valor.IndexOf(",\"Id\"");
                return valor.Substring(10,fim-10);
        }

        protected IActionResult ResultSearch(Task taskresult)
        {
            string result;
            return ResultSearch(taskresult, false, "Registro Criado" ,  out result);
        }

        protected IActionResult ResultSearch(Task taskresult, bool resultCreate, string createTitle )
        {
            string result;
            return ResultSearch(taskresult, resultCreate, createTitle, out result);
        }        

        protected IActionResult ResultSearch(Task taskresult, bool resultCreate, string createTitle ,  out string result)
        {
            try
            {
                string resultado = ResultTask(taskresult);
                result = resultado;

                if (resultado == "null")
                    return NotFound($"nenhum registro encontrado");
                else
                {
                    if (resultCreate)
                        return CreatedAtAction(createTitle ,resultado);
                    else
                        return Ok(resultado);
                }
            }
            catch (ArgumentException exception)
            {
                result = "";
                return BadRequest($"{exception.Message}");
            }            
            catch (Exception exception)
            {
                result = "";
                _logger.LogError(exception, exception.Message);
                return new StatusCodeResult(500);
            }
        }         
    
        protected string JsonFromBody()
        {
           string jsonStringFromBody = "";

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                jsonStringFromBody = reader.ReadToEndAsync().Result;
            }

            return jsonStringFromBody;
        }    

    
    }
}