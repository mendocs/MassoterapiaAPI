using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Massoterapia.Domain.Entities;
using SharedCore.Repositories;

namespace Massoterapia.Domain.Interfaces
{
    public interface IPatientRepository : IRepositoryWrite<Patient>
    {
        Task<IList<Patient>> QueryByName(string name);

        Task<IList<Patient>> QueryByNameOrPhone(string name, string Phone);

        Task<IList<Patient>> QueryLikeNamePhoneScheduledateRange (string name, string phone, IList<DateTime> ScheduledateRange);

        Task<IList<Patient>> QueryScheduledateRangeForScheduleFree(IList<DateTime> ScheduledateRange);

    }
}