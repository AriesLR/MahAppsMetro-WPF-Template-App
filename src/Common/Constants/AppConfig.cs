using System.Diagnostics;
using System.IO;
using System.Reflection;

/// <summary>
/// AppConfig.cs holds various strings used within the application, including where the app's config folder gets generated.
/// </summary>

namespace Metro_WPF_Template_App.Common.Constants
{
    public class AppConfig
    {
        // App Name
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name ?? "Generic-AriesLR-App"; // Fallback to "Generic-AriesLR-App"

        // App Author
        public static readonly string AppAuthor = "AriesLR";

        // App Version
        public static string TitleAppVersion => $"v{Assembly.GetExecutingAssembly().GetName().Version}";

        public static readonly string AppVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion ?? "0.0.0"; // Fallback to v0.0.0

        // App Config Paths
        public static readonly string appConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppAuthor, AppName);

        public static readonly string appSettingsPath = Path.Combine(appConfigFolder, "AppSettings.json");

        public static readonly string appLogsFolder = Path.Combine(appConfigFolder, "Logs");
    }
}