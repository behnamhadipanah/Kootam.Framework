using Kootam.Extensions.Cqrs.Abstractions.Models;

namespace Kootam.Extensions.Cqrs.Abstractions.Commands;

public interface IRequestDispatcher
{
    Task<Result<TResult>> Send<TResult>(IRequest<TResult> command, CancellationToken cancellationToken = default);
    Task<Result> Send(IRequest command, CancellationToken cancellationToken = default);

}
