using System;
using System.Diagnostics.CodeAnalysis;

namespace UnitGenius_GenerationService.Model
{
    /// <summary>
    /// Represents a generation request.
    /// </summary>
    public class GenerationRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the request.
        /// </summary>
        public Guid RequestId { get; set; }
        
        /// <summary>
        /// Gets or sets the unique identifier for the user making the request.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the type of generation to perform.
        /// </summary>
        public GenerationType GenerationType { get; set; }

        /// <summary>
        /// Gets or sets the status of the generation request.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets the generated code.
        /// </summary>
        [AllowNull]
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the parameter for the generation request.
        /// </summary>
        [AllowNull]
        public string? Parameter { get; set; }

        /// <summary>
        /// Gets or sets the result of the generation request.
        /// </summary>
        [AllowNull]
        public string? Result { get; set; }
    }

    /// <summary>
    /// Represents the type of generation to perform.
    /// </summary>
    public enum GenerationType
    {
        /// <summary>
        /// Generate unit tests.
        /// </summary>
        UnitTest,

        /// <summary>
        /// Generate documentation.
        /// </summary>
        Documentation,

        DocumentationOpenAI,
            // change based on the genearation type (OpenAI or Tabnine, etc), needs to be done in all services

    }

    /// <summary>
    /// Represents the status of a generation request.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// The request has been requested but not yet completed.
        /// </summary>
        Requested,

        /// <summary>
        /// The request has been completed successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// The request has failed.
        /// </summary>
        Failed
    }
}
