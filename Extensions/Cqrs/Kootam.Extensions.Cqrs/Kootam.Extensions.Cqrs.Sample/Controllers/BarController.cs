using Kootam.Extensions.Cqrs.Abstractions.Commands;
using Kootam.Extensions.Cqrs.Abstractions.Models;
using Kootam.Extensions.Cqrs.Sample.Commands.FooCommands.Create;
using Microsoft.AspNetCore.Mvc;

namespace Kootam.Extensions.Cqrs.Sample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BarController(IRequestDispatcher requestDispatcher) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateBarCommand command)
    {
        Result result = await requestDispatcher.Send(command, HttpContext.RequestAborted);

        return Ok(result);
    }
}