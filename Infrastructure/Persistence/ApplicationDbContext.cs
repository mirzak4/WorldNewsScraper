using Domain;
using Infrastructure.Configuration;
using Infrastructure.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DatabaseConfiguration _dbConfig;
        public DbSet<Domain.Article> Article { get; set; }
        public DbSet<Log> Log { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<DatabaseConfiguration> dbConfigOptions) : base(options)
        {
            _dbConfig = dbConfigOptions.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_dbConfig.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.MapArticle();
            modelBuilder.MapLog();
        }
    }
}
