using BusinessLayer.Service;
using Tesseract;
using PdfiumViewer;
using System.Drawing.Imaging;
using System.IO;
using DataAccessLayer.Entity;
using Newtonsoft.Json;

namespace OcrWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQService _rabbitMQService; // Nutze den RabbitMQService aus dem Business Layer

        public Worker(ILogger<Worker> logger, RabbitMQService rabbitMQService)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OCR Worker started.");

            await _rabbitMQService.ListenAsync("ocr_queue", async (message) =>
            {
                _logger.LogInformation($"Received message: {message}");

                // Deserialisiere die Nachricht in das Document-Objekt
                var documentMessage = JsonConvert.DeserializeObject<Document>(message);

                // Extrahiere den byte[]-Inhalt der PDF-Datei aus der Nachricht
                var pdfContent = documentMessage.Content; // Angenommen, der Inhalt ist als byte[] gespeichert

                // OCR-Verarbeitung für das Dokument
                var ocrText = await ProcessDocumentAsync(documentMessage.Id, pdfContent);

                _logger.LogInformation($"Processed OCR for Document ID: {documentMessage.Id}");
                await _rabbitMQService.SendMessageAsync("ocr_result_queue", ocrText);
            });
        }

        private async Task<string> ProcessDocumentAsync(Guid documentId, byte[] pdfContent)
        {
            // Speichere die PDF-Datei temporär, um sie in ein Bild umzuwandeln
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), $"{documentId}.pdf");
            await File.WriteAllBytesAsync(tempPdfFilePath, pdfContent);

            // PDF in Bild umwandeln
            var imagePath = ConvertPdfToImage(tempPdfFilePath);

            // OCR mit Tesseract durchführen
            var extractedText = PerformOcr(imagePath);

            return extractedText;
        }

        private string ConvertPdfToImage(string pdfFilePath)
        {
            var imagePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");

            using (var pdfDocument = PdfDocument.Load(pdfFilePath))
            {
                // Die erste Seite in ein Bild umwandeln
                var firstPageImage = pdfDocument.Render(0, 300, 300, true); // Seite 0, mit 300 DPI
                firstPageImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png); // Verwende ImageFormat.Png aus System.Drawing.Imaging
            }

            return imagePath;
        }

        private string PerformOcr(string imagePath)
        {
            // OCR mit Tesseract durchführen
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
}
