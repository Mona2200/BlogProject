using BlogProject.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BlogProject.Data
{
   public class BlogDbContext : DbContext
   {
      public DbSet<Comment> Comment { get; set; } = null;
      public DbSet<Post> Post { get; set; } = null;
      public DbSet<Tag> Tag { get; set; } = null;
      public DbSet<User> User { get; set; } = null;
      public DbSet<TagPost> TagPost { get; set; } = null;
      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
         var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "C:/Games/blogDb.db" };
         var connectionString = connectionStringBuilder.ToString();
         var connection = new SqliteConnection(connectionString);

         optionsBuilder.UseSqlite(connection);
      }
   }
}
