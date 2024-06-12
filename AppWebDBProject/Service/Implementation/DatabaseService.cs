using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Domain;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IRepository<Database> _databaseRepository;
        private readonly IUserRepository _userRepository;

        public DatabaseService(IRepository<Database> repository, IUserRepository userRepository)
        {
            _databaseRepository = repository;
            _userRepository = userRepository;
        }

        public void CreateNewDatabase(Database p, string userId)
        {
            var user = _userRepository.Get(userId);
            if(user.Databases == null)
            {
                user.Databases = new List<Database>();
            }
            user.Databases.Add(p);
            this._databaseRepository.Insert(p);
            _userRepository.Update(user);

            var novUs = _userRepository.Get(user.Id);
            foreach(var u in novUs.Databases) {
                Console.WriteLine("User "+user.Id+" with database "+ u.DatabaseName);
            }
           
        }

        public void DeleteDatabase(Guid id, string userId)
        {
            var user = _userRepository.Get(userId);
            var database = _databaseRepository.Get(id);
            user.Databases.Remove(database);
            _userRepository.Update(user);
            this._databaseRepository.Delete(database);
        }

        public List<Database> GetAllDatabases()
        {
            return this._databaseRepository.GetAll().ToList();
        }

        public List<Database> GetDatabasesForUser(string userId)
        {
            var loggedInUser = _userRepository.Get(userId);
            var loggedInUserDatabases = loggedInUser.Databases;
            if (loggedInUserDatabases == null)
            {
                loggedInUserDatabases = new List<Database>();
            }
            return loggedInUserDatabases.ToList();
        }

        public Database GetDetailsForDatabase(Guid? id)
        {
            var database = this._databaseRepository.Get(id);
            return database;
        }

        public Database GetDetailsForDatabase(BaseEntity id)
        {
            return this._databaseRepository.Get(id);
        }

        public void UpdateExistingDatabase(Database p, string userId)
        {
            _databaseRepository.Update(p);
            var db = _databaseRepository.Get(p.Id);
            var user = _userRepository.Get(userId);
            user.Databases.Remove(db);
            user.Databases.Add(db);
            _userRepository.Update(user);
        }
    }
}
