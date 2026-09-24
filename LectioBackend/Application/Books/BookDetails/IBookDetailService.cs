using Lectio.Api.Application.Books.BookDetails.Dtos;

namespace Lectio.Api.Application.Books.BookDetails;

public interface IBookDetailService
{
    Task<BookDetailDto> GetBookDetailAsync(int bookId, CancellationToken cancellationToken);
}