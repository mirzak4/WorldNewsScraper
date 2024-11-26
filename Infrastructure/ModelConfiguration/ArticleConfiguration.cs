using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
