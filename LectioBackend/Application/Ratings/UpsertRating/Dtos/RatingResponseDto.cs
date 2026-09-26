namespace Lectio.Api.Application.Ratings.UpsertRating.Dtos;

public class RatingResponseDto
{
    public string Message { get; init; } = string.Empty;
    public int RatingId { get; init; }
    public int BookId { get; init; }
    public int UserId { get; init; }
    public int Stars { get; init; }
    public string? Comment { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}