using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddArticlesAsync(IEnumerable<Article> articles)
        {
            await _dbContext.AddRangeAsync(articles);
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Article>> GetArticlesBatch(int offset, int limit)
        {
            return await _dbContext
                    .Article
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();
        }

        public async Task<List<string>> GetFailedArticlesUrls()
        {
            return await _dbContext
                    .Log
                    .Where(x => x.Level == "Error" && x.Message.Contains("2021") && x.Message.Contains("failed."))
                    .Select(x => x.Message.Substring(30, x.Message.Length - 31))
                    .ToListAsync();
        }
    }
}
