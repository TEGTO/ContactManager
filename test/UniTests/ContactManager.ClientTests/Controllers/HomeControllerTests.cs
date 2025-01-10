using AutoMapper;
using ContactManager.Client.Services;
using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Update;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Net;

namespace ContactManager.Client.Controllers.Tests
{
    [TestFixture]
    internal class HomeControllerTests
    {
        private Mock<IContactService> contactServiceMock;
        private Mock<IFileValidationService> fileValidationServiceMock;
        private Mock<IMapper> mapperMock;
        private HomeController controller;

        [SetUp]
        public void SetUp()
        {
            contactServiceMock = new Mock<IContactService>();
            fileValidationServiceMock = new Mock<IFileValidationService>();
            mapperMock = new Mock<IMapper>();

            controller = new HomeController(contactServiceMock.Object, fileValidationServiceMock.Object, mapperMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [TearDown]
        public void TearDown()
        {
            controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithContacts()
        {
            // Arrange
            var contacts = new List<ContactResponse>
            {
                new ContactResponse { Id = "1", Name = "John Doe" }
            };
            contactServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(contacts);

            // Act
            var result = await controller.Index(CancellationToken.None);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(contacts));
        }

        [Test]
        public async Task UploadCsv_InvalidFile_ReturnsRedirectWithError()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var exception = "Invalid request.";

            fileValidationServiceMock.Setup(v => v.ValidateCsvFile(fileMock.Object, out exception))
                .Returns(false);

            // Act
            var result = await controller.UploadCsv(fileMock.Object, CancellationToken.None);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(controller.TempData["Error"], Is.EqualTo(exception));
        }

        [Test]
        public async Task UploadCsv_ValidFile_SuccessfulUpload_ReturnsRedirectWithSuccess()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileValidationServiceMock.Setup(v => v.ValidateCsvFile(fileMock.Object, out It.Ref<string>.IsAny))
                .Returns(true);

            contactServiceMock.Setup(c => c.UploadFileToApiAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            var result = await controller.UploadCsv(fileMock.Object, CancellationToken.None);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(controller.TempData["Success"], Is.EqualTo("File uploaded successfully."));
        }

        [Test]
        public async Task UploadCsv_InvalidModel_ReturnsRedirectToIndex()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileValidationServiceMock.Setup(v => v.ValidateCsvFile(fileMock.Object, out It.Ref<string>.IsAny))
                .Returns(true);

            contactServiceMock.Setup(c => c.UploadFileToApiAsync(fileMock.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await controller.UploadCsv(fileMock.Object, CancellationToken.None);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task UpdateContact_ContactNotFound_ReturnsRedirectToIndex()
        {
            // Arrange
            contactServiceMock.Setup(c => c.GetByIdAsync("1", It.IsAny<CancellationToken>()))
                .ReturnsAsync((ContactResponse)null!);

            // Act
            var result = await controller.UpdateContact("1", CancellationToken.None);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task UpdateContact_ContactFound_ReturnsViewWithModel()
        {
            // Arrange
            var contact = new ContactResponse { Id = "1", Name = "John Doe" };
            var updateRequest = new UpdateRequest { Id = "1", Name = "John Doe" };

            contactServiceMock.Setup(c => c.GetByIdAsync("1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            mapperMock.Setup(m => m.Map<UpdateRequest>(contact))
                .Returns(updateRequest);

            // Act
            var result = await controller.UpdateContact("1", CancellationToken.None);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.ViewName, Is.EqualTo("UpdateContact"));
            Assert.That(viewResult.Model, Is.EqualTo(updateRequest));
        }

        [Test]
        public async Task UpdateContactForm_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var request = new UpdateRequest();

            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await controller.UpdateContactForm(request, CancellationToken.None);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.That(viewResult.ViewName, Is.EqualTo("UpdateContact"));
            Assert.That(viewResult.Model, Is.EqualTo(request));
        }

        [Test]
        public async Task UpdateContactForm_ServiceReturnsFailure_ReturnsErrorMessage()
        {
            // Arrange
            var request = new UpdateRequest { Id = "123", Name = "John Doe" };
            var cancellationToken = CancellationToken.None;
            var failureResult = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "Bad Request"
            };

            contactServiceMock.Setup(service => service.UpdateAsync(It.IsAny<UpdateRequest>(), cancellationToken))
                               .ReturnsAsync(failureResult);

            // Act
            var result = await controller.UpdateContactForm(request, cancellationToken);

            // Assert
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.NotNull(redirectToActionResult);
            Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
            Assert.That(controller.TempData["Error"], Is.EqualTo("Error updating: Bad Request"));
        }

        [Test]
        public async Task UpdateContactForm_ServiceReturnsSuccess_RedirectsToIndex()
        {
            // Arrange
            var request = new UpdateRequest { Id = "123", Name = "John Doe" };
            var cancellationToken = CancellationToken.None;
            var successResult = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            contactServiceMock.Setup(service => service.UpdateAsync(It.IsAny<UpdateRequest>(), cancellationToken))
                               .ReturnsAsync(successResult);

            // Act
            var result = await controller.UpdateContactForm(request, cancellationToken);

            // Assert
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.NotNull(redirectToActionResult);
            Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task DeleteById_SuccessfulDelete_RedirectsToIndex()
        {
            // Arrange
            contactServiceMock.Setup(c => c.DeleteByIdAsync("1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            var result = await controller.DeleteById("1", CancellationToken.None);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task DeleteById_UnsuccessfulDelete_SetsErrorMessage()
        {
            // Arrange
            contactServiceMock.Setup(c => c.DeleteByIdAsync("1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Bad Request"
                });

            // Act
            var result = await controller.DeleteById("1", CancellationToken.None);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(controller.TempData["Error"], Is.EqualTo("Error deleting: Bad Request"));
        }
    }
}