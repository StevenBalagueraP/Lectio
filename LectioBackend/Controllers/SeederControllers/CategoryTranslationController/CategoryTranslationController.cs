using Lectio.Api.Application.Seeding.CategoryTranslation;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Controllers.CategoryTranslation;

[ApiController]
[Route("api/seed/categories")]
public class CategoryTranslationController : ControllerBase
{
    private readonly ICategoryTranslationService _service;

    public CategoryTranslationController(ICategoryTranslationService service)
    {
        _service = service;
    }

    [HttpPost("translate")]
    public async Task<IActionResult> TranslateCategories()
    {
        var changes = await _service.TranslateExistingCategoriesAsync();

        return Ok(new
        {
            updatedCount = changes.Count,
            changes
        });
    }
}