namespace Lectio.Api.Domain.Entities;

public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string? PublisherName { get; set; }
    public DateOnly? PublishedDate { get; set; }
    public string? Description { get; set; }
    public int? PageCount { get; set; }
    public string? ImageLink { get; set; }
    public string? Language { get; set; }
    public decimal AverageRating { get; set; } = 0;
    public int RatingCount { get; set; } = 0;
    public string Isbn { get; set; } = null!;
    public bool IsAvailable { get; set; } = true;
}