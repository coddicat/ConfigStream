namespace ConfigStream.Admin.Redis
{
    //public interface IConfigStreamService
    //{
    //    ConfigScope CreateScope(string scopeId = null);
    //}
    //public class ConfigStreamService : IConfigStreamService
    //{
    //    private readonly IDatabase _database;
    //    private readonly string _environment;

    //    public ConfigStreamService(IDatabase database, string environment)
    //    {
    //        _database = database;
    //        _environment = environment;
    //    }
    //    public ConfigScope CreateScope(string scopeId = null)
    //    {
    //        return new ConfigScope(_database, scopeId ?? GenerateScopeId(), _environment);
    //    }
    //    private string GenerateScopeId()
    //    {
    //        Guid guid = Guid.NewGuid();
    //        return Convert.ToBase64String(guid.ToByteArray());
    //    }
    //}
}
