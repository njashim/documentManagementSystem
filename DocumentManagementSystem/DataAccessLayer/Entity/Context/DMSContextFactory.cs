using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Entity.Context
{
    public class DMSContextFactory : IDesignTimeDbContextFactory<DMSContext>
    {
        public DMSContext CreateDbContext(string[] args)
        {
            // Determination of the current environment
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Read the connection string from appsettings.{environment}.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../API"))
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DMSContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DMSDBConnection"));

            return new DMSContext(optionsBuilder.Options, configuration);
        }
    }
}