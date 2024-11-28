using BusinessLayer.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return Ok(documents);
        }

        //GET/documents/{documentId}
        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocumentById(Guid documentId)
        {
            var document = await _documentService.GetDocumentByIdAsync(documentId);

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
                return BadRequest("Document data is null.");
            }

            await _documentService.AddDocumentAsync(newDocumentModel);

            return CreatedAtAction(nameof(GetDocumentById), new { documentId = newDocumentModel.Id }, newDocumentModel);
        }

        //PUT/documents/{documentId}
        [HttpPut("{documentId}")]
        public async Task<IActionResult> UpdateDocument(Guid documentId, [FromBody] DocumentModel updatedDocumentModel)
        {
            if (updatedDocumentModel == null || documentId != updatedDocumentModel.Id)
            {
                return BadRequest("Invalid document data.");
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
            var document = await _documentService.GetDocumentByIdAsync(documentId);

            if (document == null)
            {
                return NotFound($"Document with ID {documentId} not found.");
            }

            await _documentService.DeleteDocumentAsync(documentId);
            return NoContent();
        }
    }
}
