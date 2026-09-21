using System.Net;
using System.Net.Http.Json;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks.Dtos;
namespace Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;

public class GoogleBooksApiClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private const int MaxAttempts = 4;

    public GoogleBooksApiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["GoogleBooks:ApiKey"]
            ?? throw new InvalidOperationException("Missing GoogleBooks:ApiKey in configuration");
    }

    private static string EstimatedNextReset()
    {
        var pacificZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        var pacificNow = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, pacificZone);
        var nextMidnight = pacificNow.Date.AddDays(1);
        var pacificReset = new DateTimeOffset(nextMidnight, pacificZone.GetUtcOffset(nextMidnight));

        var bogotaZone = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
        var bogotaReset = TimeZoneInfo.ConvertTime(pacificReset, bogotaZone);
        return bogotaReset.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public async Task<GoogleBooksResponse?> SearchAsync(string query, int startIndex, int pageSize)
    {
        var url = $"https://www.googleapis.com/books/v1/volumes?q={Uri.EscapeDataString(query)}" +
                  $"&maxResults={pageSize}&startIndex={startIndex}&langRestrict=es&key={_apiKey}";

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                return await _http.GetFromJsonAsync<GoogleBooksResponse>(url);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new GoogleBooksQuotaExceededException(
                    $"Google Books quota exhausted on query '{query}' at {DateTime.Now:yyyy-MM-dd HH:mm:ss}. " +
                    $"Estimated reset (Bogotá time): {EstimatedNextReset()}.");
            }
            catch (HttpRequestException ex) when (
                ex.StatusCode == HttpStatusCode.ServiceUnavailable ||
                ex.StatusCode == HttpStatusCode.InternalServerError ||
                ex.StatusCode == HttpStatusCode.GatewayTimeout ||
                ex.StatusCode == HttpStatusCode.BadGateway ||
                ex.StatusCode is null)
            {
                if (attempt == MaxAttempts) throw;

                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                Console.WriteLine($"[GoogleBooksApiClient] {(ex.StatusCode?.ToString() ?? "no status (timeout/network)")} " +
                    $"on '{query}' (attempt {attempt}/{MaxAttempts}). Retrying in {delay.TotalSeconds}s...");
                await Task.Delay(delay);
            }
        }

        return null;
    }
}