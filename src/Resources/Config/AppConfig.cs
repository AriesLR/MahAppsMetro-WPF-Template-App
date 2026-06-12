using System.IO;
using System.Reflection;

namespace Metro_WPF_Template_App.Resources.Config
{
    public class AppConfig
    {
        // App Version
        public static string AppVersion => $"v{Assembly.GetExecutingAssembly().GetName().Version}";

        // App Config Paths
        public static readonly string appConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AriesLR", "Metro_WPF_Template_App");

        public static readonly string appSettingsPath = Path.Combine(appConfigFolder, "AppSettings.json");
    }
}