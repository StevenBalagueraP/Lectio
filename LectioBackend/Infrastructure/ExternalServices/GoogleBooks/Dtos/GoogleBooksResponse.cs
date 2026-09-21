using System.Text.Json.Serialization;

namespace Lectio.Api.Infrastructure.ExternalServices.GoogleBooks.Dtos;

public record GoogleBooksResponse([property: JsonPropertyName("items")] List<VolumeItem>? Items);

public record VolumeItem([property: JsonPropertyName("volumeInfo")] VolumeInfo VolumeInfo);

public record VolumeInfo(
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

public record ImageLinks([property: JsonPropertyName("thumbnail")] string? Thumbnail);

public record IndustryIdentifier(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("identifier")] string Identifier
);