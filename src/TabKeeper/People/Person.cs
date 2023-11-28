using Core;
using Core.Abstractions;
using FluentValidation;

namespace TabKeeper.People;

public sealed record Person : IValid, IEquatable<Uuid>
{
    public required Uuid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool Equals(Uuid other)
    {
        return Id.Equals(other);
    }

    public static IValidator Validator { get; } = InlineValidator.For<Person>(data =>
    {
        data.RuleFor(x => x.Id)
            .NotEmpty();

        data.RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    });
}
