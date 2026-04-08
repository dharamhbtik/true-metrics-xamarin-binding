# TrueMetrics SDK Demo Application

This is a complete Xamarin.Forms demonstration application for the TrueMetrics SDK, showcasing all available API methods with a comprehensive MVVM architecture using Prism for navigation.

## Features

### SDK Methods Demonstrated

1. **Initialization**
   - `InitializeAsync(apiKey, userId)` - Standard initialization
   - `InitializeAsync(apiKey, userId, endpoint)` - Custom endpoint initialization
   - `Reset()` - Reset SDK state

2. **Event Tracking**
   - `TrackEvent(eventName)` - Simple event tracking
   - `TrackEvent(eventName, parameters)` - Event with parameters
   - `TrackScreenView(screenName)` - Screen view tracking

3. **User Properties**
   - `SetUserProperty(key, value)` - Set single property
   - `SetUserProperties(properties)` - Set multiple properties

4. **Session Management**
   - `StartSession()` - Start tracking session
   - `EndSession()` - End tracking session
   - `GetSessionIdAsync()` - Get current session ID

5. **Exercise Tracking**
   - `TrackExercise(type, duration, distance)` - Track exercise
   - `TrackExercise(type, duration, distance, metadata)` - Track with metadata

6. **Location Services**
   - `SetLocation(latitude, longitude)` - Set location
   - `SetLocation(latitude, longitude, accuracy)` - Set with accuracy
   - Get current device location via Xamarin.Essentials

7. **Debug & Logging**
   - `EnableDebugLogging(enable)` - Toggle debug logging
   - `LogException(exception)` - Log exceptions
   - `LogException(exception, context)` - Log with context

8. **Data Management**
   - `FlushEvents()` - Flush pending events
   - `FlushEventsAsync()` - Flush asynchronously
   - `GetDeviceInfoAsync()` - Get device information

### On-Screen Logging System

The app features a comprehensive logging system that displays all SDK calls and responses:

- **Log Levels**: Info, Warning, Error, Success, Debug, SDK Call, SDK Response
- **Color-coded entries** for easy identification
- **Timestamped** entries for debugging
- **Copy to clipboard** functionality for log export
- **Clear logs** capability

### Test All Methods

Click the "TEST ALL METHODS" button to run a comprehensive automated test suite that exercises all SDK methods in sequence.

## Architecture

### MVVM Pattern

```
TrueMetricsDemo/
├── Services/
│   ├── ILogService.cs          # Logging interface
│   ├── LogService.cs           # Observable log collection
│   ├── ITrueMetricsService.cs  # SDK wrapper interface
│   └── TrueMetricsService.cs  # SDK implementation with logging
├── ViewModels/
│   └── MainPageViewModel.cs   # Main page logic with all commands
└── Views/
    ├── MainPage.xaml          # UI with buttons and log viewer
    └── MainPage.xaml.cs       # Code-behind with converters
```

### Prism Integration

- **Navigation**: `NavigationService` for view navigation
- **Dependency Injection**: `IContainerRegistry` for service registration
- **Commands**: `DelegateCommand` with `ObservesProperty` for reactive UI

## Dependencies

All dependencies are inherited from `MyPackSmall.Droid` with exact versions:

### Core Dependencies
- Xamarin.Forms 5.0.0.2662
- Prism.DryIoc.Forms 8.1.97
- TrueMetrics.Xamarin.Android 1.6.2
- Xamarin.Kotlin.StdLib 1.9.0
- Xamarin.KotlinX.Coroutines.Android 1.7.3

### AndroidX & Firebase
- All AndroidX packages (Activity, AppCompat, Core, Lifecycle, etc.)
- Firebase Analytics, Crashlytics, Messaging
- Google Play Services (Location, Maps, Base, etc.)

### Additional Libraries
- Xamarin.Essentials 1.8.1
- Xamarin.CommunityToolkit 2.0.6
- Newtonsoft.Json 13.0.3
- System.Reactive.* 4.1.6
- And many more (see .csproj files)

## Getting Started

### Prerequisites

1. Visual Studio 2022 with Xamarin workload
2. Android SDK 13.0+ installed
3. TrueMetrics SDK API key

### Setup

1. Open `MyPackSmall.sln` in Visual Studio
2. Set `TrueMetricsDemo.Droid` as startup project
3. Update the API key in the UI or code:
   ```csharp
   ApiKey = "your_api_key_here";
   ```
4. Build and deploy to Android device/emulator

### Using the App

1. **Initialize SDK**: Enter API key and User ID, then click "Initialize"
2. **Track Events**: Use event tracking buttons with customizable event names
3. **Set Properties**: Configure user properties for better analytics
4. **Track Exercise**: Input exercise type, duration, and distance
5. **Set Location**: Manually enter coordinates or use "Get Current" for GPS
6. **Debug Mode**: Enable debug logging to see detailed SDK output
7. **Export Logs**: Copy logs to clipboard for troubleshooting

## Troubleshooting

### Kotlin UUID Error
If you encounter `NoClassDefFoundError: kotlin.uuid.Uuid`, ensure:
- Kotlin stdlib 1.9.0 is referenced
- ProGuard rules are configured (see proguard.cfg in Droid folder)
- Device cache is cleared

### Linker Issues
If Prism/DryIoc fails at runtime, add to `proguard.cfg`:
```
-keep class DryIoc.** { *; }
-keep class Prism.** { *; }
```

### Build Errors
1. Clean solution: `Build > Clean Solution`
2. Delete bin/obj folders
3. Restore NuGet packages
4. Rebuild

## Project Structure

```
true_metrics_integration/
├── TrueMetricsDemo/              # Shared .NET Standard project
│   ├── App.xaml                  # Prism application
│   ├── Services/                 # Business logic
│   ├── ViewModels/              # MVVM view models
│   └── Views/                   # XAML pages
├── TrueMetricsDemo.Droid/        # Android application
│   ├── MainActivity.cs          # Entry point
│   ├── MainApplication.cs       # Application class
│   ├── Resources/               # Android resources
│   └── Properties/              # Manifest & assembly info
└── MyPackSmall.sln              # Solution file (updated)
```

## Key Files

### Entry Points
- `MainActivity.cs` - Android activity with Prism initialization
- `App.xaml.cs` - Prism application setup and navigation

### Services
- `TrueMetricsService.cs` - Wraps SDK with comprehensive logging
- `LogService.cs` - Observable collection of log entries

### UI
- `MainPage.xaml` - Complete UI with all SDK method buttons
- `MainPageViewModel.cs` - All commands and business logic

## License

This demo project is provided as-is for TrueMetrics SDK integration testing.
