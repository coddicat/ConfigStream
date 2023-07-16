using ConfigStream.Abstractions;

namespace ConfigStream.Builder
{
    internal class ConfigStreamOptions
    {
        public HandlerResolverDelegate EnvironmentReadHandlerResolver;
        public HandlerResolverDelegate ScopeReadHandlerResolver;
        public HandlerResolverDelegate ScopeWriteHandlerResolver;
        public ScopePropertyResolverDelegate EnvironmentResolver;
        public ScopePropertyResolverDelegate ScopeIdResolver;
    }
}
