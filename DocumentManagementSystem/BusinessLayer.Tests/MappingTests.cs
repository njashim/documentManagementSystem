using AutoMapper;
using BusinessLayer.Mapping;
using DataAccessLayer.Entity;
using Model;

namespace BusinessLayer.Tests
{
    [TestFixture]
    public class MappingTests
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
        }

        [Test]
        public void MappingConfiguration_IsValid()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            config.AssertConfigurationIsValid();
        }

        [Test]
        public void Should_Map_Document_To_DocumentModel()
        {
            var document = new Document
            {
                Id = Guid.NewGuid(),
                Name = "Sample Document"
            };

            var documentModel = _mapper.Map<DocumentModel>(document);

            Assert.That(documentModel.Id, Is.EqualTo(document.Id));
            Assert.That(documentModel.Name, Is.EqualTo(document.Name));
        }

        [Test]
        public void Should_Map_DocumentModel_To_Document()
        {
            var documentModel = new DocumentModel
            {
                Id = Guid.NewGuid(),
                Name = "Sample Document Model"
            };

            var document = _mapper.Map<Document>(documentModel);

            Assert.That(document.Id, Is.EqualTo(documentModel.Id));
            Assert.That(document.Name, Is.EqualTo(documentModel.Name));
        }

        [Test]
        public void Should_Map_ListOfDocument_To_ListOfDocumentModel()
        {
            var documents = new List<Document>
            {
                new Document
                {
                    Id = Guid.NewGuid(),
                    Name = "Document 1"
                },
                new Document
                {
                    Id = Guid.NewGuid(),
                    Name = "Document 2"
                }
            };

            var documentModels = _mapper.Map<List<DocumentModel>>(documents);

            Assert.That(documentModels.Count, Is.EqualTo(documents.Count));
            for (int i = 0; i < documents.Count; i++)
            {
                Assert.That(documentModels[i].Id, Is.EqualTo(documents[i].Id));
                Assert.That(documentModels[i].Name, Is.EqualTo(documents[i].Name));
            }
        }

        [Test]
        public void Should_Map_ListOfDocumentModel_To_ListOfDocument()
        {
            var documentModels = new List<DocumentModel>
            {
                new DocumentModel
                {
                    Id = Guid.NewGuid(),
                    Name = "DocumentModel 1"
                },
                new DocumentModel
                {
                    Id = Guid.NewGuid(),
                    Name = "DocumentModel 2"
                }
            };

            var documents = _mapper.Map<List<Document>>(documentModels);

            Assert.That(documents.Count, Is.EqualTo(documentModels.Count));
            for (int i = 0; i < documentModels.Count; i++)
            {
                Assert.That(documents[i].Id, Is.EqualTo(documentModels[i].Id));
                Assert.That(documents[i].Name, Is.EqualTo(documentModels[i].Name));
            }
        }

        [Test]
        public void Should_Map_Empty_List()
        {
            var documents = new List<Document>();

            var documentModels = _mapper.Map<List<DocumentModel>>(documents);

            Assert.That(documentModels, Is.Empty);
        }

        [Test]
        public void Should_Map_List_With_Null_Element()
        {
            var documents = new List<Document>
            {
                new Document { Id = Guid.NewGuid(), Name = "Valid Document" },
                null
            };

            var documentModels = _mapper.Map<List<DocumentModel>>(documents);

            Assert.That(documentModels.Count, Is.EqualTo(2));
            Assert.That(documentModels[0].Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(documentModels[1], Is.Null);
        }

        [Test]
        public void Should_Map_Null_Document_To_Null_DocumentModel()
        {
            Document document = null;

            var documentModel = _mapper.Map<DocumentModel>(document);

            Assert.That(documentModel, Is.Null);
        }

        [Test]
        public void Should_Map_Null_DocumentModel_To_Null_Document()
        {
            DocumentModel documentModel = null;

            var document = _mapper.Map<Document>(documentModel);

            Assert.That(document, Is.Null);
        }

        [Test]
        public void Should_Handle_Null_Properties()
        {
            var document = new Document
            {
                Id = Guid.NewGuid(),
                Name = null
            };

            var documentModel = _mapper.Map<DocumentModel>(document);

            Assert.That(documentModel.Name, Is.Null);
        }

        [Test]
        public void Should_Map_Back_And_Forth()
        {
            var document = new Document
            {
                Id = Guid.NewGuid(),
                Name = "Test Document"
            };

            var documentModel = _mapper.Map<DocumentModel>(document);
            var mappedBackDocument = _mapper.Map<Document>(documentModel);

            Assert.That(mappedBackDocument.Id, Is.EqualTo(document.Id));
            Assert.That(mappedBackDocument.Name, Is.EqualTo(document.Name));
        }
    }
}