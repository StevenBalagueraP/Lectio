namespace Lectio.Api.Application.Books.BrowseBooks.Dtos;

public class BrowseBooksResponseDto
{
    public IReadOnlyList<BookSummaryDto> Data { get; init; } = Array.Empty<BookSummaryDto>();
    public PaginationDto Pagination { get; init; } = new();
}