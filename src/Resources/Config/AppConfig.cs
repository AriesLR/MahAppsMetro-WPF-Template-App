using System.Diagnostics;
using System.IO;
using System.Reflection;

/// <summary>
/// AppConfig.cs holds various strings used within the application, including where the app's config folder gets generated.
/// </summary>

namespace Metro_WPF_Template_App.Resources.Config
{
    public class AppConfig
    {
        // App Version
        public static string TitleAppVersion => $"v{Assembly.GetExecutingAssembly().GetName().Version}";

        public static readonly string AppVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion ?? "0.0.0";

        // App Config Paths
        public static readonly string appConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AriesLR", "Metro_WPF_Template_App");

        public static readonly string appSettingsPath = Path.Combine(appConfigFolder, "AppSettings.json");
    }
}