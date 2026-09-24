using Lectio.Api.Application.Books.TopRatedBooks;
using Lectio.Api.Application.Books.TopRatedBooks.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Controllers.BooksController;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly ITopRatedBooksService _topRatedBooksService;

    public BooksController(ITopRatedBooksService topRatedBooksService)
    {
        _topRatedBooksService = topRatedBooksService;
    }

    /// <summary>
    /// Returns the top 5 highest rated books. If categoryId is provided,
    /// the ranking is scoped to that category; otherwise it covers the whole catalog.
    /// </summary>
    [HttpGet("top-rated")]
    [ProducesResponseType(typeof(TopRatedBooksResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TopRatedBooksResponseDto>> GetTopRatedBooks(
        [FromQuery] int? categoryId,
        CancellationToken cancellationToken)
    {
        var response = await _topRatedBooksService.GetTopRatedBooksAsync(categoryId, cancellationToken);
        return Ok(response);
    }
}
