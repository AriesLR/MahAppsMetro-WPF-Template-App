using Metro_WPF_Template_App.Common.Constants;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http;

/// <summary>
/// UpdateService.cs holds the methods used for checking if the application has an update. This will use the URL set for "UpdateUrl" in the AppUrls.cs file.
/// </summary>

namespace Metro_WPF_Template_App.Services
{
    public class UpdateService
    {
        // Source Context For Logger
        private static ILogger _log => Log.ForContext("SourceContext", nameof(UpdateService));

        // Check for updates on startup
        public static async void UpdateCheckOnStartup()
        {
            _log.Debug("Triggering silent update check on startup sequence.");
            await UpdateService.CheckForUpdatesAsyncSilent(AppUrls.UpdateUrl);
        }

        // ============ Main Methods ============
        public static async Task CheckForUpdatesAsync(string jsonUrl)
        {
            _log.Information("Initiating manual update check against endpoint: {UpdateUrl}", jsonUrl);

            try
            {
                using var client = new HttpClient();
                string response = await client.GetStringAsync(jsonUrl);

                _log.Debug("Successfully fetched update payload response from server.");
                var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(response);

                if (updateInfo?.LatestVersion == null || updateInfo.DownloadUrl == null)
                {
                    _log.Warning("Update check failed: Received schema was invalid or incomplete. Raw JSON: {RawJson}", response);
                    await MessageService.ShowError("Failed to retrieve valid update information.");
                    return;
                }

                string latestVersion = updateInfo.LatestVersion;
                string currentVersion = AppConfig.AppVersion;

                int versionComparison = CompareVersions(currentVersion, latestVersion);
                _log.Information("Version comparison complete. Local: {LocalVersion} | Remote: {RemoteVersion}", currentVersion, latestVersion);

                if (versionComparison < 0)
                {
                    _log.Information("A newer version is available. Prompting user for download authorization.");
                    bool userConfirmed = await MessageService.ShowYesNo("Check For Updates", $"A new version is available: {latestVersion}\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}\n\nWould you like to download the new version?");

                    if (userConfirmed)
                    {
                        _log.Information("User accepted update redirection. Requesting external browser routing to: {DownloadUrl}", updateInfo.DownloadUrl);
                        UrlService.OpenUrlAsync(updateInfo.DownloadUrl);
                    }
                    else
                    {
                        _log.Debug("User dismissed update prompt download payload execution.");
                    }
                }
                else if (versionComparison > 0)
                {
                    _log.Warning("Easter egg triggered! Local environment version ({LocalVersion}) exceeds server version ({RemoteVersion}).", currentVersion, latestVersion);
                    await MessageService.ShowInfo("Check For Updates", $"You're a wizard, harry!\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}\n\nTell AriesLR he's a goofball and forgot to update the version number.");
                }
                else
                {
                    _log.Debug("Application is completely up to date.");
                    await MessageService.ShowInfo("Check For Updates", $"You are already using the latest version.\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "An unhandled exception occurred during manual update processing cycle targeting: {UpdateUrl}", jsonUrl);
                await MessageService.ShowError($"Failed to check for updates: {ex.Message}");
            }
        }

        public static async Task CheckForUpdatesAsyncSilent(string jsonUrl)
        {
            try
            {
                using var client = new HttpClient();
                string response = await client.GetStringAsync(jsonUrl);
                var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(response);

                if (updateInfo?.LatestVersion == null || updateInfo.DownloadUrl == null)
                {
                    _log.Warning("Silent update check failed: Malformed metadata payload returned from endpoint.");
                    return;
                }

                string latestVersion = updateInfo.LatestVersion;
                string currentVersion = AppConfig.AppVersion;

                int versionComparison = CompareVersions(currentVersion, latestVersion);

                if (versionComparison < 0)
                {
                    _log.Information("Silent check discovered an available update ({RemoteVersion}). Triggering notification prompt.", latestVersion);
                    bool userConfirmed = await MessageService.ShowYesNo("Check For Updates", $"A new version is available: {latestVersion}\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}\n\nWould you like to download the new version?");

                    if (userConfirmed)
                    {
                        _log.Information("User authorized download stream redirection via silent prompt pipeline.");
                        UrlService.OpenUrlAsync(updateInfo.DownloadUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Background silent startup update validation pipeline encountered an unexpected error.");
            }
        }

        private static int CompareVersions(string currentVersion, string latestVersion)
        {
            try
            {
                var currentParts = currentVersion.Split('.');
                var latestParts = latestVersion.Split('.');

                int maxLength = Math.Max(currentParts.Length, latestParts.Length);

                for (int i = 0; i < maxLength; i++)
                {
                    int currentPart = i < currentParts.Length ? int.Parse(currentParts[i]) : 0;
                    int latestPart = i < latestParts.Length ? int.Parse(latestParts[i]) : 0;

                    if (currentPart < latestPart) return -1;
                    if (currentPart > latestPart) return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to compare versions. Strings: Local='{Local}' Remote='{Remote}'", currentVersion, latestVersion);
                return 0;
            }
        }

        public class UpdateInfo
        {
            public string? LatestVersion { get; set; }
            public string? DownloadUrl { get; set; }
        }
    }
}