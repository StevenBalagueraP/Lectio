using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Infrastructure.ExternalServices;

public class GoogleBooksSeederService
{
    private readonly HttpClient _http;
    private readonly AppDbContext _context;
    private readonly string _apiKey;

    private static readonly (string CategoryName, string GoogleQuery)[] SeedCategories =
    {
        ("Ficción", "novela"),
        ("Historia", "historia")
    };

    public GoogleBooksSeederService(HttpClient http, AppDbContext context, IConfiguration config)
    {
        _http = http;
        _context = context;
        _apiKey = config["GoogleBooks:ApiKey"]
            ?? throw new InvalidOperationException("Falta GoogleBooks:ApiKey en la configuración");
    }

    public async Task<int> SeedAsync(int booksPerCategory = 5)
{
    int totalInserted = 0;

    foreach (var (categoryName, query) in SeedCategories)
    {
        var category = await GetOrCreateCategoryAsync(categoryName);

        int insertedForThisCategory = 0;
        int startIndex = 0;
        int totalRevisados = 0;
        int descartadosPorIdioma = 0, descartadosPorPaginas = 0, descartadosPorImagen = 0, descartadosPorIsbn = 0;
        const int pageSize = 40;
        const int maxPages = 10;

        for (int page = 0; page < maxPages && insertedForThisCategory < booksPerCategory; page++)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q={query}&maxResults={pageSize}&startIndex={startIndex}&langRestrict=es&key={_apiKey}";
            var response = await _http.GetFromJsonAsync<GoogleBooksResponse>(url);

            if (response?.Items is null || response.Items.Count == 0) break;

            foreach (var item in response.Items)
            {
                if (insertedForThisCategory >= booksPerCategory) break;
                totalRevisados++;

                var info = item.VolumeInfo;

                if (info.Language != "es") { descartadosPorIdioma++; continue; }
                if (info.PageCount is null) { descartadosPorPaginas++; continue; }
                if (string.IsNullOrWhiteSpace(info.ImageLinks?.Thumbnail)) { descartadosPorImagen++; continue; }

                var isbn13 = info.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier;
                if (string.IsNullOrWhiteSpace(isbn13)) { descartadosPorIsbn++; continue; }

                var exists = await _context.Books.AnyAsync(b => b.Isbn == isbn13);
                if (exists) continue;

                var book = new Book
                {
                    Title = info.Title,
                    PublisherName = info.Publisher,
                    PublishedDate = ParsePublishedDate(info.PublishedDate),
                    Description = info.Description,
                    PageCount = info.PageCount,
                    ImageLink = info.ImageLinks!.Thumbnail,
                    Language = info.Language,
                    Isbn = isbn13,
                    IsAvailable = true
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                _context.BookCategories.Add(new BookCategory { BookId = book.BookId, CategoryId = category.CategoryId });

                foreach (var authorName in info.Authors ?? new List<string>())
                {
                    var author = await GetOrCreateAuthorAsync(authorName);
                    _context.BookAuthors.Add(new BookAuthor { BookId = book.BookId, AuthorId = author.AuthorId });
                }

                await _context.SaveChangesAsync();

                insertedForThisCategory++;
                totalInserted++;
            }

            startIndex += pageSize;
        }

        Console.WriteLine($"[{categoryName}] revisados: {totalRevisados}, insertados: {insertedForThisCategory}, " +
            $"descartados -> idioma: {descartadosPorIdioma}, sin páginas: {descartadosPorPaginas}, sin imagen: {descartadosPorImagen}, sin ISBN: {descartadosPorIsbn}");
    }

    return totalInserted;
}

    private async Task<Category> GetOrCreateCategoryAsync(string name)
    {
        var existing = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == name);
        if (existing is not null) return existing;

        var category = new Category { CategoryName = name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    private async Task<Author> GetOrCreateAuthorAsync(string name)
    {
        var trimmed = name.Trim();
        var existing = await _context.Authors.FirstOrDefaultAsync(a => a.AuthorName == trimmed);
        if (existing is not null) return existing;

        var author = new Author { AuthorName = trimmed };
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }

    private static DateOnly? ParsePublishedDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        // Puede venir como "2024-02-29" o "2025-03-19T00:00:00Z"
        var datePart = raw.Split('T')[0];
        return DateOnly.TryParse(datePart, out var date) ? date : null;
    }
}

record GoogleBooksResponse([property: JsonPropertyName("items")] List<VolumeItem>? Items);
record VolumeItem([property: JsonPropertyName("volumeInfo")] VolumeInfo VolumeInfo);
record VolumeInfo(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("authors")] List<string>? Authors,
    [property: JsonPropertyName("publisher")] string? Publisher,
    [property: JsonPropertyName("publishedDate")] string? PublishedDate,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("pageCount")] int? PageCount,
    [property: JsonPropertyName("imageLinks")] ImageLinks? ImageLinks,
    [property: JsonPropertyName("industryIdentifiers")] List<IndustryIdentifier>? IndustryIdentifiers,
    [property: JsonPropertyName("language")] string? Language
);
record ImageLinks([property: JsonPropertyName("thumbnail")] string? Thumbnail);
record IndustryIdentifier(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("identifier")] string Identifier
);