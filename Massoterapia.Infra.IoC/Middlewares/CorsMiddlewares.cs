using Massoterapia.Infra.IoC.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Massoterapia.Infra.IoC.Middlewares
{
    public static class CorsMiddlewares
    {
        public static void AddCorsPolicyMiddleware(this IServiceCollection services, IConfiguration configuration)
        {
           var corsSettings = configuration.GetSection("CorsSetting").Get<CorsSetting>();

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                    //builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build();
                    builder.WithOrigins(corsSettings.Origins);
                });
            });   
            
        }                
    }
}