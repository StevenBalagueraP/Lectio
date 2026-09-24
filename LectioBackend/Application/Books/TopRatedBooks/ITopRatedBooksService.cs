using Lectio.Api.Application.Books.TopRatedBooks.Dtos;

namespace Lectio.Api.Application.Books.TopRatedBooks;

public interface ITopRatedBooksService
{
    /// <summary>
    /// Returns the top rated books, optionally scoped to a single category.
    /// </summary>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TopRatedBooksResponseDto> GetTopRatedBooksAsync(int? categoryId, CancellationToken cancellationToken);
}
