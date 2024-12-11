using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ModelConfiguration
{
    public static class LogConfiguration
    {
        public static void MapLog(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Log>();
            entity.ToTable($"{nameof(Log)}s", "scraping");
        }
    }
}
