using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Core.Domain.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MediSearch.Infrastructure.Persistence.Shared.Interceptors;

internal sealed class AuditableEntityInterceptor(
    IDateTimeProvider dateTimeProvider,
    ICurrentUser currentUser
) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        string? currentUserId = currentUser.Id?.ToString();
        DateTimeOffset utcNow = dateTimeProvider.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (ShouldSkipUpdate(entry))
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;

                if (currentUserId != null)
                {
                    entry.Entity.CreatedBy = currentUserId;
                }
            }

            entry.Entity.LastModifiedAt = utcNow;

            if (currentUserId != null)
            {
                entry.Entity.LastModifiedBy = currentUserId;
            }
        }
    }

    private static bool ShouldSkipUpdate(EntityEntry<IAuditableEntity> entry)
    {
        return entry.State is not (EntityState.Added or EntityState.Modified)
            && !HasChangedOwnedEntities(entry);
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null
            && r.TargetEntry.Metadata.IsOwned()
            && (
                r.TargetEntry.State == EntityState.Added
                || r.TargetEntry.State == EntityState.Modified
            )
        );
}
