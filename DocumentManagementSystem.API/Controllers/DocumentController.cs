using DocumentManagementSystem.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagementSystem.API.Controllers
{
    [ApiController]
    [Route("documents/")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
        }

        //GET/documents
        [HttpGet]
        public IActionResult GetDocuments()
        {
            var documents = new List<DocumentModel>
            {
                new DocumentModel { Id = Guid.NewGuid(), Title = "Sample Document 1" },
                new DocumentModel { Id = Guid.NewGuid(), Title = "Sample Document 2" }
            };

            return Ok(documents);
        }

        //GET/documents/{documentId}
        [HttpGet("{documentId}")]
        public IActionResult GetDocumentById(Guid documentId)
        {
            var document = new DocumentModel
            {
                Id = documentId,
                Title = "Sample Document"
            };

            return Ok(document);
        }

        //POST/documents
        [HttpPost]
        public IActionResult UploadDocument([FromBody] DocumentModel newDocument)
        {
            var createdDocument = new DocumentModel
            {
                Id = Guid.NewGuid(),
                Title = newDocument.Title
            };

            return CreatedAtAction(nameof(GetDocumentById), new { documentId = createdDocument.Id }, createdDocument);
        }

        //PUT/documents/{documentId}
        [HttpPut("{documentId}")]
        public IActionResult UpdateDocument(Guid documentId, [FromBody] DocumentModel updateDocument)
        {
            var updatedDocument = new DocumentModel
            {
                Id = documentId,
                Title = updateDocument.Title
            };

            return Ok(updatedDocument);
        }

        //DELETE/documents/{documentId}
        [HttpDelete("{documentId}")]
        public IActionResult DeleteDocument(Guid documentId)
        {
            return NoContent();
        }
    }
}
