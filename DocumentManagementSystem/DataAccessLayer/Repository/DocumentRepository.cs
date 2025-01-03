using DataAccessLayer.Entity;
using DataAccessLayer.Entity.Context;
using DataAccessLayer.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DMSContext _context;
        private readonly ILogger<DocumentRepository> _logger;

        public DocumentRepository(DMSContext context, ILogger<DocumentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddDocumentAsync(Document newDocument)
        {
            try
            {
                _logger.LogInformation("Adding a new document with ID {DocumentId}.", newDocument.Id);
                _context.Documents.Add(newDocument);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Document with ID {DocumentId} added successfully.", newDocument.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a document with ID {DocumentId}.", newDocument.Id);
                throw;
            }
        }

        public async Task<List<Document>> GetDocumentsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all documents.");
                var documents = await _context.Documents.ToListAsync();
                _logger.LogInformation("Successfully fetched {Count} documents.", documents.Count);
                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all documents.");
                throw;
            }
        }

        public async Task<Document> GetDocumentByIdAsync(Guid documentId)
        {
            try
            {
                _logger.LogInformation("Fetching document with ID {DocumentId}.", documentId);
                var document = await _context.Documents.FindAsync(documentId);
                if (document != null)
                {
                    _logger.LogInformation("Document with ID {DocumentId} fetched successfully.", documentId);
                }
                else
                {
                    _logger.LogWarning("Document with ID {DocumentId} not found.", documentId);
                }
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the document with ID {DocumentId}.", documentId);
                throw;
            }
        }

        public async Task UpdateDocumentAsync(Document updatedDocument)
        {
            try
            {
                _logger.LogInformation("Updating document with ID {DocumentId}.", updatedDocument.Id);
                _context.Documents.Update(updatedDocument);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Document with ID {DocumentId} updated successfully.", updatedDocument.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the document with ID {DocumentId}.", updatedDocument.Id);
                throw;
            }
        }

        public async Task DeleteDocumentAsync(Guid documentId)
        {
            try
            {
                _logger.LogInformation("Attempting to delete document with ID {DocumentId}.", documentId);
                var document = await _context.Documents.FindAsync(documentId);
                if (document != null)
                {
                    _context.Documents.Remove(document);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Document with ID {DocumentId} deleted successfully.", documentId);
                }
                else
                {
                    _logger.LogWarning("Document with ID {DocumentId} not found. Nothing to delete.", documentId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the document with ID {DocumentId}.", documentId);
                throw;
            }
        }
    }
}