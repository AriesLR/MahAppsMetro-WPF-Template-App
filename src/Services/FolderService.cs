using Serilog;
using System.Diagnostics;
using System.IO;

namespace Metro_WPF_Template_App.Services
{
    public static class FolderService
    {
        // Source Context For Logger
        private static ILogger _log => Log.ForContext("SourceContext", nameof(FolderService));

        public static async Task OpenFolderAsync(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                _log.Warning("Target folder path was null or empty.");
                return;
            }

            _log.Information("Attempting to open folder at: {TargetFolder}", folderPath);

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    _log.Information("Target directory does not exist on disk: {TargetFolder}", folderPath);
                    await MessageService.ShowWarning($"The target directory does not exist.\n\nPath: {folderPath}");
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = folderPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to open folder in File Explorer. Path: {TargetFolder}", folderPath);
                await MessageService.ShowError($"Failed to open folder: {ex.Message}");
            }
        }
    }
}
