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
    private const int AuthorNameMaxLength = 255;

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
        var stateFile = $"seed-progress-{groupKey}.json";

        var categories = new Dictionary<string, Category>();
        var maxTargets = new Dictionary<string, int>();
        var minTargets = new Dictionary<string, int>();
        var insertedPerCategory = new Dictionary<string, int>();

        foreach (var (categoryName, config) in categoryConfig)
        {
            var category = await GetOrCreateCategoryAsync(categoryName);
            categories[categoryName] = category;
            maxTargets[categoryName] = config.MaxTarget;
            minTargets[categoryName] = config.MinTarget;
            insertedPerCategory[categoryName] = await _context.BookCategories
                .CountAsync(bc => bc.CategoryId == category.CategoryId);
        }

        var booksByIsbn = await _context.Books
            .Where(b => b.Isbn != null)
            .ToDictionaryAsync(b => b.Isbn!, b => b);

        var existingCategoryLinks = new HashSet<string>(
            await _context.BookCategories.Select(bc => bc.BookId + "|" + bc.CategoryId).ToListAsync());

        var states = _progressStore.LoadOrCreate(stateFile, categoryConfig);
        bool quotaExhausted = false;

        while (!quotaExhausted)
        {
            bool madeProgress = false;

            foreach (var categoryName in categoryConfig.Keys)
            {
                if (insertedPerCategory[categoryName] >= maxTargets[categoryName]) continue;

                var next = states.FirstOrDefault(s => s.CategoryName == categoryName && !s.Exhausted);
                if (next is null) continue;

                madeProgress = true;
                var counter = new DiscardCounter();

                GoogleBooksResponse? response;
                try
                {
                    response = await _apiClient.SearchAsync(next.Query, next.StartIndex, PageSize);
                }
                catch (GoogleBooksQuotaExceededException ex)
                {
                    Console.WriteLine($"[QUOTA EXHAUSTED] {ex.Message} Saving progress for tomorrow.");
                    quotaExhausted = true;
                    break;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"[PERSISTENT ERROR] {ex.StatusCode} on '{next.Query}' ({categoryName}) after several retries. Saving progress, run the endpoint again later.");
                    quotaExhausted = true;
                    break;
                }

                if (response?.Items is null || response.Items.Count == 0)
                {
                    next.Exhausted = true;
                    Console.WriteLine($"[{categoryName} / \"{next.Query}\"] naturally exhausted at startIndex {next.StartIndex}.");
                    continue;
                }

                var newCandidates = new List<(Book NewBook, List<string> Authors)>();
                var existingBooks = new List<Book>();
                var isbnsInThisPage = new HashSet<string>();
                var category = categories[categoryName];

                foreach (var item in response.Items)
                {
                    if (insertedPerCategory[categoryName] + newCandidates.Count + existingBooks.Count >= maxTargets[categoryName])
                        break;

                    var result = VolumeItemProcessor.Process(item, booksByIsbn, existingCategoryLinks, isbnsInThisPage, category, counter);
                    if (result is null) continue;

                    if (result.NewBook is not null)
                        newCandidates.Add((result.NewBook, result.NewBookAuthors!));
                    else
                        existingBooks.Add(result.ExistingBook!);
                }

                if (newCandidates.Count > 0)
                {
                    _context.Books.AddRange(newCandidates.Select(c => c.NewBook));
                    await _context.SaveChangesAsync();

                    foreach (var (book, authors) in newCandidates)
                    {
                        _context.BookCategories.Add(new BookCategory { BookId = book.BookId, CategoryId = category.CategoryId });
                        existingCategoryLinks.Add($"{book.BookId}|{category.CategoryId}");
                        booksByIsbn[book.Isbn!] = book;

                        foreach (var authorName in authors)
                        {
                            var author = await GetOrCreateAuthorAsync(authorName);
                            if (author is null) continue;

                            _context.BookAuthors.Add(new BookAuthor { BookId = book.BookId, AuthorId = author.AuthorId });
                        }
                    }
                }

                foreach (var existingBook in existingBooks)
                {
                    _context.BookCategories.Add(new BookCategory { BookId = existingBook.BookId, CategoryId = category.CategoryId });
                }

                if (newCandidates.Count > 0 || existingBooks.Count > 0)
                {
                    await _context.SaveChangesAsync();
                    insertedPerCategory[categoryName] += newCandidates.Count + existingBooks.Count;
                }

                next.StartIndex += PageSize;
                if (next.StartIndex / PageSize >= MaxPagesPerSubquery) next.Exhausted = true;

                Console.WriteLine($"[{categoryName} / \"{next.Query}\"] reviewed: {counter.Reviewed}, " +
                    $"new: {newCandidates.Count}, category added to existing: {existingBooks.Count}, " +
                    $"discarded -> language: {counter.Language}, no pages: {counter.NoPages}, no image: {counter.NoImage}, " +
                    $"no ISBN: {counter.NoIsbn}, no authors: {counter.NoAuthors}, duplicates: {counter.Duplicate}. Total {categoryName}: {insertedPerCategory[categoryName]}");
            }

            if (quotaExhausted) break;
            if (!madeProgress) break;
        }

        _progressStore.Save(stateFile, states);

        foreach (var categoryName in categoryConfig.Keys)
        {
            if (insertedPerCategory[categoryName] < minTargets[categoryName])
                Console.WriteLine($"[WARNING] '{categoryName}' has {insertedPerCategory[categoryName]} books (minimum {minTargets[categoryName]}). Run the endpoint again when the quota renews.");
        }

        return insertedPerCategory;
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

    private async Task<Author?> GetOrCreateAuthorAsync(string name)
    {
        var trimmed = name?.Trim();
        if (string.IsNullOrEmpty(trimmed)) return null;

        if (trimmed.Length > AuthorNameMaxLength)
        {
            Console.WriteLine($"[WARNING] Author name too long ({trimmed.Length} chars), truncated to {AuthorNameMaxLength}: \"{trimmed}\"");
            trimmed = trimmed[..AuthorNameMaxLength].TrimEnd();
        }

        var existing = await _context.Authors.FirstOrDefaultAsync(a => a.AuthorName == trimmed);
        if (existing is not null) return existing;

        var author = new Author { AuthorName = trimmed };
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }
}