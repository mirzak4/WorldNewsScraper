using Application;
using Application.DailyArticlesProcessor;
using Application.GlobalArticlesProcessor;
using Application.SingleArticleScraper;
using Application.TheGuardianClient;
using Infrastructure.Persistence;
using Infrastructure.TheGuardianClient;
using Infrastructure.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace TheGuardianScraper
{
    public static class Bootstrapper
    {
        public static void AddRequired(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
            {
                var dbContext = provider.GetRequiredService<ApplicationDbContext>();
                return new UnitOfWork(dbContext);
            });
            services.AddScoped<IWebClient, WebClient>();
            services.AddScoped<SingleArticleScrapperFactory>();
            services.AddScoped<DailyArticlesProcessorFactory>();
            services.AddScoped<GlobalArticlesProcessorFactory>();
        }
    }
}
