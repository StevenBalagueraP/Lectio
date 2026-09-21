using System.Text.Json;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;

namespace Lectio.Api.Infrastructure.ExternalServices.SeedState;

public class SeedProgressStore
{
    public List<QueryState> LoadOrCreate(
        string stateFile,
        Dictionary<string, BookSeedCategoryConfig> categoryConfig)
    {
        List<QueryState> states;

        if (File.Exists(stateFile))
        {
            var json = File.ReadAllText(stateFile);
            states = JsonSerializer.Deserialize<List<QueryState>>(json) ?? new List<QueryState>();
        }
        else
        {
            states = new List<QueryState>();
        }

        foreach (var (categoryName, config) in categoryConfig)
        {
            foreach (var query in config.Subqueries)
            {
                if (!states.Any(s => s.CategoryName == categoryName && s.Query == query))
                {
                    states.Add(new QueryState { CategoryName = categoryName, Query = query, StartIndex = 0, Exhausted = false });
                }
            }
        }

        return states;
    }

    public void Save(string stateFile, List<QueryState> states)
    {
        var json = JsonSerializer.Serialize(states, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(stateFile, json);
    }
}