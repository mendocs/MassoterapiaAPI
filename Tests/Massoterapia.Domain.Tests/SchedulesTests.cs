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

namespace Massoterapia.Domain.Tests
{
    public class SchedulesTests
    {
        [Fact]
        public void schedule_contract_not_valid()
        {
            var schedule = new  Schedule(DateTime.Now.AddDays(-13),50);            

            ScheduleValidationContract scheduleValidationContract = new ScheduleValidationContract(schedule);

            var msg = scheduleValidationContract.Notifications.AllInvalidations();

            Assert.False(scheduleValidationContract.IsValid);

        }        

        [Fact]
        public void schedule_contract_valid()
        {
            var schedule = new  Schedule(DateTime.Now.AddDays(1),50);            

            ScheduleValidationContract scheduleValidationContract = new ScheduleValidationContract(schedule);

            Assert.True(scheduleValidationContract.IsValid);

        } 

        [Fact]
        public void schedule_contract_confirmed_30h()
        {
            var schedule = new  Schedule(DateTime.Now.AddHours(20),50);            
            
            Assert.True(schedule.Confirmed);

        } 


        [Fact]
        public void schedule_contract_not_confirmed_30h()
        {

            var ddd = TimeZoneInfo.GetSystemTimeZones();
            var schedule = new  Schedule(DateTime.Now.AddHours(70),50);            
            
            Assert.False(schedule.Confirmed);

        } 



    }
}