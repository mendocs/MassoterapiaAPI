using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Infra.Data.Mongo.Base;
using MongoDB.Driver;
using Massoterapia.Infra.Data.Mongo.Context;
using SharedCore.Repositories;
using System;

namespace Massoterapia.Infra.Data.Mongo.Repositories
{
    public class PatientRepository : RepositoryWrite<Patient>, IPatientRepository
    {

        public PatientRepository(IMongoCollection<Patient> collectionName) : base(collectionName)
        {
        }

        public PatientRepository(IConnectionFactory connectionFactory, string databaseName, string collectionName)
            : base(connectionFactory, databaseName, collectionName)
        {
        }
        public Task<IList<Patient>> QueryByNameOrPhone(string name, string phone)
        {
            var patientsFromDB = _collectionName.AsQueryable<Patient>().Where(w => (w.Name == name || w.Phone == phone));
            IList<Patient> patientsResult = new List<Patient>(patientsFromDB);
            return Task.FromResult(patientsResult);
        }

        public Task<IList<Patient>> QueryLikeNamePhoneScheduledateRange(string name, string phone, IList<DateTime> ScheduledateRange)
        {

            var patientsFromDBQuery = _collectionName.AsQueryable<Patient>().Where(patient => true);

            if (!string.IsNullOrEmpty(name) )
                patientsFromDBQuery = patientsFromDBQuery.Where(patient => patient.Name.Contains(name));

            if (!string.IsNullOrEmpty(phone) )
                patientsFromDBQuery = patientsFromDBQuery.Where(patient => patient.Phone.Contains(phone));

            if (ScheduledateRange != null && ScheduledateRange.Count == 2 && ScheduledateRange[0] != null && ScheduledateRange[1] != null) 
                patientsFromDBQuery = patientsFromDBQuery.Where( patient => patient.Schedules.Any(schedule => schedule.StartdDate >= ScheduledateRange[0].ToUniversalTime() && schedule.StartdDate <= ScheduledateRange[1].ToUniversalTime()));
                            

            IList<Patient> patientsResult = new List<Patient>(patientsFromDBQuery);
            return Task.FromResult(patientsResult);
        }

        Task<Patient> IRepositoryWrite<Patient>.Insert(Patient obj)
        {
            _collectionName.InsertOne(obj);

            Patient patient = _collectionName.AsQueryable<Patient>().FirstOrDefault(w => w.Name == obj.Name && w.Phone == obj.Phone);

            return Task.FromResult(patient);
        }

        
    }
}