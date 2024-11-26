namespace Domain
{
    public class Article
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateOnly Date { get; set; }
        public string? Content { get; set; }
    }
}
