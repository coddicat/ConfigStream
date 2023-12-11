namespace ConfigStream.Abstractions.Handler
{
    public interface IHandlerContext
    {
        string ScopeId { get; }
        string EnvironmentName { get; }
        string GroupName { get; }
        string ConfigName { get; }
        string TargetName { get; }

        /// <summary>
        /// Value of the ConfigStream, including its status and data
        /// </summary>
        ConfigStreamValue Value { get; set; }
    }
}
