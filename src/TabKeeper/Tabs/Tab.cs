using Core;
using System.Text.Json;

namespace TabKeeper.Tabs;

public sealed record Tab
{
    public required Uuid Id { get; init; }

    public required string Name { get; set; }

    public string Place { get; set; } = string.Empty;

    public DateOnly? Date { get; set; }

    public HashSet<Product> Products { get; set; } = [];

    public HashSet<PersonTab> People { get; set; } = [];

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public string Formatted()
    {
        return Place is { Length: > 0 } ? $"{Name} @ {Place}" : Name;
    }
}
