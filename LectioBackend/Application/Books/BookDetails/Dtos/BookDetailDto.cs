namespace Lectio.Api.Application.Books.BookDetails.Dtos;

public class BookDetailDto
{
    public int IdBook { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string? Imagen { get; init; }
    public string? Descripcion { get; init; }
    public int? NumeroPaginas { get; init; }
    public int? AnoPublicacion { get; init; }
    public string? Lenguaje { get; init; }
    public IReadOnlyList<BookAuthorDetailDto> Autores { get; init; } = Array.Empty<BookAuthorDetailDto>();
    public IReadOnlyList<BookCategoryDetailDto> Categorias { get; init; } = Array.Empty<BookCategoryDetailDto>();
}