using System;
using System.Text;
using System.Collections.Generic;
using UnitGenius_GenerationService.Model;
using UnitGenius_GenerationService.Service.Abstraction;

namespace UnitGenius_GenerationService.Service
{
    /// <summary>
    /// Service class for generating unit tests.
    /// </summary>
    public class UnitTestGenerationService : IGenerationService
    {
        /// <summary>
        /// Generates a unit test based on the provided generation request.
        /// </summary>
        /// <param name="request">The generation request containing the code to generate the unit test for.</param>
        /// <returns>The updated generation request with the generated unit test result.</returns>
        public GenerationRequest Generate(GenerationRequest request)
        {
            // Validate request
            if (request.GenerationType != GenerationType.UnitTest)
            {
                request.Status = Status.Failed;
                throw new ArgumentException("Invalid generation type. Only UnitTest generation is supported.");
            }
            if (string.IsNullOrEmpty(request.Code))
            {
                request.Status = Status.Failed;
                throw new ArgumentException("Code is required for generating unit tests.");
            }

            // Extract code function
            string codeFunction = request.Code;

            // Generate unit test
            StringBuilder unitTests = new StringBuilder();

            // Extract function name
            string[] words = codeFunction.Split(' ');
            string functionName = words[2];

            // Generate unit test
            unitTests.AppendLine($"[TestMethod]");
            unitTests.AppendLine($"public void Test_{functionName}_ValidInput()");
            unitTests.AppendLine($"{{");
            unitTests.AppendLine($"    // Arrange");
            unitTests.AppendLine($"    // Provide necessary input values and setup");

            // Placeholder for function call
            unitTests.AppendLine($"    // Act");
            unitTests.AppendLine($"    // Call {functionName} function with test inputs");

            // Placeholder for assertions
            unitTests.AppendLine($"    // Assert");
            unitTests.AppendLine($"    // Verify the result meets the expectation");

            unitTests.AppendLine($"}}");

            request.Result = unitTests.ToString();

            request.Status = Status.Completed;
            return request;
        }
        
    }
}
