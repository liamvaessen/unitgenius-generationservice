using UnitGenius_GenerationService.Model;

namespace UnitGenius_GenerationService.Service.Abstraction
{
    /// <summary>
    /// Represents a generation service that generates a generation request.
    /// All generation services should implement this interface and provide their own implementation.
    /// This ensures that all generation services can be used interchangeably and that the generation service can be easily replaced.
    /// </summary>
    public interface IGenerationService
    {
        /// <summary>
        /// Generates a generation request.
        /// </summary>
        /// <param name="request">The generation request.</param>
        /// <returns>The generated generation request.</returns>
        GenerationRequest Generate(GenerationRequest request);
    }
}
