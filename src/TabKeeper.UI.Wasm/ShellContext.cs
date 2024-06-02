using System.Reactive.Subjects;

namespace TabKeeper;

public sealed class ShellContext
{
    private static readonly BehaviorSubject<string> titleSub = new(string.Empty);

    public static IObservable<string> TitleChanged { get; } = titleSub.AsObservable();

    public static string Title
    {
        get => titleSub.Value;
        set => titleSub.OnNext(value);
    }
}
