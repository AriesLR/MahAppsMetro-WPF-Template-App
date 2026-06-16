using ControlzEx.Theming;
using Metro_WPF_Template_App.Models;
using Serilog;
using System.Diagnostics;
using Application = System.Windows.Application;

/// <summary>
/// ThemeService.cs provides the core functionality for switching themes.
/// </summary>

namespace Metro_WPF_Template_App.Services
{
    public static class ThemeService
    {
        // Source Context For Logger
        private static ILogger _log => Log.ForContext("SourceContext", nameof(ThemeService));

        public static List<string> AvailableBaseThemes { get; private set; } = new();
        public static List<Theme> AvailableAccents { get; private set; } = new();

        public static void Initialize(AppSettings settings)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
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

                _log.Debug("Discovered {BaseThemeCount} base themes and {AccentCount} color accents from ControlzEx.", AvailableBaseThemes.Count, AvailableAccents.Count);

                ApplyTheme(settings.BaseTheme, settings.AccentColor);

                stopwatch.Stop();

                _log.Information("ThemeService initialized in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to initialize application themes. Core theme engine failed.");
            }
        }

        public static void SetBaseTheme(string baseTheme)
        {
            if (string.IsNullOrWhiteSpace(baseTheme))
            {
                _log.Warning("SetBaseTheme rejected: Provided base theme string was null or empty.");
                return;
            }

            _log.Debug("Changing base theme to: {BaseTheme}", baseTheme);
            AppSettingsService.CurrentSettings.BaseTheme = baseTheme;
            ApplyTheme(baseTheme, AppSettingsService.CurrentSettings.AccentColor);
        }

        public static void SetAccentColor(string accentColor)
        {
            if (string.IsNullOrWhiteSpace(accentColor))
            {
                _log.Warning("SetAccentColor rejected: Provided accent color string was null or empty.");
                return;
            }

            _log.Debug("Changing color accent to: {AccentColor}", accentColor);
            AppSettingsService.CurrentSettings.AccentColor = accentColor;
            ApplyTheme(AppSettingsService.CurrentSettings.BaseTheme, accentColor);
        }

        private static void ApplyTheme(string baseTheme, string accentColor)
        {
            try
            {
                ThemeManager.Current.ChangeTheme(Application.Current, baseTheme, accentColor);

                _log.Debug("Theme service successfully rendered {BaseTheme}.{AccentColor}. Saving configuration to disk.", baseTheme, accentColor);
                AppSettingsService.SaveAppSettings(AppSettingsService.CurrentSettings);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to apply theme combination. Base: {BaseTheme}, Accent: {AccentColor}", baseTheme, accentColor);
            }
        }
    }
}