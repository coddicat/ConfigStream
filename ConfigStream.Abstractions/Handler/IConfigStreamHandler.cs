using System.Threading.Tasks;

namespace ConfigStream.Abstractions.Handler
{
    /// <summary>
    /// Represents a handler in the ConfigStream pipeline.
    /// Each handler is responsible for a specific aspect of processing the configuration data,
    /// such as fetching, caching, validating, or transforming the data.
    /// </summary>
    public interface IConfigStreamHandler
    {
        /// <summary>
        /// Handles the ConfigStream, passing control to the next handler in the pipeline.
        /// </summary>
        Task HandleAsync(IHandlerContext context, HandlerDelegate next);
    }
}
