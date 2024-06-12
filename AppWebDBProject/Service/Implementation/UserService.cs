using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Identity;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public DBUser GetUser(string id)
        {
            return _userRepository.Get(id);
        }

        public List<DBUser> GetUsers()
        {
            return _userRepository.GetAll().ToList();
        }
    }
}
