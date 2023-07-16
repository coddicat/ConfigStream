using System.Threading.Tasks;

namespace ConfigStream.Abstractions
{
    public interface IConfigStreamScope
    {
        string ScopeId { get; }
        string EnvironmentName { get; }
        Task<ConfigStreamValue> ReadAsync(string groupName, string configName, string targetName = null);
        Task WriteAsync(string value, string groupName, string configName, string targetName = null);
    }
}
