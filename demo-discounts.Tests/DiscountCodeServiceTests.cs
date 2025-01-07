using demo_discounts_api.Constants;
using demo_discounts_api.Helpers;
using demo_discounts_api.Models;
using demo_discounts_api.Repositories.Contracts;
using demo_discounts_api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace demo_discounts.Tests
{
    [TestClass]
    public class DiscountCodeServiceTests
    {
        private Mock<IDiscountCodeRepository> _mockRepository;
        private DiscountCodeService _service;

        [TestInitialize]
        public void Setup()
        {
            // Set up the mock repository
            _mockRepository = new Mock<IDiscountCodeRepository>();

            // Initialize the service with the mocked repository
            _service = new DiscountCodeService(_mockRepository.Object);

            // Set static application limits
            ApplicationLimits.MinCount = 1;
            ApplicationLimits.MaxCount = 2000;
            ApplicationLimits.MinLength = 5;
            ApplicationLimits.MaxLength = 10;
        }

        [TestMethod]
        public async Task GenerateCodes_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var mockCodes = new List<string> { "ABCDE", "FGHIJ" };
            _mockRepository
                .Setup(repo => repo.GenerateCodes(2, 5))
                .ReturnsAsync(mockCodes);

            // Act
            var result = await _service.GenerateCodes(2, 5);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Reason.Length);
            CollectionAssert.AreEqual(mockCodes, result.Payload);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(123456)]
        public async Task GenerateCodes_InvalidCount_ReturnsError(int count)
        {
            //Arrange
            string expectedReason = ErrorMessageHelper.GenerateCountInputError(count);
            
            // Act
            var result = await _service.GenerateCodes(count, 5);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(expectedReason, result.Reason);
            Assert.AreEqual(0, result.Payload.Count);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(12)]
        public async Task GenerateCodes_InvalidLength_ReturnsError(int length)
        {
            // Arrange
            string expectedReason = ErrorMessageHelper.GenerateLengthInputError(length);

            // Act
            var result = await _service.GenerateCodes(6, length);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(expectedReason, result.Reason);
            Assert.AreEqual(0, result.Payload.Count);
        }

        [TestMethod]
        public async Task UseCode_ValidCode_ReturnsSuccess()
        {
            // Arrange
            _mockRepository
                .Setup(repo => repo.UseCode("ABCDE"))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UseCode("ABCDE");

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(true, result.Payload);
        }

        [TestMethod]
        public async Task UseCode_InvalidCode_ReturnsFailure()
        {
            // Arrange
            _mockRepository
                .Setup(repo => repo.UseCode("INVALID"))
                .ReturnsAsync(false);

            // Act
            var result = await _service.UseCode("INVALID");

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(false, result.Payload);
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("ABCDEFGHIJKLMNO")]
        public async Task UseCode_InvalidLengthCode_ReturnsFailureWithoutCheckingRepo(string code)
        {
            // Arrange
            _mockRepository
                .Setup(repo => repo.UseCode(It.IsAny<string>()))
                .ReturnsAsync(true); // Simulates the repository returning true
            string expectedReason = ErrorMessageHelper.GenerateLengthInputError(code.Length);

            // Act
            var result = await _service.UseCode(code);
            
            // Assert
            Assert.IsFalse(result.Success); // The service should fail for invalid length
            Assert.AreEqual(expectedReason, result.Reason); // The reason should match the expected error message
            Assert.AreEqual(false, result.Payload); // Payload should be false
        }

    }
}
