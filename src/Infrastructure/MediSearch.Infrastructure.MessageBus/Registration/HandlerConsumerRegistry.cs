using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MediSearch.Infrastructure.MessageBus.Registration;

internal static class HandlerConsumerRegistry
{
    public static void RegisterHandlerConsumers(
        this IRegistrationConfigurator configurator,
        Type openGenericHandlerInterface,
        Type openGenericConsumer,
        Assembly assembly,
        string instanceId
    )
    {
        GetHandlerConsumerMappings(assembly, openGenericHandlerInterface)
            .ForEach(m =>
                configurator
                    .AddConsumer(openGenericConsumer.MakeGenericType(m.HandlerType, m.MessageType))
                    .Endpoint(c => c.InstanceId = instanceId)
            );
    }

    public static IServiceCollection AddHandlers(
        this IServiceCollection services,
        Type openGenericHandlerInterface,
        Assembly assembly
    ) =>
        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(c => c.AssignableTo(openGenericHandlerInterface))
                .AsSelf()
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

    private static List<(Type HandlerType, Type MessageType)> GetHandlerConsumerMappings(
        Assembly assembly,
        Type openGenericHandlerInterface
    ) =>
        [
            .. assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(handlerType =>
                    handlerType
                        .GetInterfaces()
                        .Where(i =>
                            i.IsGenericType
                            && i.GetGenericTypeDefinition() == openGenericHandlerInterface
                        )
                        .Select(i =>
                            (HandlerType: handlerType, MessageType: i.GetGenericArguments()[0])
                        )
                ),
        ];
}
