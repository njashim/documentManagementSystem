using DocumentManagementSystem.Services;
using RabbitMQ.Client;
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
            builder.Services.AddSingleton<RabbitMQService>(serviceProvider =>
            {
                return new RabbitMQService("dms_rabbitmq", "document_queue");
            });

            var app = builder.Build();

            // Apply migrations during application startup
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DMSContext>();
                dbContext.Database.Migrate();
            }

            // RabbitMQ Queue Initialize
            InitializeRabbitMQQueue();

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

        private static void InitializeRabbitMQQueue()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "dms_rabbitmq", 
                Port = 5672
            };

            bool isConnected = false;
            int retryCount = 0;
            const int maxRetryCount = 10;
            const int delayMilliseconds = 5000;

            while (!isConnected && retryCount < maxRetryCount)
            {
                try
                {
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(queue: "ocr_queue", 
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    Console.WriteLine("RabbitMQ Queue 'ocr_queue' wurde erfolgreich erstellt.");
                    isConnected = true;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Console.WriteLine($"Fehler beim Verbinden mit RabbitMQ. Versuch {retryCount}/{maxRetryCount}: {ex.Message}");
                    if (retryCount < maxRetryCount)
                    {
                        Thread.Sleep(delayMilliseconds);
                    }
                    else
                    {
                        Console.WriteLine("Maximale Anzahl von Verbindungsversuchen erreicht. Die Anwendung wird beendet.");
                        throw;
                    }
                }
            }
        }
    }
}

