using System.Security.Claims;
using Lectio.Api.Application.Loans.CreateLoan;
using Lectio.Api.Application.Loans.CreateLoan.Dtos;
using Lectio.Api.Application.Loans.ReturnLoan;
using Lectio.Api.Application.Loans.ReturnLoan.Dtos;
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
    private readonly ICreateLoanService _createLoanService;
    private readonly IReturnLoanService _returnLoanService;

    public UsersController(
        IUserLoansService userLoansService,
        ICreateLoanService createLoanService,
        IReturnLoanService returnLoanService)
    {
        _userLoansService = userLoansService;
        _createLoanService = createLoanService;
        _returnLoanService = returnLoanService;
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
        if (!IsSelfOrAdmin(userId))
        {
            return Forbid();
        }

        var response = await _userLoansService.GetUserLoansAsync(userId, page, pageSize, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{userId:int}/loans")]
    [ProducesResponseType(typeof(CreateLoanResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateLoanResponseDto>> CreateLoan(
        int userId,
        [FromBody] CreateLoanRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!IsSelfOrAdmin(userId))
        {
            return Forbid();
        }

        var response = await _createLoanService.CreateLoanAsync(userId, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("{userId:int}/loans/{bookId:int}/return")]
    [ProducesResponseType(typeof(ReturnLoanResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReturnLoanResponseDto>> ReturnLoan(
        int userId,
        int bookId,
        CancellationToken cancellationToken)
    {
        if (!IsSelfOrAdmin(userId))
        {
            return Forbid();
        }

        var response = await _returnLoanService.ReturnLoanAsync(userId, bookId, cancellationToken);
        return Ok(response);
    }

    private bool IsSelfOrAdmin(int userId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        return User.IsInRole("admin") || currentUserId == userId;
    }
}