using AutoMapper;
using ContactManager.Api.Data.Entities;
using ContactManager.Api.Data.Repositories;
using ContractManager.Communication.Dtos.Endpoints.Update;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContactManager.Api.Endpoints.Update.Tests
{
    [TestFixture]
    internal class UpdateControllerTests
    {
        private Mock<IContactRepository> repositoryMock;
        private Mock<IMapper> mapperMock;
        private UpdateController controller;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new Mock<IContactRepository>();
            mapperMock = new Mock<IMapper>();

            controller = new UpdateController(repositoryMock.Object, mapperMock.Object);
        }

        [Test]
        public async Task UpdateContanct_ContanctDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateRequest
            {
                Id = "123",
                Name = "Updated Contact Name"
            };

            repositoryMock.Setup(r => r.GetContactByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contact?)null);

            // Act 
            var result = await controller.UpdateContactAsync(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            repositoryMock.Verify(r => r.GetContactByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(r => r.UpdateContactAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task UpdateContact_ValidRequest_UpdatesContactReturnsOk()
        {
            // Arrange
            var request = new UpdateRequest
            {
                Id = "123",
                Name = "Updated Contact Name"
            };

            var existingContact = new Contact
            {
                Id = "123",
                Name = "Old Contact Name"
            };
            var updatedContact = new Contact
            {
                Id = request.Id,
                Name = request.Name
            };

            repositoryMock.Setup(r => r.GetContactByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingContact);

            mapperMock.Setup(m => m.Map<Contact>(request))
                .Returns(updatedContact);

            repositoryMock.Setup(r => r.UpdateContactAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.UpdateContactAsync(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());

            repositoryMock.Verify(r => r.GetContactByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(r => r.UpdateContactAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<Contact>(request), Times.Once);
        }
    }
}