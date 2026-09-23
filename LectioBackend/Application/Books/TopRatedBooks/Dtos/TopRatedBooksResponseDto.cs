namespace Lectio.Api.Application.Books.TopRatedBooks.Dtos;

public class TopRatedBooksResponseDto
{
    public IReadOnlyList<TopRatedBookDto> Data { get; init; } = Array.Empty<TopRatedBookDto>();
}
