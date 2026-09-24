namespace Lectio.Api.Application.Books.TopRatedBooks.Dtos;

public class TopRatedBookDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public BookRatingDto Rating { get; init; } = new();
    public IReadOnlyList<BookAuthorSummaryDto> Authors { get; init; } = Array.Empty<BookAuthorSummaryDto>();
    public IReadOnlyList<BookCategorySummaryDto> Categories { get; init; } = Array.Empty<BookCategorySummaryDto>();
}