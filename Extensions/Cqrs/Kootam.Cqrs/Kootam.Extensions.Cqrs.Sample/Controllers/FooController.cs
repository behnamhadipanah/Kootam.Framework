using Kootam.Extensions.Cqrs.Abstractions.Commands;
using Kootam.Extensions.Cqrs.Abstractions.Models;
using Kootam.Extensions.Cqrs.Abstractions.Queries;
using Kootam.Extensions.Cqrs.Sample.Commands.FooCommands.Create;
using Kootam.Extensions.Cqrs.Sample.Queries.FooQueries;
using Microsoft.AspNetCore.Mvc;

namespace Kootam.Extensions.Cqrs.Sample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FooController(IRequestDispatcher requestDispatcher,
    IQueryDispatcher queryDispatcher) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateFooCommand command)
    {
      Result<bool> result= await requestDispatcher.Send(command,HttpContext.RequestAborted);

     return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var query = new GetFooListQuery();
       var result= await queryDispatcher.Execute(query);
       return Ok(result);    
    }
}
