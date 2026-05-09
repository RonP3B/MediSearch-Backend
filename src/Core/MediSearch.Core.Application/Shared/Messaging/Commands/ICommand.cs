using MediatR;

namespace MediSearch.Core.Application.Shared.Messaging.Commands;

public interface IBaseCommand;

public interface ICommand : IRequest, IBaseCommand;

public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand;
