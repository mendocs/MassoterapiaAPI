using System;
using Flunt.Validations;
using Massoterapia.Domain.Entities;

namespace Massoterapia.Domain.Validations
{
    public class ScheduleValidationContract: Contract<Schedule>
    {
        public ScheduleValidationContract(Schedule schedule)
        {
            Requires()
                .IsNotNull(schedule.StartdDate,"Data Atendimento","Data não pode ser vazio")
                .IsGreaterOrEqualsThan(schedule.StartdDate, DateTime.Now,"Data Atendimento",$"Data {SharedCore.tools.DateTimeTools.ConvertDateToString(schedule.StartdDate)} do atendimento não pode ser menor que a data atual {SharedCore.tools.DateTimeTools.ConvertDateToString(DateTime.Now)}")
            ;
        }              
    }
}