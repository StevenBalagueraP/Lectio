namespace Lectio.Api.Domain.Entities;

public class Rating
{
    public int RatingId { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int Stars { get; set; }
    public string? Comment { get; set; }

    public int LoanId { get; set; }
    public Loan Loan { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}