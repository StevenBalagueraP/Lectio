namespace Lectio.Api.Application.Books.BrowseBooks.Dtos;

public class PaginationDto
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}