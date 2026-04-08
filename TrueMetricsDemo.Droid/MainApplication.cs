using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using TrueMetricsDemo.Services;
using Xamarin.Forms;

namespace TrueMetricsDemo.Droid
{
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            // Register Activity lifecycle callbacks
            RegisterActivityLifecycleCallbacks(this);

            // Initialize CrossCurrentActivity
            CrossCurrentActivity.Current.Init(this);

            // Initialize TrueMetrics SDK if needed at app level
            // Note: Actual initialization happens through the service
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Android.App.Activity activity, Android.OS.Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Android.App.Activity activity)
        {
        }

        public void OnActivityPaused(Android.App.Activity activity)
        {
        }

        public void OnActivityResumed(Android.App.Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Android.App.Activity activity, Android.OS.Bundle outState)
        {
        }

        public void OnActivityStarted(Android.App.Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Android.App.Activity activity)
        {
        }
    }
}
