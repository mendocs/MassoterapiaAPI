using System.Globalization;
using System;
using SharedCore.Entities;
using System.Collections.Generic;

namespace Massoterapia.Domain.Entities
{
    public class Schedule: Entity
    {
        public DateTime StartdDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string Comments { get; private set; }
        public bool Confirmed { get; private set; }
        public bool Executed { get; private set; }
        public bool Canceled { get; private set; }
        public int Duration { get; private set; }
        public string Protocol { get; private set; }
        public string Package { get; private set; }
        public decimal Price { get; private set; }        
        public int PackageSession { get; private set; }        


        public Schedule(
            Guid key,
            DateTime startdDate,
            bool canceled,
            string comments,
            bool confirmed,
            int duration,
            DateTime endDate,
            bool executed,
            string protocol,
            string package,
            decimal price,
            int packageSession

        ){
            this.SetKey(key);
            this.StartdDate = startdDate;
            this.EndDate = endDate;
            this.Comments = comments;
            this.Confirmed = confirmed;
            this.Executed = executed;
            this.Canceled = canceled;
            this.Duration = duration;
            this.Protocol = protocol;
            this.Package = package;
            this.Price = price;
            this.PackageSession = packageSession;

            
            this.SetEndDate();

            if (String.IsNullOrEmpty(this.Protocol))
                this.Protocol = "--Nenhum--";

            if (String.IsNullOrEmpty(this.Package))
                this.Package = "--Nenhum--";

            if (!this.Confirmed)
                this.ConfirmedWhenCreated();

            if (!this.isFromDatabase())
                this.Canceled = false;    
        }

        public void SetEndDate() => this.EndDate = this.StartdDate.AddMinutes(this.Duration);

        public void SetStartdDate(DateTime _startdDate) 
        {
            this.StartdDate = _startdDate;
            this.SetEndDate();
        }

        public void SetDuration(int _duration) 
        {
            this.Duration = _duration;
            this.SetEndDate();
        }

        public override void SetItensConstructor()
        {
            base.SetItensConstructor();
            this.ConfirmedWhenCreated();
        }

        private void ConfirmedWhenCreated()        
        {
            if (!this.isFromDatabase())    
            {
                var hours = (this.StartdDate - DateTime.Now).TotalHours;

                if (hours <= 30)
                    this.Confirmed = true;
                else    
                    this.Confirmed = false;
            }
        }

        public void SetExecuted(bool value) => this.Executed = value;

        public void SetConfirmed(bool value) => this.Confirmed = value;

        public bool isStartDateDurationModified(IList<Schedule> originalSchedules)
        {
            bool result = true;
            ((List<Schedule>)originalSchedules).ForEach(_originalSchedule => 
                {
                    if(_originalSchedule.StartdDate == this.StartdDate && _originalSchedule.Duration == this.Duration)
                        result = false;
                }
            );
            return result;
        }

    }



}