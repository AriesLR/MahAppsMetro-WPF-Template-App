using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Metro_WPF_Template_App.Common.Constants;
using Metro_WPF_Template_App.Services;
using Metro_WPF_Template_App.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Metro_WPF_Template_App.Views.Flyouts
{
    /// <summary>
    /// Interaction logic for MainWindowFlyoutsView.xaml
    /// </summary>

    public partial class MainWindowFlyoutsView : FlyoutsControl
    {
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
            UrlService.OpenUrlAsync(AppUrls.BuyMeACoffeeUrl);
        }

        // Open Patreon Button
        private void OpenPatreon_Click(object sender, RoutedEventArgs e)
        {
            UrlService.OpenUrlAsync(AppUrls.PatreonUrl);
        }

        // Check For App Updates Button
        private async void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            await UpdateService.CheckForUpdatesAsync(AppUrls.UpdateUrl);
        }

        // App Settings Button Click
        private void AppSettingsFlyout_Click(object? sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).OpenAppSettingsFlyout();
        }

        // ============ App Settings Event Handlers ============

        // Check App Updates Toggled
        private void CheckAppUpdatesToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!MainWindow._isLoaded || AppSettingsService.CurrentSettings == null) return;

            AppSettingsService.CurrentSettings.CheckForUpdatesOnStartup = CheckAppUpdatesToggle.IsOn;

            AppSettingsService.SaveAppSettings(AppSettingsService.CurrentSettings);
        }

        // Base Theme Selection Changed
        /*private void BaseThemeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded || BaseThemeDropdown.SelectedItem is not string selectedBase)
                return;

            ThemeService.SetBaseTheme(selectedBase);
        }*/

        // Accent Color Selection Changed
        private void AccentColorDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MainWindow._isLoaded || AccentColorDropdown.SelectedItem is not Theme selectedAccent)
                return;

            ThemeService.SetAccentColor(selectedAccent.ColorScheme);
        }

        // ============ UI Helpers ============
        private void SyncUISelections()
        {
            var settings = AppSettingsService.CurrentSettings;
            if (settings == null) return;

            // Sync Check For Updates Toggle
            CheckAppUpdatesToggle.IsOn = settings.CheckForUpdatesOnStartup;

            // Sync Base Theme Dropdown (Light/Dark)
            //BaseThemeDropdown.SelectedItem = settings.BaseTheme;

            // Sync Accent Color Dropdown (Blue/Lime/Indigo/etc.)
            AccentColorDropdown.SelectedItem = ThemeService.AvailableAccents
                .FirstOrDefault(t => string.Equals(t.ColorScheme, settings.AccentColor, StringComparison.OrdinalIgnoreCase));
        }
    }
}