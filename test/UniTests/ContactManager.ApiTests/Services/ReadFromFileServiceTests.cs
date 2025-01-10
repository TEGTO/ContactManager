using ContactManager.Api.Data.Entities;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace ContactManager.Api.Services.Tests
{
    [TestFixture]
    internal class ReadFromFileServiceTests
    {
        private ReadFromFileService readFromFileService;

        [SetUp]
        public void SetUp()
        {
            readFromFileService = new ReadFromFileService();
        }

        [Test]
        public void ReadFromFile_ValidCsvFile_ReturnsExpectedRecords()
        {
            // Arrange
            var csvContent =
                 "Name,DateOfBirth,Married,Phone,Salary\n" +
                 "John Doe,1990/05/15,true,123456789,50000\n" +
                 "Jane Doe,1985-03-22,false,,60000\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("text/csv");
            fileMock.Setup(f => f.FileName).Returns("test.csv");

            // Act
            var records = readFromFileService.ReadFromFile<FileContact>(fileMock.Object).ToList();

            // Assert
            Assert.That(records.Count, Is.EqualTo(2));

            Assert.That(records[0].Name, Is.EqualTo("John Doe"));
            Assert.That(records[0].Married, Is.True);
            Assert.That(records[0].Phone, Is.EqualTo("123456789"));
            Assert.That(records[0].Salary, Is.EqualTo(50000));

            Assert.That(records[1].Name, Is.EqualTo("Jane Doe"));
            Assert.That(records[1].Married, Is.False);
            Assert.That(records[1].Phone, Is.Empty);
            Assert.That(records[1].Salary, Is.EqualTo(60000));
        }

        [Test]
        public void ReadFromFile_InvalidCsvFile_ThrowsException()
        {
            // Arrange
            var invalidCsvContent = "Name,DateOfBirth,Married,Phone,Salary\nJohn Doe,1990/05/15,InvalidValue,123456789,50000\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidCsvContent));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("text/csv");
            fileMock.Setup(f => f.FileName).Returns("test.csv");

            // Act & Assert
            Assert.Throws<TypeConverterException>(() =>
            {
                readFromFileService.ReadFromFile<FileContact>(fileMock.Object).ToList();
            });
        }

        [Test]
        public void ReadFromFile_EmptyCsvFile_ReturnsEmptyList()
        {
            // Arrange
            var emptyCsvContent = "Name,DateOfBirth,Married,Phone,Salary\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(emptyCsvContent));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("text/csv");
            fileMock.Setup(f => f.FileName).Returns("test.csv");

            // Act
            var records = readFromFileService.ReadFromFile<FileContact>(fileMock.Object).ToList();

            // Assert
            Assert.That(records, Is.Empty);
        }

        [Test]
        public void ReadFromFile_FileIsNull_ThrowsNullReferenceException()
        {
            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
            {
                readFromFileService.ReadFromFile<FileContact>(null!);
            });
        }

        [Test]
        public void ReadFromFile_FileHasInvalidContentType_ThrowsInvalidOperationException()
        {
            // Arrange
            var csvContent = "Name,DateOfBirth,Married,Phone,Salary\nJohn Doe,1990/05/15,true,123456789,50000\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("application/octet-stream"); // Invalid content type
            fileMock.Setup(f => f.FileName).Returns("test.csv");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                readFromFileService.ReadFromFile<FileContact>(fileMock.Object).ToList();
            });
        }

        [Test]
        public void ReadFromFile_FileHasInvalidExtension_ThrowsInvalidOperationException()
        {
            // Arrange
            var csvContent = "Name,DateOfBirth,Married,Phone,Salary\nJohn Doe,1990/05/15,true,123456789,50000\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("text/csv");
            fileMock.Setup(f => f.FileName).Returns("test.txt"); // Invalid file extension

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                readFromFileService.ReadFromFile<FileContact>(fileMock.Object).ToList();
            });
        }

        [Test]
        public void ReadFromFile_FileIsEmpty_ThrowsInvalidOperationException()
        {
            // Arrange
            var emptyStream = new MemoryStream();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(emptyStream);
            fileMock.Setup(f => f.Length).Returns(emptyStream.Length);
            fileMock.Setup(f => f.ContentType).Returns("text/csv");
            fileMock.Setup(f => f.FileName).Returns("test.csv");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                readFromFileService.ReadFromFile<FileContact>(fileMock.Object).ToList();
            });
        }
    }
}