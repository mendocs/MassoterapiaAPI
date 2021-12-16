using System;
using System.Collections.Generic;

namespace Massoterapia.Application.Patient.Models
{
    public class PatientInputModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public IList<DateTime> ScheduledateRange { get; set; }
        public DateTime Scheduledate  { get; set; }
        public DateTime Scheduletime { get; set; }
        public int Duration { get; set; }

        public PatientInputModel()
        {
            ScheduledateRange = new List<DateTime>();
        }
    }
}