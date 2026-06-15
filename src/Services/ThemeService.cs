using ControlzEx.Theming;
using Metro_WPF_Template_App.Models;
using Application = System.Windows.Application;

/// <summary>
/// ThemeService.cs provides the core functionality for switching themes.
/// </summary>

namespace Metro_WPF_Template_App.Services
{
    public static class ThemeService
    {
        public static List<string> AvailableBaseThemes { get; private set; } = new();
        public static List<Theme> AvailableAccents { get; private set; } = new();

        public static void Initialize(AppSettings settings)
        {
            AvailableBaseThemes = ThemeManager.Current.Themes
                .GroupBy(x => x.BaseColorScheme)
                .Select(g => g.Key)
                .OrderBy(s => s)
                .ToList();

            AvailableAccents = ThemeManager.Current.Themes
                .GroupBy(x => x.ColorScheme)
                .Select(g => g.First())
                .OrderBy(t => t.ColorScheme)
                .ToList();

            ApplyTheme(settings.BaseTheme, settings.AccentColor);
        }

        public static void SetBaseTheme(string baseTheme)
        {
            if (string.IsNullOrWhiteSpace(baseTheme)) return;

            AppSettingsService.CurrentSettings.BaseTheme = baseTheme;
            ApplyTheme(baseTheme, AppSettingsService.CurrentSettings.AccentColor);
        }

        public static void SetAccentColor(string accentColor)
        {
            if (string.IsNullOrWhiteSpace(accentColor)) return;

            AppSettingsService.CurrentSettings.AccentColor = accentColor;
            ApplyTheme(AppSettingsService.CurrentSettings.BaseTheme, accentColor);
        }

        private static void ApplyTheme(string baseTheme, string accentColor)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, baseTheme, accentColor);

            AppSettingsService.SaveAppSettings(AppSettingsService.CurrentSettings);
        }
    }
}