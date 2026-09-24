// Controllers/CategoriesController/CategoriesController.cs
using Lectio.Api.Application.Categories;
using Lectio.Api.Application.Categories.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Controllers.CategoriesController;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categoriesService;

    public CategoriesController(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CategoriesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoriesResponseDto>> GetAllCategories(CancellationToken cancellationToken)
    {
        var response = await _categoriesService.GetAllCategoriesAsync(cancellationToken);
        return Ok(response);
    }
}