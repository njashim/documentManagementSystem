using DataAccessLayer.Entity;
using DataAccessLayer.Entity.Context;
using DataAccessLayer.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tests
{
    [TestFixture]
    public class DocumentRepositoryTests
    {
        private DMSContext _context;
        private DocumentRepository _repository;
        private Mock<ILogger<DocumentRepository>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<DMSContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockConfiguration = new Mock<IConfiguration>();
            _context = new DMSContext(options, mockConfiguration.Object);
            _mockLogger = new Mock<ILogger<DocumentRepository>>();
            _repository = new DocumentRepository(_context, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddDocumentAsync_ShouldAddDocument()
        {
            // Arrange
            var document = new Document
            {
                Id = Guid.NewGuid(),
                Name = "Test Document",
                Tags = "Test Tags",
                Content = Encoding.UTF8.GetBytes("This is the content of the document.")  // Content als byte[]
            };

            // Act
            await _repository.AddDocumentAsync(document);

            // Assert
            Assert.That(_context.Documents.Count(), Is.EqualTo(1));
            Assert.That(_context.Documents.First().Name, Is.EqualTo("Test Document"));
            Assert.That(_context.Documents.First().Content, Is.EqualTo(Encoding.UTF8.GetBytes("This is the content of the document.")));  // Überprüfe Content als byte[]
        }

        [Test]
        public async Task GetDocumentsAsync_ShouldReturnAllDocuments()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Id = Guid.NewGuid(), Name = "Doc 1", Tags = "Tag 1", Content = Encoding.UTF8.GetBytes("Content of Doc 1") },
                new Document { Id = Guid.NewGuid(), Name = "Doc 2", Tags = "Tag 2", Content = Encoding.UTF8.GetBytes("Content of Doc 2") }
            };

            _context.Documents.AddRange(documents);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetDocumentsAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Select(d => d.Name), Is.EquivalentTo(documents.Select(d => d.Name)));
            Assert.That(result.Select(d => d.Content), Is.EquivalentTo(documents.Select(d => d.Content)));
        }

        [Test]
        public async Task GetDocumentByIdAsync_ShouldReturnCorrectDocument()
        {
            // Arrange
            var document = new Document
            {
                Id = Guid.NewGuid(),
                Name = "Test Document",
                Tags = "Test Tags",
                Content = Encoding.UTF8.GetBytes("This is the content of the document.")
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetDocumentByIdAsync(document.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(document.Id));
            Assert.That(result.Name, Is.EqualTo("Test Document"));
            Assert.That(result.Tags, Is.EqualTo("Test Tags"));
            Assert.That(result.Content, Is.EqualTo(Encoding.UTF8.GetBytes("This is the content of the document.")));  // Überprüfe Content als byte[]
        }

        [Test]
        public async Task UpdateDocumentAsync_ShouldUpdateDocument()
        {
            // Arrange
            var document = new Document
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                Tags = "old Tags",
                Content = Encoding.UTF8.GetBytes("Old content")
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            document.Tags = "updated Tags";
            document.Content = Encoding.UTF8.GetBytes("Updated content");  // Ändere den Content

            // Act
            await _repository.UpdateDocumentAsync(document);

            // Assert
            var updatedDocument = _context.Documents.First();
            Assert.That(updatedDocument.Tags, Is.EqualTo("updated Tags"));
            Assert.That(updatedDocument.Content, Is.EqualTo(Encoding.UTF8.GetBytes("Updated content")));  // Überprüfe den neuen Content
        }

        [Test]
        public async Task DeleteDocumentAsync_ShouldDeleteDocument()
        {
            // Arrange
            var document = new Document
            {
                Id = Guid.NewGuid(),
                Name = "Test Document",
                Tags = "Test Tags",
                Content = Encoding.UTF8.GetBytes("This is the content of the document.")
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteDocumentAsync(document.Id);

            // Assert
            Assert.That(_context.Documents.Count(), Is.EqualTo(0));
        }
    }
}
