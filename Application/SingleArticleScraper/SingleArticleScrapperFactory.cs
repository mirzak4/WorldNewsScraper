using Application.TheGuardianClient;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application.SingleArticleScraper
{
    public class SingleArticleScrapperFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SingleArticleScrapperFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ISingleArticleScraper Get(WebsiteType type)
        {
            var webClient = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IWebClient>();
            var logger = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ILogger>();

            return type switch
            {
                WebsiteType.TheGuardian => new TheGuardianSingleArticleScraper(webClient, logger),
                WebsiteType.Breitbart => new BreitbartSingleArticleScraper(webClient, logger),
                _ => throw new ArgumentException("Invalid website type")
            };
        }
    }
}
