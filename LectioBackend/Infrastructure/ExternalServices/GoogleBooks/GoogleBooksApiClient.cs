using System.Net;
using System.Net.Http.Json;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks.Dtos;
namespace Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;

public class GoogleBooksApiClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private const int MaxIntentos = 4;

    public GoogleBooksApiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["GoogleBooks:ApiKey"]
            ?? throw new InvalidOperationException("Falta GoogleBooks:ApiKey en la configuración");
    }
    private static string ProximoResetEstimado()
    {
        var zonaPacifico = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        var ahoraPacifico = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, zonaPacifico);
        var medianocheSiguiente = ahoraPacifico.Date.AddDays(1);
        var resetPacifico = new DateTimeOffset(medianocheSiguiente, zonaPacifico.GetUtcOffset(medianocheSiguiente));

        var zonaBogota = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
        var resetBogota = TimeZoneInfo.ConvertTime(resetPacifico, zonaBogota);
        return resetBogota.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public async Task<GoogleBooksResponse?> BuscarAsync(string query, int startIndex, int pageSize)
    {
        var url = $"https://www.googleapis.com/books/v1/volumes?q={Uri.EscapeDataString(query)}" +
                  $"&maxResults={pageSize}&startIndex={startIndex}&langRestrict=es&key={_apiKey}";

        for (int intento = 1; intento <= MaxIntentos; intento++)
        {
            try
            {
                return await _http.GetFromJsonAsync<GoogleBooksResponse>(url);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new GoogleBooksQuotaExceededException(
                    $"Cuota de Google Books agotada en la consulta '{query}' a las {DateTime.Now:yyyy-MM-dd HH:mm:ss}. " +
                    $"Reset estimado (hora Bogotá): {ProximoResetEstimado()}.");
            }
            catch (HttpRequestException ex) when (
                ex.StatusCode == HttpStatusCode.ServiceUnavailable ||
                ex.StatusCode == HttpStatusCode.InternalServerError ||
                ex.StatusCode == HttpStatusCode.GatewayTimeout ||
                ex.StatusCode == HttpStatusCode.BadGateway ||
                ex.StatusCode is null)
            {
                if (intento == MaxIntentos) throw;

                var espera = TimeSpan.FromSeconds(Math.Pow(2, intento));
                Console.WriteLine($"[GoogleBooksApiClient] {(ex.StatusCode?.ToString() ?? "sin status (timeout/red)")} " +
                    $"en '{query}' (intento {intento}/{MaxIntentos}). Reintentando en {espera.TotalSeconds}s...");
                await Task.Delay(espera);
            }
        }

        return null;
    }
}