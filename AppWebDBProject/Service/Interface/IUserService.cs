using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Identity;

namespace Service.Interface
{
    public interface IUserService
    {
        List<DBUser> GetUsers();
        DBUser GetUser(string id);

    }
}
