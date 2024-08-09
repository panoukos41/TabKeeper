using Core;
using Core.Abstractions;
using FluentValidation;
using TabKeeper.People;

namespace TabKeeper.Tabs;

/// <summary>
/// Represents a tab of a person.
/// </summary>
public sealed record TabPerson : ISnapshotCode, IValid<TabPerson>
{
    /// <summary>
    /// The person this tab belongs to.
    /// </summary>
    public Person Person { get; }

    /// <summary>
    /// A list of product ids that this person shares with other people.
    /// </summary>
    public HashSet<Uuid> ProductIds { get; init; } = [];

    /// <summary>
    /// A list of products that belong to this person only.
    /// </summary>
    public TabProductCollection Products { get; init; } = [];

    /// <summary>
    /// Initialize a new instance of <see cref="TabPerson"/> providing the person.
    /// </summary>
    public TabPerson(Person person)
    {
        Person = person;
    }

    /// <inheritdoc/>
    public int GetSnapshotCode()
    {
        return HashCode.Combine(Person.GetSnapshotCode(), ProductIds.GetAggregateHashCode(), Products.GetSnapshotCode());
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Person.Id);
    }

    /// <inheritdoc/>
    public bool Equals(TabPerson? other)
    {
        return Person.Equals(other?.Person);
    }

    public static IValidator<TabPerson> Validator { get; } = InlineValidator.For<TabPerson>(data =>
    {
        data.RuleFor(x => x.Person).Valid();
        data.RuleForEach(x => x.ProductIds).NotEmpty();
        data.RuleFor(x => x.Products).Valid();
    });
}
