using System.Threading.Tasks;

namespace ConfigStream.Abstractions
{
    /// <summary>
    /// Represents a scope within which Config Values are read and written.
    /// A scope is defined by identifiers like scopeId (correllationId) and environment name
    /// </summary>
    public interface IConfigStreamScope
    {
        string ScopeId { get; }
        string EnvironmentName { get; }

        /// <summary>
        /// Reads the config value
        /// </summary>
        Task<ConfigStreamValue> ReadAsync(string groupName, string configName, string targetName = null);

        /// <summary>
        /// Writes the config value
        /// </summary>
        Task WriteAsync(string value, string groupName, string configName, string targetName = null);
    }
}
