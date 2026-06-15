using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Application = System.Windows.Application;

/// <summary>
/// MessageService.cs holds the methods used to trigger/construct dialogs within the application.
/// </summary>

namespace Metro_WPF_Template_App.Services
{
    public static class MessageService
    {
        private static MetroWindow GetMainWindow()
        {
            return (Application.Current.MainWindow as MetroWindow)!;
        }

        // ============ Basic Dialogs ============

        // Info
        public static async Task ShowInfo(string title, string message)
        {
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            await mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative);
        }

        // Warning
        public static async Task ShowWarning(string message)
        {
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            await mainWindow.ShowMessageAsync("Warning", message, MessageDialogStyle.Affirmative);
        }

        // Error
        public static async Task ShowError(string message)
        {
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            await mainWindow.ShowMessageAsync("Error", message, MessageDialogStyle.Affirmative);
        }

        // Show Progress Bar
        public static async Task ShowProgress(string title, string message, Func<IProgress<double>, Task> operation)
        {
            if (Application.Current.MainWindow is not MetroWindow mainWindow)
                throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            var controller = await mainWindow.ShowProgressAsync(title, message);
            controller.SetIndeterminate();

            try
            {
                var progress = new Progress<double>(value => controller.SetProgress(value));
                await operation(progress);
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

            return result == MessageDialogResult.Affirmative;
        }

        // Yes/Cancel
        public static async Task<bool> ShowYesCancel(string title, string message)
        {
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

            return result == MessageDialogResult.Affirmative;
        }

        // ============ Confirmation Dialogs ============

        // Ok
        public static async Task<bool> ShowOk(string title, string message)
        {
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

            return result ?? string.Empty;
        }

        // Folder Browser Prompt
        public static async Task<bool> ShowBrowseCancel(string title, string message)
        {
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

            return result == MessageDialogResult.Affirmative;
        }
    }
}