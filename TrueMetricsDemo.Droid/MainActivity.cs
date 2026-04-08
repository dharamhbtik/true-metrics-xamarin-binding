using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Prism;
using Prism.Ioc;
using TrueMetricsDemo.Droid;
using TrueMetricsDemo.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace TrueMetricsDemo.Droid
{
    [Activity(
        Label = "TrueMetrics Demo",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            // Initialize Xamarin.Essentials
            Platform.Init(this, savedInstanceState);

            // Initialize Forms
            Forms.Init(this, savedInstanceState);

            // Load the application with Prism
            LoadApplication(new App(new AndroidInitializer()));

            // Log startup
            var logService = (App.Current as App)?.Container.Resolve<ILogService>();
            logService?.LogInfo($"MainActivity created on Android {Build.VERSION.Release}");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnStart()
        {
            base.OnStart();
            var logService = (App.Current as App)?.Container.Resolve<ILogService>();
            logService?.LogDebug("MainActivity started");
        }

        protected override void OnResume()
        {
            base.OnResume();
            var logService = (App.Current as App)?.Container.Resolve<ILogService>();
            logService?.LogDebug("MainActivity resumed");
        }

        protected override void OnPause()
        {
            base.OnPause();
            var logService = (App.Current as App)?.Container.Resolve<ILogService>();
            logService?.LogDebug("MainActivity paused");
        }

        protected override void OnStop()
        {
            base.OnStop();
            var logService = (App.Current as App)?.Container.Resolve<ILogService>();
            logService?.LogDebug("MainActivity stopped");
        }

        protected override void OnDestroy()
        {
            var logService = (App.Current as App)?.Container.Resolve<ILogService>();
            logService?.LogInfo("MainActivity destroyed");
            base.OnDestroy();
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform-specific implementations here
            // For example, if you have Android-specific services:
            // containerRegistry.Register<IAndroidNotificationService, AndroidNotificationService>();
        }
    }
}
