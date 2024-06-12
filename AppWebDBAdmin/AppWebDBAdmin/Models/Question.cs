namespace AppWebDBAdmin.Models
{
    public class Question
    {
        public string? QuestionText { get; set; }
        public string? QuestionAnswer { get; set; }
        public Guid? DatabaseId { get; set; }

        public Database? Database { get; set; }
    }
}
