using FluidIrc.ViewModels;
using FluidIrc.ViewModels.Navigation;
using Prism.Windows.Mvvm;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FluidIrc
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShell : SessionStateAwarePage, INotifyPropertyChanged
    {

        public AppShell(AppShellViewModel viewModel)
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            DataContext = viewModel;
        }

        public void SetContentFrame(Frame frame)
        {
            ContentFrame.Content = frame;
        }

        public AppShellViewModel ConcreteDataContext => DataContext as AppShellViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConcreteDataContext)));
        }

        private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {

            }
            else
            {
                if (args.InvokedItemContainer is NavigationViewItem item)
                {
                    if (item.DataContext is NavigationGotoItemViewModel gotoVm)
                    {
                        gotoVm.Command?.Execute(null);
                    }
                    else if (item.DataContext is NavigationChannelViewModel channelVm)
                    {
                        channelVm.Command?.Execute(null);
                    }
                }
            }
        }
    }
}
