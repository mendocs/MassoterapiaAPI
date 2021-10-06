using System.Linq;
using System;
using System.Collections.Generic;
using Flunt.Notifications;
using Massoterapia.Domain.Validations;
using SharedCore.Entities;

namespace Massoterapia.Domain.Entities
{
    public class Patient : Entity
    {
        public string Phone { get; private set; }
        public string Name { get; private set; }
        public DateTime DateFillData { get; private set; }
        public string MaritalStatus { get; private set; }
        public int Sons { get; private set; }
        public DateTime DateBorn { get; private set; }
        public string Weight { get; private set; }
        public string PhoneContact { get; private set; }
        public string Occupation { get; private set; }
        public List<string> Motivation { get; private set; }
        public string MainComplaint { get; private set; }
        public string HistoryComplaint { get; private set; }
        public string SecundaryComplaint { get; private set; }
        public string IngestionLiquid { get; private set; }
        public string LiquidTypes { get; private set; }
        public string Smoking { get; private set; }
        public string Alcohol { get; private set; }
        public string Psychoactive { get; private set; }
        public string Addiction { get; private set; }
        public string PhysicalActivity { get; private set; }
        public string PhysicalActivityFrequency { get; private set; }
        public string QualitySleep { get; private set; }
        public string Food { get; private set; }
        public string Feces { get; private set; }
        public string LastFeces { get; private set; }
        public string LeisureActivities { get; private set; }
        public string LeisureRestWork { get; private set; }
        public List<string> Disease { get; private set; }
        public List<string> HealthChanges { get; private set; }
        public List<string> Treatments { get; private set; }
        public string OthersTreatments { get; private set; }
        public string Medicines { get; private set; }
        public bool Diu { get; private set; }
        public string Dum { get; private set; }
        public bool Subcutaneous { get; private set; }
        public string SubcutaneousOther { get; private set; }
        public string Surgeries { get; private set; }
        public string FamilyIllnessess { get; private set; }
        
        public IList<Schedule> Schedules { get; private set; }

        public string Comments { get; private set; }

        
        public Patient(){
        }

        public Patient(string name,string phone, DateTime scheduletime)
        {
            this.Name = name;
            this.Phone = phone;

            Schedule schedule = new Schedule(scheduletime);
            this.Schedules = new List<Schedule>();
            this.Schedules.Add(schedule);
            this.InitializateScheduleNotifications();
            this.InitializeArrays();
        }
        
        private void InitializeArrays()
        {
            this.Motivation = this.Motivation ?? new List<string>();
            this.Disease = this.Disease ?? new List<string>();
            this.HealthChanges = this.HealthChanges ?? new List<string>();
            this.Treatments = this.Treatments ?? new List<string>();
            this.Schedules = this.Schedules ?? new List<Schedule>();
        }

       public override void SetItensConstructor()
       {
           base.SetItensConstructor();

           this.InitializeArrays();
           this.InitializateScheduleNotifications();

           foreach (Schedule schedule in this.Schedules)
                schedule.SetItensConstructor();

       }

        public bool SchedulesIsValid(Func<DateTime, string> validatioinDB)
        {
            this.InitializateScheduleNotifications();
            
            var schedules = this.Schedules.Select((value, index) => new { value, index });    

            foreach (var currentSchedule in schedules)
            {
                this.VerifyScheduleValidation(currentSchedule.value ,currentSchedule.index, validatioinDB); 
            } 

            return (this.ScheduleNotifications.Count==0) ;
        }        

        private void InitializateScheduleNotifications()
        {
            this.ScheduleNotifications = this.ScheduleNotifications ?? new List<string>();
        }

        private IList<String> ScheduleNotifications;
        private void VerifyScheduleValidation(Schedule schedule, int index, Func<DateTime, string> validactInDB)
        {
            if ( !schedule.Executed && !schedule.Canceled)
            {
                if (!schedule.isFromDatabase())
                {
                    ScheduleValidationContract scheduleValidationContract = new ScheduleValidationContract(schedule);      

                    foreach(Notification notification in scheduleValidationContract.Notifications)
                        this.AddScheduleNotifications(notification.Message, index);                    
                }

                var ScheduleDateintervalFound = validactInDB(schedule.StartdDate);

                if (!string.IsNullOrEmpty(ScheduleDateintervalFound) && !ScheduleDateintervalFound.Contains(this.Name))
                    this.AddScheduleNotifications($"Já existe atendimento neste horário para: {ScheduleDateintervalFound}", index);

            }            
        }
        
        public void AddScheduleNotifications(string message, int index)
        {
            ScheduleNotifications.Add($"Atendimento {index + 1 } : {message}");
        }


        public string GetScheduleNotifications()
        {
            string retorns = "";
            foreach(string notification in ScheduleNotifications)
                retorns += $"{notification} ; ";

            return retorns;            
        }

        public void SetNamePhone(string name, string phone)
        {
            this.Name = name;
            this.Phone = phone;
        }        


    }
}