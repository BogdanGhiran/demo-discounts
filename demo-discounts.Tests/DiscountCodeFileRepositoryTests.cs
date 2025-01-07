using demo_discounts_api.Models;
using demo_discounts_api.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace demo_discounts.Tests
{
    [TestClass]
    public class DiscountCodeFileRepositoryTests
    {
        private const string TestFilePath = "test_data/discount_codes.json";
        private DiscountCodeFileRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            // Ensure the test file directory exists
            if (Directory.Exists("test_data"))
            {
                Directory.Delete("test_data", true);
            }

            // Initialize the repository with a test file path
            _repository = new DiscountCodeFileRepository(TestFilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Delete the test data file after each test
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }
        }

        [TestMethod]
        public async Task GenerateCodes_ShouldCreateUniqueCodes()
        {
            // Act
            var codes = await _repository.GenerateCodes(5, 7);

            // Assert
            Assert.AreEqual(5, codes.Count);
            Assert.AreEqual(codes.Distinct().Count(), codes.Count);

            // Verify the file contains the generated codes
            var fileContent = File.ReadAllText(TestFilePath);
            Assert.IsTrue(fileContent.Contains(codes.First()));
        }

        [TestMethod]
        public async Task GenerateCodes_ShouldNotCreateDuplicateCodes()
        {
            // Arrange
            await _repository.GenerateCodes(5, 7);

            // Act
            var additionalCodes = await _repository.GenerateCodes(5, 7);

            // Assert
            Assert.AreEqual(5, additionalCodes.Count);
            var allCodes = additionalCodes.Concat(await _repository.GenerateCodes(0, 7));
            Assert.AreEqual(allCodes.Distinct().Count(), allCodes.Count()); // Ensure no duplicates
        }

        [TestMethod]
        public async Task UseCode_ShouldMarkCodeAsUsed()
        {
            // Arrange
            var codes = await _repository.GenerateCodes(1, 7);
            var codeToUse = codes.First();

            // Act
            var useResult = await _repository.UseCode(codeToUse);

            // Assert
            Assert.IsTrue(useResult);

            // Verify the code is marked as used in the file
            var fileContent = File.ReadAllText(TestFilePath);
            Assert.IsTrue(fileContent.Contains("\"IsUsed\": true"));
        }

        [TestMethod]
        public async Task UseCode_ShouldFailForInvalidCode()
        {
            // Act
            var useResult = await _repository.UseCode("INVALID_CODE");

            // Assert
            Assert.IsFalse(useResult);
        }

        [TestMethod]
        public async Task UseCode_ShouldFailForAlreadyUsedCode()
        {
            // Arrange
            var codes = await _repository.GenerateCodes(1, 7);
            var codeToUse = codes.First();

            // Act
            await _repository.UseCode(codeToUse); // First use
            var secondUseResult = await _repository.UseCode(codeToUse); // Second use

            // Assert
            Assert.IsFalse(secondUseResult);
        }

        [TestMethod]
        public async Task ConcurrentAccess_ShouldBeThreadSafe()
        {
            // Arrange
            await _repository.GenerateCodes(100, 7);
            var codes = await _repository.GenerateCodes(10, 7);

            // Act
            var tasks = codes.Select(async code =>
            {
                var result = await _repository.UseCode(code);
                return result;
            });

            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.AreEqual(10, results.Count(r => r)); // All 10 codes should be used successfully
            Assert.AreEqual(0, results.Count(r => !r)); // No duplicate successes
        }

        [TestMethod]
        public async Task ConcurrentAccess_ShouldOnlyAllowOneUsage()
        {
            // Arrange
            await _repository.GenerateCodes(100, 7);
            var codes = await _repository.GenerateCodes(10, 7);
            var firstCode = codes[0];

            // Act
            var tasks = codes.Select(async code =>
            {
                var result = await _repository.UseCode(firstCode);
                return result;
            });

            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.AreEqual(1, results.Count(r => r)); // All 10 codes should be used successfully
            Assert.AreEqual(9, results.Count(r => !r)); // No duplicate successes
        }
    }
}
