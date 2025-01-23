using BusinessLayer.Service;
using BusinessLayer.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Model;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("documents/")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly IRabbitMQService _rabbitMQService; // Füge dies hinzu

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger, IRabbitMQService rabbitMQService)
        {
            _documentService = documentService;
            _logger = logger;
            _rabbitMQService = rabbitMQService; // Speichere es als private Instanzvariable
        }

        //GET/documents
        [HttpGet]
        public async Task<IActionResult> GetDocuments()
        {
            var documents = await _documentService.GetDocumentsAsync();

            _logger.LogInformation("GetDocuments-Log: GetDocuments used");

            return Ok(documents);
        }

        //GET/documents/{documentId}
        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocumentById(Guid documentId)
        {
            var document = await _documentService.GetDocumentByIdAsync(documentId);
            _logger.LogInformation("GetDocumentbyId-Log: GetDocumentbyId used");

            if (document == null)
            {
                _logger.LogError($"GetDocumentbyId-Log: Document with ID {documentId} not found.");
                return NotFound($"Document with ID {documentId} not found.");
            }

            return Ok(document);
        }
        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentModel newDocumentModel, [FromForm] IFormFile file)
        {
            if (newDocumentModel == null)
            {
                _logger.LogWarning("UploadDocument: Missing document data.");
                return BadRequest(new { error = "Document data is missing." });
            }

            if (file == null)
            {
                _logger.LogWarning("UploadDocument: Missing file.");
                return BadRequest(new { error = "File is missing." });
            }

            try
            {
                _logger.LogInformation("Uploading document: {Name}", newDocumentModel.Name);

                // Dateiinhalt als Byte-Array auslesen
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] contentBytes = memoryStream.ToArray();

                // Überprüfen, ob der Dateiinhalt korrekt ist
                if (contentBytes == null || contentBytes.Length == 0)
                {
                    _logger.LogWarning("UploadDocument: Empty file content.");
                    return BadRequest(new { error = "File content is empty." });
                }

                // Content direkt setzen
                newDocumentModel.Content = contentBytes;

                // Logge die Länge des Inhalts
                _logger.LogInformation("Content set. Length of content: {Length}", newDocumentModel.Content.Length);

                // Dokument speichern
                await _documentService.AddDocumentAsync(newDocumentModel, newDocumentModel.Content);

                _logger.LogInformation("Document uploaded successfully: {Name}", newDocumentModel.Name);

                // Nachricht an RabbitMQ senden
                var message = new { DocumentId = newDocumentModel.Id, FileName = file.FileName };
                await _rabbitMQService.SendMessageAsync("ocr_queue", message);

                return CreatedAtAction(nameof(GetDocumentById), new { documentId = newDocumentModel.Id }, newDocumentModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading document.");
                return StatusCode(500, "An error occurred while uploading the document.");
            }
        }
















        //PUT/documents/{documentId}
        [HttpPut("{documentId}")]
        public async Task<IActionResult> UpdateDocument(Guid documentId, [FromBody] DocumentModel updatedDocumentModel)
        {
            _logger.LogInformation("UpdateDocument-Log: UpdateDocument used");

            if (updatedDocumentModel == null || documentId != updatedDocumentModel.Id)
            {
                _logger.LogError("UpdateDocument-Log: Document data is invalid.");
                return BadRequest("Document data is invalid.");
            }

            try
            {
                await _documentService.UpdateDocumentAsync(updatedDocumentModel);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError($"UpdateDocument-Log: Document with ID {documentId} not found.");
                return NotFound($"Document with ID {documentId} not found.");
            }
        }

        //DELETE/documents/{documentId}
        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(Guid documentId)
        {
            _logger.LogInformation($"DeleteDocument-Log: Deleting Document with ID {documentId}.");
            await _documentService.DeleteDocumentAsync(documentId);
            _logger.LogInformation($"DeleteDocument-Log: Deleted Document with ID {documentId}.");
            return NoContent();
        }
    }
}