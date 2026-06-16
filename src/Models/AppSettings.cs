namespace Metro_WPF_Template_App.Models
{
    /// <summary>
    /// AppSettings.cs holds the data model for the application's settings file.
    /// </summary>

    public class AppSettings
    {
        // ================ General Settings ================
        public bool CheckForUpdatesOnStartup { get; set; } = true;

        // ================ Application Behavior Settings ================
        public bool StartWithWindows { get; set; } = false;

        // ================ Debug Logging Settings ================
        public bool EnableDebugLogging { get; set; } = false;

        // ================ Appearance Settings ================
        // Base Theme (Light/Dark)
        public string BaseTheme { get; set; } = "Dark";

        // Accent Color (Amber/Blue/Brown/Cobalt/etc)
        public string AccentColor { get; set; } = "Blue";

        // Combined Theme Name
        public string CombinedThemeName => $"{BaseTheme}.{AccentColor}";
    }
}