using Lectio.Api.Application.Categories.Dtos;

namespace Lectio.Api.Application.Categories;

public class CategoriesResponseDto
{
    public IReadOnlyList<CategorySummaryDto> Data { get; init; } = Array.Empty<CategorySummaryDto>();
}