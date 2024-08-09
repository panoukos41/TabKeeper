using Core;
using Core.Abstractions;
using FluentValidation;
using System.Text.Json.Serialization;

namespace TabKeeper.People;

public sealed record Person : IEntity, ISnapshotCode, IValid<Person>
{
    public Uuid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public Person() : this(Uuid.NewUuid())
    {
    }

    [JsonConstructor]
    public Person(Uuid id)
    {
        Id = id;
    }

    public int GetSnapshotCode()
    {
        return HashCode.Combine(Id, Name);
    }

    public static IValidator<Person> Validator { get; } = InlineValidator.For<Person>(data =>
    {
        data.RuleFor(x => x.Id)
            .NotEmpty();

        data.RuleFor(x => x.Name)
            .Length(1, 100);
    });
}
