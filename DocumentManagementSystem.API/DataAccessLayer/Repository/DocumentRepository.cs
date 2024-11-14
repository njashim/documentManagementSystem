using DataAccessLayer.Repository.Interface;
using DataAccessLayer.Entity;
using DataAccessLayer.Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DMSContext _context;

        public DocumentRepository(DMSContext context)
        {
            _context = context;
        }

        public async Task AddDocumentAsync(Document newDocument)
        {
            _context.Documents.Add(newDocument);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Document>> GetDocumentsAsync()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task<Document> GetDocumentByIdAsync(Guid documentId)
        {
            return await _context.Documents.FindAsync(documentId);
        }

        public async Task UpdateDocumentAsync(Document updatedDocument)
        {
            _context.Documents.Update(updatedDocument);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(Guid documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document != null)
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
            }
        }
    }
}
