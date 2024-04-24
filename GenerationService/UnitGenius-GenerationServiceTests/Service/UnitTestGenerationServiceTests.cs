using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitGenius_GenerationService.Model;
using UnitGenius_GenerationService.Service;

namespace UnitGenius_GenerationServiceTests.Service
{
    [TestClass()]
    public class UnitTestGenerationServiceTests
    {
        private UnitTestGenerationService _service;

        [TestInitialize]
        public void SetUp()
        {
            _service = new UnitTestGenerationService();
        }

        [TestMethod()]
        public void GenerateTest_WhenGenerationTypeIsNotUnitTest_ShouldFail()
        {
            // Arrange
            var request = new GenerationRequest
            {
                GenerationType = GenerationType.Documentation, // Not a UnitTest
                Code = "public void MyMethod() {}"
            };

            // Act
            var result = _service.Generate(request);

            // Assert
            Assert.AreEqual(Status.Failed, result.Status);
        }

        [TestMethod()]
        public void GenerateTest_WhenCodeIsEmpty_ShouldFail()
        {
            // Arrange
            var request = new GenerationRequest
            {
                GenerationType = GenerationType.UnitTest,
                Code = string.Empty // Empty code
            };

            // Act
            var result = _service.Generate(request);

            // Assert
            Assert.AreEqual(Status.Failed, result.Status);
        }

        [TestMethod()]
        public void GenerateTest_WhenValidRequest_ShouldGenerateUnitTest()
        {
            // Arrange
            var request = new GenerationRequest
            {
                GenerationType = GenerationType.UnitTest,
                Code = "public void MyMethod() {}"
            };

            // Act
            var result = _service.Generate(request);

            // Assert
            Assert.AreEqual(Status.Completed, result.Status);
            Assert.IsNotNull(result.Result);
        }
    }
}