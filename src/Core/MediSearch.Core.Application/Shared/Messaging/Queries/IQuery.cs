using MediatR;

namespace MediSearch.Core.Application.Shared.Messaging.Queries;

public interface IQuery<TResponse> : IRequest<TResponse>;
