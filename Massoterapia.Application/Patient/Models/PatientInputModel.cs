using System;
using System.Collections.Generic;
using Massoterapia.Domain.Entities;

namespace Massoterapia.Application.Patient.Models
{
    public class PatientInputModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public IList<DateTime> ScheduledateRange { get; set; }
        public Schedule ScheduleData { get; set; }
        public PatientInputModel()
        {
            ScheduledateRange = new List<DateTime>();
        }
    }
}