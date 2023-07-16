using System.Threading.Tasks;

namespace ConfigStream.Abstractions
{
    public interface IConfigStreamService
    {
        string EnvironmentName { get; }
        IConfigStreamScope CreateScope(string scopeId = null);
        Task<ConfigStreamValue> ReadAsync(string groupName, string configName, string targetName = null);
    }
}
