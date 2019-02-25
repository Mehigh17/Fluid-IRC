using FluidIrc.Model.Models;
using FluidIrc.Model.Providers;
using FluidIrc.Services;
using FluidIrc.ViewModels.ChannelBox;
using IrcDotNet;
using IrcDotNet.Collections;
using Prism.Commands;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FluidIrc.ViewModels
{
    public class HomePageViewModel : ExtendedViewModelBase
    {

        public ObservableCollection<ServerCardViewModel> ServerCards { get; set; } = new ObservableCollection<ServerCardViewModel>();
        public ChannelMessageBoxViewModel ChannelMessageBoxViewModel { get; set; } = new ChannelMessageBoxViewModel();
        public MessageBarViewModel MessageBarViewModel { get; set; } = new MessageBarViewModel();

        private bool _showProgress;
        public bool ShowProgress
        {
            get => _showProgress;
            set => SetProperty(ref _showProgress, value);
        }

        private readonly IIrcClient _client;
        private readonly IDataProvider _dataProvider;

        public HomePageViewModel(IIrcClient client, IDataProvider dataProvider, INavigationService navService)
        {
            _client = client;
            _dataProvider = dataProvider;

            _client.Connected += ClientOnConnected;
            _client.Disconnected += ClientOnDisconnected;
            _client.Error += ClientOnError;
            _client.ErrorMessageReceived += ClientOnErrorMessageReceived;
            _client.ConnectFailed += ClientOnError;
            _client.Registered += ClientOnRegistered;
            _client.MotdReceived += ClientOnMotdReceived;
            _client.ProtocolError += ClientOnProtocolError;

            ShowProgress = false;
            MessageBarViewModel.SendCommand = new DelegateCommand(SendCommand);
        }

        private void ClientOnDisconnected(object sender, EventArgs e)
        {
            _client.LocalUser.NoticeReceived -= LocalUserOnNoticeReceived;
            _client.LocalUser.MessageReceived -= LocalUserOnMessageReceived;
        }

        private void LocalUserOnMessageReceived(object sender, IrcMessageEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                ChannelMessageBoxViewModel.Messages.Add(new NoticeViewModel
                {
                    Notice = $"{e.Source.Name}: {e.Text}"
                });
            });
        }

        private void ClientOnProtocolError(object sender, IrcProtocolErrorEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                ChannelMessageBoxViewModel.Messages.Add(new NoticeViewModel
                {
                    Notice = $"{e.Message} ({e.Code})"
                });
            });
        }

        private void ClientOnMotdReceived(object sender, EventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                ChannelMessageBoxViewModel.Messages.Add(new NoticeViewModel
                {
                    Notice = _client.MessageOfTheDay
                });
            });
        }
        
        private void ClientOnErrorMessageReceived(object sender, IrcErrorMessageEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                ShowProgress = false;
                ChannelMessageBoxViewModel.Messages.Add(new NoticeViewModel
                {
                    Notice = e.Message
                });
            });
        }

        private void ClientOnError(object sender, IrcErrorEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                ShowProgress = false;
                ChannelMessageBoxViewModel.Messages.Add(new NoticeViewModel
                {
                    Notice = e.Error.Message
                });
            });
        }

        private void ClientOnRegistered(object sender, EventArgs e)
        {
            ExecuteOnUiThread(() => { ShowProgress = false; });
        }

        private void ClientOnConnected(object sender, EventArgs e)
        {
            _client.LocalUser.NoticeReceived += LocalUserOnNoticeReceived;
            _client.LocalUser.MessageReceived += LocalUserOnMessageReceived;

            var msg = new StringBuilder()
                .AppendLine($"You are identified as {_client.CurrentServer.UserProfile.Nickname}")
                .AppendLine("Connected to the server, please wait for the registration to complete...")
                .ToString();

            ExecuteOnUiThread(() =>
            {
                ChannelMessageBoxViewModel.Messages.Add(new NoticeViewModel
                {
                    Notice = msg
                });
            });
        }

        private void SendCommand()
        {
            ExecuteOnUiThread(() =>
            {
                var msg = MessageBarViewModel.CommandText;
                if (!string.IsNullOrEmpty(msg))
                {
                    var cmdArgs = msg.Split(' ');
                    if (cmdArgs[0].StartsWith("/"))
                        cmdArgs[0] = cmdArgs[0].Remove(0, 1);

                    _client.SendCommand(cmdArgs[0], cmdArgs.Skip(1).ToArray());
                    MessageBarViewModel.CommandText = string.Empty;
                }
            });
        }

        private void LocalUserOnNoticeReceived(object sender, IrcMessageEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                ChannelMessageBoxViewModel.Messages.Add(new NoticeViewModel
                {
                    Notice = $"{e.Source.Name}: {e.Text}",
                    ReceivedAt = DateTime.Now
                });
            });
        }

        private void ConnectToServer(Server server)
        {
            if (_client.IsConnecting || _client.IsConnected)
            {
                ExecuteOnUiThread(() =>
                {
                    ChannelMessageBoxViewModel.Messages.Add(new NoticeViewModel
                    {
                        Notice = $"You are already connected to a server ({server.Address.ToUpper()}). Please quit before connecting to another one."
                    });
                });
            }
            else
            {
                ShowProgress = true;
                
                _client.Connect(server);
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            var serversTask = _dataProvider.GetServers();
            serversTask.Wait();
            var servers = serversTask.Result;

            var serverCardViewModels = servers.Select(BuildServerCardViewModel).ToList();

            ServerCards.Clear();
            ServerCards.AddRange(serverCardViewModels);
        }

        private ServerCardViewModel BuildServerCardViewModel(Server s)
        {
            var vm = new ServerCardViewModel
            {
                ServerName = s.Name,
                ServerAddress = s.Port <= 0 ? s.Address : $"{s.Address}:{s.Port}",
                Command = new DelegateCommand(() => ConnectToServer(s), () => true),

            };

            vm.DeleteCommand = new DelegateCommand(() =>
            {
                _dataProvider.RemoveServer(s);
                ExecuteOnUiThread(() => { ServerCards.Remove(vm); });
            });

            return vm;
        }

    }
}
