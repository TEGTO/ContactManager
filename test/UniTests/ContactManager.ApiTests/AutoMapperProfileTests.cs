using AutoMapper;
using ContactManager.Api.Data.Entities;
using ContractManager.Communication.Dtos.Endpoints.Update;

namespace ContactManager.Api.Tests
{
    [TestFixture]
    internal class AutoMapperProfileTests
    {
        private IMapper mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            mapper = config.CreateMapper();
        }

        [Test]
        public void Map_UpdateRequest_To_Contact()
        {
            // Arrange
            var request = new UpdateRequest
            {
                Id = "Id",
                Name = "NewContact",
                Married = true,
                Phone = "phone",
                Salary = 100
            };

            // Act
            var result = mapper.Map<Contact>(request);

            // Assert
            Assert.That(result.Id, Is.EqualTo(request.Id));
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Married, Is.EqualTo(request.Married));
            Assert.That(result.Phone, Is.EqualTo(request.Phone));
            Assert.That(result.Salary, Is.EqualTo(request.Salary));
        }

        [Test]
        public void Map_FileContact_To_Contact()
        {
            // Arrange
            var request = new FileContact
            {
                Name = "NewContact",
                Married = true,
                Phone = "phone",
                Salary = 100
            };

            // Act
            var result = mapper.Map<Contact>(request);

            // Assert
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Married, Is.EqualTo(request.Married));
            Assert.That(result.Phone, Is.EqualTo(request.Phone));
            Assert.That(result.Salary, Is.EqualTo(request.Salary));
        }

        [Test]
        public void Map_Contact_To_ContactResponse()
        {
            // Arrange
            var contact = new Contact
            {
                Id = "Id",
                Name = "NewContact",
                Married = true,
                Phone = "phone",
                Salary = 100
            };

            // Act
            var result = mapper.Map<Contact>(contact);

            // Assert
            Assert.That(result.Id, Is.EqualTo(contact.Id));
            Assert.That(result.Name, Is.EqualTo(contact.Name));
            Assert.That(result.Married, Is.EqualTo(contact.Married));
            Assert.That(result.Phone, Is.EqualTo(contact.Phone));
            Assert.That(result.Salary, Is.EqualTo(contact.Salary));
        }
    }
}