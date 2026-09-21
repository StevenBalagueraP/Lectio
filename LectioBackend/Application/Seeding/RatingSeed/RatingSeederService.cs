using Lectio.Api.Domain.Entities;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Seeding.RatingSeed;

public class RatingSeederService
{
    private readonly AppDbContext _context;
    private readonly RatingDataGenerator _generator;
    private readonly ILogger<RatingSeederService> _logger;

    public RatingSeederService(AppDbContext context, RatingDataGenerator generator, ILogger<RatingSeederService> logger)
    {
        _context = context;
        _generator = generator;
        _logger = logger;
    }

    public async Task<RatingSeedResult> SeedRatingsAsync(CancellationToken cancellationToken = default)
    {
        var completedLoans = await _context.Loans
            .Where(l => l.ReturnDate != null)
            .Select(l => new { l.LoanId, l.BookId, l.UserId, l.ReturnDate })
            .ToListAsync(cancellationToken);

        var existingRatedPairs = new HashSet<(int UserId, int BookId)>(
            (await _context.Ratings
                .Select(r => new { r.UserId, r.BookId })
                .ToListAsync(cancellationToken))
                .Select(x => (x.UserId, x.BookId)));

        var ratingsToInsert = new List<Rating>();

        foreach (var loan in completedLoans)
        {
            var key = (loan.UserId, loan.BookId);

            if (!existingRatedPairs.Add(key))
                continue; 

            var createdAt = loan.ReturnDate!.Value;
            ratingsToInsert.Add(_generator.BuildRating(loan.BookId, loan.UserId, loan.LoanId, createdAt));
        }

        if (ratingsToInsert.Count > 0)
        {
            await _context.Ratings.AddRangeAsync(ratingsToInsert, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Seed de ratings: {Created} creados sobre {Loans} préstamos completados.",
            ratingsToInsert.Count, completedLoans.Count);

        return new RatingSeedResult(ratingsToInsert.Count, completedLoans.Count);
    }
}

public record RatingSeedResult(int RatingsCreated, int CompletedLoansEvaluated);