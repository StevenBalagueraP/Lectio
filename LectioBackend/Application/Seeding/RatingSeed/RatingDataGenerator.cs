using Lectio.Api.Domain.Entities;

namespace Lectio.Api.Application.Seeding.RatingSeed;

public class RatingDataGenerator
{
    private static readonly Dictionary<int, string[]> CommentsByStars = new()
    {
        [5] = new[]
        {
            "Excelente libro, superó mis expectativas.",
            "Una de las mejores lecturas que he tenido este año.",
            "Lo recomiendo sin dudarlo, historia y personajes muy bien logrados.",
            "Difícil de soltar, lo terminé en pocos días.",
            "Un clásico que vale totalmente la pena."
        },
        [4] = new[]
        {
            "Muy buen libro, aunque el ritmo decae un poco a mitad.",
            "Me gustó bastante, lo recomiendo.",
            "Buena historia, algunos personajes pudieron desarrollarse más.",
            "Entretenido de principio a fin, con algún que otro bache.",
            "Vale la pena leerlo, aunque no es perfecto."
        },
        [3] = new[]
        {
            "Está bien, ni me encantó ni me decepcionó.",
            "Cumple, pero esperaba un poco más.",
            "Lectura decente para pasar el rato.",
            "Tiene partes buenas y partes que se hacen lentas.",
            "Ni fu ni fa, se deja leer."
        },
        [2] = new[]
        {
            "No me convenció del todo, esperaba más.",
            "El ritmo es muy lento para mi gusto.",
            "Algunas partes buenas, pero en general flojo.",
            "Me costó terminarlo.",
            "No es lo que esperaba después de las reseñas."
        },
        [1] = new[]
        {
            "No lo recomiendo, no logró engancharme.",
            "Esperaba mucho más, bastante decepcionante.",
            "Me costó mucho terminarlo, no es para mí.",
            "No cumplió mis expectativas en absoluto.",
            "Uno de los libros que menos he disfrutado."
        }
    };

    public int GenerateStars() => Random.Shared.Next(1, 6);

    public string GenerateComment(int stars)
    {
        var pool = CommentsByStars[stars];
        return pool[Random.Shared.Next(pool.Length)];
    }

    public Rating BuildRating(int bookId, int userId, int loanId, DateTime createdAt)
    {
        var stars = GenerateStars();

        return new Rating
        {
            BookId = bookId,
            UserId = userId,
            LoanId = loanId,
            Stars = stars,
            Comment = GenerateComment(stars),
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
    }
}