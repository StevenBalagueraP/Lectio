using Lectio.Api.Domain.Entities;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks.Dtos;

namespace Lectio.Api.Application.Seeding;

public static class VolumeItemProcessor
{
    public static ResultadoLibro? Procesar(
        VolumeItem item,
        Dictionary<string, Book> librosPorIsbn,
        HashSet<string> categoriaLinksExistentes,
        HashSet<string> isbnsEnEstaPagina,
        Category category,
        ContadorDescartes contador)
    {
        contador.Revisados++;
        var info = item.VolumeInfo;

        if (info.Language != "es") { contador.Idioma++; return null; }
        if (info.PageCount is null) { contador.SinPaginas++; return null; }
        if (string.IsNullOrWhiteSpace(info.ImageLinks?.Thumbnail)) { contador.SinImagen++; return null; }

        var isbn13 = info.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier;
        if (string.IsNullOrWhiteSpace(isbn13)) { contador.SinIsbn++; return null; }

        if (!isbnsEnEstaPagina.Add(isbn13))
        {
            contador.Duplicado++;
            return null;
        }

        if (librosPorIsbn.TryGetValue(isbn13, out var libroExistente))
        {
            var linkKey = $"{libroExistente.BookId}|{category.CategoryId}";
            if (!categoriaLinksExistentes.Add(linkKey))
            {
                contador.Duplicado++;
                return null;
            }

            contador.CategoriaAgregada++;
            return new ResultadoLibro(null, null, libroExistente);
        }

        var libro = new Book
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

        return new ResultadoLibro(libro, info.Authors ?? new List<string>(), null);
    }

    private static DateOnly? ParsePublishedDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var datePart = raw.Split('T')[0];
        return DateOnly.TryParse(datePart, out var date) ? date : null;
    }
}

public record ResultadoLibro(Book? NuevoLibro, List<string>? AutoresNuevoLibro, Book? LibroExistente);

public class ContadorDescartes
{
    public int Revisados { get; set; }
    public int Idioma { get; set; }
    public int SinPaginas { get; set; }
    public int SinImagen { get; set; }
    public int SinIsbn { get; set; }
    public int Duplicado { get; set; }
    public int CategoriaAgregada { get; set; }
}