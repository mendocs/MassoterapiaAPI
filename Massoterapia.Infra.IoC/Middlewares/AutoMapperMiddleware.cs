using System.Runtime.Serialization;
using System.Net.Security;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Massoterapia.Infra.IoC.Middlewares
{
    public static class AutoMapperMiddleware
    {
        public static void AddAutoMapperMiddleware(this IServiceCollection services)
        {
           var myProfile = new Massoterapia.Application.user.Mappings.UserDomainToUserTobeCreatedMappingProfile();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

            IMapper mapper = configuration.CreateMapper();  //new Mapper(configuration);

            //services.AddSingleton(mapper);
            services.AddSingleton<IMapper>(p => mapper);
            
        }        
    }
}