using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Models
{
    public class CommunityContext : IdentityDbContext<CommunityUser>
    {
        public DbSet<Resident> Residents { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Log> Logs { get; set; }
        public CommunityContext(DbContextOptions options) : base(options)
        { 
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
				optionsBuilder.UseInMemoryDatabase("MyDb");
				optionsBuilder.UseSqlServer("Server=tcp:dt-database-server.database.windows.net,1433;Initial Catalog=communitydb;Persist Security Info=False;User ID=Dtgroup27;Password=Dtpass27;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
				base.OnConfiguring(optionsBuilder);
			}
        }
    }
}
