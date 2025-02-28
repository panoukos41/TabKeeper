using Core;
using Core.Abstractions;
using FluentValidation;

namespace TabKeeper.Tabs;

public sealed class TabPeopleCollection : DictionaryCollection<Uuid, TabPerson>, IValid<TabPeopleCollection>
{
    public override Uuid GetKeyForItem(TabPerson item)
    {
        return item.Person.Id;
    }

    public static IValidator<TabPeopleCollection> Validator { get; } = InlineValidator.For<TabPeopleCollection>(data =>
    {
        data.RuleForEach(x => x).Valid();
    });
}
