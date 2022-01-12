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
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;

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
            var patientsFromDB = _collectionName.AsQueryable<Patient>().Where(w => (w.Name.ToLower() == name.ToLower() || w.Phone == phone));
            
            return ResultPatientsFromDbQuery(patientsFromDB);
        }


        public Task<IList<Patient>> QueryByName(string name)
        {
            var patientsFromDB = _collectionName.AsQueryable<Patient>().Where(w => w.Name.ToLower() == name.ToLower());
            
            return ResultPatientsFromDbQuery(patientsFromDB);
        }
        
        public Task<IList<Patient>> QueryLikeNamePhoneScheduledateRange(string name, string phone, IList<DateTime> ScheduledateRange)
        {
            var patientsFromDBQuery = CreateQueriableBase();

            if (!string.IsNullOrEmpty(name) )
                patientsFromDBQuery = patientsFromDBQuery.Where(patient => patient.Name.ToLower().Contains(name.ToLower()));
            
            if (!string.IsNullOrEmpty(phone) )
                patientsFromDBQuery = patientsFromDBQuery.Where(patient => patient.Phone.Contains(phone));

            if (ScheduledateRange?.Count == 2) 
            {
                patientsFromDBQuery = patientsFromDBQuery.Where( patient => patient.Schedules.Any(schedule => schedule.StartdDate >= ScheduledateRange[0] && schedule.StartdDate <= ScheduledateRange[1]));
            }               

            return ResultPatientsFromDbQuery(patientsFromDBQuery);
        }

        private IMongoQueryable<Patient> CreateQueriableBase()
        {
            
            return _collectionName.AsQueryable<Patient>().Where(patient => true);
        }
        private Task<IList<Patient>> ResultPatientsFromDbQuery(IQueryable<Patient> patientsFromDBQuery)
        {
            IList<Patient> patientsResult = new List<Patient>(patientsFromDBQuery);
            return Task.FromResult(patientsResult);
        }

        public Task<IList<Patient>> QueryScheduledateRangeForScheduleFree(IList<DateTime> ScheduledateRange)
        {
            var patientsFromDBQuery = CreateQueriableBase();
           
           patientsFromDBQuery = patientsFromDBQuery.Where( patient => 
            patient.Schedules.Any(_schedule => 
                (_schedule.StartdDate >= ScheduledateRange[0] && _schedule.StartdDate <= ScheduledateRange[1]) ||
                (_schedule.EndDate >= ScheduledateRange[0] && _schedule.EndDate <= ScheduledateRange[1])
                ));

            return ResultPatientsFromDbQuery(patientsFromDBQuery);   
        }


        private Func<Schedule, bool> ScheduleHourbetween(DateTime scheduleDatetimeComparation)
        {
            return (_schedule => _schedule.StartdDate <= scheduleDatetimeComparation && _schedule.EndDate >= scheduleDatetimeComparation);
        }

        Task<Patient> IRepositoryWrite<Patient>.Insert(Patient obj)
        {
            _collectionName.InsertOne(obj);

            Patient patient = _collectionName.AsQueryable<Patient>().FirstOrDefault(w => w.Name == obj.Name && w.Phone == obj.Phone);

            return Task.FromResult(patient);
        }

        
    }
}