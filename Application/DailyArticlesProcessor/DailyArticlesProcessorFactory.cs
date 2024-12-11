using Application.SingleArticleScraper;
using Application.TheGuardianClient;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application.DailyArticlesProcessor
{
    public class DailyArticlesProcessorFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DailyArticlesProcessorFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IDailyArticlesProcessor Get(WebsiteType type)
        {
            var webClient = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IWebClient>();
            var logger = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ILogger>();
            var singleArticleScraperFactory = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<SingleArticleScrapperFactory>();

            return type switch
            {
                WebsiteType.TheGuardian => new TheGuardianDailyArticlesProcessor(webClient, logger, singleArticleScraperFactory),
                WebsiteType.Breitbart => new BreitbartDailyArticlesProcessor(webClient, logger, singleArticleScraperFactory),
                _ => throw new ArgumentException("Invalid website type")
            };
        }
    }
}
