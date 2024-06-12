namespace AppWebDBAdmin.Models
{
    public class Database
    {
        public Guid Id { get; set; }
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
