using System.Reflection;
using Massoterapia.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Massoterapia.Domain.Tests.ConfigureJson
{
    public class JsonIgnoreContractResolver: DefaultContractResolver
    {
        public static JsonIgnoreContractResolver Instance { get; } = new JsonIgnoreContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);        
            if (typeof(Patient).IsAssignableFrom(member.DeclaringType) && member.Name == nameof(Patient.Updated))
            {
                property.Ignored = true;
            }
            return property;
        }        
    }
}



