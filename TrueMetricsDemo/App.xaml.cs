using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using TrueMetricsDemo.Services;
using TrueMetricsDemo.ViewModels;
using TrueMetricsDemo.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TrueMetricsDemo
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            var logService = Container.Resolve<ILogService>();
            logService.LogInfo("Application initialized");

            var result = await NavigationService.NavigateAsync("NavigationPage/MainPage");
            if (!result.Success)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation failed: {result.Exception?.Message}");
                logService.LogError($"Navigation failed: {result.Exception?.Message}");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register Services
            containerRegistry.RegisterSingleton<ILogService, LogService>();
            containerRegistry.RegisterSingleton<ITrueMetricsService, TrueMetricsService>();

            // Register Navigation Pages
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            var logService = Container.Resolve<ILogService>();
            logService.LogInfo("Application started");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            var logService = Container.Resolve<ILogService>();
            logService.LogInfo("Application sleeping");
        }

        protected override void OnResume()
        {
            base.OnResume();
            var logService = Container.Resolve<ILogService>();
            logService.LogInfo("Application resumed");
        }
    }
}
