using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Seeding.CategoryTranslation;

public class CategoryTranslationService : ICategoryTranslationService
{
    private readonly AppDbContext _context;

    private static readonly Dictionary<string, string> Translations = new()
    {
        ["Ficción"] = "Fiction",
        ["Fantasía"] = "Fantasy",
        ["Terror"] = "Horror",
        ["Misterio"] = "Mystery",
        ["Aventura"] = "Adventure",
        ["Historia"] = "History",
        ["Ciencia"] = "Science",
        ["Biografía"] = "Biography",
        ["Poesía"] = "Poetry",
        ["Autoayuda"] = "Self-help",
        ["Ensayo"] = "Essay",
        ["Filosofía"] = "Philosophy",
        ["Cómics"] = "Comics",
        ["Infantil"] = "Children's",
        ["Viajes"] = "Travel",
        ["Gastronomía"] = "Food & Cooking",
        ["Negocios"] = "Business",
        ["Tecnología"] = "Technology",
        ["Populares"] = "Popular",
        // "Romance" ya está en inglés en ambos idiomas, no necesita entrada
    };

    public CategoryTranslationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<string, string>> TranslateExistingCategoriesAsync()
    {
        var changes = new Dictionary<string, string>();

        var categories = await _context.Categories
            .Where(c => Translations.Keys.Contains(c.CategoryName))
            .ToListAsync();

        foreach (var category in categories)
        {
            var oldName = category.CategoryName;
            var newName = Translations[oldName];

            category.CategoryName = newName;
            changes[oldName] = newName;
        }

        if (changes.Count > 0)
        {
            await _context.SaveChangesAsync();
        }

        return changes;
    }
}