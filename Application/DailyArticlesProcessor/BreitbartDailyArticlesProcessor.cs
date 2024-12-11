using Application.SingleArticleScraper;
using Application.TheGuardianClient;
using Domain;
using HtmlAgilityPack;
using Serilog;

namespace Application.DailyArticlesProcessor
{
    public class BreitbartDailyArticlesProcessor : IDailyArticlesProcessor
    {
        private readonly IWebClient _webClient;
        private readonly ISingleArticleScraper _singleArticleScraper;
        private readonly ILogger _logger;

        public BreitbartDailyArticlesProcessor(IWebClient guardianClient, ILogger logger, SingleArticleScrapperFactory singleArticleScrapperFactory)
        {
            _webClient = guardianClient;
            _singleArticleScraper = singleArticleScrapperFactory.Get(WebsiteType.Breitbart);
            _logger = logger;
        }

        public async Task<IEnumerable<Article>> FetchArticlesAsync(DateOnly date)
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
            string url = $"https://www.breitbart.com/clips/{date.Year}/{date.Month.ToString("D2")}/{date.Day.ToString("D2")}";

            try
            {
                _logger.Information($"Fetching articles for {date} started.");
                var response = await _webClient.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                return htmlDoc
                        .DocumentNode
                        .SelectNodes("//article/a\r\n")
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
