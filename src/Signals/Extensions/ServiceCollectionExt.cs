using Microsoft.Extensions.DependencyInjection;
using Signals.Handlers;
using Signals.Pipelines;
using Signals.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Signals.Extensions
{
    public static class ServiceCollectionExt
    {
        public static IServiceCollection AddSignalProcessor(this IServiceCollection services)
        {
            services.AddTransient<ISignalProcessor, SignalProcessor>();
            return services;
        }

        public static IServiceCollection AddSignalHandlers(this IServiceCollection services)
        {
            var rtn = services.AddSignalHandlers(Assembly.GetCallingAssembly());
            return rtn;
        }

        public static IServiceCollection AddSignalHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly.GetTypes(typeof(ISignalHandler));
                foreach (var type in handlerTypes)
                {
                    services.AddTransient(typeof(ISignalHandler), type);
                }

                var pipelineHandlers = assembly.GetTypes(typeof(IPipelineHandler));
                foreach (var type in pipelineHandlers)
                {
                    services.AddTransient(typeof(IPipelineHandler), type);
                }
            }
            return services;
        }
    }

    internal static class AssemblyExt
    {
        public static List<Type> GetTypes(this Assembly targetAssembly, Type serviceType)
        {
            return targetAssembly
                        .GetTypes()
                            .Where(x => serviceType.IsAssignableFrom(x))
                            .Where(x => x.IsInterface == false)
                            .Where(x => x.IsAbstract == false)
                            .ToList();
        }
    }
}
