using BusinessLayer.Service;
using Tesseract;
using PdfiumViewer;
using DataAccessLayer.Entity;
using Newtonsoft.Json;
using DataAccessLayer.Repository.Interface;

namespace OcrWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQService _rabbitMQService;
        private readonly IDocumentRepository _documentRepository;

        public Worker(ILogger<Worker> logger, RabbitMQService rabbitMQService, IDocumentRepository documentRepository)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
            _documentRepository = documentRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OCR Worker started.");

            _rabbitMQService.InitializeRabbitMQQueue();

            await _rabbitMQService.ListenAsync("ocr_queue", async (message) =>
            {
                _logger.LogInformation($"Received message: {message}");

                try
                {
                    var documentMessage = JsonConvert.DeserializeObject<DocumentMessage>(message);

                    if (documentMessage == null || documentMessage.Id == Guid.Empty)
                    {
                        _logger.LogError("Invalid message format or missing ID: {Message}", message);
                        return;
                    }

                    if (string.IsNullOrEmpty(documentMessage.Name))
                    {
                        _logger.LogError("Document name is missing in message: {Message}", message);
                        return;
                    }

                    if (documentMessage.Content == null || documentMessage.Content.Length == 0)
                    {
                        _logger.LogError("Document content is missing or empty: {Message}", message);
                        return;
                    }

                    _logger.LogInformation("Processing document ID: {DocumentId}, Name: {DocumentName}", documentMessage.Id, documentMessage.Name);

                    // OCR-Verarbeitung starten
                    var ocrText = await ProcessDocumentAsync(documentMessage.Id, documentMessage.Content);

                    _logger.LogInformation($"OCR Result for Document ID {documentMessage.Id}: {ocrText}");

                    // Ergebnis in die Ergebnisse-Queue senden
                    await _rabbitMQService.SendMessageAsync("ocr_results_queue", ocrText);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "JSON deserialization failed for message: {Message}", message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while processing the message.");
                }
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task<string> ProcessDocumentAsync(Guid documentId, byte[] pdfContent)
        {
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), $"{documentId}.pdf");
            await File.WriteAllBytesAsync(tempPdfFilePath, pdfContent);

            if (!File.Exists(tempPdfFilePath))
            {
                _logger.LogError("Failed to save PDF file for Document ID: {DocumentId}", documentId);
                return string.Empty;
            }

            _logger.LogInformation("PDF saved successfully: {PdfFilePath}", tempPdfFilePath);

            var imagePath = ConvertPdfToImage(tempPdfFilePath);

            if (!File.Exists(imagePath))
            {
                _logger.LogError("PDF-to-Image conversion failed for Document ID: {DocumentId}", documentId);
                return string.Empty;
            }

            _logger.LogInformation("Image conversion successful: {ImagePath}", imagePath);

            var extractedText = PerformOcr(imagePath);

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                _logger.LogWarning("OCR extraction returned empty text for Document ID: {DocumentId}", documentId);
            }

            return extractedText;
        }

        private string ConvertPdfToImage(string pdfFilePath)
        {
            var imagePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");

            using (var pdfDocument = PdfDocument.Load(pdfFilePath))
            {
                var firstPageImage = pdfDocument.Render(0, 300, 300, true);
                firstPageImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
            }

            return imagePath;
        }

        private string PerformOcr(string imagePath)
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imagePath))
                {
                    var result = engine.Process(img);
                    return result.GetText();
                }
            }
        }
    }

    public class DocumentMessage
    {
        [JsonProperty("DocumentId")]
        public Guid Id { get; set; }

        [JsonProperty("FileName")]
        public string Name { get; set; }

        [JsonProperty("Content")]
        public byte[] Content { get; set; }
    }
}