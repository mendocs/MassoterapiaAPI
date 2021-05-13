using System;
using System.Collections.Generic;
using System.Text;

namespace Massoterapia.Infra.IoC.Settings
{
    public sealed class MongoDBSetting
    {
        public string DatabaseName { get; set; }
        public string CollectionNameUser { get; set; }
        public string CollectionNamePatient { get; set; }
        public string ConnectionString { get; set; }
    }
}
