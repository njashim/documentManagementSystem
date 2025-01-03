using AutoMapper;
using BusinessLayer.Service;
using BusinessLayer.Service.Interface;
using DataAccessLayer.Entity;
using DataAccessLayer.Repository.Interface;
using Microsoft.Extensions.Logging;
using Model;
using Moq;

namespace BusinessLayer.Tests
{
    [TestFixture]
    public class DocumentServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IDocumentRepository> _repositoryMock;
        private Mock<IRabbitMQService> _rabbitMQServiceMock;
        private Mock<ILogger<DocumentService>> _loggerMock;
        private DocumentService _service;

        [SetUp]
        public void SetUp()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IDocumentRepository>();
            _rabbitMQServiceMock = new Mock<IRabbitMQService>();
            _loggerMock = new Mock<ILogger<DocumentService>>();
            _service = new DocumentService(_repositoryMock.Object, _mapperMock.Object, _rabbitMQServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task AddDocumentAsync_ShouldMapAndCallRepository()
        {
            // Arrange
            var model = new DocumentModel { Id = Guid.NewGuid(), Name = "Test Document" };
            var entity = new Document { Id = model.Id, Name = model.Name };
            _mapperMock.Setup(m => m.Map<Document>(model)).Returns(entity);

            // Act
            await _service.AddDocumentAsync(model);

            // Assert
            _mapperMock.Verify(m => m.Map<Document>(model), Times.Once);
            _repositoryMock.Verify(r => r.AddDocumentAsync(entity), Times.Once);
        }

        [Test]
        public async Task AddDocumentAsync_ShouldSendMessageToQueue()
        {
            // Arrange
            var model = new DocumentModel { Id = Guid.NewGuid(), Name = "Test Document" };
            var entity = new Document { Id = model.Id, Name = model.Name };
            _mapperMock.Setup(m => m.Map<Document>(model)).Returns(entity);

            // Act
            await _service.AddDocumentAsync(model);

            // Assert
            _rabbitMQServiceMock.Verify(rmq => rmq.SendMessage(It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task GetDocumentsAsync_ShouldReturnMappedDocumentModels()
        {
            // Arrange
            var entities = new List<Document>
            {
                new Document { Id = Guid.NewGuid(), Name = "Doc1" },
                new Document { Id = Guid.NewGuid(), Name = "Doc2" }
            };
            var models = new List<DocumentModel>
            {
                new DocumentModel { Id = entities[0].Id, Name = entities[0].Name },
                new DocumentModel { Id = entities[1].Id, Name = entities[1].Name }
            };
            _repositoryMock.Setup(r => r.GetDocumentsAsync()).ReturnsAsync(entities);
            _mapperMock.Setup(m => m.Map<List<DocumentModel>>(entities)).Returns(models);

            // Act
            var result = await _service.GetDocumentsAsync();

            // Assert
            Assert.That(result, Is.EqualTo(models));
            _repositoryMock.Verify(r => r.GetDocumentsAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<DocumentModel>>(entities), Times.Once);
        }

        [Test]
        public async Task GetDocumentByIdAsync_ShouldReturnMappedDocumentModel()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var entity = new Document { Id = documentId, Name = "Test Doc" };
            var model = new DocumentModel { Id = documentId, Name = "Test Doc" };
            _repositoryMock.Setup(r => r.GetDocumentByIdAsync(documentId)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<DocumentModel>(entity)).Returns(model);

            // Act
            var result = await _service.GetDocumentByIdAsync(documentId);

            // Assert
            Assert.That(result, Is.EqualTo(model));
            _repositoryMock.Verify(r => r.GetDocumentByIdAsync(documentId), Times.Once);
            _mapperMock.Verify(m => m.Map<DocumentModel>(entity), Times.Once);
        }

        [Test]
        public async Task UpdateDocumentAsync_ShouldUpdateExistingDocument()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var updatedModel = new DocumentModel { Id = documentId, Name = "Name", Tags = "updated Tags" };
            var existingEntity = new Document { Id = documentId, Name = "Name", Tags = "old Tags" };

            _repositoryMock.Setup(r => r.GetDocumentByIdAsync(documentId)).ReturnsAsync(existingEntity);

            // Act
            await _service.UpdateDocumentAsync(updatedModel);

            // Assert
            Assert.That(existingEntity.Name, Is.EqualTo(updatedModel.Name));
            _repositoryMock.Verify(r => r.UpdateDocumentAsync(existingEntity), Times.Once);
        }

        [Test]
        public void UpdateDocumentAsync_ShouldThrowException_WhenDocumentNotFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var updatedModel = new DocumentModel { Id = documentId, Name = "Updated Name" };
            _repositoryMock.Setup(r => r.GetDocumentByIdAsync(documentId)).ReturnsAsync((Document)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateDocumentAsync(updatedModel));
            Assert.That(ex.Message, Is.EqualTo("Document not found."));
        }

        [Test]
        public async Task DeleteDocumentAsync_ShouldCallRepository()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            // Act
            await _service.DeleteDocumentAsync(documentId);

            // Assert
            _repositoryMock.Verify(r => r.DeleteDocumentAsync(documentId), Times.Once);
        }
    }
}