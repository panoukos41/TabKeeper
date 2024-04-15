using System.Diagnostics.CodeAnalysis;

namespace System.Reactive.Linq;

public static class IObservableMixins
{
    public static IObservable<bool> IsNotNull<T>(this IObservable<T> observable)
        => observable.Select(static x => x is { });

    [return: NotNull]
    public static IObservable<T> WhereNotNull<T>(this IObservable<T> observable)
        => observable.Where(static x => x is { });
}
