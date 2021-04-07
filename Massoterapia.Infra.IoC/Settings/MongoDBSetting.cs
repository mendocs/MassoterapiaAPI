using System;
using System.Collections.Generic;
using System.Text;

namespace Massoterapia.Infra.IoC.Settings
{
    public sealed class MongoDBSetting
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
    }
}
