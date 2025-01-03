using BusinessLayer.Service.Interface;
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

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
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

                _logger.LogInformation("UploadDocument: Sending document with ID {DocumentId} to RabbitMQ.", newDocumentModel.Id);

                await _documentService.AddDocumentAsync(newDocumentModel);

                _logger.LogInformation("UploadDocument: Document with ID {DocumentId} stored successfully.", newDocumentModel.Id);

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