using MahApps.Metro.Controls;
using Metro_WPF_Template_App.Common.Constants;
using Metro_WPF_Template_App.Services;
using Metro_WPF_Template_App.ViewModels;
using Serilog;
using System.Windows;

namespace Metro_WPF_Template_App
{
    public partial class MainWindow : MetroWindow
    {
        // Source Context For Logger
        private static ILogger _log => Log.ForContext("SourceContext", nameof(MainWindow));

        public static bool _isLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            this.Loaded += (s, e) =>
            {
                this.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    AppSettingsService.InitializeAppSettings(ref _isLoaded);
                }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            };
        }

        // ============ Button Clicks ============

        // Open Github Repo
        private void OpenGithubRepo_Click(object sender, RoutedEventArgs e)
        {
            _log.Debug("User clicked Github Repo button.");
            UrlService.OpenUrlAsync(AppUrls.GithubRepoUrl);
        }

        // End of Class
    }
}