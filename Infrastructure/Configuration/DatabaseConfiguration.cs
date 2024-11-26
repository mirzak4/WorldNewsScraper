namespace Infrastructure.Configuration
{
    public class DatabaseConfiguration
    {
        public static string Name => "Database";
        public string ConnectionString { get; set; }
    }
}
