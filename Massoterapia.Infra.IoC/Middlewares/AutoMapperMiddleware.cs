using System.Runtime.Serialization;
using System.Net.Security;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Massoterapia.Infra.IoC.Middlewares
{
    public static class AutoMapperMiddleware
    {
        public static void AddAutoMapperMiddleware(this IServiceCollection services)
        {
           var myProfileUser = new Massoterapia.Application.user.Mappings.UserDomainToUserTobeCreatedMappingProfile();
           var myProfilePatient = new Massoterapia.Application.Patient.Mappings.PatientDomainToPatientViewModelListMappingProfile();


            List<Profile> profiles  = new List<Profile>();
            profiles.Add(myProfileUser);
            profiles.Add(myProfilePatient);


            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(profiles) );

            IMapper mapper = configuration.CreateMapper();  //new Mapper(configuration);

            //services.AddSingleton(mapper);
            services.AddSingleton<IMapper>(p => mapper);
            
        }        
    }
}