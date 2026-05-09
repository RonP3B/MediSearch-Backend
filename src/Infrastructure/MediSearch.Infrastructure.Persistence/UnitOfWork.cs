using MassTransit;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Core.Domain.SharedKernel.Bases;

namespace MediSearch.Infrastructure.Persistence;

internal sealed class UnitOfWork(AppDbContext appDbContext, IPublishEndpoint publishEndpoint)
    : IUnitOfWork
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await PublishDomainEventsAsync(cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        List<DomainEvent> domainEvents =
        [
            .. _appDbContext
                .ChangeTracker.Entries<BaseEntity>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents.Count != 0)
                .SelectMany(entity =>
                {
                    var events = entity.DomainEvents.ToList();
                    entity.ClearDomainEvents();
                    return events;
                }),
        ];

        if (domainEvents.Count == 0)
        {
            return;
        }

        var tasks = domainEvents.Select(e =>
            _publishEndpoint.Publish(e, e.GetType(), cancellationToken)
        );

        await Task.WhenAll(tasks);
    }
}
