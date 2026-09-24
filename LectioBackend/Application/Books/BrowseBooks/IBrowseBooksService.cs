using Lectio.Api.Application.Books.BrowseBooks.Dtos;

namespace Lectio.Api.Application.Books.BrowseBooks;

public interface IBrowseBooksService
{
    Task<BrowseBooksResponseDto> GetBooksAsync(
        string? search,
        int? categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}