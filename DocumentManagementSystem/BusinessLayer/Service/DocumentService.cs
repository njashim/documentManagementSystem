using AutoMapper;
using BusinessLayer.Service.Interface;
using DataAccessLayer.Entity;
using DataAccessLayer.Repository.Interface;
using Microsoft.Extensions.Logging;
using Model;

namespace BusinessLayer.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly IMapper _mapper;
        private readonly IDocumentRepository _documentRepository;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(IDocumentRepository documentRepository, IMapper mapper, IRabbitMQService rabbitMQService, ILogger<DocumentService> logger)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
            _rabbitMQService = rabbitMQService;
            _logger = logger;
        }

        public async Task AddDocumentAsync(DocumentModel newDocumentModel)
        {
            try
            {
                _logger.LogInformation("Adding a new document with ID {DocumentId}.", newDocumentModel.Id);
                var document = _mapper.Map<Document>(newDocumentModel);
                await _documentRepository.AddDocumentAsync(document);
                _rabbitMQService.SendMessage(newDocumentModel);
                _logger.LogInformation("Document with ID {DocumentId} added successfully and message sent to RabbitMQ.", newDocumentModel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a document with ID {DocumentId}.", newDocumentModel.Id);
                throw;
            }
        }

        public async Task<List<DocumentModel>> GetDocumentsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all documents.");
                var documents = await _documentRepository.GetDocumentsAsync();
                var documentsModel = _mapper.Map<List<DocumentModel>>(documents);
                _logger.LogInformation("Successfully fetched {Count} documents.", documentsModel.Count);
                return documentsModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all documents.");
                throw;
            }
        }

        public async Task<DocumentModel> GetDocumentByIdAsync(Guid documentModelId)
        {
            try
            {
                _logger.LogInformation("Fetching document with ID {DocumentId}.", documentModelId);
                var document = await _documentRepository.GetDocumentByIdAsync(documentModelId);

                if (document == null)
                {
                    _logger.LogWarning("Document with ID {DocumentId} not found.", documentModelId);
                    return null;
                }

                var documentModel = _mapper.Map<DocumentModel>(document);
                _logger.LogInformation("Document with ID {DocumentId} fetched successfully.", documentModelId);
                return documentModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the document with ID {DocumentId}.", documentModelId);
                throw;
            }
        }

        public async Task UpdateDocumentAsync(DocumentModel updatedDocumentModel)
        {
            try
            {
                _logger.LogInformation("Updating document with ID {DocumentId}.", updatedDocumentModel.Id);
                var existingDocument = await _documentRepository.GetDocumentByIdAsync(updatedDocumentModel.Id);

                if (existingDocument != null)
                {
                    existingDocument.Tags = updatedDocumentModel.Tags;
                    await _documentRepository.UpdateDocumentAsync(existingDocument);
                    _logger.LogInformation("Document with ID {DocumentId} updated successfully.", updatedDocumentModel.Id);
                }
                else
                {
                    _logger.LogWarning("Document with ID {DocumentId} not found. Update failed.", updatedDocumentModel.Id);
                    throw new KeyNotFoundException("Document not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the document with ID {DocumentId}.", updatedDocumentModel.Id);
                throw;
            }
        }

        public async Task DeleteDocumentAsync(Guid documentModelId)
        {
            try
            {
                _logger.LogInformation("Attempting to delete document with ID {DocumentId}.", documentModelId);
                await _documentRepository.DeleteDocumentAsync(documentModelId);
                _logger.LogInformation("Document with ID {DocumentId} deleted successfully.", documentModelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the document with ID {DocumentId}.", documentModelId);
                throw;
            }
        }
    }
}