using Application.DailyArticlesProcessor;
using Application.GlobalArticlesProcessor;
using Application.SingleArticleScraper;
using HtmlAgilityPack;

namespace Application.Services
{
    public class TheGuardianGlobalArticlesProcessor : IGlobalArticlesProcessor
    {
        private readonly IDailyArticlesProcessor _articlesProcessor;
        private readonly ISingleArticleScraper _singleArticleScraper;
        private readonly IUnitOfWork _unitOfWork;

        public TheGuardianGlobalArticlesProcessor(IUnitOfWork unitOfWork, DailyArticlesProcessorFactory dailyArticlesProcessorFactory, SingleArticleScrapperFactory singleArticleScrapperFactory)
        {
            _articlesProcessor = dailyArticlesProcessorFactory.Get(Domain.WebsiteType.TheGuardian);
            _unitOfWork = unitOfWork;
            _singleArticleScraper = singleArticleScrapperFactory.Get(Domain.WebsiteType.TheGuardian);
        }

        public async Task StartProcessingAsync(DateOnly startDate, DateOnly endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be less than end date");
            }

            var current = startDate;
            while (current <= endDate)
            {
                var articles = await _articlesProcessor.FetchArticlesAsync(current) ?? [];
                // Save articles to db
                if (articles.Any())
                {
                    await _unitOfWork.AddArticlesAsync(articles);
                    await _unitOfWork.CommitAsync();
                }
                current = current.AddDays(1);
            }
        }

        public async Task RetryFailedDays(string date)
        {
            var dateOnly = DateOnly.ParseExact(date.Trim(), "MM/dd/yyyy");
            var articles = await _articlesProcessor.FetchArticlesAsync(dateOnly) ?? [];
            // Save articles to db
            if (articles.Any())
            {
                await _unitOfWork.AddArticlesAsync(articles);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task RetryFailedArticles()
        {
            var failedArticlesUrls = await _unitOfWork.GetFailedArticlesUrls();

            for (int i = 0; i < failedArticlesUrls.Count; i += 30)
            {
                var tasks = failedArticlesUrls
                    .Skip(i)
                    .Take(30)
                    .Select(async url =>
                    {
                        return await _singleArticleScraper.GetArticleAsync(url, DateOnly.MinValue);
                    });

                var articles = await Task.WhenAll(tasks);
                var articlesToProcess = articles?.Where(article => article is not null)?.ToList() ?? [];

                if (articlesToProcess is not null && articlesToProcess.Any())
                {
                    await _unitOfWork.AddArticlesAsync(articlesToProcess!);
                }
            }

            await _unitOfWork.CommitAsync();

        }

        public async Task ClearContent()
        {
            const int batchSize = 20;

            for (int i = 0; i < 5966; i += batchSize)
            {
                var articles = await _unitOfWork.GetArticlesBatch(i, batchSize);

                foreach (var article in articles)
                {
                    // Parse and update content
                    var doc = new HtmlDocument();
                    doc.LoadHtml(article.Content);
                    var node = doc.DocumentNode.SelectSingleNode("//*[@id='maincontent']");
                    article.Content = node?.InnerText ?? article.Content;
                }

                await _unitOfWork.CommitAsync();
            }

        }
    }
}
