namespace Lectio.Api.Application.Books.BookRatings.Dtos;

public class BookRatingsResponseDto
{
    public decimal AverageRating { get; init; }
    public int NumberOfRatings { get; init; }
    public IReadOnlyList<BookRatingUserDto> Users { get; init; } = Array.Empty<BookRatingUserDto>();
}