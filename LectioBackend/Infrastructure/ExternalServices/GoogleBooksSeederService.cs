using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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

    private const int PageSize = 40;
    private const int MaxPagesPerSubquery = 15;

    private static readonly Dictionary<string, (string[] Subqueries, int MinTarget, int MaxTarget)> FiccionRomanceConfig = new()
    {
        ["Ficción"] = (
            new[]
            {
                "novela", "ficción literaria", "cuentos", "relatos cortos",
                "ciencia ficción", "ciencia ficción espacial", "fantasía épica", "fantasía oscura",
                "aventuras", "novela de aventuras", "realismo mágico", "distopía",
                "ficción distópica juvenil", "terror", "cuentos de terror", "misterio",
                "novela negra", "thriller", "literatura latinoamericana", "literatura colombiana",
                "literatura española", "literatura contemporánea", "clásicos de la literatura",
                "novela histórica", "literatura juvenil", "saga fantástica", "ucronía",
                "ficción especulativa", "steampunk", "literatura fantástica"
            },
            500, 2000
        ),
        ["Romance"] = (
            new[]
            {
                "novela romántica", "romance contemporáneo", "romance histórico", "romance de época",
                "romance victoriano", "amor", "amor prohibido", "amor de verano",
                "comedia romántica", "romance juvenil", "romance adolescente", "romance new adult",
                "romance paranormal", "romance fantástico", "suspenso romántico", "romance policíaco",
                "enemigos a amantes", "segunda oportunidad amor", "triángulo amoroso", "romance navideño",
                "romance en la playa", "romance millonario", "bodas novela", "chica busca chico"
            },
            500, 2000
        )
    };

    private static readonly Dictionary<string, (string[] Subqueries, int MinTarget, int MaxTarget)> FantasiaTerrorConfig = new()
    {
        ["Fantasía"] = (
            new[]
            {
                "fantasía épica", "fantasía oscura", "alta fantasía", "fantasía juvenil",
                "saga fantástica", "dragones novela", "magia y hechicería", "criaturas mágicas",
                "mundos fantásticos", "fantasía medieval", "elfos y enanos", "fantasía urbana",
                "fantasía histórica", "cuentos de hadas", "mitología fantástica", "literatura fantástica",
                "reinos y reyes fantasía", "fantasía heroica", "espada y brujería", "fantasía romántica",
                "portal fantástico", "fantasía steampunk", "fantasía gótica", "profecía y elegido",
                "academia de magia", "brujas novela", "vampiros fantasía", "hombres lobo novela",
                "ángeles y demonios", "fantasía apocalíptica"
            },
            500, 2000
        ),
        ["Terror"] = (
            new[]
            {
                "terror", "terror psicológico", "terror sobrenatural", "casas embrujadas",
                "fantasmas novela", "posesión demoníaca", "terror gótico", "terror clásico",
                "relatos de terror", "cuentos de miedo", "horror cósmico", "criaturas del terror",
                "zombis novela", "vampiros terror", "hombres lobo terror", "brujería oscura",
                "terror paranormal", "terror juvenil", "terror victoriano", "monstruos novela",
                "terror slasher", "exorcismo novela", "terror rural", "terror urbano",
                "pesadillas novela", "terror lovecraftiano", "terror asiático", "terror sobrenatural juvenil",
                "terror apocalíptico", "leyendas de terror"
            },
            500, 2000
        )
    };

    private static readonly Dictionary<string, (string[] Subqueries, int MinTarget, int MaxTarget)> MisterioAventuraConfig = new()
    {
        ["Misterio"] = (
            new[]
            {
                "misterio", "novela de misterio", "novela negra", "thriller psicológico",
                "detectives novela", "crimen novela", "suspenso", "misterio policíaco",
                "asesinato misterio", "investigación criminal novela", "misterio clásico",
                "misterio juvenil", "espionaje novela", "misterio victoriano", "misterio nórdico",
                "misterio de época", "misterio urbano", "conspiración novela", "robo novela",
                "misterio sobrenatural"
            },
            500, 2000
        ),
        ["Aventura"] = (
            new[]
            {
                "aventuras", "novela de aventuras", "exploración novela", "piratas novela",
                "supervivencia novela", "expedición novela", "viajes y aventuras",
                "aventura juvenil", "aventura histórica", "tesoros perdidos novela",
                "selva novela", "aventura de acción", "náufragos novela", "aventura marítima",
                "aventura en el desierto", "montañismo novela", "cazatesoros novela"
            },
            500, 2000
        )
    };

    public GoogleBooksSeederService(HttpClient http, AppDbContext context, IConfiguration config)
    {
        _http = http;
        _context = context;
        _apiKey = config["GoogleBooks:ApiKey"]
            ?? throw new InvalidOperationException("Falta GoogleBooks:ApiKey en la configuración");
    }

    public Task<Dictionary<string, int>> SeedFictionAndRomanceAsync() =>
        SeedGroupAsync("ficcion-romance", FiccionRomanceConfig);

    public Task<Dictionary<string, int>> SeedFantasyAndHorrorAsync() =>
        SeedGroupAsync("fantasia-terror", FantasiaTerrorConfig);

    public Task<Dictionary<string, int>> SeedMysteryAndAdventureAsync() =>
        SeedGroupAsync("misterio-aventura", MisterioAventuraConfig);

    private async Task<Dictionary<string, int>> SeedGroupAsync(
        string groupKey,
        Dictionary<string, (string[] Subqueries, int MinTarget, int MaxTarget)> categoryConfig)
    {
        var archivoEstado = $"seed-progress-{groupKey}.json";

        var categorias = new Dictionary<string, Category>();
        var maxTargets = new Dictionary<string, int>();
        var minTargets = new Dictionary<string, int>();
        var insertadosCategoria = new Dictionary<string, int>();

        foreach (var (categoryName, config) in categoryConfig)
        {
            var category = await GetOrCreateCategoryAsync(categoryName);
            categorias[categoryName] = category;
            maxTargets[categoryName] = config.MaxTarget;
            minTargets[categoryName] = config.MinTarget;
            insertadosCategoria[categoryName] = await _context.BookCategories
                .CountAsync(bc => bc.CategoryId == category.CategoryId);
        }

        var librosPorIsbn = await _context.Books
            .Where(b => b.Isbn != null)
            .ToDictionaryAsync(b => b.Isbn!, b => b);

        var categoriaLinksExistentes = new HashSet<string>(
            await _context.BookCategories.Select(bc => bc.BookId + "|" + bc.CategoryId).ToListAsync());

        var estados = CargarOCrearEstados(archivoEstado, categoryConfig);
        bool cuotaAgotada = false;

        while (!cuotaAgotada)
        {
            bool avanzoAlgo = false;

            foreach (var categoryName in categoryConfig.Keys)
            {
                if (insertadosCategoria[categoryName] >= maxTargets[categoryName]) continue;

                var siguiente = estados.FirstOrDefault(e => e.CategoryName == categoryName && !e.Exhausted);
                if (siguiente is null) continue;

                avanzoAlgo = true;
                var contador = new ContadorDescartes();

                var url = $"https://www.googleapis.com/books/v1/volumes?q={Uri.EscapeDataString(siguiente.Query)}" +
                          $"&maxResults={PageSize}&startIndex={siguiente.StartIndex}&langRestrict=es&key={_apiKey}";

                GoogleBooksResponse? response;
                try
                {
                    response = await _http.GetFromJsonAsync<GoogleBooksResponse>(url);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine($"[CUOTA AGOTADA] 429 en '{siguiente.Query}' ({categoryName}). Guardando progreso para mañana.");
                    cuotaAgotada = true;
                    break;
                }

                if (response?.Items is null || response.Items.Count == 0)
                {
                    siguiente.Exhausted = true;
                    Console.WriteLine($"[{categoryName} / \"{siguiente.Query}\"] agotada de forma natural en startIndex {siguiente.StartIndex}.");
                    continue;
                }

                var candidatosNuevos = new List<(Book Libro, List<string> Autores)>();
                var relacionesExistentes = new List<Book>();
                var isbnsEnEstaPagina = new HashSet<string>();
                var category = categorias[categoryName];

                foreach (var item in response.Items)
                {
                    if (insertadosCategoria[categoryName] + candidatosNuevos.Count + relacionesExistentes.Count >= maxTargets[categoryName])
                        break;

                    var resultado = ProcesarItem(item, librosPorIsbn, categoriaLinksExistentes, isbnsEnEstaPagina, category, contador);
                    if (resultado is null) continue;

                    if (resultado.NuevoLibro is not null)
                        candidatosNuevos.Add((resultado.NuevoLibro, resultado.AutoresNuevoLibro!));
                    else
                        relacionesExistentes.Add(resultado.LibroExistente!);
                }

                if (candidatosNuevos.Count > 0)
                {
                    _context.Books.AddRange(candidatosNuevos.Select(c => c.Libro));
                    await _context.SaveChangesAsync(); 

                    foreach (var (libro, autores) in candidatosNuevos)
                    {
                        _context.BookCategories.Add(new BookCategory { BookId = libro.BookId, CategoryId = category.CategoryId });
                        categoriaLinksExistentes.Add($"{libro.BookId}|{category.CategoryId}");
                        librosPorIsbn[libro.Isbn!] = libro; 

                        foreach (var authorName in autores)
                        {
                            var author = await GetOrCreateAuthorAsync(authorName);
                            _context.BookAuthors.Add(new BookAuthor { BookId = libro.BookId, AuthorId = author.AuthorId });
                        }
                    }
                }

                foreach (var libroExistente in relacionesExistentes)
                {
                    _context.BookCategories.Add(new BookCategory { BookId = libroExistente.BookId, CategoryId = category.CategoryId });
                }

                if (candidatosNuevos.Count > 0 || relacionesExistentes.Count > 0)
                {
                    await _context.SaveChangesAsync();
                    insertadosCategoria[categoryName] += candidatosNuevos.Count + relacionesExistentes.Count;
                }

                siguiente.StartIndex += PageSize;
                if (siguiente.StartIndex / PageSize >= MaxPagesPerSubquery) siguiente.Exhausted = true;

                Console.WriteLine($"[{categoryName} / \"{siguiente.Query}\"] revisados: {contador.Revisados}, " +
                    $"nuevos: {candidatosNuevos.Count}, categoría sumada a existente: {relacionesExistentes.Count}, " +
                    $"descartes -> idioma: {contador.Idioma}, sin páginas: {contador.SinPaginas}, sin imagen: {contador.SinImagen}, " +
                    $"sin ISBN: {contador.SinIsbn}, duplicados: {contador.Duplicado}. Total {categoryName}: {insertadosCategoria[categoryName]}");
            }

            if (cuotaAgotada) break;
            if (!avanzoAlgo) break;
        }

        GuardarEstados(archivoEstado, estados);

        foreach (var categoryName in categoryConfig.Keys)
        {
            if (insertadosCategoria[categoryName] < minTargets[categoryName])
                Console.WriteLine($"[ADVERTENCIA] '{categoryName}' tiene {insertadosCategoria[categoryName]} libros (mínimo {minTargets[categoryName]}). Corre el endpoint otra vez cuando se renueve la cuota.");
        }

        return insertadosCategoria;
    }

    private static ResultadoLibro? ProcesarItem(
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

    private List<QueryState> CargarOCrearEstados(
        string archivoEstado,
        Dictionary<string, (string[] Subqueries, int MinTarget, int MaxTarget)> categoryConfig)
    {
        List<QueryState> estados;

        if (File.Exists(archivoEstado))
        {
            var json = File.ReadAllText(archivoEstado);
            estados = JsonSerializer.Deserialize<List<QueryState>>(json) ?? new List<QueryState>();
        }
        else
        {
            estados = new List<QueryState>();
        }

        foreach (var (categoryName, config) in categoryConfig)
        {
            foreach (var query in config.Subqueries)
            {
                if (!estados.Any(e => e.CategoryName == categoryName && e.Query == query))
                {
                    estados.Add(new QueryState { CategoryName = categoryName, Query = query, StartIndex = 0, Exhausted = false });
                }
            }
        }

        return estados;
    }

    private void GuardarEstados(string archivoEstado, List<QueryState> estados)
    {
        var json = JsonSerializer.Serialize(estados, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(archivoEstado, json);
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

        var datePart = raw.Split('T')[0];
        return DateOnly.TryParse(datePart, out var date) ? date : null;
    }

    private record ResultadoLibro(Book? NuevoLibro, List<string>? AutoresNuevoLibro, Book? LibroExistente);

    private class ContadorDescartes
    {
        public int Revisados { get; set; }
        public int Idioma { get; set; }
        public int SinPaginas { get; set; }
        public int SinImagen { get; set; }
        public int SinIsbn { get; set; }
        public int Duplicado { get; set; }
        public int CategoriaAgregada { get; set; }
    }

    private class QueryState
    {
        public string CategoryName { get; set; } = "";
        public string Query { get; set; } = "";
        public int StartIndex { get; set; }
        public bool Exhausted { get; set; }
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