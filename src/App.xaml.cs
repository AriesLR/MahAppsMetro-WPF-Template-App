using Metro_WPF_Template_App.Resources.DataModels;
using Metro_WPF_Template_App.Resources.Functions.Services;
using System.Windows;
using Application = System.Windows.Application;

namespace Metro_WPF_Template_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = AppSettingsService.LoadAppSettings() ?? new AppSettings();
            AppSettingsService.CurrentSettings = settings;

            ThemeService.Initialize(settings);
        }
    }
}