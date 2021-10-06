using System.IO;
using Massoterapia.Infra.Data.Mongo.Context;
using Massoterapia.Infra.IoC.Settings;
using Microsoft.Extensions.Configuration;

namespace Massoterapia.Application.Tests
{
    public static class RepositoryConfiguration
    {

        public static string DatabaseName;

        public static string CollectionUser;
        public static string CollectionPatient;

        public static string CollectionBlog;

        public static ConnectionFactory ConnFactoryCreated;
        public static ConnectionFactory ConnFactory ()
        {

            if (ConnFactoryCreated!=null)
                return ConnFactoryCreated;

            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(@"J:\Desenvolvimento\Projetos\Massoterapia\MassoterapiaAPI\Massoterapia.API\appsettings.Development.json")
            .Build();

            var mongoDbSettings = config.GetSection("MongoDBSetting").Get<MongoDBSetting>();    

            DatabaseName = mongoDbSettings.DatabaseName;    

            CollectionUser = mongoDbSettings.CollectionNameUser;

            CollectionPatient = mongoDbSettings.CollectionNamePatient;

            CollectionBlog = mongoDbSettings.CollectionNameBlog;

            Massoterapia.Infra.Data.Mongo.Configurations.GlobalConfigurations.ConventionPack_IgnoreExtraElements();

            Massoterapia.Infra.Data.Mongo.Configurations.UserConfiguration.UserMapping();

            var connectionFactory = new ConnectionFactory(mongoDbSettings.ConnectionString);     

            ConnFactoryCreated = connectionFactory;

            return connectionFactory;  
        }
        
    }
}