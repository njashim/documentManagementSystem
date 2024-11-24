using Model;

namespace BusinessLayer.Service.Interface
{
    public interface IDocumentService
    {
        Task AddDocumentAsync(DocumentModel newDocumentModel);

        Task<List<DocumentModel>> GetDocumentsAsync();

        Task<DocumentModel> GetDocumentByIdAsync(Guid documentModelId);

        Task UpdateDocumentAsync(DocumentModel updatedDocumentModel);

        Task DeleteDocumentAsync(Guid documentModelId);
    }
}