using Kootam.Cqrs.Abstractions.Commands;
using Kootam.Cqrs.Abstractions.Models;
using Kootam.Cqrs.Abstractions.Queries;
using Kootam.Cqrs.Sample.Commands.FooCommands.Create;
using Kootam.Cqrs.Sample.Queries.FooQueries;
using Microsoft.AspNetCore.Mvc;

namespace Kootam.Cqrs.Sample.Controllers;

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
