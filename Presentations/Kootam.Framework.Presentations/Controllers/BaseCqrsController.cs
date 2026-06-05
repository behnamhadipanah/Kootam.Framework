using Kootam.Cqrs.Abstractions.Commands;
using Kootam.Cqrs.Abstractions.Models;
using Kootam.Cqrs.Abstractions.Queries;
using Kootam.Framework.Presentations.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Framework.Presentations.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseCqrsController : BaseApiController
{

    protected IRequestDispatcher Commands =>
     HttpContext.RequestServices.GetRequiredService<IRequestDispatcher>();

    protected IQueryDispatcher Queries =>
        HttpContext.RequestServices.GetRequiredService<IQueryDispatcher>();


    #region Create
    protected async Task<IActionResult> Create<TCommand, TResult>(TCommand command)
       where TCommand : class, IRequest<TResult>
    {
        Result<TResult> result = await Commands.Send<TResult>(command);

        return result.ToActionResult<TResult>();
    }

    protected async Task<IActionResult> Create<TCommand>(TCommand command)
        where TCommand : class, IRequest
    {
        Result result = await Commands.Send(command);
        return result.ToActionResult();
    }
    #endregion


    #region Update
    protected async Task<IActionResult> Update<TCommand, TResult>(TCommand command)
    where TCommand : class, IRequest<TResult>
    {
        Result<TResult> result = await Commands.Send<TResult>(command);
        return result.ToActionResult();
    }

    protected async Task<IActionResult> Update<TCommand>(TCommand command)
        where TCommand : class, IRequest
    {
        Result result = await Commands.Send(command);
        return result.ToActionResult();
    }
    #endregion

    #region Delete
    protected async Task<IActionResult> Delete<TCommand, TResult>(TCommand command)
          where TCommand : class, IRequest<TResult>
    {
        Result<TResult> result =await Commands.Send<TResult>(command);

        return result.ToActionResult<TResult>();
    }

    protected async Task<IActionResult> Delete<TCommand>(TCommand command)
        where TCommand : class, IRequest
    {
        Result result = await Commands.Send(command);
        return result.ToActionResult();
    }
    #endregion
    #region Query
    protected async Task<IActionResult> Query<TResult>(IQuery<TResult> query)
    {
        var result =await Queries.Execute<TResult>(query);
        return result.ToActionResult();
        //if (result.Success)
        //    return Ok(result.Data);

        //return BadRequest(result.Message);
    }
    #endregion

}
