using Lectio.Api.Domain.Entities;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks.Dtos;

namespace Lectio.Api.Application.Seeding;

public static class VolumeItemProcessor
{
    public static BookResult? Process(
        VolumeItem item,
        Dictionary<string, Book> booksByIsbn,
        HashSet<string> existingCategoryLinks,
        HashSet<string> isbnsInThisPage,
        Category category,
        DiscardCounter counter)
    {
        counter.Reviewed++;
        var info = item.VolumeInfo;

        if (info.Language != "es") { counter.Language++; return null; }
        if (info.PageCount is null) { counter.NoPages++; return null; }
        if (string.IsNullOrWhiteSpace(info.ImageLinks?.Thumbnail)) { counter.NoImage++; return null; }

        var isbn13 = info.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier;
        if (string.IsNullOrWhiteSpace(isbn13)) { counter.NoIsbn++; return null; }

        if (!isbnsInThisPage.Add(isbn13))
        {
            counter.Duplicate++;
            return null;
        }

        if (booksByIsbn.TryGetValue(isbn13, out var existingBook))
        {
            var linkKey = $"{existingBook.BookId}|{category.CategoryId}";
            if (!existingCategoryLinks.Add(linkKey))
            {
                counter.Duplicate++;
                return null;
            }

            counter.CategoryAdded++;
            return new BookResult(null, null, existingBook);
        }

        // Un libro nuevo sin autores válidos no se inserta (evita libros huérfanos en BookAuthors).
        var authors = info.Authors?
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(a => a.Trim())
            .ToList();

        if (authors is null || authors.Count == 0)
        {
            counter.NoAuthors++;
            return null;
        }

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

        return new BookResult(book, authors, null);
    }

    private static DateOnly? ParsePublishedDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var datePart = raw.Split('T')[0];
        return DateOnly.TryParse(datePart, out var date) ? date : null;
    }
}

public record BookResult(Book? NewBook, List<string>? NewBookAuthors, Book? ExistingBook);

public class DiscardCounter
{
    public int Reviewed { get; set; }
    public int Language { get; set; }
    public int NoPages { get; set; }
    public int NoImage { get; set; }
    public int NoIsbn { get; set; }
    public int NoAuthors { get; set; }
    public int Duplicate { get; set; }
    public int CategoryAdded { get; set; }
}