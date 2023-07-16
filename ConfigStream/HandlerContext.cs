using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Handler;

namespace ConfigStream
{
    public class HandlerContext : IHandlerContext
    {
        public ConfigStreamValue Value { get; set; }
        public string ScopeId { get; set; }
        public string EnvironmentName { get; set; }
        public string GroupName { get; set; }
        public string ConfigName { get; set; }
        public string TargetName { get; set; }
    }
}
