using Lectio.Api.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Controllers;

[ApiController]
[Route("api/seed")]
public class SeedController : ControllerBase
{
    private readonly GoogleBooksSeederService _seeder;

    public SeedController(GoogleBooksSeederService seeder)
    {
        _seeder = seeder;
    }

    [HttpPost("ficcion-romance")]
    public async Task<IActionResult> SeedFiccionYRomance()
    {
        var resultados = await _seeder.SeedFictionAndRomanceAsync();
        return Ok(resultados);
    }

    [HttpPost("fantasia-terror")]
    public async Task<IActionResult> SeedFantasiaYTerror()
    {
        var resultados = await _seeder.SeedFantasyAndHorrorAsync();
        return Ok(resultados);
    }

    [HttpPost("misterio-aventura")]
    public async Task<IActionResult> SeedMisterioYAventura()
    {
        var resultados = await _seeder.SeedMysteryAndAdventureAsync();
        return Ok(resultados);
    }
}