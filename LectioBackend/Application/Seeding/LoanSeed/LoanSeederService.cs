using Lectio.Api.Domain.Entities;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Seeding.LoanSeed;

public class LoanSeederService
{
    private const int BooksPerCategory = 10;
    private const int RequiredUsers = 10;
    private static readonly DateTime DefaultStartDate = new(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc);

    private readonly AppDbContext _context;
    private readonly LoanDataGenerator _generator;
    private readonly ILogger<LoanSeederService> _logger;

    public LoanSeederService(AppDbContext context, LoanDataGenerator generator, ILogger<LoanSeederService> logger)
    {
        _context = context;
        _generator = generator;
        _logger = logger;
    }

    public async Task<LoanSeedResult> SeedLoansAsync(DateTime? startDate = null, CancellationToken cancellationToken = default)
    {
        var baseDate = DateTime.SpecifyKind(startDate ?? DefaultStartDate, DateTimeKind.Utc);

        var userIds = await _context.Users
            .OrderBy(u => u.UserId)
            .Take(RequiredUsers)
            .Select(u => u.UserId)
            .ToListAsync(cancellationToken);

        if (userIds.Count < RequiredUsers)
        {
            throw new InvalidOperationException(
                $"Se necesitan al menos {RequiredUsers} usuarios para sembrar préstamos; hay {userIds.Count}. Ejecuta primero el seeder de usuarios.");
        }

        var categoryBookPairs = await (
            from bc in _context.BookCategories
            join b in _context.Books on bc.BookId equals b.BookId
            select new { bc.CategoryId, Book = b }
        ).ToListAsync(cancellationToken);

        var booksByCategory = categoryBookPairs
            .GroupBy(x => x.CategoryId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Book).DistinctBy(b => b.BookId).OrderBy(b => b.BookId).ToList());

        var bookIdsWithLoans = new HashSet<int>(
            await _context.Loans
                .Select(l => l.BookId)
                .Distinct()
                .ToListAsync(cancellationToken));

        var loansToInsert = new List<Loan>();
        var categoriesWithFewBooks = new List<int>();

        foreach (var (categoryId, books) in booksByCategory)
        {
            var alreadyLoanedInCategory = books.Count(b => bookIdsWithLoans.Contains(b.BookId));
            var slotsRemaining = BooksPerCategory - alreadyLoanedInCategory;

            if (slotsRemaining <= 0)
                continue; 

            var candidateBooks = books
                .Where(b => !bookIdsWithLoans.Contains(b.BookId))
                .Take(slotsRemaining)
                .ToList();

            if (alreadyLoanedInCategory + candidateBooks.Count < BooksPerCategory)
                categoriesWithFewBooks.Add(categoryId);

            foreach (var book in candidateBooks)
            {
                var loans = _generator.GenerateLoansForBook(book.BookId, userIds, baseDate);
                loansToInsert.AddRange(loans);
                bookIdsWithLoans.Add(book.BookId);
            }
        }

        if (loansToInsert.Count > 0)
        {
            await _context.Loans.AddRangeAsync(loansToInsert, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Seed de préstamos: {Created} creados en {Categories} categorías.",
            loansToInsert.Count, booksByCategory.Count);

        return new LoanSeedResult(loansToInsert.Count, booksByCategory.Count, categoriesWithFewBooks);
    }
}

public record LoanSeedResult(int LoansCreated, int CategoriesProcessed, IReadOnlyList<int> CategoriesWithFewerThan10Books);