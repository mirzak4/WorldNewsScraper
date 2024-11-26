using Infrastructure.Configuration;

namespace TheGuardianScraper
{
    public static class Bootstrapper
    {
        public static void AddRequired(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<DatabaseConfiguration>(configuration.GetSection(DatabaseConfiguration.Name));
            services.AddRequired();
        }
    }
}
