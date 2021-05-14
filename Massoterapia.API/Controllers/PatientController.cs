using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Massoterapia.Application.Patient.Interfaces;
using Massoterapia.Application.Patient.Models;
using Massoterapia.Application.user.Interfaces;
using Massoterapia.Application.user.models;
using Massoterapia.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Massoterapia.API.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class PatientController: ControllerBase
    {
        
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger;         
        private readonly IConfiguration _configuration;


        public PatientController(IPatientService patientService, ILogger<PatientController> logger,  IConfiguration configuration)
        {
            _patientService = patientService;
            _logger = logger;
            _configuration = configuration;

        }


        [HttpGet]
        [Route("getkey/{key}")]
        public ActionResult<Boolean> GetPatientByKey( Guid key)
        {
            try
            {
                Patient result = _patientService.SearchByKey(key).Result;

                if (result == null)
                    return NotFound($"{key} não encontrado");
                else
                    return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message, key);
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("search")]
        public ActionResult<Boolean> GetPatientByNamePhoneDateRangeSchedule(string? name ="", string? phone ="" , DateTime? dateStart =null, DateTime? dateEnd =null)
        {

            PatientInputModel patientInputModel = new PatientInputModel();
            patientInputModel.Name = name ?? "";
            patientInputModel.Phone = phone ?? "";

            
            if (dateStart is not null)
                patientInputModel.ScheduledateRange.Add(dateStart.Value);

            if (dateEnd is not null)
                patientInputModel.ScheduledateRange.Add(dateEnd.Value);
            
            try
            {
                IList<PatientViewModelList> result = _patientService.SearchByLikeNamePhoneScheduleDateRange(patientInputModel).Result;

                if (result == null)
                    return NotFound($"nenhum registro encontrado");
                else
                    return Ok( result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message, patientInputModel);
                return new StatusCodeResult(500);
            }
        }        

        


        [HttpPost]
        public ActionResult<Boolean> CreatePatient( [FromBody] PatientInputModel patientInputModel )
        {
           
            try
            {
                IList<PatientViewModelList> result = _patientService.CreatePatient(patientInputModel).Result;

                if ( SharedCore.tools.DateTimeTools.CompareDateTime(result[0].Schedules[0].StartdDate,patientInputModel.Scheduletime))
                
                    return CreatedAtAction("CreatePatient",result);
                else
                    return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest($"{exception.Message}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message, patientInputModel);
                return new StatusCodeResult(500);
                //return BadRequest($"{exception.Message}");
            }
        }


        [HttpPut]
        public ActionResult<Boolean> UpdatePatient()// [FromBody] string patientInput )
        {

           string jsonFromBody = "";

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                jsonFromBody = reader.ReadToEndAsync().Result;
            }


            try
            {
                Patient PatientFromBody = Massoterapia.Domain.Tools.JsonTools.loadPatientFromJson(jsonFromBody);
                long result = _patientService.UpdatePatient(PatientFromBody).Result;

                if ( result == 1 )
                    return Ok(result);
                else
                    return NotFound($"nenhum registro encontrado");
            }
            catch (ArgumentException exception)
            {
                return BadRequest($"{exception.Message}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message, jsonFromBody);
                return new StatusCodeResult(500);
            }
        }



    }
}