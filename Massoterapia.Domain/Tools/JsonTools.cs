using Massoterapia.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Massoterapia.Domain.Tools
{
    public static class JsonTools
    {

        private static JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }


        public static Patient loadPatientFromJson(string JsonBody )
        {
          var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            //var patientFromJson = JsonConvert.DeserializeObject<Patient>(JsonBody,settings);
            var patientFromJson = new Patient();

            JsonConvert.PopulateObject(JsonBody,patientFromJson,settings);

            return patientFromJson;
        }

        public static Blog loadBlogFromJson(string JsonBody )
        {
         
            var blogFromJson = new Blog();

            JsonConvert.PopulateObject(JsonBody,blogFromJson,GetSettings());

            return blogFromJson;
        }          
                 
    }
}