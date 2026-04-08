using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Truemetrics.Sdk;
using Com.Truemetrics.Sdk.Config;
using Com.Truemetrics.Sdk.Events;
using Com.Truemetrics.Sdk.Exceptions;
using Com.Truemetrics.Sdk.Location;
using Com.Truemetrics.Sdk.Exercise;
using Xamarin.Essentials;
using Java.Lang;
using Java.Util;

namespace TrueMetricsDemo.Services
{
    public class TrueMetricsService : ITrueMetricsService
    {
        private readonly ILogService _logService;
        private TrueMetricsClient _client;
        
        public bool IsInitialized => _client != null;

        public TrueMetricsService(ILogService logService)
        {
            _logService = logService;
        }

        public async Task<bool> InitializeAsync(string apiKey, string userId)
        {
            _logService.LogSdkCall("Initialize", $"apiKey: {apiKey?.Substring(0, Math.Min(10, apiKey?.Length ?? 0))}..., userId: {userId}");
            
            try
            {
                var config = new TruemetricsConfig.Builder(apiKey)
                    .SetUserId(userId)
                    .SetAutoTrackSessions(true)
                    .Build();
                
                _client = TrueMetricsClient.Initialize(Android.App.Application.Context, config);
                
                _logService.LogSdkResponse("Initialize", "Success - Client initialized");
                return true;
            }
            catch (TruemetricsInitializationException ex)
            {
                _logService.LogSdkError("Initialize", ex);
                return false;
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("Initialize", ex);
                return false;
            }
        }

        public async Task<bool> InitializeAsync(string apiKey, string userId, string endpoint)
        {
            _logService.LogSdkCall("InitializeWithEndpoint", $"apiKey: {apiKey?.Substring(0, Math.Min(10, apiKey?.Length ?? 0))}..., userId: {userId}, endpoint: {endpoint}");
            
            try
            {
                var config = new TruemetricsConfig.Builder(apiKey)
                    .SetUserId(userId)
                    .SetEndpoint(endpoint)
                    .SetAutoTrackSessions(true)
                    .Build();
                
                _client = TrueMetricsClient.Initialize(Android.App.Application.Context, config);
                
                _logService.LogSdkResponse("InitializeWithEndpoint", "Success - Client initialized with custom endpoint");
                return true;
            }
            catch (TruemetricsInitializationException ex)
            {
                _logService.LogSdkError("InitializeWithEndpoint", ex);
                return false;
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("InitializeWithEndpoint", ex);
                return false;
            }
        }

        public void TrackEvent(string eventName)
        {
            _logService.LogSdkCall("TrackEvent", $"eventName: {eventName}");
            
            try
            {
                _client?.TrackEvent(eventName);
                _logService.LogSdkResponse("TrackEvent", $"Event '{eventName}' tracked successfully");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("TrackEvent", ex);
            }
        }

        public void TrackEvent(string eventName, string parameters)
        {
            _logService.LogSdkCall("TrackEventWithParams", $"eventName: {eventName}, parameters: {parameters}");
            
            try
            {
                var props = new Java.Util.HashMap();
                if (!string.IsNullOrEmpty(parameters))
                {
                    props.Put("parameters", parameters);
                }
                
                _client?.TrackEvent(eventName, props);
                _logService.LogSdkResponse("TrackEventWithParams", $"Event '{eventName}' tracked with parameters");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("TrackEventWithParams", ex);
            }
        }

        public void TrackScreenView(string screenName)
        {
            _logService.LogSdkCall("TrackScreenView", $"screenName: {screenName}");
            
            try
            {
                _client?.TrackScreenView(screenName);
                _logService.LogSdkResponse("TrackScreenView", $"Screen view '{screenName}' tracked");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("TrackScreenView", ex);
            }
        }

        public void SetUserProperty(string key, string value)
        {
            _logService.LogSdkCall("SetUserProperty", $"key: {key}, value: {value}");
            
            try
            {
                _client?.SetUserProperty(key, value);
                _logService.LogSdkResponse("SetUserProperty", $"Property '{key}' set to '{value}'");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("SetUserProperty", ex);
            }
        }

        public void SetUserProperties(Dictionary<string, string> properties)
        {
            var propsString = string.Join(", ", properties?.Select(p => $"{p.Key}={p.Value}") ?? Array.Empty<string>());
            _logService.LogSdkCall("SetUserProperties", propsString);
            
            try
            {
                var javaMap = new Java.Util.HashMap();
                if (properties != null)
                {
                    foreach (var prop in properties)
                    {
                        javaMap.Put(prop.Key, prop.Value);
                    }
                }
                
                _client?.SetUserProperties(javaMap);
                _logService.LogSdkResponse("SetUserProperties", $"{properties?.Count ?? 0} properties set");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("SetUserProperties", ex);
            }
        }

        public void LogException(Exception exception)
        {
            _logService.LogSdkCall("LogException", $"exception: {exception?.GetType().Name}: {exception?.Message}");
            
            try
            {
                if (exception != null)
                {
                    var javaException = new Java.Lang.Exception(exception.Message);
                    _client?.LogException(javaException);
                }
                _logService.LogSdkResponse("LogException", "Exception logged");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("LogException", ex);
            }
        }

        public void LogException(Exception exception, string context)
        {
            _logService.LogSdkCall("LogExceptionWithContext", $"exception: {exception?.GetType().Name}, context: {context}");
            
            try
            {
                if (exception != null)
                {
                    var javaException = new Java.Lang.Exception($"{context}: {exception.Message}");
                    _client?.LogException(javaException);
                }
                _logService.LogSdkResponse("LogExceptionWithContext", "Exception with context logged");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("LogExceptionWithContext", ex);
            }
        }

        public void StartSession()
        {
            _logService.LogSdkCall("StartSession");
            
            try
            {
                _client?.StartSession();
                _logService.LogSdkResponse("StartSession", "Session started");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("StartSession", ex);
            }
        }

        public void EndSession()
        {
            _logService.LogSdkCall("EndSession");
            
            try
            {
                _client?.EndSession();
                _logService.LogSdkResponse("EndSession", "Session ended");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("EndSession", ex);
            }
        }

        public void TrackExercise(string exerciseType, int duration, double distance)
        {
            _logService.LogSdkCall("TrackExercise", $"type: {exerciseType}, duration: {duration}s, distance: {distance}m");
            
            try
            {
                var exercise = new Exercise.Builder(exerciseType)
                    .SetDuration(duration)
                    .SetDistance(distance)
                    .Build();
                
                _client?.TrackExercise(exercise);
                _logService.LogSdkResponse("TrackExercise", "Exercise tracked successfully");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("TrackExercise", ex);
            }
        }

        public void TrackExercise(string exerciseType, int duration, double distance, string metadata)
        {
            _logService.LogSdkCall("TrackExerciseWithMetadata", $"type: {exerciseType}, duration: {duration}s, distance: {distance}m, metadata: {metadata}");
            
            try
            {
                var exercise = new Exercise.Builder(exerciseType)
                    .SetDuration(duration)
                    .SetDistance(distance)
                    .SetMetadata(metadata)
                    .Build();
                
                _client?.TrackExercise(exercise);
                _logService.LogSdkResponse("TrackExerciseWithMetadata", "Exercise with metadata tracked");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("TrackExerciseWithMetadata", ex);
            }
        }

        public void SetLocation(double latitude, double longitude)
        {
            _logService.LogSdkCall("SetLocation", $"lat: {latitude:F6}, lng: {longitude:F6}");
            
            try
            {
                var location = new LocationCoordinates(latitude, longitude);
                _client?.SetLocation(location);
                _logService.LogSdkResponse("SetLocation", "Location set");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("SetLocation", ex);
            }
        }

        public void SetLocation(double latitude, double longitude, double accuracy)
        {
            _logService.LogSdkCall("SetLocationWithAccuracy", $"lat: {latitude:F6}, lng: {longitude:F6}, accuracy: {accuracy}m");
            
            try
            {
                var location = new LocationCoordinates(latitude, longitude, accuracy);
                _client?.SetLocation(location);
                _logService.LogSdkResponse("SetLocationWithAccuracy", "Location with accuracy set");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("SetLocationWithAccuracy", ex);
            }
        }

        public async Task<string> GetDeviceInfoAsync()
        {
            _logService.LogSdkCall("GetDeviceInfo");
            
            try
            {
                var info = _client?.GetDeviceInfo();
                var result = info?.ToString() ?? "No device info available";
                _logService.LogSdkResponse("GetDeviceInfo", result);
                return result;
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("GetDeviceInfo", ex);
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> GetSessionIdAsync()
        {
            _logService.LogSdkCall("GetSessionId");
            
            try
            {
                var sessionId = _client?.GetSessionId();
                _logService.LogSdkResponse("GetSessionId", sessionId ?? "No active session");
                return sessionId ?? "No active session";
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("GetSessionId", ex);
                return $"Error: {ex.Message}";
            }
        }

        public void EnableDebugLogging(bool enable)
        {
            _logService.LogSdkCall("EnableDebugLogging", $"enable: {enable}");
            
            try
            {
                _client?.EnableDebugLogging(enable);
                _logService.LogSdkResponse("EnableDebugLogging", $"Debug logging {(enable ? "enabled" : "disabled")}");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("EnableDebugLogging", ex);
            }
        }

        public void SetLogLevel(int level)
        {
            _logService.LogSdkCall("SetLogLevel", $"level: {level}");
            
            try
            {
                _client?.SetLogLevel(level);
                _logService.LogSdkResponse("SetLogLevel", $"Log level set to {level}");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("SetLogLevel", ex);
            }
        }

        public void FlushEvents()
        {
            _logService.LogSdkCall("FlushEvents");
            
            try
            {
                _client?.FlushEvents();
                _logService.LogSdkResponse("FlushEvents", "Events flushed");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("FlushEvents", ex);
            }
        }

        public async Task<bool> FlushEventsAsync()
        {
            _logService.LogSdkCall("FlushEventsAsync");
            
            try
            {
                await Task.Run(() => _client?.FlushEvents());
                _logService.LogSdkResponse("FlushEventsAsync", "Events flushed asynchronously");
                return true;
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("FlushEventsAsync", ex);
                return false;
            }
        }

        public void Reset()
        {
            _logService.LogSdkCall("Reset");
            
            try
            {
                _client?.Reset();
                _client = null;
                _logService.LogSdkResponse("Reset", "SDK reset - client cleared");
            }
            catch (Exception ex)
            {
                _logService.LogSdkError("Reset", ex);
            }
        }
    }
}
