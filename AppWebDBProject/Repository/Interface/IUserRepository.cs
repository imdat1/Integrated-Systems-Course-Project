using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Identity;

namespace Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<DBUser> GetAll();
        DBUser Get(string? id);
        void Insert(DBUser entity);
        void Update(DBUser entity);
        void Delete(DBUser entity);
    }
}
