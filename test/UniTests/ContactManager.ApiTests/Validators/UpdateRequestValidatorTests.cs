using ContractManager.Communication.Dtos.Endpoints.Update;
using FluentValidation.TestHelper;

namespace ContactManager.Api.Validators.Tests
{
    [TestFixture]
    internal class UpdateRequestValidatorTests
    {
        private UpdateRequestValidator validator;

        [SetUp]
        public void SetUp()
        {
            validator = new UpdateRequestValidator();
        }

        private static IEnumerable<TestCaseData> ValidationTestCases()
        {
            yield return new TestCaseData(null, "name", "phone", false, "Id")
                .SetDescription("Id cannot be null.");

            yield return new TestCaseData(string.Empty, "name", "phone", false, "Id")
                .SetDescription("Id cannot be empty.");

            yield return new TestCaseData(new string('A', 257), "name", "phone", false, "Id")
                .SetDescription("Id cannot exceed 256 characters.");

            yield return new TestCaseData("id", null, "phone", false, "Name")
                  .SetDescription("Name cannot be null.");

            yield return new TestCaseData("id", string.Empty, "phone", false, "Name")
                .SetDescription("Name cannot be empty.");

            yield return new TestCaseData("id", new string('A', 513), "phone", false, "Name")
                .SetDescription("Name cannot exceed 512 characters.");

            yield return new TestCaseData("id", "name", new string('A', 513), false, "Phone")
                .SetDescription("Phone cannot exceed 512 characters.");

            yield return new TestCaseData("id", "name", "phone", true, null)
                .SetDescription("Request is valid.");
        }

        [Test]
        [TestCaseSource(nameof(ValidationTestCases))]
        public void Validate_ValidationTestCases(string id, string name, string phone, bool isValid, string? invalidPropertyName)
        {
            // Arrange
            var request = new UpdateRequest { Id = id, Name = name, Phone = phone };

            // Act
            var result = validator.TestValidate(request);

            // Assert
            if (isValid)
            {
                result.ShouldNotHaveAnyValidationErrors();
            }
            else
            {
                result.ShouldHaveValidationErrorFor(invalidPropertyName);
            }
        }
    }
}