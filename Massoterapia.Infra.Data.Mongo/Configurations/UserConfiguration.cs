using System.Security.Cryptography;
using Massoterapia.Domain.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Massoterapia.Infra.Data.Mongo.Configurations
{
    public static class UserConfiguration
    {
        public static void UserMapping()
        {

            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();// Automap the Role class
                cm.UnmapProperty(c => c.Password_Text);
                cm.UnmapProperty(c => c.Nhash);
                cm.UnmapProperty(c => c.NumberSalt);
                cm.UnmapProperty(c => c.Interation);
            });             
        }
        
    }
}