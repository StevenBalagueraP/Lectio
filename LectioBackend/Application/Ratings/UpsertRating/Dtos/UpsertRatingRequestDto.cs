using System.ComponentModel.DataAnnotations;

namespace Lectio.Api.Application.Ratings.UpsertRating.Dtos;

public class UpsertRatingRequestDto
{
    [Required]
    public int BookId { get; init; }

    [Required]
    [Range(1, 5)]
    public int Stars { get; init; }

    [MaxLength(1000)]
    public string? Comment { get; init; }
}