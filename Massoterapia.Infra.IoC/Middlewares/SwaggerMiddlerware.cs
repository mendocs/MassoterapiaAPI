
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace Massoterapia.Infra.IoC.Middlewares
{
    public static class SwaggerMiddlerware
    {
        public static void AddSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MassoterapiaAPI", Version = "v1", Description = "MassoterapiaApi Api" });
                /*
                c.SwaggerDoc("v1", 
                    new Info
                    {
                        Title = "MassoterapiaApi",
                        Version = "v1",
                        Description = "MassoterapiaApi Api",
                        Contact = new Contact
                        {
                            Name = "Ulisses Mendonça Jr",
                            Url = "https://github.com/alexandrebl/MongoDbCosmosDbApi"
                        }
                    });

                var caminhoAplicacao =
                    PlatformServices.Default.Application.ApplicationBasePath;
                var caminhoXmlDoc =
                    Path.Combine(caminhoAplicacao, $"MongoDbCosmosDbApi.xml");

                c.IncludeXmlComments(caminhoXmlDoc);*/
            });
        }

        public static void UseSwaggerApp(this IApplicationBuilder app, string routePrefix)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "MassoterapiaApi");

                c.RoutePrefix = routePrefix;
            });
        }
    }
}