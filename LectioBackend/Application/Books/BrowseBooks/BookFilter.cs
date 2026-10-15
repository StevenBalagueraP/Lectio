namespace Lectio.Api.Application.Books.BrowseBooks;

/// <summary>
/// Filtros/órdenes opcionales y combinables para GET /api/books
/// (?filter=TopRated&filter=Oldest&filter=Shortest). Solo Available filtra; el resto ordena.
/// </summary>
public enum BookFilter
{
    /// <summary>Mejor rateados primero (AverageRating DESC).</summary>
    TopRated,

    /// <summary>Solo libros disponibles (IsAvailable = true).</summary>
    Available,

    /// <summary>Más recientes primero (PublishedDate DESC, nulls al final).</summary>
    Newest,

    /// <summary>Más viejos primero (PublishedDate ASC, nulls al final).</summary>
    Oldest,

    /// <summary>Más páginas primero (PageCount DESC, nulls al final).</summary>
    Longest,

    /// <summary>Menos páginas primero (PageCount ASC, nulls al final).</summary>
    Shortest
}