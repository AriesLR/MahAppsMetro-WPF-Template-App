namespace Metro_WPF_Template_App.Resources.DataModels
{
    /// <summary>
    /// AppSettings.cs holds the data model for the application's settings file.
    /// </summary>

    public class AppSettings
    {
        // ================ General Settings ================

        public bool CheckForUpdatesOnStartup { get; set; } = true;

        // ================ Appearance Settings ================

        // Base Theme (Light/Dark)
        public string BaseTheme { get; set; } = "Dark";

        // Accent Color (Amber/Blue/Brown/Cobalt/etc)
        public string AccentColor { get; set; } = "Blue";

        // Combined Theme Name
        public string CombinedThemeName => $"{BaseTheme}.{AccentColor}";
    }
}