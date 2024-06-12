using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Identity;

namespace Domain.Domain
{
    public class Database : BaseEntity
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string DatabaseName { get; set; }
        public string? OwnerId { get; set; }
        public DBUser? Owner { get; set; }
        public ICollection<Question>? Questions { get; set; }
    }
}
