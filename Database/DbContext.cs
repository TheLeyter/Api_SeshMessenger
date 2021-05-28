using Microsoft.EntityFrameworkCore;
using AuthApiSesh.Model;
namespace AuthApiSesh.Database{

    public class AppDbContext : DbContext {

        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set;}
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options){
            
            Database.EnsureCreated();
            //Database.Migrate();
        }
    }

}