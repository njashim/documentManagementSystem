using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Entity.Context
{
    public class DMSContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DMSContext(DbContextOptions dbContextOptions, IConfiguration configuration) : base(dbContextOptions)
        {
            _configuration = configuration;
            Database.Migrate();
        }

        public DbSet<Document> Documents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DMSDBConnection"));
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}