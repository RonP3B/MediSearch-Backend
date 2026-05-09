using MediatR;
using MediSearch.Core.Application.Accounts.Commands.RequestAccountEmailConfirmation;
using MediSearch.Core.Domain.Users.DomainEvents;

namespace MediSearch.Core.Application.Users.DomainEventHandlers.UserCreated;

public sealed class RequestEmailConfirmationOnUserCreated(ISender sender)
    : DomainEventHandler<UserCreatedDomainEvent>
{
    private readonly ISender _sender = sender;

    public override async Task Handle(
        UserCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        await _sender.Send(
            new RequestAccountEmailConfirmationCommand(domainEvent.Username),
            cancellationToken
        );
    }
}
