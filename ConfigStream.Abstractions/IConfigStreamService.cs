using System.Threading.Tasks;

namespace ConfigStream.Abstractions
{
    /// <summary>
    /// Provides services for managing ConfigStream in a particular environment.
    /// This includes the creation of scopes and methods for reading and writing.
    /// </summary>
    public interface IConfigStreamService
    {
        string EnvironmentName { get; }

        /// <summary>
        /// Creates a new scope for ConfigStream management.
        /// </summary>
        IConfigStreamScope CreateScope(string scopeId = null);

        /// <summary>
        /// Reads the Config Value.
        /// </summary>
        Task<ConfigStreamValue> ReadAsync(string groupName, string configName, string targetName = null);
    }
}
