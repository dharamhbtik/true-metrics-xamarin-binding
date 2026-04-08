using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrueMetricsDemo.Services;

namespace TrueMetricsDemo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }

    public class LogLevelToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogLevel level)
            {
                var resourceName = level switch
                {
                    LogLevel.Error => "LogEntryErrorStyle",
                    LogLevel.Success => "LogEntrySuccessStyle",
                    LogLevel.Warning => "LogEntryWarningStyle",
                    LogLevel.SdkCall or LogLevel.SdkResponse => "LogEntrySdkStyle",
                    _ => "LogEntryInfoStyle"
                };

                if (Application.Current.Resources.TryGetValue(resourceName, out var style))
                {
                    return (Style)style;
                }
            }

            return Application.Current.Resources["LogEntryInfoStyle"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
