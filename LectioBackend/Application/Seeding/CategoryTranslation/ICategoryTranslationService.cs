namespace Lectio.Api.Application.Seeding.CategoryTranslation;

public interface ICategoryTranslationService
{
    Task<Dictionary<string, string>> TranslateExistingCategoriesAsync();
}