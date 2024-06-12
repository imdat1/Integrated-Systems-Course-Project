using Microsoft.AspNetCore.Identity;

namespace AppWebDB.Models
{
    public class DBUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string HuggingFaceAPIToken{ get; set; }
    }
}
