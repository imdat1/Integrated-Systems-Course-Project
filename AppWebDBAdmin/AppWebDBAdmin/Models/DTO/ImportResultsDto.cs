namespace AppWebDBAdmin.Models.DTO
{
    public class ImportResultsDto
    {
        public bool Status { get; set; }
        public List<UserRegistrationDto>? InvalidUsers { get; set; }
    }
}
