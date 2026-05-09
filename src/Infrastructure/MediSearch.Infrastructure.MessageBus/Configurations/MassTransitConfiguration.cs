using MediSearch.Core.Application.Shared.Exceptions;
using MediSearch.Infrastructure.MessageBus.Registration;
using MediSearch.Infrastructure.Persistence.Shared.Contexts;
using MediSearch.Infrastructure.Persistence.Shared.Exceptions;
using MediSearch.Shared.Constants;

namespace MediSearch.Infrastructure.MessageBus.Configurations;

internal static class MassTransitConfiguration
{
    public static void Configure(
        IBusRegistrationConfigurator configurator,
        IHostApplicationBuilder builder,
        MessageBusOptions options
    )
    {
        string instanceId = InstanceIdBuilder.Build(builder);

        configurator.SetKebabCaseEndpointNameFormatter();

        configurator.AddEntityFrameworkOutbox<AppDbContext>(outbox =>
        {
            outbox.UsePostgres();
            outbox.UseBusOutbox();
            outbox.QueryDelay = TimeSpan.FromSeconds(options.OutboxQueryDelaySeconds);
            outbox.DuplicateDetectionWindow = TimeSpan.FromMinutes(
                options.DuplicateDetectionWindowMinutes
            );
        });

        configurator.AddConfigureEndpointsCallback(
            (context, name, endpointCfg) =>
            {
                endpointCfg.UseMessageRetry(r =>
                {
                    r.Exponential(
                        options.RetryCount,
                        TimeSpan.FromSeconds(options.RetryInitialIntervalSeconds),
                        TimeSpan.FromSeconds(options.RetryMaxIntervalSeconds),
                        TimeSpan.FromSeconds(options.RetryIntervalDeltaSeconds)
                    );

                    r.Ignore<ExpectedResultNotFoundException>();
                    r.Ignore<CorruptedInvariantException>();
                });

                endpointCfg.UseConcurrencyLimit(options.ConcurrentMessageLimit);
                endpointCfg.UseEntityFrameworkOutbox<AppDbContext>(context);
            }
        );

        foreach (var mapping in HandlerConsumerMappings.All)
        {
            configurator.RegisterHandlerConsumers(
                mapping.HandlerInterface,
                mapping.ConsumerType,
                mapping.Assembly,
                instanceId
            );
        }

        configurator.UsingRabbitMq(
            (context, cfg) =>
            {
                cfg.Host(
                    Guard.Against.NullOrWhiteSpace(
                        builder.Configuration.GetConnectionString(ServiceNames.RabbitMq),
                        $"Connection string '{ServiceNames.RabbitMq}' is missing or empty."
                    )
                );
                cfg.PrefetchCount = options.PrefetchCount;
                cfg.ConfigureEndpoints(context);
            }
        );
    }
}
