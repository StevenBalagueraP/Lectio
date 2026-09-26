using Lectio.Api.Application.Ratings.UpsertRating.Dtos;

namespace Lectio.Api.Application.Ratings.UpsertRating;

public interface IUpsertRatingService
{
    Task<RatingResponseDto> UpsertRatingAsync(
        int userId,
        UpsertRatingRequestDto request,
        CancellationToken cancellationToken);
}