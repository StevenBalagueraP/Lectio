using Lectio.Api.Application.Books.TopRatedBooks.Dtos;

namespace Lectio.Api.Application.Books.BookDetails.Dtos;

public class BookDetailDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public string? Description { get; init; }
    public int? PageCount { get; init; }
    public int? PublishedYear { get; init; }
    public string? Publisher { get; init; }
    public string? Language { get; init; }
    public IReadOnlyList<BookAuthorSummaryDto> Authors { get; init; } = Array.Empty<BookAuthorSummaryDto>();
    public IReadOnlyList<BookCategorySummaryDto> Categories { get; init; } = Array.Empty<BookCategorySummaryDto>();
}