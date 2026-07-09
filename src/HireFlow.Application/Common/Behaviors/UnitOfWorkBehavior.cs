using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Common.Behaviors;

/// <summary>
/// Commits the database context exactly once, after a handler completes
/// successfully. Repositories only stage changes — this is the single
/// place persistence actually happens. Safe to run for queries too:
/// EF Core's SaveChangesAsync is a no-op when nothing was tracked for change.
/// </summary>
public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}
