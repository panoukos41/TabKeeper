using Core.Preferences.Abstract;

namespace TabKeeper.Components.Preferences.Abstract;

public abstract class PreferenceValueComponentBase<TPreference> : PreferenceComponentBase<TPreference>
    where TPreference : PreferenceValueBase
{
    protected string GetValueSummary()
        => Root?.SummaryTransformer is { } transformer ? transformer(Preference.ValueSummary) : Preference.ValueSummary;
}

public abstract class PreferenceValueComponentBase<TPreference, TValue> : PreferenceValueComponentBase<TPreference>
    where TPreference : PreferenceValueBase<TValue>
    where TValue : IParsable<TValue>, new()
{
}
