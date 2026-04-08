using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using TrueMetricsDemo.Services;
using System.Linq;

namespace TrueMetricsDemo.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private readonly ITrueMetricsService _trueMetricsService;
        private readonly ILogService _logService;
        
        private string _apiKey = "your_api_key_here";
        private string _userId = "demo_user_123";
        private string _eventName = "button_click";
        private string _screenName = "MainPage";
        private string _propertyKey = "user_type";
        private string _propertyValue = "premium";
        private string _exerciseType = "running";
        private int _exerciseDuration = 3600;
        private double _exerciseDistance = 5000;
        private double _latitude = 37.7749;
        private double _longitude = -122.4194;
        private double _locationAccuracy = 10.0;
        private bool _isInitialized;
        private string _statusMessage = "Ready to initialize SDK";
        private bool _isBusy;

        public ObservableCollection<LogEntry> LogEntries => _logService.LogEntries;

        public string ApiKey
        {
            get => _apiKey;
            set => SetProperty(ref _apiKey, value);
        }

        public string UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public string EventName
        {
            get => _eventName;
            set => SetProperty(ref _eventName, value);
        }

        public string ScreenName
        {
            get => _screenName;
            set => SetProperty(ref _screenName, value);
        }

        public string PropertyKey
        {
            get => _propertyKey;
            set => SetProperty(ref _propertyKey, value);
        }

        public string PropertyValue
        {
            get => _propertyValue;
            set => SetProperty(ref _propertyValue, value);
        }

        public string ExerciseType
        {
            get => _exerciseType;
            set => SetProperty(ref _exerciseType, value);
        }

        public int ExerciseDuration
        {
            get => _exerciseDuration;
            set => SetProperty(ref _exerciseDuration, value);
        }

        public double ExerciseDistance
        {
            get => _exerciseDistance;
            set => SetProperty(ref _exerciseDistance, value);
        }

        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public double LocationAccuracy
        {
            get => _locationAccuracy;
            set => SetProperty(ref _locationAccuracy, value);
        }

        public bool IsInitialized
        {
            get => _isInitialized;
            set => SetProperty(ref _isInitialized, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        // Commands
        public ICommand InitializeCommand { get; }
        public ICommand InitializeWithEndpointCommand { get; }
        public ICommand TrackEventCommand { get; }
        public ICommand TrackEventWithParamsCommand { get; }
        public ICommand TrackScreenViewCommand { get; }
        public ICommand SetUserPropertyCommand { get; }
        public ICommand SetMultiplePropertiesCommand { get; }
        public ICommand LogExceptionCommand { get; }
        public ICommand StartSessionCommand { get; }
        public ICommand EndSessionCommand { get; }
        public ICommand TrackExerciseCommand { get; }
        public ICommand TrackExerciseWithMetadataCommand { get; }
        public ICommand SetLocationCommand { get; }
        public ICommand SetLocationWithAccuracyCommand { get; }
        public ICommand GetDeviceInfoCommand { get; }
        public ICommand GetSessionIdCommand { get; }
        public ICommand EnableDebugLoggingCommand { get; }
        public ICommand DisableDebugLoggingCommand { get; }
        public ICommand FlushEventsCommand { get; }
        public ICommand FlushEventsAsyncCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ClearLogsCommand { get; }
        public ICommand ExportLogsCommand { get; }
        public ICommand GetCurrentLocationCommand { get; }
        public ICommand TestAllMethodsCommand { get; }

        public MainPageViewModel(ITrueMetricsService trueMetricsService, ILogService logService)
        {
            _trueMetricsService = trueMetricsService;
            _logService = logService;

            InitializeCommand = new DelegateCommand(async () => await InitializeAsync(), () => !IsBusy)
                .ObservesProperty(() => IsBusy);
            InitializeWithEndpointCommand = new DelegateCommand(async () => await InitializeWithEndpointAsync(), () => !IsBusy)
                .ObservesProperty(() => IsBusy);
            TrackEventCommand = new DelegateCommand(TrackEvent, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            TrackEventWithParamsCommand = new DelegateCommand(TrackEventWithParams, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            TrackScreenViewCommand = new DelegateCommand(TrackScreenView, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            SetUserPropertyCommand = new DelegateCommand(SetUserProperty, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            SetMultiplePropertiesCommand = new DelegateCommand(SetMultipleProperties, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            LogExceptionCommand = new DelegateCommand(LogTestException, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            StartSessionCommand = new DelegateCommand(StartSession, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            EndSessionCommand = new DelegateCommand(EndSession, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            TrackExerciseCommand = new DelegateCommand(TrackExercise, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            TrackExerciseWithMetadataCommand = new DelegateCommand(TrackExerciseWithMetadata, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            SetLocationCommand = new DelegateCommand(SetLocation, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            SetLocationWithAccuracyCommand = new DelegateCommand(SetLocationWithAccuracy, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            GetDeviceInfoCommand = new DelegateCommand(async () => await GetDeviceInfoAsync(), () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            GetSessionIdCommand = new DelegateCommand(async () => await GetSessionIdAsync(), () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            EnableDebugLoggingCommand = new DelegateCommand(() => EnableDebugLogging(true), () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            DisableDebugLoggingCommand = new DelegateCommand(() => EnableDebugLogging(false), () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            FlushEventsCommand = new DelegateCommand(FlushEvents, () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            FlushEventsAsyncCommand = new DelegateCommand(async () => await FlushEventsAsync(), () => IsInitialized && !IsBusy)
                .ObservesProperty(() => IsInitialized).ObservesProperty(() => IsBusy);
            ResetCommand = new DelegateCommand(Reset, () => !IsBusy)
                .ObservesProperty(() => IsBusy);
            ClearLogsCommand = new DelegateCommand(async () => await ClearLogsAsync(), () => !IsBusy)
                .ObservesProperty(() => IsBusy);
            ExportLogsCommand = new DelegateCommand(ExportLogs, () => !IsBusy)
                .ObservesProperty(() => IsBusy);
            GetCurrentLocationCommand = new DelegateCommand(async () => await GetCurrentLocationAsync(), () => !IsBusy)
                .ObservesProperty(() => IsBusy);
            TestAllMethodsCommand = new DelegateCommand(async () => await TestAllMethodsAsync(), () => !IsBusy)
                .ObservesProperty(() => IsBusy);

            _logService.LogAdded += OnLogAdded;
        }

        private void OnLogAdded(object sender, LogEntry entry)
        {
            // Log entry already added to collection in service
        }

        private async Task InitializeAsync()
        {
            IsBusy = true;
            StatusMessage = "Initializing SDK...";
            
            var result = await _trueMetricsService.InitializeAsync(ApiKey, UserId);
            IsInitialized = result;
            StatusMessage = result ? "SDK initialized successfully" : "SDK initialization failed";
            
            IsBusy = false;
        }

        private async Task InitializeWithEndpointAsync()
        {
            IsBusy = true;
            StatusMessage = "Initializing SDK with custom endpoint...";
            
            var result = await _trueMetricsService.InitializeAsync(ApiKey, UserId, "https://custom.truemetrics.io/api/v1");
            IsInitialized = result;
            StatusMessage = result ? "SDK initialized with custom endpoint" : "SDK initialization failed";
            
            IsBusy = false;
        }

        private void TrackEvent()
        {
            _trueMetricsService.TrackEvent(EventName);
        }

        private void TrackEventWithParams()
        {
            var parameters = $"{{\"button\":\"test\",\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}}}";
            _trueMetricsService.TrackEvent(EventName, parameters);
        }

        private void TrackScreenView()
        {
            _trueMetricsService.TrackScreenView(ScreenName);
        }

        private void SetUserProperty()
        {
            _trueMetricsService.SetUserProperty(PropertyKey, PropertyValue);
        }

        private void SetMultipleProperties()
        {
            var properties = new System.Collections.Generic.Dictionary<string, string>
            {
                { "user_type", "premium" },
                { "subscription", "annual" },
                { "language", "en" },
                { "platform", DeviceInfo.Platform.ToString() },
                { "app_version", AppInfo.VersionString }
            };
            _trueMetricsService.SetUserProperties(properties);
        }

        private void LogTestException()
        {
            try
            {
                throw new InvalidOperationException("Test exception for TrueMetrics logging");
            }
            catch (Exception ex)
            {
                _trueMetricsService.LogException(ex, "Test exception context");
            }
        }

        private void StartSession()
        {
            _trueMetricsService.StartSession();
        }

        private void EndSession()
        {
            _trueMetricsService.EndSession();
        }

        private void TrackExercise()
        {
            _trueMetricsService.TrackExercise(ExerciseType, ExerciseDuration, ExerciseDistance);
        }

        private void TrackExerciseWithMetadata()
        {
            var metadata = $"{{\"route\":\"morning_run\",\"weather\":\"sunny\",\"temperature\":22}}";
            _trueMetricsService.TrackExercise(ExerciseType, ExerciseDuration, ExerciseDistance, metadata);
        }

        private void SetLocation()
        {
            _trueMetricsService.SetLocation(Latitude, Longitude);
        }

        private void SetLocationWithAccuracy()
        {
            _trueMetricsService.SetLocation(Latitude, Longitude, LocationAccuracy);
        }

        private async Task GetDeviceInfoAsync()
        {
            IsBusy = true;
            var info = await _trueMetricsService.GetDeviceInfoAsync();
            await Application.Current.MainPage.DisplayAlert("Device Info", info, "OK");
            IsBusy = false;
        }

        private async Task GetSessionIdAsync()
        {
            IsBusy = true;
            var sessionId = await _trueMetricsService.GetSessionIdAsync();
            await Application.Current.MainPage.DisplayAlert("Session ID", sessionId, "OK");
            IsBusy = false;
        }

        private void EnableDebugLogging(bool enable)
        {
            _trueMetricsService.EnableDebugLogging(enable);
        }

        private void FlushEvents()
        {
            _trueMetricsService.FlushEvents();
        }

        private async Task FlushEventsAsync()
        {
            IsBusy = true;
            await _trueMetricsService.FlushEventsAsync();
            IsBusy = false;
        }

        private void Reset()
        {
            _trueMetricsService.Reset();
            IsInitialized = false;
            StatusMessage = "SDK reset. Ready to re-initialize.";
        }

        private async Task ClearLogsAsync()
        {
            await _logService.ClearLogsAsync();
        }

        private async void ExportLogs()
        {
            var logs = _logService.ExportLogs();
            await Clipboard.SetTextAsync(logs);
            await Application.Current.MainPage.DisplayAlert("Logs Exported", "Logs copied to clipboard. Paste them in an email or text editor.", "OK");
        }

        private async Task GetCurrentLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(10)
                    });
                }

                if (location != null)
                {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                    LocationAccuracy = location.Accuracy ?? 0;
                    _logService.LogSuccess($"Current location: {Latitude:F6}, {Longitude:F6} (accuracy: {LocationAccuracy:F1}m)");
                }
            }
            catch (FeatureNotSupportedException)
            {
                _logService.LogError("Geolocation not supported on this device");
            }
            catch (PermissionException)
            {
                _logService.LogError("Location permission not granted");
            }
            catch (Exception ex)
            {
                _logService.LogError("Error getting location", ex);
            }
        }

        private async Task TestAllMethodsAsync()
        {
            IsBusy = true;
            StatusMessage = "Running all SDK method tests...";
            _logService.LogInfo("=== Starting comprehensive SDK test ===");

            // Initialize
            if (!IsInitialized)
            {
                await InitializeAsync();
            }

            if (!IsInitialized)
            {
                _logService.LogError("Cannot run tests - SDK not initialized");
                IsBusy = false;
                return;
            }

            // Test events
            TrackEvent();
            await Task.Delay(100);
            TrackEventWithParams();
            await Task.Delay(100);
            TrackScreenView();
            await Task.Delay(100);

            // Test user properties
            SetUserProperty();
            await Task.Delay(100);
            SetMultipleProperties();
            await Task.Delay(100);

            // Test session
            StartSession();
            await Task.Delay(100);
            EndSession();
            await Task.Delay(100);

            // Test exercise
            TrackExercise();
            await Task.Delay(100);
            TrackExerciseWithMetadata();
            await Task.Delay(100);

            // Test location
            SetLocation();
            await Task.Delay(100);
            SetLocationWithAccuracy();
            await Task.Delay(100);

            // Test debug logging
            EnableDebugLogging(true);
            await Task.Delay(100);
            EnableDebugLogging(false);
            await Task.Delay(100);

            // Test flush
            FlushEvents();
            await Task.Delay(100);

            // Get info
            var deviceInfo = await _trueMetricsService.GetDeviceInfoAsync();
            _logService.LogInfo($"Device Info: {deviceInfo}");
            
            var sessionId = await _trueMetricsService.GetSessionIdAsync();
            _logService.LogInfo($"Session ID: {sessionId}");

            _logService.LogSuccess("=== All SDK method tests completed ===");
            StatusMessage = "All tests completed. Check logs for results.";
            IsBusy = false;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            _logService.LogAdded -= OnLogAdded;
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            // Restore log handler if needed
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
        }
    }
}
