using System.Globalization;
using System.Text;
using Lectio.Api.Domain.Entities;

namespace Lectio.Api.Application.Seeding.UserSeed;

public class UserDataGenerator
{
    private const string DefaultPassword = "123456";

    private static readonly (string FullName, string Email)[] FixedAccounts =
    {
        ("Steven Balaguera", "jerstebalpe@gmail.com"),
        ("Steven Balaguera (Test 2)", "steven2@gmail.com"),
        ("Steven Balaguera (Test 3)", "steven3@gmail.com"),
        ("Steven Balaguera (Test 4)", "steven4@gmail.com"),
    };

    private static readonly string[] FirstNames =
    {
        "Juan", "Carlos", "Andrés", "Camila", "Valentina", "Mariana", "Santiago",
        "Sebastián", "Laura", "Daniela", "Felipe", "Alejandra", "Diego", "Paula",
        "Julián", "Natalia", "Cristian", "Juliana", "Óscar", "Tatiana", "Nicolás",
        "Sofía", "David", "Karen", "Miguel", "Angie", "Fabián", "Lorena", "Iván",
        "Melissa"
    };

    private static readonly string[] LastNames =
    {
        "Rodríguez", "Gómez", "González", "Martínez", "Pérez", "Sánchez", "Ramírez",
        "Torres", "Rojas", "Vargas", "Castro", "Ortiz", "Suárez", "Moreno",
        "Cárdenas", "Peña", "Guerrero", "Salazar", "Molina", "Delgado"
    };

    public List<User> GenerateFixedUsers()
    {
        return FixedAccounts
            .Select(a => new User
            {
                FullName = a.FullName,
                Email = a.Email,
                Password = DefaultPassword,
                Phone = "3" + Random.Shared.Next(100000000, 999999999)
            })
            .ToList();
    }

    public List<User> GenerateRandomUsers(int count, HashSet<string> existingEmails)
    {
        var usedEmails = new HashSet<string>(existingEmails, StringComparer.OrdinalIgnoreCase);
        var users = new List<User>(count);

        while (users.Count < count)
        {
            var firstName = FirstNames[Random.Shared.Next(FirstNames.Length)];
            var lastName = LastNames[Random.Shared.Next(LastNames.Length)];
            var email = BuildUniqueEmail(firstName, lastName, usedEmails);

            usedEmails.Add(email);
            users.Add(new User
            {
                FullName = $"{firstName} {lastName}",
                Email = email,
                Password = DefaultPassword,
                Phone = "3" + Random.Shared.Next(100000000, 999999999)
            });
        }

        return users;
    }

    private static string BuildUniqueEmail(string firstName, string lastName, HashSet<string> usedEmails)
    {
        var baseLocal = RemoveDiacritics($"{firstName}.{lastName}").ToLowerInvariant();
        var email = $"{baseLocal}@gmail.com";
        var suffix = 1;

        while (usedEmails.Contains(email))
        {
            email = $"{baseLocal}{suffix}@gmail.com";
            suffix++;
        }

        return email;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}