using Lectio.Api.Application.Books.TopRatedBooks.Dtos;

namespace Lectio.Api.Application.Books.BrowseBooks.Dtos;

public class BookSummaryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public BookRatingDto Rating { get; init; } = new();
    public IReadOnlyList<BookAuthorSummaryDto> Authors { get; init; } = Array.Empty<BookAuthorSummaryDto>();
    public IReadOnlyList<BookCategorySummaryDto> Categories { get; init; } = Array.Empty<BookCategorySummaryDto>();
}