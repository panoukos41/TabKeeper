using Core.Preferences;
using Core.Preferences.Abstract;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using TabKeeper.Components.Preferences.Components;

namespace TabKeeper.Components.Preferences.Abstract;

public abstract class PreferenceCollectionComponentBase<TPreferenceCollection> : PreferenceComponentBase<TPreferenceCollection>
    where TPreferenceCollection : PreferenceCollectionBase
{
    protected RenderFragment RenderPreference(PreferenceBase preference) => builder =>
    {
        _ = preference switch
        {
            PreferenceCategory categoryPreference => Render<PreferenceCategoryComponent, PreferenceCategory>(ref builder, ref categoryPreference),
            EditTextPreference editTextPreference => Render<EditTextPreferenceComponent, EditTextPreference>(ref builder, ref editTextPreference),
            SwitchPreference switchPreference => Render<SwitchPreferenceComponent, SwitchPreference>(ref builder, ref switchPreference),
            ListBoxPreference listBoxPreference => Render<ListBoxPreferenceComponent, ListBoxPreference>(ref builder, ref listBoxPreference),
            _ => Nothing.Value
        };
    };

    private static Nothing Render<TComponent, TPreference>(ref RenderTreeBuilder builder, ref TPreference preference)
        where TComponent : notnull, PreferenceComponentBase<TPreference>
        where TPreference : PreferenceBase
    {
        builder.OpenComponent<TComponent>(0);
        builder.AddComponentParameter(1, nameof(Preference), preference);
        builder.CloseComponent();
        return Nothing.Value;
    }
}
