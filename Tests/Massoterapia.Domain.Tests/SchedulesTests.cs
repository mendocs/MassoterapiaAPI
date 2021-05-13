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
            var schedule = new  Schedule(DateTime.Now.AddDays(-13));            

            ScheduleValidationContract scheduleValidationContract = new ScheduleValidationContract(schedule);

            var msg = scheduleValidationContract.Notifications.AllInvalidations();

            Assert.False(scheduleValidationContract.IsValid);

        }        

        [Fact]
        public void schedule_contract_valid()
        {
            var schedule = new  Schedule(DateTime.Now.AddDays(1));            

            ScheduleValidationContract scheduleValidationContract = new ScheduleValidationContract(schedule);

            Assert.True(scheduleValidationContract.IsValid);

        }                
    }
}