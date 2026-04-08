using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TrueMetricsDemo.Services
{
    public interface ILogService
    {
        ObservableCollection<LogEntry> LogEntries { get; }
        event EventHandler<LogEntry> LogAdded;
        
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogError(string message, Exception exception);
        void LogSuccess(string message);
        void LogDebug(string message);
        void LogSdkCall(string methodName, string parameters = null);
        void LogSdkResponse(string methodName, string response);
        void LogSdkError(string methodName, Exception exception);
        
        Task ClearLogsAsync();
        string ExportLogs();
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        
        public string FormattedMessage => $"[{Timestamp:HH:mm:ss.fff}] [{Level}] {Message}";
        
        public ColorCode ColorCode
        {
            get
            {
                switch (Level)
                {
                    case LogLevel.Info: return ColorCode.Blue;
                    case LogLevel.Warning: return ColorCode.Orange;
                    case LogLevel.Error: return ColorCode.Red;
                    case LogLevel.Success: return ColorCode.Green;
                    case LogLevel.Debug: return ColorCode.Gray;
                    case LogLevel.SdkCall: return ColorCode.Purple;
                    case LogLevel.SdkResponse: return ColorCode.Teal;
                    default: return ColorCode.Default;
                }
            }
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Success,
        SdkCall,
        SdkResponse
    }

    public enum ColorCode
    {
        Default,
        Blue,
        Green,
        Orange,
        Red,
        Gray,
        Purple,
        Teal
    }
}
