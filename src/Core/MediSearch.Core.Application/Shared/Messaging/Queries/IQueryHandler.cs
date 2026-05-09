using MediatR;

namespace MediSearch.Core.Application.Shared.Messaging.Queries;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
