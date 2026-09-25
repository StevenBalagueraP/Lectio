using System.Security.Claims;
using Lectio.Api.Application.Loans.UserLoans;
using Lectio.Api.Application.Loans.UserLoans.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Lectio.Api.Controllers.UsersController;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserLoansService _userLoansService;

    public UsersController(IUserLoansService userLoansService)
    {
        _userLoansService = userLoansService;
    }

    [HttpGet("{userId:int}/loans")]
    [ProducesResponseType(typeof(UserLoansResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserLoansResponseDto>> GetUserLoans(
        int userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var isAdmin = User.IsInRole("admin");

        if (!isAdmin && currentUserId != userId)
        {
            return Forbid();
        }

        var response = await _userLoansService.GetUserLoansAsync(userId, page, pageSize, cancellationToken);
        return Ok(response);
    }
}