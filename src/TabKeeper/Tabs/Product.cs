using Core;
using Core.Abstractions;
using FluentValidation;

namespace TabKeeper.Tabs;

public sealed record Product : IValid<Product>
{
    public required Uuid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public decimal Quantity { get; init; } = 1;

    public decimal Total
        => Price is < 0 || Quantity is < 0 ? 0 : Price * Quantity;

    public static IValidator<Product> Validator { get; } = InlineValidator.For<Product>(data =>
    {
        data.RuleFor(x => x.Id)
            .NotEmpty();

        data.RuleFor(x => x.Name)
            .Length(3, 100);

        data.RuleFor(x => x.Price)
            .GreaterThan(0);

        data.RuleFor(x => x.Quantity)
            .GreaterThan(0);
    });
}
