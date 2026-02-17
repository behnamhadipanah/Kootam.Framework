using Kootam.Framework.Utilities.Common;
using Kootam.Framework.Utilities.Common.Responses;
using MediatR;


namespace Kootam.Framework.Application.MediatRWrapper;

public interface IRequestWrapper<T> : IRequest<ApiResponse<T>> { }

