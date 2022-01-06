using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Netcorext.Grpc.Mediator;
using Netcorext.Grpc.Mediator.Abstractions;
using Netcorext.Grpc.Mediator.Internal;
using Netcorext.Grpc.Mediator.Pipelines;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        services.AddTransient<IDispatcher, Dispatcher>();

        // Add Handlers
        var namespacePattern = typeof(IMessageHandler).Namespace ?? string.Empty;

        var types = Assembly.GetEntryAssembly()
                            .GetTypes()
                            .Cast<TypeInfo>()
                            .Where(t => t.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length == 1
                                     && t.ImplementedInterfaces.Any(f => f.IsGenericType && !string.IsNullOrWhiteSpace(f.Namespace) && f.Namespace.StartsWith(namespacePattern, StringComparison.OrdinalIgnoreCase)))
                            .ToArray();

        return AddMediator(services, types, serviceLifetime);
    }

    public static IServiceCollection AddMediator(this IServiceCollection services, Type[] types, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        if (types == null)
            throw new ArgumentNullException(nameof(types));

        var serviceMaps = FindServices(types.Cast<TypeInfo>().ToArray());

        foreach (var map in serviceMaps)
        {
            services.TryAdd(new ServiceDescriptor(map.Interface, map.Implementation, serviceLifetime));
        }

        return services;
    }

    public static IServiceCollection AddPerformancePipeline(this IServiceCollection services, Action<IServiceProvider, PerformanceOptions> configure = default)
    {
        services.TryAddSingleton(provider =>
                                 {
                                     var opt = new PerformanceOptions();

                                     configure?.Invoke(provider, opt);

                                     return opt;
                                 });

        services.AddPipeline<PerformancePipeline>();

        return services;
    }

    public static IServiceCollection AddValidatorPipeline(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly());
        services.AddPipeline<ValidatorPipeline>();

        return services;
    }

    public static IServiceCollection AddPipeline<TPipeline>(this IServiceCollection services) where TPipeline : class, IPipeline
    {
        services.AddTransient<IPipeline, TPipeline>();

        return services;
    }

    private static IEnumerable<ServiceMap> FindServices(params TypeInfo[] types)
    {
        foreach (var type in types)
        {
            var req = type.ImplementedInterfaces.FirstOrDefault(t => t.IsGenericType &&
                                                                     t.GetInterfaces()
                                                                      .Any(t2 => t2 == typeof(IMessageHandler)));

            if (req == null) continue;

            yield return new ServiceMap
                         {
                             Service = req.GenericTypeArguments.First(),
                             Interface = req,
                             Implementation = type
                         };
        }
    }
}