using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessLayer.Entity.Context
{
    public class DMSContextFactory : IDesignTimeDbContextFactory<DMSContext>
    {
        public DMSContext CreateDbContext(string[] args)
        {
            // Read the connection string from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DMSContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DMSDBConnection"));

            return new DMSContext(optionsBuilder.Options, configuration);
        }
    }
}