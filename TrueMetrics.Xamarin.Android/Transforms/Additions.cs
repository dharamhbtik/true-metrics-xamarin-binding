using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using IO.Truemetrics.Truemetricssdk.Engine.Stats;

namespace TrueMetrics.Xamarin.Android
{
    /// <summary>
    /// Comprehensive helper class for TrueMetrics SDK - wraps all public SDK methods.
    /// </summary>
    public static class TrueMetricsHelper
    {
        private static TruemetricsSdk _instance;
        private static readonly object _lock = new object();

        #region Initialization

        /// <summary>
        /// Initialize TrueMetrics SDK with just API key.
        /// </summary>
        public static bool Initialize(Context context, string apiKey)
        {
            if (context == null || string.IsNullOrWhiteSpace(apiKey))
                return false;

            lock (_lock)
            {
                if (_instance != null)
                    return true;

                try
                {
                    var config = new Config.SdkConfiguration.Builder(apiKey).Build();
                    TruemetricsSdk.Init(context.ApplicationContext, config);
                    _instance = TruemetricsSdk.Instance;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TrueMetrics Init failed: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Initialize with custom configuration.
        /// </summary>
        public static bool Initialize(Context context, Config.SdkConfiguration config)
        {
            if (context == null || config == null)
                return false;

            lock (_lock)
            {
                if (_instance != null)
                    return true;

                try
                {
                    TruemetricsSdk.Init(context.ApplicationContext, config);
                    _instance = TruemetricsSdk.Instance;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TrueMetrics Init failed: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Initialize TrueMetrics SDK with persistent notification for background recording.
        /// Call this from your MainActivity.OnCreate() before LoadApplication().
        /// </summary>
        /// <param name="context">Activity context</param>
        /// <param name="apiKey">Your TrueMetrics API key</param>
        /// <param name="notificationTitle">Title for persistent notification (optional)</param>
        /// <param name="notificationContent">Content text for persistent notification (optional)</param>
        /// <param name="smallIconResId">Resource ID for notification icon (optional, default: IcMediaPlay)</param>
        /// <returns>True if initialization succeeded</returns>
        public static bool InitializeWithNotification(Context context, string apiKey, 
            string notificationTitle = null, string notificationContent = null, int smallIconResId = 0)
        {
            if (context == null || string.IsNullOrWhiteSpace(apiKey))
                return false;

            lock (_lock)
            {
                if (_instance != null)
                    return true;

                try
                {
                    // Create notification channel first (required for Android O+)
                    TrueMetricsNotificationHelper.CreateNotificationChannel(context);

                    // Create notification builder (can be used later when starting foreground service)
                    var iconId = smallIconResId != 0 ? smallIconResId : global::Android.Resource.Drawable.IcMediaPlay;
                    var notificationBuilder = new TrueMetricsNotificationBuilder(
                        notificationTitle, 
                        notificationContent, 
                        iconId);

                    // Build configuration with API key
                    var config = new Config.SdkConfiguration.Builder(apiKey).Build();
                    
                    TruemetricsSdk.Init(context.ApplicationContext, config);
                    _instance = TruemetricsSdk.Instance;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TrueMetrics Init with Notification failed: {ex.Message}");
                    return false;
                }
            }
        }

        public static bool IsInitialized => _instance != null;

        public static void Shutdown()
        {
            lock (_lock)
            {
                try { _instance?.Deinitialize(); } catch { }
                _instance = null;
            }
        }

        #endregion

        #region Recording Control

        public static bool StartRecording()
        {
            try
            {
                if (_instance == null) return false;
                if (_instance.IsRecordingInProgress) return true;
                _instance.StartRecording();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StartRecording failed: {ex.Message}");
                return false;
            }
        }

        public static bool StopRecording()
        {
            try
            {
                if (_instance == null) return false;
                if (!_instance.IsRecordingInProgress) return true;
                _instance.StopRecording();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StopRecording failed: {ex.Message}");
                return false;
            }
        }

        public static bool IsRecording => _instance?.IsRecordingInProgress ?? false;
        public static bool IsRecordingStopped => _instance?.IsRecordingStopped ?? false;
        public static long RecordingStartTime => _instance?.RecordingStartTime ?? 0;

        #endregion

        #region Device Info

        public static string DeviceId => _instance?.DeviceId;
        public static IO.Truemetrics.Truemetricssdk.Engine.Configuration.Domain.Model.Configuration ActiveConfig => _instance?.ActiveConfig;

        #endregion

        #region Metadata Operations

        public static bool LogMetadata(Dictionary<string, string> payload)
        {
            try
            {
                if (_instance == null || payload == null) return false;
                _instance.LogMetadata(payload);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogMetadata failed: {ex.Message}");
                return false;
            }
        }

        public static bool LogMetadataByTag(string tag)
        {
            try
            {
                if (_instance == null || string.IsNullOrEmpty(tag)) return false;
                return _instance.LogMetadataByTag(tag);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogMetadataByTag failed: {ex.Message}");
                return false;
            }
        }

        public static bool AppendToMetadataTag(string tag, string key, string value)
        {
            try
            {
                if (_instance == null || string.IsNullOrEmpty(tag)) return false;
                _instance.AppendToMetadataTag(tag, key, value);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AppendToMetadataTag failed: {ex.Message}");
                return false;
            }
        }

        public static bool AppendToMetadataTag(string tag, Dictionary<string, string> metadata)
        {
            try
            {
                if (_instance == null || string.IsNullOrEmpty(tag) || metadata == null) return false;
                _instance.AppendToMetadataTag(tag, metadata);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AppendToMetadataTag failed: {ex.Message}");
                return false;
            }
        }

        public static Dictionary<string, string> GetMetadataByTag(string tag)
        {
            try
            {
                if (_instance == null || string.IsNullOrEmpty(tag)) return null;
                var result = _instance.GetMetadataByTag(tag);
                return result as Dictionary<string, string>;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetMetadataByTag failed: {ex.Message}");
                return null;
            }
        }

        public static bool RemoveFromMetadataTag(string tag, string key)
        {
            try
            {
                if (_instance == null) return false;
                return _instance.RemoveFromMetadataTag(tag, key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RemoveFromMetadataTag failed: {ex.Message}");
                return false;
            }
        }

        public static bool RemoveMetadataTag(string tag)
        {
            try
            {
                if (_instance == null) return false;
                return _instance.RemoveMetadataTag(tag);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RemoveMetadataTag failed: {ex.Message}");
                return false;
            }
        }

        public static bool ClearAllMetadata()
        {
            try
            {
                if (_instance == null) return false;
                _instance.ClearAllMetadata();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ClearAllMetadata failed: {ex.Message}");
                return false;
            }
        }

        public static ICollection<string> MetadataTags => _instance?.MetadataTags;

        #endregion

        #region Metadata Templates

        public static bool CreateMetadataTemplate(string templateName, Dictionary<string, string> templateData)
        {
            try
            {
                if (_instance == null || string.IsNullOrEmpty(templateName) || templateData == null) return false;
                _instance.CreateMetadataTemplate(templateName, templateData);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateMetadataTemplate failed: {ex.Message}");
                return false;
            }
        }

        public static Dictionary<string, string> GetMetadataTemplate(string templateName)
        {
            try
            {
                if (_instance == null || string.IsNullOrEmpty(templateName)) return null;
                var result = _instance.GetMetadataTemplate(templateName);
                return result as Dictionary<string, string>;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetMetadataTemplate failed: {ex.Message}");
                return null;
            }
        }

        public static bool CreateMetadataFromTemplate(string tag, string templateName)
        {
            try
            {
                if (_instance == null) return false;
                return _instance.CreateMetadataFromTemplate(tag, templateName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateMetadataFromTemplate failed: {ex.Message}");
                return false;
            }
        }

        public static bool RemoveMetadataTemplate(string templateName)
        {
            try
            {
                if (_instance == null) return false;
                return _instance.RemoveMetadataTemplate(templateName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RemoveMetadataTemplate failed: {ex.Message}");
                return false;
            }
        }

        public static ICollection<string> MetadataTemplateNames => _instance?.MetadataTemplateNames;

        #endregion

        #region Statistics

        public static IList<SensorStatistics> SensorStatistics => _instance?.SensorStatistics;
        public static UploadStatistics UploadStatistics => _instance?.UploadStatistics;

        #endregion

        #region Sensor Control

        public static bool AllSensorsEnabled
        {
            get => _instance?.AllSensorsEnabled ?? false;
            set
            {
                try
                {
                    if (_instance != null)
                        _instance.AllSensorsEnabled = value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"AllSensorsEnabled failed: {ex.Message}");
                }
            }
        }

        #endregion

        #region Raw SDK Access

        public static TruemetricsSdk Instance => _instance;

        #endregion
    }

    /// <summary>
    /// Original initializer class for advanced configuration.
    /// </summary>
    public static class TrueMetricsInitializer
    {
        private static TruemetricsSdk _instance;
        private static readonly object _lock = new object();

        public static TruemetricsSdk Initialize(Application application, string apiKey)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey));

            lock (_lock)
            {
                if (_instance != null)
                    return _instance;

                try
                {
                    var config = new Config.SdkConfiguration.Builder(apiKey).Build();
                    TruemetricsSdk.Init(application, config);
                    _instance = TruemetricsSdk.Instance;
                    return _instance;
                }
                catch (Java.Lang.Exception ex)
                {
                    throw new InvalidOperationException($"Failed to initialize TrueMetrics SDK: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to initialize TrueMetrics SDK: {ex.Message}", ex);
                }
            }
        }

        public static TruemetricsSdk Initialize(Application application, Config.SdkConfiguration configuration)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            lock (_lock)
            {
                if (_instance != null)
                    return _instance;

                try
                {
                    TruemetricsSdk.Init(application, configuration);
                    _instance = TruemetricsSdk.Instance;
                    return _instance;
                }
                catch (Java.Lang.Exception ex)
                {
                    throw new InvalidOperationException($"Failed to initialize TrueMetrics SDK: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to initialize TrueMetrics SDK: {ex.Message}", ex);
                }
            }
        }

        public static TruemetricsSdk Instance => _instance;
        public static bool IsInitialized => _instance != null;

        public static void Deinitialize()
        {
            lock (_lock)
            {
                _instance?.Deinitialize();
                _instance = null;
            }
        }
    }

    /// <summary>
    /// Extension methods for the TruemetricsSdk class.
    /// </summary>
    public static class TruemetricsSdkExtensions
    {
        public static bool IsRecording(this TruemetricsSdk sdk)
        {
            return sdk?.IsRecordingInProgress ?? false;
        }

        public static void ToggleRecording(this TruemetricsSdk sdk)
        {
            if (sdk == null) return;
            if (sdk.IsRecordingInProgress)
                sdk.StopRecording();
            else
                sdk.StartRecording();
        }
    }

    /// <summary>
    /// Persistent notification builder for TrueMetrics SDK foreground service.
    /// Creates a non-dismissible notification that keeps the service running in background.
    /// </summary>
    public class TrueMetricsNotificationBuilder
    {
        public const string ChannelId = "truemetrics_recording_channel";
        public const string ChannelName = "TrueMetrics Recording";
        public const int NotificationId = 1001;

        private readonly string _title;
        private readonly string _content;
        private readonly int _smallIconResId;

        /// <summary>
        /// Creates a default notification builder with standard messages.
        /// Note: You must provide a valid icon resource ID when building the notification.
        /// </summary>
        public TrueMetricsNotificationBuilder()
        {
            _title = "Recording in Progress";
            _content = "TrueMetrics is collecting sensor data";
            _smallIconResId = 0; // User must provide valid icon resource
        }

        /// <summary>
        /// Creates a custom notification builder.
        /// </summary>
        /// <param name="title">Notification title</param>
        /// <param name="content">Notification content text</param>
        /// <param name="smallIconResId">Resource ID for small icon (use Android.Resource.Drawable or your own)</param>
        public TrueMetricsNotificationBuilder(string title, string content, int smallIconResId)
        {
            _title = title ?? "Recording in Progress";
            _content = content ?? "TrueMetrics is collecting sensor data";
            _smallIconResId = smallIconResId;
        }

        /// <summary>
        /// Creates the persistent notification for the foreground service.
        /// Call this before starting recording to get a notification to pass to the service.
        /// </summary>
        public Notification BuildNotification(Context context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Create notification channel for Android O+ (API 26+)
            CreateNotificationChannel(context);

            // Build the persistent notification
            var builder = new NotificationCompat.Builder(context, ChannelId)
                .SetContentTitle(_title)
                .SetContentText(_content)
                .SetSmallIcon(_smallIconResId)
                .SetOngoing(true)                                    // Cannot be dismissed by user
                .SetAutoCancel(false)                                // Don't auto-cancel
                .SetPriority(NotificationCompat.PriorityLow)         // Low priority, no sound/vibration
                .SetCategory(NotificationCompat.CategoryService)   // Service notification
                .SetVisibility(NotificationCompat.VisibilityPublic) // Show on lock screen
                .SetShowWhen(true)                                   // Show timestamp
                .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis());

            return builder.Build();
        }

        /// <summary>
        /// Creates the notification channel for Android 8.0+ (API 26+).
        /// </summary>
        private void CreateNotificationChannel(Context context)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            var channel = new NotificationChannel(
                ChannelId,
                ChannelName,
                NotificationImportance.Low)  // Low importance = no sound/vibration
            {
                Description = "Persistent notification for TrueMetrics background recording",
                LockscreenVisibility = NotificationVisibility.Public
            };

            channel.SetShowBadge(false);
            channel.EnableVibration(false);
            channel.EnableLights(false);

            var notificationManager = NotificationManager.FromContext(context);
            notificationManager?.CreateNotificationChannel(channel);
        }
    }

    /// <summary>
    /// Helper class for TrueMetrics notification management.
    /// </summary>
    public static class TrueMetricsNotificationHelper
    {
        /// <summary>
        /// Creates a notification channel manually (call this in your MainActivity.OnCreate before initializing SDK).
        /// </summary>
        public static void CreateNotificationChannel(Context context)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            var channel = new NotificationChannel(
                TrueMetricsNotificationBuilder.ChannelId,
                TrueMetricsNotificationBuilder.ChannelName,
                NotificationImportance.Low)
            {
                Description = "TrueMetrics background recording service"
            };

            channel.SetShowBadge(false);
            channel.EnableVibration(false);
            channel.EnableLights(false);

            var notificationManager = NotificationManager.FromContext(context);
            notificationManager?.CreateNotificationChannel(channel);
        }

        /// <summary>
        /// Deletes the notification channel.
        /// </summary>
        public static void DeleteNotificationChannel(Context context)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            var notificationManager = NotificationManager.FromContext(context);
            notificationManager?.DeleteNotificationChannel(TrueMetricsNotificationBuilder.ChannelId);
        }

        /// <summary>
        /// Builds a persistent notification for the foreground service.
        /// </summary>
        public static Notification BuildPersistentNotification(Context context, string title = null, string content = null, int iconResId = 0)
        {
            var builder = new TrueMetricsNotificationBuilder(title, content, iconResId);
            return builder.BuildNotification(context);
        }
    }
}
