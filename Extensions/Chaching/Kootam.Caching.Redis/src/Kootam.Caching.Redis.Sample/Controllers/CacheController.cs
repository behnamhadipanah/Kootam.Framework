using Kootam.Caching.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Kootam.Caching.Redis.Sample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CacheController(ICacheAdapter cacheAdapter) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(string key)
    {
      string result= await cacheAdapter.GetAsync<string>(key);
      return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string key, string value)
    {
       await cacheAdapter.SetAsync(key, value);
       return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string key)
    {
       var result= await cacheAdapter.RemoveAsync(key);
       return Ok(result);
    }
}