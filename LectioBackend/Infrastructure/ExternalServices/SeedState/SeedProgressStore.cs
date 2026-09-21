using System.Text.Json;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;

namespace Lectio.Api.Infrastructure.ExternalServices.SeedState;

public class SeedProgressStore
{
    public List<QueryState> CargarOCrear(
        string archivoEstado,
        Dictionary<string, BookSeedCategoryConfig> categoryConfig)
    {
        List<QueryState> estados;

        if (File.Exists(archivoEstado))
        {
            var json = File.ReadAllText(archivoEstado);
            estados = JsonSerializer.Deserialize<List<QueryState>>(json) ?? new List<QueryState>();
        }
        else
        {
            estados = new List<QueryState>();
        }

        foreach (var (categoryName, config) in categoryConfig)
        {
            foreach (var query in config.Subqueries)
            {
                if (!estados.Any(e => e.CategoryName == categoryName && e.Query == query))
                {
                    estados.Add(new QueryState { CategoryName = categoryName, Query = query, StartIndex = 0, Exhausted = false });
                }
            }
        }

        return estados;
    }

    public void Guardar(string archivoEstado, List<QueryState> estados)
    {
        var json = JsonSerializer.Serialize(estados, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(archivoEstado, json);
    }
}