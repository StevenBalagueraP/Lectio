using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Seeding.UserSeed;

public class UserSeederService
{
    private readonly AppDbContext _context;
    private readonly UserDataGenerator _generator;
    private readonly ILogger<UserSeederService> _logger;

    public UserSeederService(AppDbContext context, UserDataGenerator generator, ILogger<UserSeederService> logger)
    {
        _context = context;
        _generator = generator;
        _logger = logger;
    }

    public async Task<UserSeedResult> SeedUsersAsync(int randomUsersCount = 56, CancellationToken cancellationToken = default)
    {
        var existingEmails = await _context.Users
            .Select(u => u.Email)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existingEmails, StringComparer.OrdinalIgnoreCase);

        var fixedUsers = _generator.GenerateFixedUsers();
        var newFixedUsers = fixedUsers.Where(u => !existingSet.Contains(u.Email)).ToList();
        var skippedFixed = fixedUsers.Count - newFixedUsers.Count;

        foreach (var u in newFixedUsers)
            existingSet.Add(u.Email);

        var randomUsers = _generator.GenerateRandomUsers(randomUsersCount, existingSet);
        var toInsert = newFixedUsers.Concat(randomUsers).ToList();

        foreach (var user in toInsert)
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        if (toInsert.Count > 0)
        {
            await _context.Users.AddRangeAsync(toInsert, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Seed de usuarios: {Created} creados, {Skipped} omitidos (ya existían).",
            toInsert.Count, skippedFixed);

        return new UserSeedResult(toInsert.Count, skippedFixed, toInsert.Select(u => u.Email).ToList());
    }
}

public record UserSeedResult(int UsersCreated, int UsersSkipped, IReadOnlyList<string> CreatedEmails);