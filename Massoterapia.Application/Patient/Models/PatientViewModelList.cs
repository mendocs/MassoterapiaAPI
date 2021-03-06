using System;
using System.Collections.Generic;
using Massoterapia.Domain.Entities;

namespace Massoterapia.Application.Patient.Models
{
    public class PatientViewModelList
    {
        public Guid Key { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public List<Schedule> Schedules{ get; set; } 
    }
}