using FluidIrc.ViewModels;
using FluidIrc.ViewModels.Navigation;
using FluidIrc.Views;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navService;

        public AppShell(AppShellViewModel viewModel, INavigationService navService, IEventAggregator eventAggregator)
        {
            _navService = navService ?? throw new ArgumentNullException(nameof(navService));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            eventAggregator.GetEvent<NavigationStateChangedEvent>().Subscribe(OnNavigationStateChanged);
        }

        public void SetContentFrame(Frame frame)
        {
            ContentFrame.Content = frame;
        }

        public AppShellViewModel ConcreteDataContext => DataContext as AppShellViewModel;

        private void OnNavigationStateChanged(NavigationStateChangedEventArgs args)
        {
            var source = NavView.MenuItemsSource as ObservableCollection<NavigationItemViewModel>;
            if (args.Sender.Content is HomePage)
            {
                NavView.SelectedItem = source[0];
            }
            else if(args.Sender.Content is AddServerPage)
            {
                NavView.SelectedItem = source[1];
            }
            else if(args.Sender.Content is ChannelPage p)
            {
                var vm = p.ConcreteDataContext;
                NavView.SelectedItem = source.FirstOrDefault(i => i.Label.Equals(vm.CurrentChannelName));
            }
        }

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
