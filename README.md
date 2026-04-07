# TrueMetrics.Xamarin.Android

Xamarin.Android binding library for the TrueMetrics SDK 1.5.0 — a sensor data collection platform for last-mile delivery applications.

## Overview

This NuGet package provides a C# binding for the native Android TrueMetrics SDK, enabling Xamarin.Forms and .NET MAUI applications to integrate comprehensive sensor-based data collection including GPS location, accelerometer, gyroscope, and barometer readings.

**Version 1.5.4** includes persistent notification support and a comprehensive `TrueMetricsHelper` class.

## Prerequisites

- Visual Studio 2022 with Xamarin workload installed
- .NET 6.0 SDK or later
- Java 11 SDK or later
- Android SDK with API Level 21+ (Android 5.0 Lollipop)
- Xamarin.Forms 5.0+ or .NET MAUI 6.0+

## Installation

### Option 1: Local NuGet Package (Recommended)

The package is built and available at:
```
/Users/dkumar/CascadeProjects/TrueMetrics.Xamarin/TrueMetrics.Xamarin.Android/bin/Release/TrueMetrics.Xamarin.Android.1.5.4.nupkg
```

Add a local NuGet source:
```bash
nuget sources Add -Name "TrueMetricsLocal" -Source "/Users/dkumar/CascadeProjects/TrueMetrics.Xamarin/TrueMetrics.Xamarin.Android/bin/Release"
```

Install in your Android project only:
```powershell
Install-Package TrueMetrics.Xamarin.Android -Version 1.5.4 -Source TrueMetricsLocal
```

### Option 2: Direct DLL Reference

Add to your `.csproj`:
```xml
<ItemGroup>
  <Reference Include="TrueMetrics.Xamarin.Android">
    <HintPath>/Users/dkumar/CascadeProjects/TrueMetrics.Xamarin/TrueMetrics.Xamarin.Android/bin/Release/TrueMetrics.Xamarin.Android.dll</HintPath>
  </Reference>
</ItemGroup>
```

### Option 3: PackageReference

```xml
<PackageReference Include="TrueMetrics.Xamarin.Android" Version="1.5.4" />
```

---

## Required Dependencies

**IMPORTANT**: This package does not include NuGet dependencies. You must manually add these packages to your Android project:

```xml
<!-- Required for the binding -->
<PackageReference Include="Xamarin.Kotlin.StdLib" Version="1.7.10" />
<PackageReference Include="Xamarin.KotlinX.Coroutines.Android" Version="1.6.4" />
<PackageReference Include="Xamarin.GooglePlayServices.Location" Version="118.0.0.1" />

<!-- Required for persistent notifications -->
<PackageReference Include="Xamarin.AndroidX.Core" Version="1.9.0.1" />
```

---

## Quick Start Guide

### Step 1: Add Permissions to AndroidManifest.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    
    <!-- Required permissions -->
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    <uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
    <uses-permission android:name="android.permission.WAKE_LOCK" />
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
    <uses-permission android:name="android.permission.ACTIVITY_RECOGNITION" />
    
    <!-- Optional: Background location for Android 10+ -->
    <uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
    
    <application android:allowBackup="true" android:label="@string/app_name">
        <service android:name="io.truemetrics.truemetricssdk.recording.service.RecordingService"
                 android:foregroundServiceType="location|dataSync"
                 android:exported="false" />
    </application>
</manifest>
```

### Step 2: Initialize SDK

In your **MainActivity.cs** or **Application.cs**:

```csharp
using Android.App;
using Android.OS;
using TrueMetrics.Xamarin.Android;

[Activity(Label = "MyApp", MainLauncher = true)]
public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        Xamarin.Forms.Forms.Init(this, savedInstanceState);
        
        // Initialize TrueMetrics with your API key
        TrueMetricsHelper.Initialize(this, "YOUR_API_KEY_HERE");
        
        LoadApplication(new App());
    }
}
```

### Step 3: Use in Your App

Once initialized, use anywhere in your code:

```csharp
using TrueMetrics.Xamarin.Android;

// Start recording sensor data
TrueMetricsHelper.StartRecording();

// Check if recording
if (TrueMetricsHelper.IsRecording)
{
    Console.WriteLine("Recording in progress...");
}

// Stop recording
TrueMetricsHelper.StopRecording();
```

---

## Using with Xamarin.Forms (Dependency Injection)

Since the NuGet package is Android-only, use dependency injection to access it from your shared code.

### 1. Create Interface in Common Project

In your `.NET Standard` / shared project:

```csharp
public interface ITrueMetricsService
{
    bool Initialize(string apiKey);
    bool StartRecording();
    bool StopRecording();
    bool IsRecording { get; }
    bool IsInitialized { get; }
    string DeviceId { get; }
    bool LogMetadata(Dictionary<string, string> data);
}
```

### 2. Implement in Android Project

```csharp
using Android.App;
using Android.Content;
using TrueMetrics.Xamarin.Android;

[assembly: Xamarin.Forms.Dependency(typeof(TrueMetricsService))]

public class TrueMetricsService : ITrueMetricsService
{
    public bool Initialize(string apiKey)
    {
        var activity = Xamarin.Essentials.Platform.CurrentActivity;
        return TrueMetricsHelper.Initialize(activity, apiKey);
    }

    public bool StartRecording() => TrueMetricsHelper.StartRecording();
    public bool StopRecording() => TrueMetricsHelper.StopRecording();
    public bool IsRecording => TrueMetricsHelper.IsRecording;
    public bool IsInitialized => TrueMetricsHelper.IsInitialized;
    public string DeviceId => TrueMetricsHelper.DeviceId;
    
    public bool LogMetadata(Dictionary<string, string> data) => 
        TrueMetricsHelper.LogMetadata(data);
}
```

### 3. Register with Prism (or your DI container)

In `MainActivity.cs` or `App.xaml.cs`:

```csharp
// For Prism.DryIoc
protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    containerRegistry.Register<ITrueMetricsService, TrueMetricsService>();
}
```

### 4. Use in ViewModels

```csharp
public class DeliveryViewModel : BindableBase
{
    private readonly ITrueMetricsService _metrics;

    public DeliveryViewModel(ITrueMetricsService metrics)
    {
        _metrics = metrics;
    }

    public void StartDelivery()
    {
        // Initialize once
        if (!_metrics.IsInitialized)
        {
            _metrics.Initialize("YOUR_API_KEY");
        }
        
        // Start recording
        _metrics.StartRecording();
        
        // Log metadata
        _metrics.LogMetadata(new Dictionary<string, string>
        {
            { "delivery_id", "DEL-12345" },
            { "driver_id", "DRV-001" },
            { "route", "Route A" }
        });
    }

    public void CompleteDelivery()
    {
        _metrics.StopRecording();
    }
}
```

---

## TrueMetricsHelper API Reference

All methods are **static** and return `bool` for success/failure (except properties).

### Initialization

```csharp
// Initialize with API key only
bool success = TrueMetricsHelper.Initialize(context, "YOUR_API_KEY");

// Initialize with custom configuration
var config = new SdkConfiguration.Builder("API_KEY")
    .SetBaseUrl("https://custom.api.com")  // Optional
    .Build();
bool success = TrueMetricsHelper.Initialize(context, config);

// Check initialization status
bool isInit = TrueMetricsHelper.IsInitialized;

// Shutdown SDK
TrueMetricsHelper.Shutdown();
```

### Recording Control

```csharp
// Start recording
bool started = TrueMetricsHelper.StartRecording();

// Stop recording
bool stopped = TrueMetricsHelper.StopRecording();

// Check recording status
bool isRecording = TrueMetricsHelper.IsRecording;
bool isStopped = TrueMetricsHelper.IsRecordingStopped;

// Get recording start time (Unix timestamp in ms)
long startTime = TrueMetricsHelper.RecordingStartTime;
```

### Device Information

```csharp
// Get unique device ID assigned by TrueMetrics
string deviceId = TrueMetricsHelper.DeviceId;

// Get active configuration
var config = TrueMetricsHelper.ActiveConfig;
```

### Metadata Operations

```csharp
// Log metadata (Dictionary)
var data = new Dictionary<string, string>
{
    { "delivery_id", "DEL-123" },
    { "customer", "John Doe" },
    { "address", "123 Main St" }
};
bool logged = TrueMetricsHelper.LogMetadata(data);

// Log predefined metadata tag
bool logged = TrueMetricsHelper.LogMetadataByTag("delivery_start");

// Append to existing metadata tag
TrueMetricsHelper.AppendToMetadataTag("delivery", "status", "in_progress");

// Append dictionary to tag
var updates = new Dictionary<string, string> { { "eta", "10:30" } };
TrueMetricsHelper.AppendToMetadataTag("delivery", updates);

// Get metadata by tag
var metadata = TrueMetricsHelper.GetMetadataByTag("delivery");

// Remove key from metadata tag
TrueMetricsHelper.RemoveFromMetadataTag("delivery", "temp_field");

// Remove entire metadata tag
TrueMetricsHelper.RemoveMetadataTag("delivery");

// Clear all metadata
TrueMetricsHelper.ClearAllMetadata();

// Get all metadata tag names
var tags = TrueMetricsHelper.MetadataTags;
```

### Metadata Templates

```csharp
// Create reusable template
var template = new Dictionary<string, string>
{
    { "type", "delivery" },
    { "status", "in_progress" }
};
TrueMetricsHelper.CreateMetadataTemplate("delivery_template", template);

// Use template to create metadata tag
TrueMetricsHelper.CreateMetadataFromTemplate("current_delivery", "delivery_template");

// Get template
var tpl = TrueMetricsHelper.GetMetadataTemplate("delivery_template");

// Remove template
TrueMetricsHelper.RemoveMetadataTemplate("delivery_template");

// Get all template names
var templates = TrueMetricsHelper.MetadataTemplateNames;
```

### Statistics

```csharp
// Get sensor statistics (readings count, quality, etc.)
var sensorStats = TrueMetricsHelper.SensorStatistics;

// Get upload statistics (bytes uploaded, errors, etc.)
var uploadStats = TrueMetricsHelper.UploadStatistics;
```

### Sensor Control

```csharp
// Enable/disable all sensors
TrueMetricsHelper.AllSensorsEnabled = false;  // Pause
TrueMetricsHelper.AllSensorsEnabled = true;     // Resume

// Check sensor status
bool enabled = TrueMetricsHelper.AllSensorsEnabled;
```

### Raw SDK Access

```csharp
// Access raw SDK instance for advanced operations
var sdk = TrueMetricsHelper.Instance;

// Use raw SDK methods not in helper
var flow = sdk.ObserveSdkStatus();  // Kotlin Flow
```

---

## Complete Usage Example

```csharp
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using TrueMetrics.Xamarin.Android;

public class DeliveryPage : ContentPage
{
    private string _currentDeliveryId;

    public DeliveryPage()
    {
        // Initialize UI
        var startBtn = new Button { Text = "Start Delivery" };
        var stopBtn = new Button { Text = "Complete Delivery" };
        var statusLabel = new Label { Text = "Not recording" };
        
        startBtn.Clicked += OnStartDelivery;
        stopBtn.Clicked += OnCompleteDelivery;
        
        Content = new StackLayout 
        { 
            Children = { startBtn, stopBtn, statusLabel }
        };
    }

    private void OnStartDelivery(object sender, EventArgs e)
    {
        try
        {
            // Initialize SDK (only once)
            if (!TrueMetricsHelper.IsInitialized)
            {
                var activity = Xamarin.Essentials.Platform.CurrentActivity;
                TrueMetricsHelper.Initialize(activity, "YOUR_API_KEY");
            }

            // Generate delivery ID
            _currentDeliveryId = $"DEL-{DateTime.Now:yyyyMMdd-HHmmss}";

            // Start recording
            if (TrueMetricsHelper.StartRecording())
            {
                // Log delivery metadata
                TrueMetricsHelper.LogMetadata(new Dictionary<string, string>
                {
                    { "delivery_id", _currentDeliveryId },
                    { "start_time", DateTime.UtcNow.ToString("O") },
                    { "driver_id", "DRIVER_001" }
                });

                DisplayAlert("Success", "Delivery recording started", "OK");
            }
            else
            {
                DisplayAlert("Error", "Failed to start recording", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnCompleteDelivery(object sender, EventArgs e)
    {
        try
        {
            // Log completion metadata
            TrueMetricsHelper.AppendToMetadataTag(_currentDeliveryId, 
                "end_time", DateTime.UtcNow.ToString("O"));

            // Stop recording
            if (TrueMetricsHelper.StopRecording())
            {
                DisplayAlert("Success", "Delivery completed", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Optional: Stop recording when leaving page
        if (TrueMetricsHelper.IsRecording)
        {
            TrueMetricsHelper.StopRecording();
        }
    }
}
```

---

## Method Summary Table

| Category | Method/Property | Description |
|----------|----------------|-------------|
| **Init** | `Initialize(context, apiKey)` | Initialize with API key |
| | `Initialize(context, config)` | Initialize with custom config |
| | `IsInitialized` | Check if initialized |
| | `Shutdown()` | Cleanup and shutdown |
| **Recording** | `StartRecording()` | Start sensor recording |
| | `StopRecording()` | Stop recording |
| | `IsRecording` | Check if recording |
| | `IsRecordingStopped` | Check if stopped |
| | `RecordingStartTime` | Get start timestamp |
| **Device** | `DeviceId` | Get device ID |
| | `ActiveConfig` | Get active config |
| **Metadata** | `LogMetadata(dict)` | Log key-value data |
| | `LogMetadataByTag(tag)` | Log by tag name |
| | `AppendToMetadataTag(...)` | Append to existing tag |
| | `GetMetadataByTag(tag)` | Get metadata by tag |
| | `RemoveFromMetadataTag(...)` | Remove key from tag |
| | `RemoveMetadataTag(tag)` | Remove entire tag |
| | `ClearAllMetadata()` | Clear all metadata |
| | `MetadataTags` | Get all tag names |
| **Templates** | `CreateMetadataTemplate(...)` | Create template |
| | `GetMetadataTemplate(name)` | Get template |
| | `CreateMetadataFromTemplate(...)` | Create from template |
| | `RemoveMetadataTemplate(name)` | Remove template |
| | `MetadataTemplateNames` | Get template names |
| **Stats** | `SensorStatistics` | Get sensor stats |
| | `UploadStatistics` | Get upload stats |
| **Sensors** | `AllSensorsEnabled` | Enable/disable sensors |
| **Raw** | `Instance` | Access raw SDK |

---

## Building from Source

### Prerequisites
- Visual Studio 2022 with Mobile development workload
- .NET 6.0 or later SDK
- Java 11+

### Build Steps

```bash
cd /Users/dkumar/CascadeProjects/TrueMetrics.Xamarin

# Restore packages
dotnet restore TrueMetrics.Xamarin.Android/TrueMetrics.Xamarin.Android.csproj

# Build
msbuild TrueMetrics.Xamarin.Android/TrueMetrics.Xamarin.Android.csproj \
    /p:Configuration=Release /p:TargetFrameworkVersion=v13.0

# Pack
nuget pack TrueMetrics.Xamarin.Android/TrueMetrics.Xamarin.Android.nuspec \
    -OutputDirectory TrueMetrics.Xamarin.Android/bin/Release
```

Output:
```
TrueMetrics.Xamarin.Android/bin/Release/TrueMetrics.Xamarin.Android.1.5.4.nupkg
```

---

## Troubleshooting

### SDK Not Initialized
- Ensure `Initialize()` is called before any other method
- Pass valid Android `Context` (Activity or Application)

### Permissions Denied
- Check all permissions in AndroidManifest.xml
- Request runtime permissions for Android 6.0+:
  ```csharp
  // Request location permission at runtime
  ActivityCompat.RequestPermissions(this, 
      new[] { Manifest.Permission.AccessFineLocation }, 100);
  ```

### Recording Not Starting
- Check `IsInitialized` is true
- Verify API key is valid
- Check device has required sensors (GPS)

### Metadata Not Logging
- Ensure recording is active (`IsRecording` = true)
- Use valid string keys/values
- Check SDK status with `TrueMetricsHelper.Instance?.SdkStatus`

---

## Support

- **TrueMetrics Documentation**: https://docs.truemetrics.io
- **GitHub Issues**: https://github.com/TRUE-Metrics-io/truemetrics_android_SDK/issues
- **API Version**: 1.5.0 (AAR), 1.5.4 (Xamarin Binding)

## License

MIT License - See LICENSE file for details.

## Changelog

### 1.5.4
- Added persistent notification support for background recording
- Added `TrueMetricsNotificationBuilder` for foreground service notifications
- Added `TrueMetricsNotificationHelper` for notification channel management
- Added `InitializeWithNotification()` method for easy setup with notifications
- GitHub Actions CI/CD workflow for automated builds and releases

### 1.5.3
- Added comprehensive `TrueMetricsHelper` static class
- Wrapped all SDK public methods (DeviceId, Metadata, Templates, Statistics)
- Added `AllSensorsEnabled` property for sensor control
- Simplified usage to one-line static method calls

### 1.5.2
- Clean rebuild with minimal dependencies
- Removed transitive dependencies causing conflicts

### 1.5.1
- Fixed NuGet dependency compatibility
- Removed .NET 8-only dependencies

### 1.5.0
- Initial Xamarin.Android binding for TrueMetrics SDK 1.5.0
