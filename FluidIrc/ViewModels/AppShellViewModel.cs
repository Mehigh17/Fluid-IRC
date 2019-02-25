using FluidIrc.Services;
using FluidIrc.ViewModels.Navigation;
using IrcDotNet;
using Prism.Commands;
using Prism.Windows.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FluidIrc.ViewModels
{
    public class AppShellViewModel : ExtendedViewModelBase
    {

        public ObservableCollection<NavigationItemViewModel> NavItems { get; set; } = new ObservableCollection<NavigationItemViewModel>();

        private readonly INavigationService _navigationService;
        private readonly IIrcClient _client;

        public AppShellViewModel(IIrcClient client, INavigationService navigationService)
        {
            _navigationService = navigationService;
            _client = client;
            
            _client.Connected += ClientOnConnected;
            _client.Disconnected += ClientOnDisconnected;
            _client.JoinedChannel += ClientOnJoinedChannel;
            _client.LeftChannel += ClientOnLeftChannel;

            NavItems.Add(new NavigationGotoItemViewModel
            {
                Label = "Home",
                FontIcon = "Home",
                Command = new DelegateCommand(() =>
                {
                    _navigationService.Navigate(PageTokens.Home.ToString(), null);
                })
            });

            NavItems.Add(new NavigationGotoItemViewModel
            {
                Label = "Add Server",
                FontIcon = "Add",
                Command = new DelegateCommand(() =>
                {
                    _navigationService.Navigate(PageTokens.AddServer.ToString(), null);
                })
            });
        }

        private void ClientOnDisconnected(object sender, EventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                var items = NavItems.Where(i => i is NavigationChannelViewModel || i is NavigationHeaderViewModel).ToList();
                items.ForEach(i => NavItems.Remove(i));

                _navigationService.Navigate(PageTokens.Home.ToString(), null);
            });
        }

        private void ClientOnConnected(object sender, EventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                NavItems.Add(new NavigationHeaderViewModel
                {
                    Label = _client.CurrentServer.Name.ToUpper()
                });
            });
        }

        private void ClientOnLeftChannel(IrcChannel channel)
        {
            ExecuteOnUiThread(() =>
            {
                NavItems.Remove(NavItems.FirstOrDefault(vm =>
                    vm.Label.Equals(channel.Name, StringComparison.InvariantCultureIgnoreCase)));
            });
        }

        private void ClientOnJoinedChannel(IrcChannel channel)
        {
            ExecuteOnUiThread(() =>
            {
                NavItems.Add(new NavigationChannelViewModel
                {
                    Label = channel.Name,
                    Command = new DelegateCommand(() =>
                    {
                        _navigationService.Navigate(PageTokens.Channel.ToString(), channel);
                    })
                });
            });
        }

    }
}
