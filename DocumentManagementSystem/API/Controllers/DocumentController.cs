using BusinessLayer.Service.Interface;
using DocumentManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API.Controllers
{
    [ApiController]
    [Route("documents/")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly RabbitMQService _rabbitMQService;

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger, RabbitMQService rabbitMQService)
        {
            _documentService = documentService;
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        //GET/documents
        [HttpGet]
        public async Task<IActionResult> GetDocuments()
        {
            var documents = await _documentService.GetDocumentsAsync();

            _logger.LogWarning("GetDocument-Log: Get Documents");

            return Ok(documents);
        }

        //GET/documents/{documentId}
        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocumentById(Guid documentId)
        {
            var document = await _documentService.GetDocumentByIdAsync(documentId);
            _logger.LogWarning("GetDocumentbyId Log: GetDocumentbyId used");

            if (document == null)
            {
                return NotFound($"Document with ID {documentId} not found.");
            }

            return Ok(document);
        }

        //POST/documents
        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromBody] DocumentModel newDocumentModel)
        {
            if (newDocumentModel == null)
            {
                _logger.LogWarning("UploadDocument: Received null document data.");
                return BadRequest("Document data is null.");
            }

            try
            {
                _logger.LogInformation("UploadDocument: Storing document with ID {DocumentId}.", newDocumentModel.Id);

                await _documentService.AddDocumentAsync(newDocumentModel);

                _logger.LogInformation("UploadDocument: Document with ID {DocumentId} stored successfully.", newDocumentModel.Id);

                _logger.LogInformation("UploadDocument: Sending document with ID {DocumentId} to RabbitMQ.", newDocumentModel.Id);
                _rabbitMQService.SendMessage(newDocumentModel);

                _logger.LogInformation("UploadDocument: Document with ID {DocumentId} sent to RabbitMQ successfully.", newDocumentModel.Id);

                return CreatedAtAction(nameof(GetDocumentById), new { documentId = newDocumentModel.Id }, newDocumentModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadDocument: An error occurred while uploading the document with ID {DocumentId}.", newDocumentModel.Id);

                return StatusCode(500, "An error occurred while uploading the document.");
            }
        }

        //PUT/documents/{documentId}
        [HttpPut("{documentId}")]
        public async Task<IActionResult> UpdateDocument(Guid documentId, [FromBody] DocumentModel updatedDocumentModel)
        {
            _logger.LogInformation("UpdateDocument Log: Startet");

            if (updatedDocumentModel == null || documentId != updatedDocumentModel.Id)
            {
                return BadRequest("Document data is invalid.");
            }

            try
            {
                await _documentService.UpdateDocumentAsync(updatedDocumentModel);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Document with ID {documentId} not found.");
            }
        }

        //DELETE/documents/{documentId}
        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(Guid documentId)
        {
            _logger.LogInformation("DeleteDocument Log: Deleting Document");
            await _documentService.DeleteDocumentAsync(documentId);
            return NoContent();
        }
    }
}