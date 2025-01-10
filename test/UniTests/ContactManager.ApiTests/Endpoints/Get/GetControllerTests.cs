using AutoMapper;
using ContactManager.Api.Data.Entities;
using ContactManager.Api.Data.Repositories;
using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Get;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContactManager.Api.Endpoints.Get.Tests
{
    [TestFixture]
    internal class GetControllerTests
    {
        private Mock<IContactRepository> repositoryMock;
        private Mock<IMapper> mapperMock;
        private GetController controller;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IContactRepository>();
            mapperMock = new Mock<IMapper>();
            controller = new GetController(repositoryMock.Object, mapperMock.Object);
        }

        [Test]
        public async Task GetContractsAsync_WhenCalled_ReturnsOkResultWithContacts()
        {
            // Arrange
            var contactList = new List<Contact>
            {
                new Contact { Id = "1", Name = "John Doe", Married = true, Phone = "123456789", Salary = 50000 },
                new Contact { Id = "2", Name = "Jane Doe", Married = false, Phone = "987654321", Salary = 60000 }
            };

            var expectedResponse = new List<ContactResponse>
            {
                new ContactResponse { Id = "1", Name = "John Doe", Married = true, Phone = "123456789", Salary = 50000 },
                new ContactResponse { Id = "2", Name = "Jane Doe", Married = false, Phone = "987654321", Salary = 60000 }
            };

            repositoryMock.Setup(r => r.GetContactsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(contactList);

            foreach (var contact in contactList)
            {
                mapperMock.Setup(m => m.Map<ContactResponse>(contact))
                          .Returns(expectedResponse.First(x => x.Id == contact.Id));
            }

            // Act
            var result = await controller.GetContractsAsync(CancellationToken.None);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var response = okResult.Value as GetResponse;
            Assert.NotNull(response);
            Assert.That(response.TotalCount, Is.EqualTo(contactList.Count));

            foreach (var contactResponse in expectedResponse)
            {
                Assert.Contains(contactResponse, response.Data.ToList());
            }

            repositoryMock.Verify(r => r.GetContactsAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ContactResponse>(It.IsAny<Contact>()), Times.Exactly(contactList.Count));
        }

        [Test]
        public async Task GetContractsAsync_WhenNoContacts_ReturnsEmptyResponse()
        {
            // Arrange
            var emptyContactList = new List<Contact>();

            repositoryMock.Setup(r => r.GetContactsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(emptyContactList);

            // Act
            var result = await controller.GetContractsAsync(CancellationToken.None);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var response = okResult.Value as GetResponse;
            Assert.NotNull(response);
            Assert.That(response.TotalCount, Is.EqualTo(0));
            Assert.IsEmpty(response.Data);

            repositoryMock.Verify(r => r.GetContactsAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ContactResponse>(It.IsAny<Contact>()), Times.Never);
        }
    }
}