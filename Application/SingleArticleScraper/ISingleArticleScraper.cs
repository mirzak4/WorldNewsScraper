using Domain;

namespace Application.SingleArticleScraper
{
    public interface ISingleArticleScraper
    {
        Task<Article?> GetArticleAsync(string url, DateOnly date);
    }
}
