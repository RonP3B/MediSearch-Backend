using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.MessageBus;
using MediSearch.Infrastructure.MessageBus.Configurations;
using MediSearch.Infrastructure.MessageBus.Registration;
using MediSearch.Shared.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    private const string MessageBusSection = "MessageBus";

    public static void AddMessageBusServices(this IHostApplicationBuilder builder)
    {
        MessageBusOptions messageBusOptions = Guard.Against.Null(
            builder.Configuration.GetSection(MessageBusSection).Get<MessageBusOptions>(),
            $"Configuration key '{MessageBusSection}' is missing or empty."
        );

        builder.Services.AddScoped<IEventBus, EventBus>();

        foreach (var mapping in HandlerConsumerMappings.All)
        {
            builder.Services.AddHandlers(mapping.HandlerInterface, mapping.Assembly);
        }

        builder.Services.AddMassTransit(configurator =>
            MassTransitConfiguration.Configure(configurator, builder, messageBusOptions)
        );

        builder.AddRabbitMQClient(ServiceNames.RabbitMq);
    }
}
