using MahApps.Metro.Controls;
using Metro_WPF_Template_App.Resources.Config;
using Metro_WPF_Template_App.Resources.Functions.Services;
using Metro_WPF_Template_App.ViewModels;
using System.Windows;

namespace Metro_WPF_Template_App
{
    public partial class MainWindow : MetroWindow
    {
        public static bool _isLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            this.Loaded += (s, e) =>
            {
                this.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    AppSettingsService.InitializeAppSettings(this, ref _isLoaded);
                }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            };
        }

        // ============ Button Clicks ============

        // Open Github Repo
        private void OpenGithubRepo_Click(object sender, RoutedEventArgs e)
        {
            UrlService.OpenUrlAsync(AppUrls.GithubRepoUrl);
        }

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
            if (!_isLoaded || AppSettingsService.CurrentSettings == null) return;

            AppSettingsService.CurrentSettings.CheckForUpdatesOnStartup = CheckAppUpdatesToggle.IsOn;

            AppSettingsService.SaveAppSettings(AppSettingsService.CurrentSettings);
        }

        // End of Class
    }
}