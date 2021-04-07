using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Application.user.Interfaces;
using Massoterapia.Application.user.Services;
using Massoterapia.Infra.Data.Mongo.Context;
using Massoterapia.Infra.Data.Mongo.Repositories;
using Massoterapia.Infra.IoC.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Massoterapia.Application.user.models;

namespace Massoterapia.Infra.IoC.Middlewares
{
    public static class DependencyInjectionMiddleware
    {
        public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings = configuration.GetSection("MongoDBSetting").Get<MongoDBSetting>();

            Massoterapia.Infra.Data.Mongo.Configurations.GlobalConfigurations.ConventionPack_IgnoreExtraElements();

            Massoterapia.Infra.Data.Mongo.Configurations.UserConfiguration.UserMapping();

            var connectionFactory = new ConnectionFactory(mongoDbSettings.ConnectionString);

            services.AddSingleton<IUserRepository>(
                p => new UserRepository(connectionFactory, mongoDbSettings.DatabaseName,
                    mongoDbSettings.CollectionName));

            services.AddTransient<IUserService, UserService>();
        }
    }
}