namespace Metro_WPF_Template_App.Models
{
    /// <summary>
    /// AppSettings.cs holds the data model for the application's settings file.
    /// </summary>

    public class AppSettings
    {
        // ================ Appearance ================

        // Base Theme (Light/Dark)
        public string BaseTheme { get; set; } = "Dark";

        // Accent Color (Amber/Blue/Brown/Cobalt/etc)
        public string AccentColor { get; set; } = "Lime";

        // Combined Theme Name
        public string CombinedThemeName => $"{BaseTheme}.{AccentColor}";

        // ================ Application Behavior ================
        public bool StartWithWindows { get; set; } = false;

        public bool AlwaysOnTop { get; set; } = false;

        // ================ Updates ================
        public bool CheckForUpdatesOnStartup { get; set; } = true;

        // ================ Logging ================
        public bool EnableDebugLogging { get; set; } = false;
    }
}