using Core.Preferences.Builders;
using Core.Preferences.Controls;

namespace TabKeeper;

public static class Preferences
{
    public const string Language = "lang";

    public const string Theme = "theme";

    public static PreferenceScreen Screen { get; } = PreferenceScreenBuilder
        .CreateEmpty()
        .AddCategory(b => b
            .WithTitle("settings.app.title")
            .AddListBox(new()
            {
                Key = Preferences.Theme,
                Title = "settings.theme.title",
                AllowedValues = [
                    "auto",
                    "fall-light",
                    "fall-dark",
                    "spring-light",
                    "spring-dark",
                    "summer-light",
                    "summer-dark",
                    "winter-light",
                    "winter-dark",
                ],
                DefaultValue = "auto",
                SummaryProvider = value => $"settings.theme.{value}",
            })
            .AddListBox(new()
            {
                Key = Preferences.Language,
                Title = "settings.lang.title",
                AllowedValues = ["en", "el"],
                DefaultValue = "en",
                SummaryProvider = value => $"settings.lang.{value}",
            })
        )
        .Build();
}
