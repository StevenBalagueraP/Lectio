namespace Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;

public class GoogleBooksQuotaExceededException : Exception
{
    public GoogleBooksQuotaExceededException(string message) : base(message) { }
}