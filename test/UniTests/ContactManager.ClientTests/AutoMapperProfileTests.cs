using AutoMapper;
using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Update;

namespace ContactManager.Client.Tests
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
        public void Map_ContactResponse_To_UpdateRequest()
        {
            // Arrange
            var request = new ContactResponse
            {
                Id = "Id",
                Name = "NewContact",
                Married = true,
                Phone = "phone",
                Salary = 100
            };

            // Act
            var result = mapper.Map<UpdateRequest>(request);

            // Assert
            Assert.That(result.Id, Is.EqualTo(request.Id));
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Married, Is.EqualTo(request.Married));
            Assert.That(result.Phone, Is.EqualTo(request.Phone));
            Assert.That(result.Salary, Is.EqualTo(request.Salary));
        }
    }
}