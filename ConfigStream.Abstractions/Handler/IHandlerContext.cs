namespace ConfigStream.Abstractions.Handler
{
    public interface IHandlerContext
    {
        string ScopeId { get; }
        string EnvironmentName { get; }
        string GroupName { get; }
        string ConfigName { get; }
        string TargetName { get; }
        ConfigStreamValue Value { get; set; }
    }
}
