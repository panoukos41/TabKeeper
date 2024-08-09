using Core;
using Core.Abstractions;
using FluentValidation;

namespace TabKeeper.Tabs;

public sealed class TabProductCollection : DictionaryCollection<Uuid, TabProduct>, IValid<TabProductCollection>
{
    public override Uuid GetKeyForItem(TabProduct item)
    {
        return item.Id;
    }

    public static IValidator<TabProductCollection> Validator { get; } = InlineValidator.For<TabProductCollection>(data =>
    {
        data.RuleForEach(x => x).Valid();
    });
}
