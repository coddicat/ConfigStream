using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Builder;
using ConfigStream.Abstractions.Handler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigStream.Builder
{
    internal class PipelineBuilder : IPipelineBuilder
    {
        private readonly List<Type> _handlerChain = new List<Type>();

        public PipelineBuilder(IServiceCollection services)
        {
            Services = services;
        }
        public IServiceCollection Services { get; }
        public IPipelineBuilder Use<T>() where T : IConfigStreamHandler
        {
            Type handlerType = typeof(T);
            _handlerChain.Insert(0, handlerType);
            return this;
        }

        internal HandlerResolverDelegate Build()
        {
            HandlerResolverDelegate next = (x) => (y) => Task.CompletedTask;

            foreach (var type in _handlerChain)
            {
                Func<IServiceProvider, IConfigStreamHandler> handlerResolver = (serviceProvider) =>
                {
                    IConfigStreamHandler currentStep = serviceProvider.GetService(type) as IConfigStreamHandler;
                    return currentStep ?? (IConfigStreamHandler) ActivatorUtilities.CreateInstance(serviceProvider, type);
                };

                HandlerResolverDelegate currentNext = next;

                next = (serviceProvider) =>
                {
                    HandlerDelegate configStreamDelegate = (context) => handlerResolver(serviceProvider).HandleAsync(context, currentNext(serviceProvider));
                    return configStreamDelegate;
                };
            }

            return next;
        }
    }
}
