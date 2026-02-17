using Kootam.Framework.Utilities.Common.Responses;
using MediatR;

namespace Kootam.Framework.Application.MediatRWrapper;

public interface IHandlerWrapper<TIn, TOut> : IRequestHandler<TIn, ApiResponse<TOut>>
    where TIn : IRequestWrapper<TOut>
{
}