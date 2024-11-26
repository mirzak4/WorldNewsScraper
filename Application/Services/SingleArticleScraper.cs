using Application.TheGuardianClient;
using Domain;
using HtmlAgilityPack;
using Serilog;

namespace Application.Services
{
    public class SingleArticleScraper
    {
        private readonly ITheGuardianClient _guardianClient;
        private readonly ILogger _logger;

        public SingleArticleScraper(ITheGuardianClient guardianClient, ILogger logger)
        {
            _guardianClient = guardianClient;
            _logger = logger;
        }

        public async Task<Article?> GetArticle(string url, DateOnly date)
        {
            try
            {
                _logger.Information($"Article scraping started. URL: {url}");
                var response = await _guardianClient.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                _logger.Information($"Article scraping finished. URL: {url}");

                var title = htmlDoc.DocumentNode.SelectSingleNode("//h1")?.InnerText;
                var standFirstText = htmlDoc.DocumentNode.SelectSingleNode("//*[@data-gu-name='standfirst']")?.InnerText;
                var mainContent = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='maincontent']")?.InnerText;

                return new Article
                {
                    Title = title,
                    Date = date,
                    Content = $"{standFirstText}. {mainContent}"
                };
            }
            catch (Exception ex)
            {
                _logger.Error($"Article scraping failed. URL: {url}.");
                return null;
            }
        }
    }
}

