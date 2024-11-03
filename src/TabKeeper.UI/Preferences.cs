using Core.Preferences;
using Core.Preferences.Builders;

namespace TabKeeper;

public static class Preferences
{
    public const string Language = "lang";

    public const string Theme = "theme";

    public static PreferenceScreen Screen { get; } = PreferenceScreenBuilder
        .CreateEmpty()
        .AddCategory(b => b
            .WithTitle("settings.lang.title")
            .AddListBox(new()
            {
                Key = Preferences.Language,
                Title = "settings.lang.title",
                AllowedValues = ["en", "el"],
                DefaultValue = "en",
                SummaryProvider = value => $"settings.lang.{value}",
            })
        )
        .AddCategory(b => b
            .WithTitle("settings.theme.title")
            .AddListBox(new()
            {
                Key = Preferences.Theme,
                Title = "settings.theme.title",
                AllowedValues = ["auto", "dark", "light"],
                DefaultValue = "auto",
                SummaryProvider = value => $"settings.theme.{value}",
            })
        )
    .Build();
}
