namespace Lectio.Api.Application.Books.BookRatings.Dtos;

public class BookRatingUserDto
{
    public int IdUser { get; init; }
    public string FullName { get; init; } = string.Empty; 
    public string? Comment { get; init; }
    public decimal Rating { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}