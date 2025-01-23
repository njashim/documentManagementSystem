using BusinessLayer.Service;
using DataAccessLayer.Repository.Interface;
using DataAccessLayer.Repository;
using OcrWorkerService;
using DataAccessLayer.Entity.Context;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Konfiguration für RabbitMQ aus appsettings.json laden
        var configuration = hostContext.Configuration;

        // PostgreSQL-Datenbankverbindung einrichten
        var connectionString = configuration.GetConnectionString("DMSDBConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Database connection string is not configured.");
        }

        services.AddDbContext<DMSContext>(options =>
            options.UseNpgsql(connectionString));

        // Einzelne Parameter für RabbitMQ auslesen
        var rabbitMqHost = configuration["RabbitMQ:Host"];
        var rabbitMqPortStr = configuration["RabbitMQ:Port"];
        var rabbitMqUsername = configuration["RabbitMQ:Username"];
        var rabbitMqPassword = configuration["RabbitMQ:Password"];
        var rabbitMqQueueName = configuration["RabbitMQ:QueueName"];

        if (string.IsNullOrWhiteSpace(rabbitMqHost) || string.IsNullOrWhiteSpace(rabbitMqQueueName))
        {
            throw new ArgumentException("RabbitMQ settings are not configured correctly.");
        }

        // Versuche, den Port in einen int zu konvertieren
        if (!int.TryParse(rabbitMqPortStr, out var rabbitMqPort))
        {
            throw new ArgumentException("Invalid RabbitMQ port.");
        }

        // RabbitMQ ConnectionString dynamisch erstellen
        var rabbitMqConnectionString = $"amqp://{rabbitMqUsername}:{rabbitMqPassword}@{rabbitMqHost}:{rabbitMqPort}/";

        services.AddScoped<IDocumentRepository, DocumentRepository>();

        // RabbitMQService registrieren
        services.AddSingleton<RabbitMQService>(serviceProvider =>
            new RabbitMQService(
                rabbitMqHost,                                          // Hostname
                rabbitMqPort,                                          // Port
                rabbitMqUsername,                                       // Benutzername
                rabbitMqPassword,                                       // Passwort
                rabbitMqQueueName,                                      // Warteschlangenname
                serviceProvider.GetRequiredService<ILogger<RabbitMQService>>()));  // Logger

        // Worker-Service registrieren
        services.AddHostedService<Worker>();
    })
    .ConfigureAppConfiguration(config =>
    {
        // appsettings.json hinzufügen
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .Build();

await host.RunAsync();