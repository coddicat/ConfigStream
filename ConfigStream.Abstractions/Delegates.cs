using ConfigStream.Abstractions.Handler;
using System;
using System.Threading.Tasks;

namespace ConfigStream.Abstractions
{
    public delegate Task HandlerDelegate(IHandlerContext context);
    public delegate HandlerDelegate HandlerResolverDelegate(IServiceProvider serviceProvider);
    public delegate string ScopePropertyResolverDelegate(IServiceProvider serviceProvider);
}
