using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Get;
using ContractManager.Communication.Dtos.Endpoints.Update;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;

namespace ContactManager.Client.Services.Tests
{
    [TestFixture]
    internal class ContactServiceTests
    {
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private Mock<HttpMessageHandler> messageHandlerMock;
        private ContactService contactService;

        [SetUp]
        public void SetUp()
        {
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            messageHandlerMock = new Mock<HttpMessageHandler>();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c[ConfigurationKeys.API_URL]).Returns("https://api.example.com");

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.example.com")
            };

            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            contactService = new ContactService(httpClientFactoryMock.Object, configurationMock.Object);
        }

        [TestCase("123", HttpStatusCode.OK, true)]
        [TestCase("nonexistent", HttpStatusCode.NotFound, false)]
        public async Task GetByIdAsync_TestCases(string contactId, HttpStatusCode statusCode, bool isValidResponse)
        {
            // Arrange
            ContactResponse? expectedContact = isValidResponse
                ? new ContactResponse
                {
                    Id = contactId,
                    Name = "John Doe",
                    DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Married = true,
                    Phone = "123-456-7890",
                    Salary = 50000
                }
                : null;

            var responseContent = isValidResponse ? JsonContent.Create(expectedContact) : null;

            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri == new Uri($"https://api.example.com/{contactId}")
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = responseContent
                });

            // Act & Assert
            if (isValidResponse)
            {
                var result = await contactService.GetByIdAsync(contactId, CancellationToken.None);

                Assert.IsNotNull(result);
                Assert.That(result!.Id, Is.EqualTo(expectedContact!.Id));
                Assert.That(result.Name, Is.EqualTo(expectedContact.Name));
                Assert.That(result.DateOfBirth, Is.EqualTo(expectedContact.DateOfBirth));
                Assert.That(result.Married, Is.EqualTo(expectedContact.Married));
                Assert.That(result.Phone, Is.EqualTo(expectedContact.Phone));
                Assert.That(result.Salary, Is.EqualTo(expectedContact.Salary));
            }
            else
            {
                Assert.ThrowsAsync<HttpRequestException>(async () =>
                    await contactService.GetByIdAsync(contactId, CancellationToken.None));
            }
        }

        [TestCase(HttpStatusCode.OK, true)]
        [TestCase(HttpStatusCode.NotFound, false)]
        public async Task GetAllAsync_TestCases(HttpStatusCode statusCode, bool isValidResponse)
        {
            // Arrange
            var expectedContacts = isValidResponse
                ? new GetResponse
                {
                    Data = new List<ContactResponse>
                    {
                    new ContactResponse { Id = "1", Name = "John Doe", Salary = 50000 },
                    new ContactResponse { Id = "2", Name = "Jane Doe", Salary = 60000 }
                    },
                    TotalCount = 2
                }
                : null;

            var responseContent = isValidResponse ? JsonContent.Create(expectedContacts) : null;

            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("https://api.example.com")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = responseContent
                });

            // Act
            if (isValidResponse)
            {
                var result = await contactService.GetAllAsync(CancellationToken.None);

                Assert.IsNotNull(result);
                Assert.That(result.Count(), Is.EqualTo(expectedContacts!.Data.Count()));
            }
            else
            {
                Assert.ThrowsAsync<HttpRequestException>(async () =>
                    await contactService.GetAllAsync(CancellationToken.None));
            }
        }

        [TestCase("1", HttpStatusCode.NoContent)]
        [TestCase("nonexistent", HttpStatusCode.NotFound)]
        public async Task DeleteByIdAsync_TestCases(string contactId, HttpStatusCode statusCode)
        {
            // Arrange
            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete && req.RequestUri == new Uri($"https://api.example.com/{contactId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = statusCode });

            // Act
            var response = await contactService.DeleteByIdAsync(contactId, CancellationToken.None);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
        }

        [TestCase("1", HttpStatusCode.OK)]
        [TestCase("nonexistent", HttpStatusCode.BadRequest)]
        public async Task UpdateAsync_TestCases(string contactId, HttpStatusCode statusCode)
        {
            // Arrange
            var updateRequest = new UpdateRequest
            {
                Id = contactId,
                Name = "Updated Name",
                Phone = "123-456-7890"
            };

            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put && req.RequestUri == new Uri("https://api.example.com")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = statusCode });

            // Act
            var response = await contactService.UpdateAsync(updateRequest, CancellationToken.None);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
        }

        [TestCase(HttpStatusCode.OK, true)]
        [TestCase(HttpStatusCode.BadRequest, false)]
        public async Task UploadFileToApiAsync_TestCases(HttpStatusCode statusCode, bool isSuccess)
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "This is a test file";
            var fileName = "test.csv";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);

            await writer.WriteAsync(content);
            await writer.FlushAsync();
            memoryStream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.ContentType).Returns("text/csv");

            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("https://api.example.com/upload") &&
                        req.Content is MultipartFormDataContent),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode
                });

            // Act
            var response = await contactService.UploadFileToApiAsync(fileMock.Object, CancellationToken.None);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
            if (isSuccess)
            {
                Assert.IsTrue(response.IsSuccessStatusCode);
            }
            else
            {
                Assert.IsFalse(response.IsSuccessStatusCode);
            }
        }
    }
}