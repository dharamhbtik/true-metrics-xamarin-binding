using System;
using System.Threading.Tasks;

namespace TrueMetricsDemo.Services
{
    public interface ITrueMetricsService
    {
        bool IsInitialized { get; }
        
        Task<bool> InitializeAsync(string apiKey, string userId);
        Task<bool> InitializeAsync(string apiKey, string userId, string endpoint);
        
        void TrackEvent(string eventName);
        void TrackEvent(string eventName, string parameters);
        
        void TrackScreenView(string screenName);
        
        void SetUserProperty(string key, string value);
        void SetUserProperties(System.Collections.Generic.Dictionary<string, string> properties);
        
        void LogException(Exception exception);
        void LogException(Exception exception, string context);
        
        void StartSession();
        void EndSession();
        
        void TrackExercise(string exerciseType, int duration, double distance);
        void TrackExercise(string exerciseType, int duration, double distance, string metadata);
        
        void SetLocation(double latitude, double longitude);
        void SetLocation(double latitude, double longitude, double accuracy);
        
        Task<string> GetDeviceInfoAsync();
        Task<string> GetSessionIdAsync();
        
        void EnableDebugLogging(bool enable);
        void SetLogLevel(int level);
        
        void FlushEvents();
        Task<bool> FlushEventsAsync();
        
        void Reset();
    }
}
