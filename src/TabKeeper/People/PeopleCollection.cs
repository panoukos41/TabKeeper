using Core;
using Core.Abstractions;
using FluentValidation;

namespace TabKeeper.People;

public sealed class PeopleCollection : DictionaryCollection<Uuid, Person>, IValid<PeopleCollection>
{
    public override Uuid GetKeyForItem(Person item)
    {
        return item.Id;
    }

    public static IValidator<PeopleCollection> Validator { get; } = InlineValidator.For<PeopleCollection>(data =>
    {
        data.RuleForEach(x => x).Valid();
    });
}
