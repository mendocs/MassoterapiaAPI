using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Massoterapia.Application.Patient.Models;


namespace Massoterapia.Application.Patient.Interfaces
{
    public interface IPatientService
    {
        Task<IList<PatientViewModelList>> CreatePatient(PatientInputModel patientViewModel);

        Task<IList<PatientViewModelList>> SearchForCreate(PatientInputModel patientInputModel);        

        Task<IList<PatientViewModelList>> SearchByLikeNamePhoneScheduleDateRange(PatientInputModel patientInputModel);

        Task<long> UpdatePatient(Domain.Entities.Patient patient);

        Task<Domain.Entities.Patient> SearchByKey(Guid key);
         
    }
}