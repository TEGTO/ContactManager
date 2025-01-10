using AutoMapper;
using ContactManager.Api.Data.Entities;
using ContactManager.Api.Data.Repositories;
using ContactManager.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContactManager.Api.Endpoints.Upload.Tests
{
    [TestFixture]
    internal class UploadControllerTests
    {
        private Mock<IContactRepository> repositoryMock;
        private Mock<IReadFromFileService> readFromFileServiceMock;
        private Mock<IMapper> mapperMock;
        private UploadController controller;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IContactRepository>();
            readFromFileServiceMock = new Mock<IReadFromFileService>();
            mapperMock = new Mock<IMapper>();

            controller = new UploadController(repositoryMock.Object, readFromFileServiceMock.Object, mapperMock.Object);
        }

        [Test]
        public async Task UploadContacts_ValidFile_ReturnsOk()
        {
            // Arrange
            var fileContacts = new List<FileContact>
            {
                new FileContact { Name = "John Doe", Married = true, Phone = "123456789", Salary = 50000 },
                new FileContact { Name = "Jane Doe", Married = false, Phone = "987654321", Salary = 60000 }
            };

            var contacts = new List<Contact>
            {
                new Contact { Id = "1", Name = "John Doe", Married = true, Phone = "123456789", Salary = 50000 },
                new Contact { Id = "2", Name = "Jane Doe", Married = false, Phone = "987654321", Salary = 60000 }
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);

            readFromFileServiceMock
                .Setup(s => s.ReadFromFile<FileContact>(fileMock.Object))
                .Returns(fileContacts);

            foreach (var fc in fileContacts)
            {
                mapperMock.Setup(m => m.Map<Contact>(fc))
                          .Returns(contacts.First(x => x.Name == fc.Name));
            }

            repositoryMock.Setup(r => r.CreateContactAsync(It.IsAny<IEnumerable<Contact>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.UploadContacts(fileMock.Object, CancellationToken.None);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Contacts uploaded successfully."));

            repositoryMock.Verify(r => r.CreateContactAsync(It.IsAny<IEnumerable<Contact>>(), It.IsAny<CancellationToken>()), Times.Once);
            readFromFileServiceMock.Verify(s => s.ReadFromFile<FileContact>(fileMock.Object), Times.Once);
        }

        [Test]
        public async Task UploadContacts_FileIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await controller.UploadContacts(null!, CancellationToken.None);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.That(badRequestResult.Value, Is.EqualTo("File is empty or not provided."));
        }

        [Test]
        public async Task UploadContacts_FileIsEmpty_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            // Act
            var result = await controller.UploadContacts(fileMock.Object, CancellationToken.None);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.That(badRequestResult.Value, Is.EqualTo("File is empty or not provided."));
        }

        [Test]
        public async Task UploadContacts_ExceptionDuringProcessing_ReturnsServerError()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);

            readFromFileServiceMock
                .Setup(s => s.ReadFromFile<FileContact>(fileMock.Object))
                .Throws(new Exception("Test exception"));

            // Act
            var result = await controller.UploadContacts(fileMock.Object, CancellationToken.None);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.NotNull(statusCodeResult);
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
            Assert.That(statusCodeResult.Value, Does.StartWith("Error processing file: Test exception"));

            readFromFileServiceMock.Verify(s => s.ReadFromFile<FileContact>(fileMock.Object), Times.Once);
        }
    }
}