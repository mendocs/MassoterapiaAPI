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

        private void ValidationPatient(Domain.Entities.Patient patientTobeSaved, Domain.Entities.Patient? originalPatient)
        {
            PatientValidationContract patientValidationContract = new PatientValidationContract(patientTobeSaved);

            IList<Schedule> originalSchedules = originalPatient?.Schedules ?? new List<Schedule>();

            var schedulesIsValid = patientTobeSaved.SchedulesIsValid(originalSchedules,this.SearchScheduleDateTimeFree);

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
            return returnValue;

            string ScheduleDateUsed(Schedule _Schedule) => SharedCore.tools.DateTimeTools.ConvertDateToString(_Schedule.StartdDate) + " ~ " + SharedCore.tools.DateTimeTools.ConvertDateHourToString(_Schedule.EndDate);
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
                patientInput.ScheduleData
            );

            this.ValidationPatient(patientTobeSaved,null);

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

        private string VerifyNamePhoneChanged(Domain.Entities.Patient patientToBeUpdated , Domain.Entities.Patient patientFound)
        {
            string result = "";
            if (patientToBeUpdated.Name != patientFound.Name || patientToBeUpdated.Phone != patientFound.Phone)
            {
                PatientInputModel patientTobeSearched = new PatientInputModel(){
                        Name = patientToBeUpdated.Name,
                        Phone = patientToBeUpdated.Phone
                    };
                    
                IList<PatientViewModelList> patientSeached = this.SearchForCreate(patientTobeSearched).Result;

                patientSeached.ToList().ForEach(_patientViewModelList => {
                    if (_patientViewModelList.Key != patientToBeUpdated.Key)
                        result = $"já existe registro com este nome: {_patientViewModelList.Name} ou com este telefone: {_patientViewModelList.Phone}";
                    }
                );
            }
            return result;
        }

        public Task<long> UpdatePatient(Domain.Entities.Patient patientToBeUpdated)
        {
            Domain.Entities.Patient patientFound = this.SearchByKey(patientToBeUpdated.Key).Result;

            string namePhoneChanged = this.VerifyNamePhoneChanged(patientToBeUpdated,patientFound);
            if (!string.IsNullOrEmpty(namePhoneChanged))
                throw new System.ArgumentException(namePhoneChanged);

            patientToBeUpdated.SetItensConstructor();
            this.ValidationPatient(patientToBeUpdated, patientFound);
            return Task.FromResult(this.PatientRepository.Update(patientToBeUpdated).Result); 
        }
        
    }
}