using Application.TheGuardianClient;
using Domain;
using HtmlAgilityPack;
using Serilog;

namespace Application.SingleArticleScraper
{
    public class BreitbartSingleArticleScraper : ISingleArticleScraper
    {
        private readonly IWebClient _webClient;
        private readonly ILogger _logger;

        public BreitbartSingleArticleScraper(IWebClient guardianClient, ILogger logger)
        {
            _webClient = guardianClient;
            _logger = logger;
        }

        public async Task<Article?> GetArticleAsync(string url, DateOnly date)
        {
            try
            {
                _logger.Information($"Article scraping started. URL: {url}");
                var response = await _webClient.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                _logger.Information($"Article scraping finished. URL: {url}");

                var title = htmlDoc.DocumentNode.SelectSingleNode("//h1")?.InnerText;
                var mainContent = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='entry-content']")?.InnerText;

                return new Article
                {
                    Title = title,
                    Date = date,
                    Content = mainContent,
                    Orientation = WebsiteType.Breitbart
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
