using Application.SingleArticleScraper;
using Application.TheGuardianClient;
using HtmlAgilityPack;
using Serilog;

namespace Application.DailyArticlesProcessor
{
    public class TheGuardianDailyArticlesProcessor : IDailyArticlesProcessor
    {
        private readonly IWebClient _guardianClient;
        private readonly ISingleArticleScraper _singleArticleScraper;
        private readonly ILogger _logger;

        public TheGuardianDailyArticlesProcessor(IWebClient guardianClient, ILogger logger, SingleArticleScrapperFactory singleArticleScrapperFactory)
        {
            _guardianClient = guardianClient;
            _singleArticleScraper = singleArticleScrapperFactory.Get(Domain.WebsiteType.TheGuardian);
            _logger = logger;
        }

        public async Task<IEnumerable<Domain.Article>> FetchArticlesAsync(DateOnly date)
        {
            List<Domain.Article> articles = [];

            try
            {
                var articleUrls = await GetArticlesUrlsForDate(date);
                if (articleUrls is null || !articleUrls.Any())
                {
                    // Scraping failure can sometimes happen because of too many requests we send
                    // In that case wait for 1min and repeat process
                    await Task.Delay(60000);
                    articleUrls = await GetArticlesUrlsForDate(date);
                }

                foreach (var articleUrl in articleUrls)
                {
                    var article = await _singleArticleScraper.GetArticleAsync(articleUrl, date);
                    if (article is null)
                    {
                        // Scraping failure can sometimes happen because of too many requests we send
                        // In that case wait for 1min and repeat process
                        await Task.Delay(60000);
                        article = await _singleArticleScraper.GetArticleAsync(articleUrl, date);
                    }
                    if (article is not null)
                    {
                        articles.Add(article);
                    }
                }

                _logger.Information($"Fetching articles for {date} finished successfully.");
                return articles;
            }
            catch (Exception ex)
            {
                _logger.Error($"Fetching articles for {date} failed. Exception: {ex.Message}");
                return [];
            }
        }

        private async Task<IEnumerable<string>> GetArticlesUrlsForDate(DateOnly date)
        {
            string url = $"https://www.theguardian.com/us-news/{date.Year}/{date.ToString("MMM").ToLower()}/{date.Day.ToString("D2")}/all";

            try
            {
                _logger.Information($"Fetching articles for {date} started.");
                var response = await _guardianClient.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                return htmlDoc
                        .DocumentNode
                        .SelectNodes("//div[contains(@class, 'fc-item__container')]//a/@href\r\n")
                        .Select(x => x.GetAttributeValue("href", string.Empty))
                        .Distinct();
            }
            catch (Exception ex)
            {
                _logger.Error($"Fetching articles for {date} failed. Exception: {ex.Message}");
                return [];
            }
        }
    }
}
