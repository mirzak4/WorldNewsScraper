using Application.DailyArticlesProcessor;
using Application.Services;
using Application.SingleArticleScraper;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.GlobalArticlesProcessor
{
    public class GlobalArticlesProcessorFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GlobalArticlesProcessorFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IGlobalArticlesProcessor Get(WebsiteType type)
        {
            var unitOfWork = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var dailyArticlesProcessorFactory = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<DailyArticlesProcessorFactory>();
            var singleArticleScraperFactory = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<SingleArticleScrapperFactory>();

            return type switch
            {
                WebsiteType.TheGuardian => new TheGuardianGlobalArticlesProcessor(unitOfWork, dailyArticlesProcessorFactory, singleArticleScraperFactory),
                WebsiteType.Breitbart => new BreitbartGlobalArticlesProcessor(unitOfWork, dailyArticlesProcessorFactory, singleArticleScraperFactory),
                _ => throw new ArgumentException("Invalid website type")
            };
        }
    }
}
