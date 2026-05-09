using System.Reflection;
using MediSearch.Core.Application;
using MediSearch.Core.Application.Shared.Compensations.Events;
using MediSearch.Core.Application.Shared.Events.DomainEvents;
using MediSearch.Core.Application.Shared.Events.Notifications;
using MediSearch.Infrastructure.MessageBus.Consumers;

namespace MediSearch.Infrastructure.MessageBus.Registration;

internal static class HandlerConsumerMappings
{
    public static readonly HandlerConsumerMapping[] All =
    [
        new(
            typeof(IDomainEventHandler<>),
            typeof(DomainEventHandlerConsumer<,>),
            ApplicationAssemblyReference.Assembly
        ),
        new(
            typeof(INotificationHandler<>),
            typeof(NotificationHandlerConsumer<,>),
            ApplicationAssemblyReference.Assembly
        ),
        new(
            typeof(ICompensationEventHandler<>),
            typeof(CompensationEventHandlerConsumer<,>),
            ApplicationAssemblyReference.Assembly
        ),
    ];
}

internal sealed record HandlerConsumerMapping(
    Type HandlerInterface,
    Type ConsumerType,
    Assembly Assembly
);
