using BusinessLayer.Mapping;
using BusinessLayer.Service.Interface;
using BusinessLayer.Service;
using DataAccessLayer.Entity.Context;
using DataAccessLayer.Repository.Interface;
using DataAccessLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DMSContext>();

            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Register RabbitMQService
            builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<RabbitMQService>>();

                // Hier alle Parameter übergeben
                string hostName = "dms_rabbitmq";
                int port = 5672;  // Beispiel: Standardport von RabbitMQ
                string userName = "guest";  // Standard-User
                string password = "guest";  // Standard-Passwort
                string queueName = "ocr_queue";

                return new RabbitMQService(hostName, port, userName, password, queueName, logger);
            });


            var app = builder.Build();

            // Apply migrations during application startup
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DMSContext>();
                dbContext.Database.Migrate();

                // Initialize RabbitMQ Queue
                var rabbitMQService = scope.ServiceProvider.GetRequiredService<IRabbitMQService>();
                rabbitMQService.InitializeRabbitMQQueue();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapGet("/", () => Results.Ok("some hardcoded data"));

            app.MapControllers();

            app.Run();
        }
    }
}