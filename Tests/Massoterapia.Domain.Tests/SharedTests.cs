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
    public class SharedTests
    {
        [Fact]
        public void time_zone_convertToString()
        {

            DateTime data = new DateTime(2021,5,16,18,25,0,DateTimeKind.Utc);
            var dateString = SharedCore.tools.DateTimeTools.ConvertDateToString(data);
            
            
            Assert.Equal("16/05/2021 15:25", dateString);

        }         

    }
}