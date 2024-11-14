using DataAccessLayer.Repository.Interface;
using Model;
using DataAccessLayer.Entity;
using AutoMapper;
using BusinessLayer.Service.Interface;

namespace BusinessLayer.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly IMapper _mapper;
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public async Task AddDocumentAsync(DocumentModel newDocumentModel)
        {
            var document = _mapper.Map<Document>(newDocumentModel);
            await _documentRepository.AddDocumentAsync(document);
        }

        public async Task<List<DocumentModel>> GetDocumentsAsync()
        {
            var documents = await _documentRepository.GetDocumentsAsync();
            var documentsModel = _mapper.Map<List<DocumentModel>>(documents);

            return documentsModel;
        }

        public async Task<DocumentModel> GetDocumentByIdAsync(Guid documentModelId)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(documentModelId);
            var documentModel = _mapper.Map<DocumentModel>(document);

            return documentModel;
        }

        public async Task UpdateDocumentAsync(DocumentModel updatedDocumentModel)
        {
            var existingDocument = await _documentRepository.GetDocumentByIdAsync(updatedDocumentModel.Id);

            if (existingDocument != null)
            {
                existingDocument.Name = updatedDocumentModel.Name;

                await _documentRepository.UpdateDocumentAsync(existingDocument);
            }
            else
            {
                throw new KeyNotFoundException("Document not found.");
            }
        }

        public async Task DeleteDocumentAsync(Guid documentModelId)
        {
            await _documentRepository.DeleteDocumentAsync(documentModelId);
        }
    }
}
