using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ModelConfiguration
{
    public static class ArticleConfiguration
    {
        public static void MapArticle(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Article>();
            entity.ToTable(nameof(Article), "scraping");
        }
    }
}
