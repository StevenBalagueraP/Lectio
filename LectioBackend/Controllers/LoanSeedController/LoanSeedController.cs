using Lectio.Api.Application.Seeding.LoanSeed;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Controllers.LoanSeed;

[ApiController]
[Route("api/seed")]
public class LoanSeedController : ControllerBase
{
    private readonly LoanSeederService _loanSeederService;

    public LoanSeedController(LoanSeederService loanSeederService)
    {
        _loanSeederService = loanSeederService;
    }

    [HttpPost("loan")]
    public async Task<IActionResult> SeedLoans([FromQuery] DateTime? startDate, CancellationToken cancellationToken = default)
    {
        var result = await _loanSeederService.SeedLoansAsync(startDate, cancellationToken);
        return Ok(result);
    }
}