using Metro_WPF_Template_App.Common.Constants;
using Serilog;
using Serilog.Core;
using System.IO;

namespace Metro_WPF_Template_App.Services
{
    public static class LoggingService
    {
        private static readonly LoggingLevelSwitch _levelSwitch = new LoggingLevelSwitch();

        // Source Context For Logger
        private static ILogger _log => Log.ForContext("SourceContext", nameof(LoggingService));

        // Initialize Logging Service
        public static void Initialize(bool isEnabledInitially)
        {
            _levelSwitch.MinimumLevel = isEnabledInitially
                ? Serilog.Events.LogEventLevel.Debug
                : Serilog.Events.LogEventLevel.Fatal;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(_levelSwitch)
                .WriteTo.File(
                    path: Path.Combine(AppConfig.appLogsFolder, "debug-.log"),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext:l}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 25 * 1024 * 1024, // 25MB
                    retainedFileCountLimit: 10,           // Keep last 10 days of logs
                    rollOnFileSizeLimit: true
                )
                .CreateLogger();
        }

        // Toggle Logging State
        public static void SetLoggingState(bool enable)
        {
            _levelSwitch.MinimumLevel = enable
                ? Serilog.Events.LogEventLevel.Debug
                : Serilog.Events.LogEventLevel.Fatal;

            if (enable)
            {
                _log.Debug("Debug logging has been manually enabled by the user.");
            }
        }

        // Shutdown Logger
        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }
    }
}