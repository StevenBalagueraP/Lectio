using Lectio.Api.Application.Categories.Dtos;

namespace Lectio.Api.Application.Categories;

public interface ICategoriesService
{
    Task<CategoriesResponseDto> GetAllCategoriesAsync(CancellationToken cancellationToken);
}