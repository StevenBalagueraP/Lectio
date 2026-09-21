namespace Lectio.Api.Infrastructure.ExternalServices.SeedState;

public class QueryState
{
    public string CategoryName { get; set; } = "";
    public string Query { get; set; } = "";
    public int StartIndex { get; set; }
    public bool Exhausted { get; set; }
}