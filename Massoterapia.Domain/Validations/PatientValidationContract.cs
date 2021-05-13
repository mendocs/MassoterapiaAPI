using Flunt.Validations;
using Massoterapia.Domain.Entities;

namespace Massoterapia.Domain.Validations
{
    public class PatientValidationContract: Contract<Patient>
    {
        public PatientValidationContract(Patient patient)
        {
            Requires()
                .IsNotNullOrEmpty(patient.Name,"Nome","Nome não pode ser vazio")
                .IsNotNullOrEmpty(patient.Phone,"telefone","telefone não pode ser vazio")
                .IsGreaterThan (patient.Phone,10,"telefone","telefone tem que ter 11 caracteres (XX)xxxxx-xxxx")
                .IsLowerThan (patient.Phone,12,"telefone","telefone tem que ter 11 caracteres (XX)xxxxx-xxxx")
            ;
        }        
    }
}