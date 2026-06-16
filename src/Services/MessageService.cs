using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Serilog;
using Application = System.Windows.Application;

/// <summary>
/// MessageService.cs holds the methods used to trigger/construct dialogs within the application.
/// </summary>

namespace Metro_WPF_Template_App.Services
{
    public static class MessageService
    {
        // Source Context For Logger
        private static ILogger _log => Log.ForContext("SourceContext", nameof(MessageService));

        private static MetroWindow GetMainWindow()
        {
            return (Application.Current.MainWindow as MetroWindow)!;
        }

        // ============ Basic Dialogs ============

        // Info
        public static async Task ShowInfo(string title, string message)
        {
            _log.Debug("Displaying Info Dialog. Title: '{Title}', Message: '{Message}'", title, message);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            await mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative);
        }

        // Warning
        public static async Task ShowWarning(string message)
        {
            _log.Warning("Displaying User Warning Dialog. Message: '{Message}'", message);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            await mainWindow.ShowMessageAsync("Warning", message, MessageDialogStyle.Affirmative);
        }

        // Error
        public static async Task ShowError(string message)
        {
            _log.Error("Displaying User Error Dialog. Message: '{Message}'", message);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            await mainWindow.ShowMessageAsync("Error", message, MessageDialogStyle.Affirmative);
        }

        // Show Progress Bar
        public static async Task ShowProgress(string title, string message, Func<IProgress<double>, Task> operation)
        {
            if (Application.Current.MainWindow is not MetroWindow mainWindow)
                throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            _log.Debug("Launching Progress Dialog. Title: '{Title}'", title);
            var controller = await mainWindow.ShowProgressAsync(title, message);
            controller.SetIndeterminate();

            try
            {
                var progress = new Progress<double>(value => controller.SetProgress(value));
                await operation(progress);
                _log.Debug("Progress Dialog task operation completed successfully.");
            }
            catch (Exception ex)
            {
                _log.Error(ex, "An unhandled exception occurred within the Progress Dialog running operation.");
                throw;
            }
            finally
            {
                await controller.CloseAsync();
            }
        }

        // ============ Confirm/Deny Dialogs ============

        // Yes/No
        public static async Task<bool> ShowYesNo(string title, string message)
        {
            _log.Debug("Displaying Yes/No Prompt. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No"
            };

            var result = await mainWindow.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.AffirmativeAndNegative,
                settings
            );

            bool userConfirmed = result == MessageDialogResult.Affirmative;
            _log.Debug("Yes/No Prompt returned. User selected: {UserChoice}", userConfirmed ? "Yes" : "No");
            return userConfirmed;
        }

        // Yes/Cancel
        public static async Task<bool> ShowYesCancel(string title, string message)
        {
            _log.Debug("Displaying Yes/Cancel Prompt. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "Cancel"
            };

            var result = await mainWindow.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.AffirmativeAndNegative,
                settings
            );

            bool userConfirmed = result == MessageDialogResult.Affirmative;
            _log.Debug("Yes/Cancel Prompt returned. User selected: {UserChoice}", userConfirmed ? "Yes" : "Cancel");
            return userConfirmed;
        }

        // ============ Confirmation Dialogs ============

        // Ok
        public static async Task<bool> ShowOk(string title, string message)
        {
            _log.Debug("Displaying OK Confirmation Dialog. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "OK"
            };

            var result = await mainWindow.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.Affirmative,
                settings
            );

            return result == MessageDialogResult.Affirmative;
        }

        // ============ Special Dialogs ============

        // TextBox Input
        public static async Task<string> ShowInput(string title, string message)
        {
            _log.Debug("Displaying Text Input Prompt. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Ok",
                NegativeButtonText = "Cancel",
                DefaultText = "",
                AnimateShow = true,
                AnimateHide = true
            };

            var result = await mainWindow.ShowInputAsync(title, message, settings);

            if (result == null)
            {
                _log.Debug("Text Input Prompt canceled by user.");
            }
            else
            {
                _log.Debug("Text Input Prompt submitted. Received input length: {InputLength} characters.", result.Length);
            }

            return result ?? string.Empty;
        }

        // Folder Browser Prompt
        public static async Task<bool> ShowBrowseCancel(string title, string message)
        {
            _log.Debug("Displaying Browse/Cancel Prompt. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Browse",
                NegativeButtonText = "Cancel"
            };

            var result = await mainWindow.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.AffirmativeAndNegative,
                settings
            );

            bool userClickedBrowse = result == MessageDialogResult.Affirmative;
            _log.Debug("Browse/Cancel Prompt returned. User selected: {UserChoice}", userClickedBrowse ? "Browse" : "Cancel");
            return userClickedBrowse;
        }
    }
}