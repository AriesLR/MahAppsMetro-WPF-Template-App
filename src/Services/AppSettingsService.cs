using Metro_WPF_Template_App.Common.Constants;
using Metro_WPF_Template_App.Models;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.IO;

/// <summary>
/// AppSettingsService.cs holds the core methods for initializing, loading, and saving the application's settings to file.
/// </summary>

namespace Metro_WPF_Template_App.Services
{
    public static class AppSettingsService
    {
        public static AppSettings CurrentSettings { get; set; } = new AppSettings();

        // Source Context For Logger
        private static ILogger _log => Log.ForContext("SourceContext", nameof(AppSettingsService));

        // Init App Settings
        public static void InitializeAppSettings(ref bool isLoaded)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            isLoaded = false;

            CurrentSettings = LoadAppSettings() ?? new AppSettings();

            isLoaded = true;

            if (CurrentSettings.CheckForUpdatesOnStartup)
            {
                try
                {
                    _log.Debug("Startup update check is enabled. Querying update service...");
                    UpdateService.UpdateCheckOnStartup();
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "An unexpected error occurred during the startup background update check.");
                }
            }
            else
            {
                _log.Debug("Startup update check is disabled by user configuration profile.");
            }

            stopwatch.Stop();

            _log.Information("AppSettingsService initialized in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);
        }

        // Load AppSettings.json
        public static AppSettings? LoadAppSettings()
        {
            if (!File.Exists(AppConfig.appSettingsPath))
            {
                _log.Warning("Configuration file not found at path: {SettingsPath}. Application will initialize with factory defaults.", AppConfig.appSettingsPath);
                return null;
            }

            try
            {
                _log.Debug("Attempting to read and deserialize configuration file from disk.");

                string json = File.ReadAllText(AppConfig.appSettingsPath);
                var settings = JsonConvert.DeserializeObject<AppSettings>(json);

                _log.Debug("Successfully loaded application configuration profile.");
                return settings;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "An error occurred while trying to parse the configuration file at: {SettingsPath}", AppConfig.appSettingsPath);
                return null;
            }
        }

        // Save AppSettings.json
        public static void SaveAppSettings(AppSettings settings)
        {
            try
            {
                string? directory = Path.GetDirectoryName(AppConfig.appSettingsPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    _log.Information("Configuration directory does not exist. Creating directory branch: {DirectoryPath}", directory);
                    Directory.CreateDirectory(directory);
                }

                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(AppConfig.appSettingsPath, json);

                _log.Debug("Application configuration state successfully committed to disk at: {SettingsPath}", AppConfig.appSettingsPath);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to serialize or write application configuration settings to disk.");
            }
        }
    }
}