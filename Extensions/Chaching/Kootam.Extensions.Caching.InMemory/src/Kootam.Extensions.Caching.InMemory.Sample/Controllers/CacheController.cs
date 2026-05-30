using Kootam.Extensions.Caching.Abstractions;
using Kootam.Extensions.Caching.InMemory.Sample.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Kootam.Extensions.Caching.InMemory.Sample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CacheController(ICacheStore cacheStore) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(string key)
    {
        string result= await cacheStore.GetAsync<string>(key);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CacheRequest request)
    {
        await cacheStore.SetAsync(request.Key, request.Value);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string key)
    {
        var result= await cacheStore.RemoveAsync(key);
        return Ok(result);
    }
}