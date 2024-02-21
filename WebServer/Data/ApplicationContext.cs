using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebServer.Data.Models;

namespace WebServer.Data
{
    public class ApplicationContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.BlogPost)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.BlogPostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.Author)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.AuthorId);

            modelBuilder.Entity<BlogPost>()
                .HasOne(x => x.Author)
                .WithMany(x => x.BlogPosts)
                .HasForeignKey(x => x.AuthorId);
        }
    }
}
