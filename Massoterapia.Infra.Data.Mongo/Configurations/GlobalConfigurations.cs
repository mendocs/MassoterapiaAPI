using MongoDB.Bson.Serialization.Conventions;

namespace Massoterapia.Infra.Data.Mongo.Configurations
{
    public class GlobalConfigurations
    {
        public static void ConventionPack_IgnoreExtraElements()
        {
            var pack = new ConventionPack();
            pack.Add(new IgnoreExtraElementsConvention(true));
            ConventionRegistry.Register("ApplicationConventions", pack, t => true);
       
        }        
    }
}