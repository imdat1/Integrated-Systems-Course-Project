using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Domain;

namespace Service.Interface
{
    public interface IDatabaseService
    {
        List<Database> GetAllDatabases();
        Database GetDetailsForDatabase(Guid? id);
        void CreateNewDatabase(Database p, string userId);
        void UpdateExistingDatabase(Database p, string userId);
        void DeleteDatabase(Guid id, string userId);
        List<Database> GetDatabasesForUser(string userId);
        Database GetDetailsForDatabase(BaseEntity id);
    }
}
