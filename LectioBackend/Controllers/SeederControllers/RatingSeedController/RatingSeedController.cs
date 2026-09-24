using Lectio.Api.Application.Seeding.RatingSeed;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Controllers.RatingSeed;

[ApiController]
[Route("api/seed")]
public class RatingSeedController : ControllerBase
{
    private readonly RatingSeederService _ratingSeederService;

    public RatingSeedController(RatingSeederService ratingSeederService)
    {
        _ratingSeederService = ratingSeederService;
    }

    [HttpPost("rating")]
    public async Task<IActionResult> SeedRatings(CancellationToken cancellationToken = default)
    {
        var result = await _ratingSeederService.SeedRatingsAsync(cancellationToken);
        return Ok(result);
    }
}