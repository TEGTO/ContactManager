using ContactManager.Api.Data.Entities;
using ContactManager.Api.Data.Repositories;
using Moq;

namespace ContactManager.Api.Endpoints.Delete.Tests
{
    [TestFixture]
    internal class DeleteControllerTests
    {
        private Mock<IContactRepository> repositoryMock;
        private DeleteController controller;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new Mock<IContactRepository>();

            controller = new DeleteController(
                repositoryMock.Object
            );
        }

        [Test]
        public async Task DeleteContact_ContactExists_DeletesContact()
        {
            // Arrange
            var name = "name";
            var id = "123";
            var contact = new Contact { Name = name };

            repositoryMock
                .Setup(r => r.GetContactByIdAsync(It.Is<string>(s => s == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            repositoryMock
                .Setup(r => r.DeleteContactAsync(contact, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await controller.DeleteContactAsync(id, CancellationToken.None);

            // Assert
            repositoryMock.Verify(r => r.GetContactByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(r => r.DeleteContactAsync(contact, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteContact_ContactDoesNotExist_NoDeletion()
        {
            // Arrange
            var id = "123";

            repositoryMock
               .Setup(r => r.GetContactByIdAsync(It.Is<string>(s => s == id), It.IsAny<CancellationToken>()))
               .ReturnsAsync((Contact?)null);

            // Act
            await controller.DeleteContactAsync(id, CancellationToken.None);

            // Assert
            repositoryMock.Verify(r => r.GetContactByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(r => r.DeleteContactAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}