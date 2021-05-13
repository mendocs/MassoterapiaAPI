using System.Globalization;
using System;
using SharedCore.Entities;

namespace Massoterapia.Domain.Entities
{
    public class Schedule: Entity
    {
        public DateTime StartdDate { get; private set; }
        public string Comments { get; private set; }
        public bool Confirmed { get; private set; }
        public bool Executed { get; private set; }
        public bool Canceled { get; private set; }


        public Schedule(DateTime startdDate)
        {
            this.StartdDate = startdDate;
            this.ConfirmedWhenCreated();
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
            }
        }

        public void SetExecuted(bool value) => this.Executed = value;

        public void SetConfirmed(bool value) => this.Confirmed = value;
        


    }
}