using Lectio.Api.Application.Books.BookDetails;
using Lectio.Api.Application.Books.BookDetails.Dtos;
using Lectio.Api.Application.Books.BrowseBooks;
using Lectio.Api.Application.Books.BrowseBooks.Dtos;
using Lectio.Api.Application.Books.TopRatedBooks;
using Lectio.Api.Application.Books.TopRatedBooks.Dtos;
using Lectio.Api.Application.Books.BookRatings.Dtos;
using Lectio.Api.Application.Books.BookRatings;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Controllers.BooksController;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly ITopRatedBooksService _topRatedBooksService;
    private readonly IBrowseBooksService _browseBooksService;
    private readonly IBookDetailService _bookDetailService;
    private readonly IBookRatingsService _bookRatingsService;

    public BooksController(
        ITopRatedBooksService topRatedBooksService,
        IBrowseBooksService browseBooksService,
        IBookDetailService bookDetailService,
        IBookRatingsService bookRatingsService)
    {
        _topRatedBooksService = topRatedBooksService;
        _browseBooksService = browseBooksService;
        _bookDetailService = bookDetailService;
        _bookRatingsService = bookRatingsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(BrowseBooksResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrowseBooksResponseDto>> GetBooks(
        [FromQuery] string? search,
        [FromQuery] int[]? categoryIds,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _browseBooksService.GetBooksAsync(search, categoryIds, page, pageSize, cancellationToken);
        return Ok(response);
    }

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

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDetailDto>> GetBookById(int id, CancellationToken cancellationToken)
    {
        var response = await _bookDetailService.GetBookDetailAsync(id, cancellationToken);
        return Ok(response);
    }
    [HttpGet("{id:int}/ratings")]
    [ProducesResponseType(typeof(BookRatingsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookRatingsResponseDto>> GetBookRatings(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _bookRatingsService.GetBookRatingsAsync(id, page, pageSize, cancellationToken);
        return Ok(response);
    }
}
