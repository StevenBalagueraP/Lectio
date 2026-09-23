using Lectio.Api.Application.Seeding;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Controllers.BookSeed;

[ApiController]
[Route("api/seed")]
[Tags("BookSeed")]
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

    [HttpPost("historia-ciencia")]
    public async Task<IActionResult> SeedHistoriaYCiencia()
    {
        var resultados = await _seeder.SeedHistoryAndScienceAsync();
        return Ok(resultados);
    }

    [HttpPost("biografia-poesia")]
    public async Task<IActionResult> SeedBiografiaYPoesia()
    {
        var resultados = await _seeder.SeedBiographyAndPoetryAsync();
        return Ok(resultados);
    }

    [HttpPost("autoayuda")]
    public async Task<IActionResult> SeedAutoayuda()
    {
        var resultados = await _seeder.SeedSelfHelpAsync();
        return Ok(resultados);
    }

    [HttpPost("ensayo-filosofia")]
    public async Task<IActionResult> SeedEnsayoYFilosofia()
    {
        var resultados = await _seeder.SeedEssayAndPhilosophyAsync();
        return Ok(resultados);
    }

    [HttpPost("comics-infantil")]
    public async Task<IActionResult> SeedComicsYInfantil()
    {
        var resultados = await _seeder.SeedComicsAndChildrenAsync();
        return Ok(resultados);
    }

    [HttpPost("viajes-gastronomia")]
    public async Task<IActionResult> SeedViajesYGastronomia()
    {
        var resultados = await _seeder.SeedTravelAndFoodAsync();
        return Ok(resultados);
    }

    [HttpPost("negocios-tecnologia")]
    public async Task<IActionResult> SeedNegociosYTecnologia()
    {
        var resultados = await _seeder.SeedBusinessAndTechAsync();
        return Ok(resultados);
    }

    [HttpPost("populares")]
    public async Task<IActionResult> SeedPopulares()
    {
        var resultados = await _seeder.SeedPopularesAsync();
        return Ok(resultados);
    }
}