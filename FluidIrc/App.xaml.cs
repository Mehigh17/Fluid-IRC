using FluidIrc.Model.Data;
using FluidIrc.Model.Providers;
using FluidIrc.Services;
using FluidIrc.ViewModels;
using Microsoft.EntityFrameworkCore;
using Prism.Unity.Windows;
using System.Threading.Tasks;
using Unity;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluidIrc
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = Container.Resolve<AppShell>();
            shell.SetContentFrame(rootFrame);
            return shell;
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            //Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            Container.RegisterSingleton<IApplicationContextFactory, ApplicationContextFactory>();
            Container.RegisterSingleton<IIrcClient, FluidIrcClient>();
            Container.RegisterSingleton<IDataProvider, DataProvider>();
            Container.RegisterInstance(Container.Resolve<ChannelPageViewModel>());

            var contextFactory = Container.Resolve<IApplicationContextFactory>();
            using (var context = contextFactory.CreateApplicationContext())
            {
                if (context is ApplicationDbContext appContext)
                    appContext.Database.Migrate();
            }

            //return base.OnInitializeAsync(args);
            return Task.FromResult(true);
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate(PageTokens.Home.ToString(), null);
            return Task.FromResult(true);
        }
    }
}
