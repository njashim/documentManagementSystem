//using API.Controllers;
//using BusinessLayer.Service.Interface;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Model;
//using Moq;

//namespace API.Tests
//{
//    [TestFixture]
//    public class DocumentControllerTests
//    {
//        private Mock<IDocumentService> _documentServiceMock;
//        private Mock<ILogger<DocumentController>> _loggerMock;
//        private DocumentController _controller;

//        [SetUp]
//        public void SetUp()
//        {
//            _documentServiceMock = new Mock<IDocumentService>();
//            _loggerMock = new Mock<ILogger<DocumentController>>();
//            //_controller = new DocumentController(_documentServiceMock.Object, _loggerMock.Object);
//        }

//        [Test]
//        public async Task GetDocuments_ShouldReturnOkResultWithDocuments()
//        {
//            // Arrange
//            var documents = new List<DocumentModel>
//            {
//                new DocumentModel { Id = Guid.NewGuid(), Name = "Document1" },
//                new DocumentModel { Id = Guid.NewGuid(), Name = "Document2" }
//            };
//            _documentServiceMock.Setup(s => s.GetDocumentsAsync()).ReturnsAsync(documents);

//            // Act
//            var result = await _controller.GetDocuments();

//            // Assert
//            Assert.That(result, Is.TypeOf<OkObjectResult>());
//            var okResult = result as OkObjectResult;
//            Assert.That(okResult?.Value, Is.EqualTo(documents));
//        }

//        [Test]
//        public async Task GetDocumentById_ExistingId_ShouldReturnOkResultWithDocument()
//        {
//            // Arrange
//            var documentId = Guid.NewGuid();
//            var document = new DocumentModel { Id = documentId, Name = "TestDocument" };
//            _documentServiceMock.Setup(s => s.GetDocumentByIdAsync(documentId)).ReturnsAsync(document);

//            // Act
//            var result = await _controller.GetDocumentById(documentId);

//            // Assert
//            Assert.That(result, Is.TypeOf<OkObjectResult>());
//            var okResult = result as OkObjectResult;
//            Assert.That(okResult?.Value, Is.EqualTo(document));
//        }

//        [Test]
//        public async Task GetDocumentById_NonExistingId_ShouldReturnNotFound()
//        {
//            // Arrange
//            var documentId = Guid.NewGuid();
//            _documentServiceMock.Setup(s => s.GetDocumentByIdAsync(documentId)).ReturnsAsync((DocumentModel)null);

//            // Act
//            var result = await _controller.GetDocumentById(documentId);

//            // Assert
//            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
//            var notFoundResult = result as NotFoundObjectResult;
//            Assert.That(notFoundResult?.Value, Is.EqualTo($"Document with ID {documentId} not found."));
//        }

//        [Test]
//        public async Task UploadDocument_ValidDocument_ShouldReturnCreatedAtAction()
//        {
//            // Arrange
//            var newDocument = new DocumentModel { Id = Guid.NewGuid(), Name = "NewDocument" };
//            _documentServiceMock.Setup(s => s.AddDocumentAsync(newDocument)).Returns(Task.CompletedTask);

//            // Act
//            //var result = await _controller.UploadDocument(newDocument);

//            // Assert
//            Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
//            var createdResult = result as CreatedAtActionResult;
//            Assert.That(createdResult?.Value, Is.EqualTo(newDocument));
//        }

//        [Test]
//        public async Task UpdateDocument_ValidDocument_ShouldReturnNoContent()
//        {
//            // Arrange
//            var documentId = Guid.NewGuid();
//            var updatedDocument = new DocumentModel { Id = documentId, Name = "UpdatedDocument" };
//            _documentServiceMock.Setup(s => s.UpdateDocumentAsync(updatedDocument)).Returns(Task.CompletedTask);

//            // Act
//            var result = await _controller.UpdateDocument(documentId, updatedDocument);

//            // Assert
//            Assert.That(result, Is.TypeOf<NoContentResult>());
//        }

//        [Test]
//        public async Task UpdateDocument_NonMatchingId_ShouldReturnBadRequest()
//        {
//            // Arrange
//            var documentId = Guid.NewGuid();
//            var updatedDocument = new DocumentModel { Id = Guid.NewGuid(), Name = "UpdatedDocument" };

//            // Act
//            var result = await _controller.UpdateDocument(documentId, updatedDocument);

//            // Assert
//            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
//            var badRequestResult = result as BadRequestObjectResult;
//            Assert.That(badRequestResult?.Value, Is.EqualTo("Document data is invalid."));
//        }

//        [Test]
//        public async Task DeleteDocument_ValidId_ShouldReturnNoContent()
//        {
//            // Arrange
//            var documentId = Guid.NewGuid();
//            _documentServiceMock.Setup(s => s.DeleteDocumentAsync(documentId)).Returns(Task.CompletedTask);

//            // Act
//            var result = await _controller.DeleteDocument(documentId);

//            // Assert
//            Assert.That(result, Is.TypeOf<NoContentResult>());
//        }
//    }
//}