using Kootam.Cqrs.Abstractions.Commands;
using Kootam.Cqrs.Abstractions.Models;
using Kootam.Cqrs.Sample.Commands.BarCommands.Create;
using Kootam.Cqrs.Sample.Commands.FooCommands.Create;
using Microsoft.AspNetCore.Mvc;

namespace Kootam.Cqrs.Sample.Controllers;

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