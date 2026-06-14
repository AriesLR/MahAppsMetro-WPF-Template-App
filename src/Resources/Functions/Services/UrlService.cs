/// <summary>
/// UrlService.cs is used to open URLs in the user's default browser.
/// </summary>

namespace Metro_WPF_Template_App.Resources.Functions.Services
{
    public static class UrlService
    {
        public static async void OpenUrlAsync(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                await MessageService.ShowError($"Failed to open URL: {ex.Message}");
            }
        }
    }
}