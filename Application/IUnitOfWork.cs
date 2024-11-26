using Domain;

namespace Application
{
    public interface IUnitOfWork
    {
        Task AddArticlesAsync(IEnumerable<Article> articles);
        Task<List<Article>> GetArticlesBatch(int offset, int limit);
        Task<List<string>> GetFailedArticlesUrls();
        Task CommitAsync();
    }
}
