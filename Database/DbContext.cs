using Microsoft.EntityFrameworkCore;
using AuthApiSesh.Model;
using AuthApiSesh.Model.ServerModel;

namespace AuthApiSesh.Database{

    public class AppDbContext : DbContext {

        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set;}
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options){

            //Database.EnsureCreated();
            Database.Migrate();
        }

    }

}