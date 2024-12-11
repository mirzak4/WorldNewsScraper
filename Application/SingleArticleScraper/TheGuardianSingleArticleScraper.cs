using Application.TheGuardianClient;
using Domain;
using HtmlAgilityPack;
using Serilog;

namespace Application.SingleArticleScraper
{
    public class TheGuardianSingleArticleScraper : ISingleArticleScraper
    {
        private readonly IWebClient _guardianClient;
        private readonly ILogger _logger;

        public TheGuardianSingleArticleScraper(IWebClient guardianClient, ILogger logger)
        {
            _guardianClient = guardianClient;
            _logger = logger;
        }

        public async Task<Article?> GetArticleAsync(string url, DateOnly date)
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
                    Content = $"{standFirstText}. {mainContent}",
                    Orientation = 0
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

