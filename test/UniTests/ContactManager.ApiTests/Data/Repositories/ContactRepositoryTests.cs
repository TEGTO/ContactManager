using ContactManager.Api.Data.Entities;
using DatabaseControl.Repositories;
using MockQueryable.Moq;
using Moq;

namespace ContactManager.Api.Data.Repositories.Tests
{
    [TestFixture]
    internal class ContactRepositoryTests
    {
        private Mock<IDatabaseRepository<ContactDbContext>> repositoryMock;
        private ContactRepository contactRepository;
        private CancellationToken cancellationToken;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IDatabaseRepository<ContactDbContext>>();

            repositoryMock.Setup(x => x.GetDbContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => null!);

            contactRepository = new ContactRepository(repositoryMock.Object);
            cancellationToken = new CancellationToken();
        }

        private static IEnumerable<TestCaseData> CreateContactTestCases()
        {
            var contacts = new[]
            {
                new Contact { Id = "1", Name = "John Doe", Salary = 50000 },
                new Contact { Id = "2", Name = "Jane Doe", Salary = 60000 }
            };

            // Wrap the array to pass as a single argument
            yield return new TestCaseData(new object[] { contacts })
                .SetDescription("Creates multiple contacts.");
        }

        [Test]
        [TestCaseSource(nameof(CreateContactTestCases))]
        public async Task CreateContactAsync_TestCases(Contact[] contacts)
        {
            // Act
            await contactRepository.CreateContactAsync(contacts, cancellationToken);

            // Assert
            repositoryMock.Verify(repo => repo.GetDbContextAsync(cancellationToken), Times.Once);
            repositoryMock.Verify(repo => repo.AddRangeAsync(It.IsAny<ContactDbContext>(), contacts, cancellationToken), Times.Once);
            repositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<ContactDbContext>(), cancellationToken), Times.Once);
        }

        private static IEnumerable<TestCaseData> DeleteContactTestCases()
        {
            yield return new TestCaseData("Contanct1")
                .SetDescription("Deletes an existing contact.");
        }

        [Test]
        [TestCaseSource(nameof(DeleteContactTestCases))]
        public async Task DeleteContactAsync_TestCases(string name)
        {
            // Arrange
            var contact = new Contact { Id = "1", Name = name };

            // Act
            await contactRepository.DeleteContactAsync(contact, cancellationToken);

            // Assert
            repositoryMock.Verify(repo => repo.GetDbContextAsync(cancellationToken), Times.Once);
            repositoryMock.Verify(repo => repo.Remove(It.IsAny<ContactDbContext>(), contact), Times.Once);
            repositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<ContactDbContext>(), cancellationToken), Times.Once);
        }

        private static IEnumerable<TestCaseData> GetContactByIdTestCases()
        {
            yield return new TestCaseData("1", new Contact { Id = "1", Name = "John Doe" })
                .SetDescription("Gets a contact by a valid ID.");
        }

        [Test]
        [TestCaseSource(nameof(GetContactByIdTestCases))]
        public async Task GetContactByIdAsync_TestCases(string contactId, Contact expectedContact)
        {
            // Arrange
            repositoryMock.Setup(repo => repo.Query<Contact>(It.IsAny<ContactDbContext>()))
                .Returns(new[] { expectedContact }.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await contactRepository.GetContactByIdAsync(contactId, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(expectedContact));

            repositoryMock.Verify(repo => repo.GetDbContextAsync(cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetContactsAsync_ReturnsAllContacts()
        {
            // Arrange
            var expectedContacts = new[]
            {
                new Contact { Id = "1", Name = "John Doe", Salary = 50000 },
                new Contact { Id = "2", Name = "Jane Doe", Salary = 60000 }
            };

            repositoryMock.Setup(repo => repo.Query<Contact>(It.IsAny<ContactDbContext>()))
                .Returns(expectedContacts.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await contactRepository.GetContactsAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.ToArray(), Is.EqualTo(expectedContacts));

            repositoryMock.Verify(repo => repo.GetDbContextAsync(cancellationToken), Times.Once);
        }

        private static IEnumerable<TestCaseData> UpdateContactTestCases()
        {
            yield return new TestCaseData("Updated Contact")
                .SetDescription("Updates an existing contact.");
        }

        [Test]
        [TestCaseSource(nameof(UpdateContactTestCases))]
        public async Task UpdateContactAsync_TestCases(string updatedName)
        {
            // Arrange
            var contact = new Contact { Id = "1", Name = updatedName };

            // Act
            await contactRepository.UpdateContactAsync(contact, cancellationToken);

            // Assert
            repositoryMock.Verify(repo => repo.GetDbContextAsync(cancellationToken), Times.Once);
            repositoryMock.Verify(repo => repo.Update(It.IsAny<ContactDbContext>(), contact), Times.Once);
            repositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<ContactDbContext>(), cancellationToken), Times.Once);
        }
    }
}