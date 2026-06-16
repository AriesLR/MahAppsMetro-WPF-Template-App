using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Metro_WPF_Template_App.Common.Constants;
using Metro_WPF_Template_App.Services;
using Metro_WPF_Template_App.ViewModels;
using Microsoft.Win32;
using Serilog;
using System.Windows;
using System.Windows.Controls;

namespace Metro_WPF_Template_App.Views.Flyouts
{
    /// <summary>
    /// Interaction logic for MainWindowFlyoutsView.xaml
    /// </summary>
    public partial class MainWindowFlyoutsView : FlyoutsControl
    {
        // Source Context For Logger
        private static ILogger _log => Log.ForContext("SourceContext", nameof(MainWindowFlyoutsView));

        public MainWindowFlyoutsView()
        {
            InitializeComponent();

            //BaseThemeDropdown.ItemsSource = ThemeService.AvailableBaseThemes;
            AccentColorDropdown.ItemsSource = ThemeService.AvailableAccents;

            this.Loaded += MainWindowFlyoutsView_Loaded;
        }

        private void MainWindowFlyoutsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettingsService.CurrentSettings != null)
            {
                SyncUISelections();
            }
        }

        // ============ Button Clicks ============

        // Open Buy Me A Coffee Button
        private void OpenBuyMeACoffee_Click(object sender, RoutedEventArgs e)
        {
            _log.Debug("User clicked Buy Me A Coffee button.");
            UrlService.OpenUrlAsync(AppUrls.BuyMeACoffeeUrl);
        }

        // Open Patreon Button
        private void OpenPatreon_Click(object sender, RoutedEventArgs e)
        {
            _log.Debug("User clicked Patreon button.");
            UrlService.OpenUrlAsync(AppUrls.PatreonUrl);
        }

        // Check For App Updates Button
        private async void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            _log.Debug("User clicked Check for Updates button.");
            await UpdateService.CheckForUpdatesAsync(AppUrls.UpdateUrl);
        }

        // App Settings Button Click
        private void AppSettingsFlyout_Click(object? sender, RoutedEventArgs e)
        {
            _log.Debug("User clicked App Settings button.");
            ((MainWindowViewModel)DataContext).OpenAppSettingsFlyout();
        }

        // ============ App Settings Event Handlers ============

        // Check App Updates Toggled
        private void CheckAppUpdatesToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!MainWindow._isLoaded || AppSettingsService.CurrentSettings == null) return;

            bool isOn = CheckAppUpdatesToggle.IsOn;
            _log.Information("User flipped the Check App Updates Toggle to: {State}", isOn);

            AppSettingsService.CurrentSettings.CheckForUpdatesOnStartup = isOn;
            AppSettingsService.SaveAppSettings(AppSettingsService.CurrentSettings);
        }

        // Start With Windows Toggled
        private async void StartWithWindowsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!MainWindow._isLoaded || AppSettingsService.CurrentSettings == null)
                return;

            bool isOn = StartWithWindowsToggle.IsOn;
            _log.Information("User flipped the Start With Windows Toggle to: {State}", isOn);

            AppSettingsService.CurrentSettings.StartWithWindows = isOn;
            AppSettingsService.SaveAppSettings(AppSettingsService.CurrentSettings);

            const string registryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            string appName = AppConfig.AppName;

            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(registryKeyPath, true))
                {
                    if (key != null)
                    {
                        if (isOn)
                        {
                            string executablePath = Environment.ProcessPath ?? string.Empty;

                            if (!string.IsNullOrEmpty(executablePath))
                            {
                                key.SetValue(appName, $"\"{executablePath}\"");
                                _log.Debug("Successfully added the application to startup programs.");
                            }
                            else
                            {
                                _log.Warning("Failed to resolve the executable's file path.");
                            }
                        }
                        else
                        {
                            if (key.GetValue(appName) != null)
                            {
                                key.DeleteValue(appName);
                                _log.Debug("Successfully removed the application from startup programs.");
                            }
                        }
                    }
                    else
                    {
                        _log.Warning("Registry.CurrentUser.OpenSubKey returned null for the startup path.");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to update registry startup key.");
                await MessageService.ShowError($"Failed to update registry startup key: {ex.Message}");
            }
        }

        // Enable Debug Logging Toggled
        private void EnableDebugLoggingToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!MainWindow._isLoaded || AppSettingsService.CurrentSettings == null)
                return;

            bool isOn = EnableDebugLoggingToggle.IsOn;

            LoggingService.SetLoggingState(isOn);
            AppSettingsService.CurrentSettings.EnableDebugLogging = isOn;
            AppSettingsService.SaveAppSettings(AppSettingsService.CurrentSettings);

            // Changed to use local _log instance to cleanly populate [MainWindowFlyoutsView] context metadata
            _log.Information("User flipped the Enable Debug Logging Toggle to: {Status}", isOn);
        }

        // Base Theme Selection Changed
        /*private void BaseThemeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded || BaseThemeDropdown.SelectedItem is not string selectedBase)
                return;

            _log.Debug("User modified base theme to: {Theme}", selectedBase);
            ThemeService.SetBaseTheme(selectedBase);
        }*/

        // Accent Color Selection Changed
        private void AccentColorDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MainWindow._isLoaded || AccentColorDropdown.SelectedItem is not Theme selectedAccent)
                return;

            _log.Debug("User modified accent color to: {Accent}", selectedAccent.ColorScheme);
            ThemeService.SetAccentColor(selectedAccent.ColorScheme);
        }

        // ============ UI Helpers ============
        private void SyncUISelections()
        {
            var settings = AppSettingsService.CurrentSettings;
            if (settings == null) return;

            _log.Debug("Synchronizing UI elements.");

            // Sync Check For Updates Toggle
            CheckAppUpdatesToggle.IsOn = settings.CheckForUpdatesOnStartup;
            StartWithWindowsToggle.IsOn = settings.StartWithWindows;
            EnableDebugLoggingToggle.IsOn = settings.EnableDebugLogging;

            // Sync Base Theme Dropdown (Light/Dark)
            //BaseThemeDropdown.SelectedItem = settings.BaseTheme;

            // Sync Accent Color Dropdown (Blue/Lime/Indigo/etc.)
            AccentColorDropdown.SelectedItem = ThemeService.AvailableAccents
                .FirstOrDefault(t => string.Equals(t.ColorScheme, settings.AccentColor, StringComparison.OrdinalIgnoreCase));
        }
    }
}