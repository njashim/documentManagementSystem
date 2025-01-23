using DataAccessLayer.Entity;

namespace DataAccessLayer.Repository.Interface
{
    public interface IDocumentRepository
    {
        Task AddDocumentAsync(Document newDocument);

        Task<List<Document>> GetDocumentsAsync();

        Task<Document> GetDocumentByIdAsync(Guid documentId);

        Task UpdateDocumentAsync(Document updatedDocument);

        Task DeleteDocumentAsync(Guid documentId);

        Task<byte[]> GetDocumentContentAsync(Guid documentId);
    }
}