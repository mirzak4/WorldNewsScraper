using Application;
using Application.Services;
using Application.TheGuardianClient;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.TheGuardianClient;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            services.AddScoped<ITheGuardianClient, TheGuardianClient>();
            services.AddScoped<SingleArticleScraper>();
            services.AddScoped<DailyArticlesProcessor>();
            services.AddScoped<GlobalArticlesProcessor>();
        }
    }
}
