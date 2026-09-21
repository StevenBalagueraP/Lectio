using Lectio.Api.Domain.Entities;
using Lectio.Api.Infrastructure.Data;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks.Dtos;
using Lectio.Api.Infrastructure.ExternalServices.SeedState;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Seeding;

public class GoogleBooksSeederService
{
    private readonly AppDbContext _context;
    private readonly GoogleBooksApiClient _apiClient;
    private readonly SeedProgressStore _progressStore;

    private const int PageSize = 40;
    private const int MaxPagesPerSubquery = 24;

    public GoogleBooksSeederService(AppDbContext context, GoogleBooksApiClient apiClient, SeedProgressStore progressStore)
    {
        _context = context;
        _apiClient = apiClient;
        _progressStore = progressStore;
    }

    public Task<Dictionary<string, int>> SeedFictionAndRomanceAsync() =>
        SeedGroupAsync("ficcion-romance", BookSeedConfigs.FiccionRomance);

    public Task<Dictionary<string, int>> SeedFantasyAndHorrorAsync() =>
        SeedGroupAsync("fantasia-terror", BookSeedConfigs.FantasiaTerror);

    public Task<Dictionary<string, int>> SeedMysteryAndAdventureAsync() =>
        SeedGroupAsync("misterio-aventura", BookSeedConfigs.MisterioAventura);

    public Task<Dictionary<string, int>> SeedHistoryAndScienceAsync() =>
        SeedGroupAsync("historia-ciencia", BookSeedConfigs.HistoriaCiencia);

    public Task<Dictionary<string, int>> SeedBiographyAndPoetryAsync() =>
        SeedGroupAsync("biografia-poesia", BookSeedConfigs.BiografiaPoesia);

    public Task<Dictionary<string, int>> SeedSelfHelpAsync() =>
        SeedGroupAsync("autoayuda", BookSeedConfigs.Autoayuda);

    private async Task<Dictionary<string, int>> SeedGroupAsync(
        string groupKey,
        Dictionary<string, BookSeedCategoryConfig> categoryConfig)
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

        var estados = _progressStore.CargarOCrear(archivoEstado, categoryConfig);
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

                GoogleBooksResponse? response;
                try
                {
                    response = await _apiClient.BuscarAsync(siguiente.Query, siguiente.StartIndex, PageSize);
                }
                catch (GoogleBooksQuotaExceededException ex)
                {
                    Console.WriteLine($"[CUOTA AGOTADA] {ex.Message} Guardando progreso para mañana.");
                    cuotaAgotada = true;
                    break;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"[ERROR PERSISTENTE] {ex.StatusCode} en '{siguiente.Query}' ({categoryName}) tras varios reintentos. Guardando progreso, corre el endpoint de nuevo más tarde.");
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

                    var resultado = VolumeItemProcessor.Procesar(item, librosPorIsbn, categoriaLinksExistentes, isbnsEnEstaPagina, category, contador);
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

        _progressStore.Guardar(archivoEstado, estados);

        foreach (var categoryName in categoryConfig.Keys)
        {
            if (insertadosCategoria[categoryName] < minTargets[categoryName])
                Console.WriteLine($"[ADVERTENCIA] '{categoryName}' tiene {insertadosCategoria[categoryName]} libros (mínimo {minTargets[categoryName]}). Corre el endpoint otra vez cuando se renueve la cuota.");
        }

        return insertadosCategoria;
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
}