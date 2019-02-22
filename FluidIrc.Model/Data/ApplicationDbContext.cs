using FluidIrc.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace FluidIrc.Model.Data
{
    public class ApplicationDbContext : DbContext, IApplicationContext
    {

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Server> Servers { get; set; }

        public ApplicationDbContext() : this(new DbContextOptions<ApplicationDbContext>()) { }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=AppData.db");
        }
    }
}
