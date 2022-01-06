using System.Globalization;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Massoterapia.API.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class PatientController: ControllerBase_Core
    {
        
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger;         
        private readonly IConfiguration _configuration;

        JsonSerializerSettings settings;

        public PatientController(IPatientService patientService, ILogger<PatientController> logger,  IConfiguration configuration)
        : base (logger)
        {
            _patientService = patientService;
            _logger = logger;
            _configuration = configuration;

            settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };            
        }


        [HttpGet]
        [Route("getkey/{key}")]
        public IActionResult  GetPatientByKey( Guid key)
        {
            Task tarefa = new Task<Patient>(() => _patientService.SearchByKey(key).Result);
            return ResultSearch(tarefa);
        }

        [HttpGet]
        [Route("search")]
        public IActionResult GetPatientByNamePhoneDateRangeSchedule(string? name ="", string? phone ="" , DateTime? dateStart =null, DateTime? dateEnd =null)
        {
            PatientInputModel patientInputModel = new PatientInputModel();
            patientInputModel.Name = name ?? "";
            patientInputModel.Phone = phone ?? "";
            
            if (dateStart is not null)
                patientInputModel.ScheduledateRange.Add(dateStart.Value);

            if (dateEnd is not null)
                patientInputModel.ScheduledateRange.Add(dateEnd.Value);

            Task tarefa = new Task<IList<PatientViewModelList>>(() => _patientService.SearchByLikeNamePhoneScheduleDateRange(patientInputModel).Result);

            return ResultSearch(tarefa);    
        }       


        [HttpPost]
        public IActionResult CreatePatient( [FromBody] PatientInputModel patientInputModel )
        {       
            IActionResult actionResult = null;  

            Task tarefa = new Task<IList<PatientViewModelList>>(() => _patientService.CreatePatient(patientInputModel).Result);

            string resultado="";
            actionResult = ResultSearch(tarefa,true,"CreatePatient",out resultado); 

            if( !string.IsNullOrEmpty(resultado))
            {
                IList<PatientViewModelList> result = JsonConvert.DeserializeObject<IList<PatientViewModelList>>(resultado,settings);

                if ( SharedCore.tools.DateTimeTools.CompareDateTime(result[0].Schedules[0].StartdDate,patientInputModel.ScheduleData.StartdDate))
                    return CreatedAtAction("CreatePatient",resultado);
                else
                    return Ok(resultado);
            }
            else
                return actionResult;            
        }        


        [HttpPut]
        public IActionResult UpdatePatient()
        {
            Patient PatientFromBody = Massoterapia.Domain.Tools.JsonTools.loadPatientFromJson(JsonFromBody());

            Task tarefa = new Task<long>(() => _patientService.UpdatePatient(PatientFromBody).Result);
            
            return ResultSearch(tarefa);
        }

    }
}