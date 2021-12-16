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
using Massoterapia.Domain.Entities;

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


    private IEnumerable<(Guid, string)> SearchScheduleDateTimeFree(DateTime startDate,int duration)
    {
      IList<(Guid, string)> returnValue = new List<(Guid, string)>();
      string DateUsed = "";
      PatientInputModel patientInput = new PatientInputModel();
      patientInput.ScheduledateRange.Add(startDate.ToUniversalTime());
      patientInput.ScheduledateRange.Add(startDate.AddMinutes(duration).ToUniversalTime());
      IList<PatientViewModelList> result = this.SearchByScheduleDateRange(patientInput.ScheduledateRange).Result;

      if (result != null)
        result.ToList<PatientViewModelList>().ForEach(_patient =>
        {
          _patient.Schedules
          .Where(_schedule =>
          {
            if (_schedule.Canceled || _schedule.Executed)
              return false;

            return SharedCore.tools.DateTimeTools.DateTimeBetween(_schedule.StartdDate, patientInput.ScheduledateRange[0], patientInput.ScheduledateRange[1]) 
                    || SharedCore.tools.DateTimeTools.DateTimeBetween(_schedule.EndDate, patientInput.ScheduledateRange[0], patientInput.ScheduledateRange[1]);

          })
          .ToList().ForEach( _schedule => DateUsed = ScheduleDateUsed(_schedule));
          returnValue.Add((_patient.Key, _patient.Name + " em " + DateUsed));
        });
      return (IEnumerable<(Guid, string)>) returnValue;

      string ScheduleDateUsed(Schedule _Schedule) => SharedCore.tools.DateTimeTools.ConvertDateToString(_Schedule.StartdDate) + " ~ " + SharedCore.tools.DateTimeTools.ConvertDateHourToString(_Schedule.EndDate);
    }


/*
        private string SearchScheduleDateTimeFree_(DateTime startDate)
        {
            PatientInputModel patientInput = new PatientInputModel();
            patientInput.ScheduledateRange.Add(startDate.AddMinutes(-30).ToUniversalTime());
            patientInput.ScheduledateRange.Add(startDate.AddMinutes(30).ToUniversalTime()); 

            IList<PatientViewModelList> PatientsFromDatabase = this.SearchByScheduleDateRange(patientInput).Result;

            if (PatientsFromDatabase.Count > 0)
            {
                string DateUsed = "";
                PatientsFromDatabase[0].Schedules.Where(schedule => !(schedule.Canceled || schedule.Executed)).ToList().ForEach( schedule => DateUsed += ScheduleDateUsed(schedule.StartdDate) );

                return  $"{PatientsFromDatabase[0].Name} em {DateUsed}";
            }

            string ScheduleDateUsed(DateTime startdDate)
            {
                return 
                SharedCore.tools.DateTimeTools.ConvertDateToString(
                SharedCore.tools.DateTimeTools.DateTimeBetween(startdDate, patientInput.ScheduledateRange[0], patientInput.ScheduledateRange[1]));
            }
            return "";
        }
*/
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
                patientInput.Scheduletime,
                patientInput.Duration
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


        private async Task<IList<PatientViewModelList>> SearchByScheduleDateRange_(PatientInputModel patientInput)
        {
            IList<Domain.Entities.Patient> PatientsFromDatabase = 
            await this.PatientRepository.QueryLikeNamePhoneScheduledateRange (patientInput.Name, patientInput.Phone , patientInput.ScheduledateRange);
            return this.patientListCollection( PatientsFromDatabase);
        }        

    private async Task<IList<PatientViewModelList>> SearchByScheduleDateRange(IList<DateTime> scheduledateRange)
    {
      IList<Massoterapia.Domain.Entities.Patient> PatientsFromDatabase = await this.PatientRepository.QueryScheduledateRangeForScheduleFree(scheduledateRange);
      IList<PatientViewModelList> patientViewModelListList = this.patientListCollection(PatientsFromDatabase);
      PatientsFromDatabase = (IList<Massoterapia.Domain.Entities.Patient>) null;
      return patientViewModelListList;
    }


        public Task<Domain.Entities.Patient> SearchByKey(Guid key)
        {
            Domain.Entities.Patient PatientFromDatabase =  this.PatientRepository.Query(key).Result;
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