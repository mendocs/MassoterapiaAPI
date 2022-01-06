using System.IO;
using System.Net.Mime;
using System.Reflection;
using System;
using Xunit;
using Massoterapia.Domain.Entities;
using Newtonsoft.Json;
using JsonNet.ContractResolvers;
using Newtonsoft.Json.Linq;
using Massoterapia.Domain.Validations;
using System.Collections.Generic;

namespace Massoterapia.Domain.Tests
{
    public class SchedulesTests
    {
        [Fact]
        public void schedule_contract_not_valid()
        {
            var schedule = new  Schedule(DateTime.Now.AddDays(-13),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);

            ScheduleValidationContract scheduleValidationContract = new ScheduleValidationContract(schedule);

            var msg = scheduleValidationContract.Notifications.AllInvalidations();

            Assert.False(scheduleValidationContract.IsValid);

        }        

        [Fact]
        public void schedule_contract_valid()
        {
            var schedule = new  Schedule(DateTime.Now.AddDays(1),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);            

            ScheduleValidationContract scheduleValidationContract = new ScheduleValidationContract(schedule);

            Assert.True(scheduleValidationContract.IsValid);

        } 

        [Fact]
        public void schedule_contract_confirmed_30h()
        {
            var schedule = new  Schedule(DateTime.Now.AddHours(20),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);            
            
            Assert.True(schedule.Confirmed);

        } 


        [Fact]
        public void schedule_contract_not_confirmed_30h()
        {

            var ddd = TimeZoneInfo.GetSystemTimeZones();
            var schedule = new Schedule(DateTime.Now.AddHours(70),false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
            
            Assert.False(schedule.Confirmed);

        } 


        [Fact]
        public void schedule_isScheduleStartDateDurationModified_not_modified()
        {

            IList<Schedule> SchedulesList = new List<Schedule>();
            var dataSchedule1 = new DateTime(2021,12,27,13,30,00,DateTimeKind.Utc);
            var dataSchedule2 = dataSchedule1.AddDays(1);

            var schedule1 = new Schedule(dataSchedule1,false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
            var schedule2 = new Schedule(dataSchedule2,false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);

            SchedulesList.Add(schedule1);
            SchedulesList.Add(schedule2);

            var schedule3 = new Schedule(dataSchedule1,false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
            
            Assert.False(schedule3.isStartDateDurationModified(SchedulesList));

        } 


        [Fact]
        public void schedule_isScheduleStartDateDurationModified_modifiedStartDate()
        {

            IList<Schedule> SchedulesList = new List<Schedule>();
            var dataSchedule1 = new DateTime(2021,12,27,13,30,00,DateTimeKind.Utc);
            var dataSchedule2 = dataSchedule1.AddDays(1);
            var dataSchedule3 = dataSchedule1.AddDays(2);

            var schedule1 = new Schedule(dataSchedule1,false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
            var schedule2 = new Schedule(dataSchedule2,false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);

            SchedulesList.Add(schedule1);
            SchedulesList.Add(schedule2);

            var schedule3 = new Schedule(dataSchedule3,false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
            
            Assert.True(schedule3.isStartDateDurationModified(SchedulesList));

        } 

        [Fact]
        public void schedule_isScheduleStartDateDurationModified_modified_duration()
        {

            IList<Schedule> SchedulesList = new List<Schedule>();
            var dataSchedule1 = new DateTime(2021,12,27,13,30,00,DateTimeKind.Utc);
            var dataSchedule2 = dataSchedule1.AddDays(1);

            var schedule1 = new Schedule(dataSchedule1,false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
            var schedule2 = new Schedule(dataSchedule2,false,"",false,50,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);

            SchedulesList.Add(schedule1);
            SchedulesList.Add(schedule2);

            var schedule3 = new Schedule(dataSchedule1,false,"",false,51,new DateTime(),false,"--Nenhum--","--Nenhum--",0,0);
            
            Assert.True(schedule3.isStartDateDurationModified(SchedulesList));

        } 


    }
}