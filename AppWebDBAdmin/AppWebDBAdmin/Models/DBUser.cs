namespace AppWebDBAdmin.Models
{
    public class DBUser
    {
        public string Id;
        public string Email;
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string HuggingFaceAPIToken { get; set; }
        public ICollection<Database>? Databases { get; set; }
    }
}
