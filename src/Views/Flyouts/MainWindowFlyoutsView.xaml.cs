using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Metro_WPF_Template_App.Common.Constants;
using Metro_WPF_Template_App.Services;
using Metro_WPF_Template_App.ViewModels;
using Microsoft.Win32;
using Serilog;
using System.Windows;
using System.Windows.Controls;
using Application = System.Windows.Application;

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

        // App Settings Button
        private void AppSettingsFlyout_Click(object? sender, RoutedEventArgs e)
        {
            _log.Debug("User clicked App Settings button.");
            ((MainWindowViewModel)DataContext).OpenAppSettingsFlyout();
        }

        // Open Logs Folder Button
        private async void OpenLogsFolder_Click(object sender, RoutedEventArgs e)
        {
            await FolderService.OpenFolderAsync(AppConfig.appLogsFolder);
        }

        // Open App Config Folder Button
        private async void OpenAppConfigFolder_Click(object sender, RoutedEventArgs e)
        {
            await FolderService.OpenFolderAsync(AppConfig.appConfigFolder);
        }

        // Factory Reset Settings Button
        private async void ResetToFactorySettings_Click(object sender, RoutedEventArgs e)
        {
            _log.Debug("User clicked the Factory Reset Settings button.");

            bool confirmReset = await MessageService.ShowYesNo("Factory Reset Settings", "Are you absolutely sure you want to reset all application settings back to factory defaults? This action cannot be undone.");

            if (!confirmReset)
            {
                return;
            }

            bool success = AppSettingsService.ResetToFactoryDefaults();

            if (success)
            {
                SyncUISelections();

                await MessageService.ShowInfo("Reset Complete", "All application settings have been restored to factory defaults successfully.");
            }
            else
            {
                await MessageService.ShowError("An unexpected error occurred while attempting to wipe your configuration profile.");
            }
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

        // Always On Top Toggled
        private void AlwaysOnTopToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!MainWindow._isLoaded || AppSettingsService.CurrentSettings == null) return;

            bool isOn = AlwaysOnTopToggle.IsOn;
            _log.Information("User flipped the Always on Top Toggle to: {Status}", isOn);

            AppSettingsService.CurrentSettings.AlwaysOnTop = isOn;
            AppSettingsService.SaveAppSettings(AppSettingsService.CurrentSettings);

            if (Application.Current.MainWindow is Window mainWindow)
            {
                mainWindow.Topmost = isOn;
                _log.Debug("MainWindow Topmost property set to: {State}", isOn);
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

            // Sync UI Toggles
            StartWithWindowsToggle.IsOn = settings.StartWithWindows;
            AlwaysOnTopToggle.IsOn = settings.AlwaysOnTop;

            CheckAppUpdatesToggle.IsOn = settings.CheckForUpdatesOnStartup;
            EnableDebugLoggingToggle.IsOn = settings.EnableDebugLogging;

            // Sync Base Theme Dropdown (Light/Dark)
            //BaseThemeDropdown.SelectedItem = settings.BaseTheme;

            // Sync Accent Color Dropdown (Blue/Lime/Indigo/etc.)
            AccentColorDropdown.SelectedItem = ThemeService.AvailableAccents.FirstOrDefault(t => string.Equals(t.ColorScheme, settings.AccentColor, StringComparison.OrdinalIgnoreCase));
        }

        // ============ General Helpers ============

        // End of Class
    }
}