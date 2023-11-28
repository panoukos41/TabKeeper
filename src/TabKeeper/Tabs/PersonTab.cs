using Core;
using TabKeeper.People;

namespace TabKeeper.Tabs;

public sealed record PersonTab
{
    public Person Person { get; }

    public HashSet<Uuid> ProductIds { get; init; } = [];

    public PersonTab(Person person)
    {
        Person = person;
    }

    public override int GetHashCode()
    {
        return Person.Id.GetHashCode();
    }

    public bool Equals(PersonTab? other)
    {
        return Person.Id.Equals(other?.Person.Id);
    }
}
