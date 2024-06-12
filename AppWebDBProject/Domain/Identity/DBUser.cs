using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Domain;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class DBUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string HuggingFaceAPIToken { get; set; }
        public ICollection<Database>? Databases { get; set; }
    }
}
