using Core;
using Core.Abstractions;
using FluentValidation;
using System.Text.Json.Serialization;

namespace TabKeeper.Tabs;

public sealed record TabProduct : IEntity, ISnapshotCode, IValid<TabProduct>
{
    public Uuid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal Quantity { get; set; } = 1;

    public decimal Total
        => Price is < 0 || Quantity is < 0 ? 0 : Price * Quantity;

    public TabProduct() : this(Uuid.NewUuid())
    {
    }

    [JsonConstructor]
    public TabProduct(Uuid id)
    {
        Id = id;
    }

    public int GetSnapshotCode()
    {
        return HashCode.Combine(Id, Name, Price, Quantity);
    }

    public static IValidator<TabProduct> Validator { get; } = InlineValidator.For<TabProduct>(data =>
    {
        data.RuleFor(x => x.Id)
            .NotEmpty();

        data.RuleFor(x => x.Name)
            .Length(1, 100);

        data.RuleFor(x => x.Price)
            .GreaterThan(0);

        data.RuleFor(x => x.Quantity)
            .GreaterThan(0);
    });
}
