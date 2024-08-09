using Core;
using Core.Abstractions;
using FluentValidation;
using System.Text.Json.Serialization;

namespace TabKeeper.Tabs;

/// <summary>
/// Represents a tab.
/// </summary>
public sealed record Tab : IEntity, IValid<Tab>
{
    public Uuid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public string? Place { get; set; }

    public Uuid? PayerId { get; set; }

    public DateOnly? Date { get; set; }

    public bool IsLocked { get; set; }

    public TabProductCollection Products { get; set; } = [];

    public TabPeopleCollection People { get; set; } = [];

    public Tab() : this(Uuid.NewUuid())
    {
    }

    [JsonConstructor]
    public Tab(Uuid id)
    {
        Id = id;
    }

    public static IValidator<Tab> Validator { get; } = InlineValidator.For<Tab>(data =>
    {
        data.RuleFor(x => x.Id)
            .NotEmpty();

        data.RuleFor(x => x.Name)
            .Length(1, 100);

        data.RuleFor(x => x.Place)
            .MaximumLength(100);

        data.RuleFor(x => x.PayerId)
            .NotEmpty()
            .When(x => x.PayerId is { });

        data.RuleFor(x => x.Products)
            .Valid()
            .Configure(c => c.RuleSets = ["Lists", "Products"]);

        data.RuleFor(x => x.People)
            .Valid()
            .Configure(c => c.RuleSets = ["Lists", "People"]);
    });
}
