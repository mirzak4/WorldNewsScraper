using Application.DailyArticlesProcessor;
using Application.SingleArticleScraper;

namespace Application.GlobalArticlesProcessor
{
    public class BreitbartGlobalArticlesProcessor : IGlobalArticlesProcessor
    {
        private readonly IDailyArticlesProcessor _articlesProcessor;
        private readonly ISingleArticleScraper _singleArticleScraper;
        private readonly IUnitOfWork _unitOfWork;

        public BreitbartGlobalArticlesProcessor(IUnitOfWork unitOfWork, DailyArticlesProcessorFactory dailyArticlesProcessorFactory, SingleArticleScrapperFactory singleArticleScrapperFactory)
        {
            _articlesProcessor = dailyArticlesProcessorFactory.Get(Domain.WebsiteType.Breitbart);
            _unitOfWork = unitOfWork;
            _singleArticleScraper = singleArticleScrapperFactory.Get(Domain.WebsiteType.Breitbart);
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
    }
}
