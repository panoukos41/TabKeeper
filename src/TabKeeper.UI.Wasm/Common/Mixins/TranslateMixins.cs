using Annular.Translate;
using Microsoft.AspNetCore.Components;

namespace Core.Blazor.Translate;

public static class TranslateMixins
{
    public static void Update(this TranslateService translate, IComponent component)
    {
        if (component is not IHandleEvent handleEvent) return;

        var @ref = new WeakReference<IHandleEvent>(handleEvent);

        translate.OnLangChange
            .Select(e => @ref.TryGetTarget(out var target) ? target : null)
            .TakeUntil(static t => t is not null)
            .Subscribe(static t => t?.HandleEventAsync(EventCallbackWorkItem.Empty, null));
    }

    public static void Update(this TranslateService translate, CoreComponent component)
    {
        var @ref = new WeakReference<CoreComponent>(component);

        translate.OnLangChange
            .Select(e => @ref.TryGetTarget(out var target) ? target : null)
            .TakeUntil(static t => t is not null)
            .Subscribe(static t => t?.TriggerUpdate());
    }
}
