using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
