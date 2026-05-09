using MediatR;

namespace MediSearch.Core.Application.Shared.Behaviors;

internal sealed class UnitOfWorkBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICompensationManager compensationManager
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICompensationManager _compensationManager = compensationManager;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is not IBaseCommand)
        {
            return await next(cancellationToken);
        }

        try
        {
            TResponse response = await next(cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _compensationManager.Commit();

            return response;
        }
        catch (Exception)
        {
            await _compensationManager.RollbackAsync();

            throw;
        }
    }
}
