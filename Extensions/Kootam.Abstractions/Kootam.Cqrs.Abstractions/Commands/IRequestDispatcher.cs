using Kootam.Cqrs.Abstractions.Models;

namespace Kootam.Cqrs.Abstractions.Commands;

public interface IRequestDispatcher
{
    Task<Result<TResult>> Send<TResult>(IRequest<TResult> command, CancellationToken cancellationToken = default);
    Task<Result> Send(IRequest command, CancellationToken cancellationToken = default);

}
