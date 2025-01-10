using Microsoft.AspNetCore.Http;
using Moq;

namespace ContactManager.Client.Services.Tests
{
    [TestFixture]
    internal class FileValidationServiceTests
    {
        private FileValidationService fileValidationService;

        [SetUp]
        public void SetUp()
        {
            fileValidationService = new FileValidationService();
        }

        [Test]
        public void ValidateCsvFile_FileIsNull_ReturnsFalseWithErrorMessage()
        {
            // Arrange
            IFormFile file = null!;
            string errorMessage;

            // Act
            var result = fileValidationService.ValidateCsvFile(file!, out errorMessage);

            // Assert
            Assert.IsFalse(result);
            Assert.That(errorMessage, Is.EqualTo("Please upload a valid CSV file."));
        }

        [Test]
        public void ValidateCsvFile_FileIsEmpty_ReturnsFalseWithErrorMessage()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            string errorMessage;

            // Act
            var result = fileValidationService.ValidateCsvFile(fileMock.Object, out errorMessage);

            // Assert
            Assert.IsFalse(result);
            Assert.That(errorMessage, Is.EqualTo("Please upload a valid CSV file."));
        }

        [Test]
        public void ValidateCsvFile_FileExtensionIsNotCsv_ReturnsFalseWithErrorMessage()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("file.txt");
            fileMock.Setup(f => f.Length).Returns(1024);
            string errorMessage;

            // Act
            var result = fileValidationService.ValidateCsvFile(fileMock.Object, out errorMessage);

            // Assert
            Assert.IsFalse(result);
            Assert.That(errorMessage, Is.EqualTo("Only CSV files are allowed."));
        }

        [Test]
        public void ValidateCsvFile_ValidCsvFile_ReturnsTrueWithNoErrorMessage()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("file.csv");
            fileMock.Setup(f => f.Length).Returns(1024);
            string errorMessage;

            // Act
            var result = fileValidationService.ValidateCsvFile(fileMock.Object, out errorMessage);

            // Assert
            Assert.IsTrue(result);
            Assert.That(errorMessage, Is.EqualTo(string.Empty));
        }
    }
}