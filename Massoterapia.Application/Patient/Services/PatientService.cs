using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Massoterapia.Application.Patient.Interfaces;
using Massoterapia.Application.Patient.Models;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Domain.Validations;

namespace Massoterapia.Application.Patient.Services
{
    public class PatientService : IPatientService
    {

        private IPatientRepository PatientRepository; 
        private readonly IMapper _mapper;


        public PatientService(IMapper mapper, IPatientRepository patientRepository )
        {
            this.PatientRepository = patientRepository;
            this._mapper = mapper;
        }        

        private void ValidationPatient(Domain.Entities.Patient patientTobeSaved )
        {
            PatientValidationContract patientValidationContract = new PatientValidationContract(patientTobeSaved);

            var schedulesIsValid = patientTobeSaved.SchedulesIsValid(this.SearchScheduleDateTimeFree);

            if ( !(schedulesIsValid && patientValidationContract.IsValid) )
            {
                var mesagensError = patientValidationContract.Notifications.AllInvalidations() + patientTobeSaved.GetScheduleNotifications();
                throw new System.ArgumentException(mesagensError);
            }

        }  


        private string SearchScheduleDateTimeFree(DateTime startDate)
        {
            PatientInputModel patientInput = new PatientInputModel();
            patientInput.ScheduledateRange.Add(startDate.AddMinutes(-50).ToUniversalTime());
            patientInput.ScheduledateRange.Add(startDate.AddMinutes(50).ToUniversalTime());

            

            IList<PatientViewModelList> PatientsFromDatabase = this.SearchByScheduleDateRange(patientInput).Result;
            

            if (PatientsFromDatabase.Count > 0)
            {
                string DateUsed = "";
                PatientsFromDatabase[0].Schedules.Where(schedule => !(schedule.Canceled || schedule.Executed)).ToList().ForEach( schedule => DateUsed += ScheduleDateUsed(schedule.StartdDate) );

                //if (!string.IsNullOrEmpty(DateUsed))
                return  $"{PatientsFromDatabase[0].Name} em {DateUsed}";
            }

            string ScheduleDateUsed(DateTime startdDate)
            {

                return 
                SharedCore.tools.DateTimeTools.ConvertDateToString(
                SharedCore.tools.DateTimeTools.DateTimeBetween(SharedCore.tools.DateTimeTools.AjustDateTimeToLinuxFromDB(startdDate), patientInput.ScheduledateRange[0], patientInput.ScheduledateRange[1]));
            }


            return "";


        }

        private IList<PatientViewModelList> patientListCollection(Domain.Entities.Patient patient)
        {
            IList<Domain.Entities.Patient> Patients = new List<Domain.Entities.Patient>();
            Patients.Add(patient);

            return this.patientListCollection(Patients);


        }
        private IList<PatientViewModelList> patientListCollection(IList<Domain.Entities.Patient> patients)
        {
            IList<PatientViewModelList>  patientListCollectionReturn = new List<PatientViewModelList>();

            if (patients?.Count>0)
                foreach (var CurrentPatient in patients){
                    PatientViewModelList patientList = _mapper.Map<PatientViewModelList> (CurrentPatient);
                    patientListCollectionReturn.Add(patientList);
                }

            return patientListCollectionReturn;
        }

        public Task<IList<PatientViewModelList>> CreatePatient(PatientInputModel patientInput)
        {
            patientInput = this.NormalizaPatientInput(patientInput);

            Domain.Entities.Patient patientTobeSaved = new Domain.Entities.Patient(
                patientInput.Name,
                patientInput.Phone,
                patientInput.Scheduletime
            );

            this.ValidationPatient(patientTobeSaved);

            IList<PatientViewModelList> patientSeached = this.SearchForCreate(patientInput).Result;

            if  (patientSeached?.Count > 0)
                return Task.FromResult(patientSeached);
            else
            {
                Domain.Entities.Patient PatientSaved = this.PatientRepository.Insert(patientTobeSaved).Result;

                return Task.FromResult( patientListCollection(PatientSaved) );
            }    

            
        }

        public async Task<IList<PatientViewModelList>> SearchForCreate(PatientInputModel patientInput)
        {
            IList<Domain.Entities.Patient> PatientsFromDatabase =  await this.PatientRepository.QueryByNameOrPhone(patientInput.Name, patientInput.Phone);
            return this.patientListCollection( PatientsFromDatabase);
        }

        private PatientInputModel NormalizaPatientInput(PatientInputModel patientInput)
        {
            if (patientInput.Name?.Length>0)
                patientInput.Name = patientInput.Name.Trim();

           if (patientInput.Phone?.Length>0)     
                patientInput.Phone = patientInput.Phone.Trim();

            return patientInput;
        }




        public async Task<IList<PatientViewModelList>> SearchByLikeNamePhoneScheduleDateRange(PatientInputModel patientInput)
        {
            patientInput = this.NormalizaPatientInput(patientInput);
            
            if (patientInput.ScheduledateRange?.Count==2)
            {
                patientInput.ScheduledateRange[0] =  SharedCore.tools.DateTimeTools.DateTimeSetHourToZero(patientInput.ScheduledateRange[0]);
                patientInput.ScheduledateRange[1] =  SharedCore.tools.DateTimeTools.DateTimeSetHourTo2359(patientInput.ScheduledateRange[1]);
            }

            IList<Domain.Entities.Patient> PatientsFromDatabase = 
            await this.PatientRepository.QueryLikeNamePhoneScheduledateRange (patientInput.Name, patientInput.Phone , patientInput.ScheduledateRange);
            return this.patientListCollection( PatientsFromDatabase);
        }


        private async Task<IList<PatientViewModelList>> SearchByScheduleDateRange(PatientInputModel patientInput)
        {
            
            IList<Domain.Entities.Patient> PatientsFromDatabase = 
            await this.PatientRepository.QueryLikeNamePhoneScheduledateRange (patientInput.Name, patientInput.Phone , patientInput.ScheduledateRange);
            return this.patientListCollection( PatientsFromDatabase);
        }        


        public Task<Domain.Entities.Patient> SearchByKey(Guid key)
        {
            Domain.Entities.Patient PatientFromDatabase =  this.PatientRepository.Query(key);
            return Task.FromResult( PatientFromDatabase);
        }

        public Task<long> UpdatePatient(Domain.Entities.Patient patientToBeUpdated)
        {
            patientToBeUpdated.SetItensConstructor();
            this.ValidationPatient(patientToBeUpdated);
            return Task.FromResult(this.PatientRepository.Update(patientToBeUpdated).Result); 
        }
    }
}