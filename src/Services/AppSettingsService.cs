using Metro_WPF_Template_App.Common.Constants;
using Metro_WPF_Template_App.Models;
using Newtonsoft.Json;
using System.IO;

/// <summary>
/// AppSettingsService.cs holds the core methods for initializing, loading, and saving the application's settings to file.
/// </summary>

namespace Metro_WPF_Template_App.Services
{
    public static class AppSettingsService
    {
        public static AppSettings CurrentSettings { get; set; } = new AppSettings();

        // Init App Settings
        public static void InitializeAppSettings(ref bool isLoaded)
        {
            isLoaded = false;

            CurrentSettings = LoadAppSettings() ?? new AppSettings();

            isLoaded = true;

            if (CurrentSettings.CheckForUpdatesOnStartup)
            {
                UpdateService.UpdateCheckOnStartup();
            }
        }

        // Load AppSettings.json
        public static AppSettings? LoadAppSettings()
        {
            if (!File.Exists(AppConfig.appSettingsPath))
                return null;

            try
            {
                string json = File.ReadAllText(AppConfig.appSettingsPath);
                return JsonConvert.DeserializeObject<AppSettings>(json);
            }
            catch
            {
                return null;
            }
        }

        // Save AppSettings.json
        public static void SaveAppSettings(AppSettings settings)
        {
            string? directory = Path.GetDirectoryName(AppConfig.appSettingsPath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(AppConfig.appSettingsPath, json);
        }
    }
}