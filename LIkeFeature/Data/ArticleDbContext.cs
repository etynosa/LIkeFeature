using LIkeFeature.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LIkeFeature.Data
{
    public class ArticleDbContext : DbContext
    {
        public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options)
        {
        }

        public DbSet<ArticleLike> ArticleLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleLike>()
                .HasIndex(x => x.ArticleId);

            modelBuilder.Entity<ArticleLike>()
                .HasIndex(x => x.UserId);

            modelBuilder.Entity<ArticleLike>()
                .HasIndex(x => new { x.ArticleId, x.UserId })
                .IsUnique();
        }
    }
}
