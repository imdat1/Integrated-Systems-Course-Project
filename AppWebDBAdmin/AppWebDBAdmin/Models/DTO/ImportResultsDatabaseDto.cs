namespace AppWebDBAdmin.Models.DTO
{
    public class ImportResultsDatabaseDto
    {
        public bool? Status { get; set; }
        public List<Database>? InvalidDatabases { get; set; }
    }
}
