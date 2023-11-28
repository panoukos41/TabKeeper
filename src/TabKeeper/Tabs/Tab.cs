using Core;
using System.Text.Json;
using TabKeeper.People;

namespace TabKeeper.Tabs;

public sealed record Tab
{
    public required Uuid Id { get; init; }

    public required string Name { get; set; }

    public string Place { get; set; } = string.Empty;

    public DateOnly? Date { get; set; }

    public HashSet<Person> People { get; } = [];

    public HashSet<Product> Products { get; } = [];

    public HashSet<PersonTab> Tabs { get; } = [];

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public string Formatted()
    {
        return Place is { Length: > 0 } ? $"{Name} @ {Place}" : Name;
    }
}
