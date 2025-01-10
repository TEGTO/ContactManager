using AutoMapper;
using ContactManager.Api.Data.Entities;
using ContactManager.Api.Data.Repositories;
using ContractManager.Communication.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContactManager.Api.Endpoints.GetById.Tests
{
    [TestFixture]
    internal class GetByIdControllerTests
    {
        private Mock<IContactRepository> repositoryMock;
        private Mock<IMapper> mapperMock;
        private GetByIdController controller;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IContactRepository>();
            mapperMock = new Mock<IMapper>();
            controller = new GetByIdController(repositoryMock.Object, mapperMock.Object);
        }

        [Test]
        public async Task GetByIdAsync_WhenContactExists_ReturnsOkResultWithContactResponse()
        {
            // Arrange
            var contactId = "1";
            var contact = new Contact { Id = contactId, Name = "John Doe", Married = true, Phone = "123456789", Salary = 50000 };
            var expectedResponse = new ContactResponse { Id = contactId, Name = "John Doe", Married = true, Phone = "123456789", Salary = 50000 };

            repositoryMock.Setup(r => r.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(contact);

            mapperMock.Setup(m => m.Map<ContactResponse>(contact))
                      .Returns(expectedResponse);

            // Act
            var result = await controller.GetByIdAsync(contactId, CancellationToken.None);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var response = okResult.Value as ContactResponse;
            Assert.NotNull(response);
            Assert.That(response.Id, Is.EqualTo(expectedResponse.Id));
            Assert.That(response.Name, Is.EqualTo(expectedResponse.Name));
            Assert.That(response.Married, Is.EqualTo(expectedResponse.Married));
            Assert.That(response.Phone, Is.EqualTo(expectedResponse.Phone));
            Assert.That(response.Salary, Is.EqualTo(expectedResponse.Salary));

            repositoryMock.Verify(r => r.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ContactResponse>(contact), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WhenContactDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            var contactId = "1";

            repositoryMock.Setup(r => r.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((Contact)null!);

            // Act
            var result = await controller.GetByIdAsync(contactId, CancellationToken.None);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.NotNull(notFoundResult);

            repositoryMock.Verify(r => r.GetContactByIdAsync(contactId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ContactResponse>(It.IsAny<Contact>()), Times.Never);
        }
    }
}