using Serilog;
using System.Diagnostics;

/// <summary>
/// UrlService.cs is used to open URLs in the user's default browser.
/// </summary>

namespace Metro_WPF_Template_App.Services
{
    public static class UrlService
    {
        // Source Context For Logger
        private static ILogger _log => Log.ForContext("SourceContext", nameof(UrlService));

        public static async void OpenUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _log.Warning("The provided target destination URL was null or empty.");
                return;
            }

            _log.Information("Attempting to open target destination in browser. URL: {TargetUrl}", url);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to open URL: {TargetUrl}", url);
                await MessageService.ShowError($"Failed to open URL: {ex.Message}");
            }
        }
    }
}