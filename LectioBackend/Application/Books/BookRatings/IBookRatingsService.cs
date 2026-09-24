using Lectio.Api.Application.Books.BookRatings.Dtos;

namespace Lectio.Api.Application.Books.BookRatings;

public interface IBookRatingsService
{
    Task<BookRatingsResponseDto> GetBookRatingsAsync(
        int bookId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}