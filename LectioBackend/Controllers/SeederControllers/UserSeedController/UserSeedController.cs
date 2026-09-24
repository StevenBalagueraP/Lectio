using Lectio.Api.Application.Seeding.UserSeed;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Controllers.UserSeed;

[ApiController]
[Route("api/seed")]
[Tags("UserSeed")]
public class UserSeedController : ControllerBase
{
    private readonly UserSeederService _userSeederService;

    public UserSeedController(UserSeederService userSeederService)
    {
        _userSeederService = userSeederService;
    }

    [HttpPost("users")]
    public async Task<IActionResult> SeedUsers([FromQuery] int count = 56, CancellationToken cancellationToken = default)
    {
        var result = await _userSeederService.SeedUsersAsync(count, cancellationToken);
        return Ok(result);
    }
}