using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TrueMetricsDemo.Services
{
    public class LogService : ILogService
    {
        public ObservableCollection<LogEntry> LogEntries { get; private set; }
        public event EventHandler<LogEntry> LogAdded;
        
        private const int MaxLogEntries = 1000;

        public LogService()
        {
            LogEntries = new ObservableCollection<LogEntry>();
        }

        public void LogInfo(string message)
        {
            AddLogEntry(LogLevel.Info, message, "INFO");
        }

        public void LogWarning(string message)
        {
            AddLogEntry(LogLevel.Warning, message, "WARN");
        }

        public void LogError(string message)
        {
            AddLogEntry(LogLevel.Error, message, "ERROR");
        }

        public void LogError(string message, Exception exception)
        {
            var fullMessage = $"{message}\nException: {exception.GetType().Name}: {exception.Message}\nStackTrace: {exception.StackTrace}";
            AddLogEntry(LogLevel.Error, fullMessage, "ERROR");
        }

        public void LogSuccess(string message)
        {
            AddLogEntry(LogLevel.Success, message, "SUCCESS");
        }

        public void LogDebug(string message)
        {
            AddLogEntry(LogLevel.Debug, message, "DEBUG");
        }

        public void LogSdkCall(string methodName, string parameters = null)
        {
            var message = string.IsNullOrEmpty(parameters) 
                ? $"SDK Call: {methodName}()" 
                : $"SDK Call: {methodName}({parameters})";
            AddLogEntry(LogLevel.SdkCall, message, "SDK_CALL");
        }

        public void LogSdkResponse(string methodName, string response)
        {
            var message = $"SDK Response from {methodName}: {response}";
            AddLogEntry(LogLevel.SdkResponse, message, "SDK_RESPONSE");
        }

        public void LogSdkError(string methodName, Exception exception)
        {
            var message = $"SDK Error in {methodName}: {exception.GetType().Name}: {exception.Message}";
            AddLogEntry(LogLevel.Error, message, "SDK_ERROR");
        }

        private void AddLogEntry(LogLevel level, string message, string category)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var entry = new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = level,
                    Message = message,
                    Category = category
                };

                LogEntries.Add(entry);

                // Keep only the last MaxLogEntries
                while (LogEntries.Count > MaxLogEntries)
                {
                    LogEntries.RemoveAt(0);
                }

                // Also write to device log
                System.Diagnostics.Debug.WriteLine(entry.FormattedMessage);

                LogAdded?.Invoke(this, entry);
            });
        }

        public Task ClearLogsAsync()
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                LogEntries.Clear();
                LogInfo("Logs cleared");
            });
        }

        public string ExportLogs()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== TrueMetrics Demo Logs ===");
            sb.AppendLine($"Export Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Device: {DeviceInfo.Name} ({DeviceInfo.Model})");
            sb.AppendLine($"OS: {DeviceInfo.Version}");
            sb.AppendLine("==============================");
            sb.AppendLine();

            foreach (var entry in LogEntries)
            {
                sb.AppendLine(entry.FormattedMessage);
            }

            return sb.ToString();
        }
    }
}
