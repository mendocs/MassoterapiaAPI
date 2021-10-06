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

           var myProfileBlogDomainToViewModel = new Massoterapia.Application.Blog.Mappings.BlogDomainToBlogViewModelMappingProfile();
           var myProfileBlogInputToDomain = new Massoterapia.Application.Blog.Mappings.BlogInputModelToBlogDomainMappingProfile();


            List<Profile> profiles  = new List<Profile>();
            profiles.Add(myProfileUser);
            profiles.Add(myProfilePatient);
            profiles.Add(myProfileBlogDomainToViewModel);
            profiles.Add(myProfileBlogInputToDomain);


            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(profiles) );

            IMapper mapper = configuration.CreateMapper();  //new Mapper(configuration);

            //services.AddSingleton(mapper);
            services.AddSingleton<IMapper>(p => mapper);
            
        }        
    }
}