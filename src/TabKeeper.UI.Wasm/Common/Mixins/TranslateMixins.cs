using Microsoft.AspNetCore.Components;
using System.Reactive.Disposables;

namespace Annular.Translate;

public static class TranslateMixins
{
    public static IDisposable Update(this TranslateService translate, IComponent component)
    {
        if (component is not IHandleEvent handleEvent)
            return Disposable.Empty;

        var @ref = new WeakReference<IHandleEvent>(handleEvent);

        return translate.OnLangChange
            .Select(e => @ref.TryGetTarget(out var target) ? target : null)
            .TakeUntil(static t => t is not null)
            .Subscribe(static t => t?.HandleEventAsync(EventCallbackWorkItem.Empty, null));
    }

    public static IDisposable Update(this TranslateService translate, CoreComponent component)
    {
        var @ref = new WeakReference<CoreComponent>(component);

        return translate.OnLangChange
            .Select(e => @ref.TryGetTarget(out var target) ? target : null)
            .TakeUntil(static t => t is not null)
            .Subscribe(static t => t?.Update());
    }
}
