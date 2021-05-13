using AutoMapper;
using Massoterapia.Application.Patient.Models;

namespace Massoterapia.Application.Patient.Mappings
{
    public class PatientDomainToPatientViewModelListMappingProfile: Profile
    {
        public PatientDomainToPatientViewModelListMappingProfile()
        {   
                CreateMap<Domain.Entities.Patient,PatientViewModelList>();
            
        }        
    }
}