using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{

    private readonly IDistributedCacheService _distributedCacheService;
    public ProductController(IDistributedCacheService distributedCacheService)
    {
        _distributedCacheService = distributedCacheService;
    }

    [HttpGet("/documents")]
    [OutputCache(Duration = 60)]
    public IActionResult GetDocument()
    {
        string[] stringArray = ["015.838.290.80", "015.838.290.81", "015.838.290.82"];

        string rndItem = Random.Shared.GetItems(stringArray, 1)[0];
        return Ok(rndItem);
    }

    [HttpPost("/person")]
    public async Task<IActionResult> AddPerson([FromBody] Person person)
    {
        await _distributedCacheService.GetOrCreateCacheAsync<Person>("my-person", person, TimeSpan.FromMinutes(1));
        return NoContent();
    }

    [HttpGet("/person")]
    public async Task<IActionResult> GetPerson()
    {
        var result = await _distributedCacheService.GetCacheAsync<Person>("my-person");
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var document = await _distributedCacheService.GetCacheAsync<string>("my-document");
        return Ok(document);
    }

    [HttpGet("{document}")]
    public async Task<IActionResult> GetAdd([FromRoute] string document)
    {
        await _distributedCacheService.GetOrCreateCacheAsync("my-document", document, TimeSpan.FromMinutes(1));
        return NoContent();
    }

}

public class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}