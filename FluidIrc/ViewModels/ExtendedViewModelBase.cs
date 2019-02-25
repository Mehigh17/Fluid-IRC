using Prism.Windows.Mvvm;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace FluidIrc.ViewModels
{
    public class ExtendedViewModelBase : ViewModelBase
    {

        protected virtual void ExecuteOnUiThread(Action action)
        {
            Task.Factory.StartNew(async () =>
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { action?.Invoke(); });
            }).Wait();
        }

    }
}
