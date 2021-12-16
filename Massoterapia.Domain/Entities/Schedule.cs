using System.Globalization;
using System;
using SharedCore.Entities;

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

        public Schedule(DateTime startdDate, int duration)
        {
            this.StartdDate = startdDate;
            this.Duration = duration;
            this.SetEndDate();
            //this.Duration = 50; //duração padrão de atendimento
            this.ConfirmedWhenCreated();
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
        


    }
}