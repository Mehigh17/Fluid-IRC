using FluidIrc.Services;
using FluidIrc.ViewModels.ChannelBox;
using IrcDotNet;
using Prism.Commands;
using Prism.Events;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidIrc.ViewModels
{
    public class ChannelPageViewModel : ExtendedViewModelBase
    {

        private ChannelMessageBoxViewModel _channelMessageBoxViewModel;
        public ChannelMessageBoxViewModel ChannelMessageBoxViewModel
        {
            get => _channelMessageBoxViewModel;
            set => SetProperty(ref _channelMessageBoxViewModel, value);
        }

        private UsersPanelViewModel _usersPanelViewModel;
        public UsersPanelViewModel UsersPanelViewModel
        {
            get => _usersPanelViewModel;
            set => SetProperty(ref _usersPanelViewModel, value);
        }

        private MessageBarViewModel _messageBarViewModel;
        public MessageBarViewModel MessageBarViewModel
        {
            get => _messageBarViewModel;
            set => SetProperty(ref _messageBarViewModel, value);
        }

        private readonly Dictionary<IrcChannel, ChannelMessageBoxViewModel> _channelBoxInstances =
            new Dictionary<IrcChannel, ChannelMessageBoxViewModel>();

        private readonly Dictionary<IrcChannel, UsersPanelViewModel> _userPanelInstances =
            new Dictionary<IrcChannel, UsersPanelViewModel>();

        private readonly INavigationService _navService;
        private readonly IIrcClient _client;
        private readonly IEventAggregator _eventAggregator;
        private IrcChannel _currentChannel;

        public string CurrentChannelName => _currentChannel.Name;

        public ChannelPageViewModel(INavigationService navService, IIrcClient client, IEventAggregator eventAggregator)
        {
            _navService = navService;
            _client = client;
            _eventAggregator = eventAggregator;

            _client.Disconnected += ClientOnDisconnected;
            _client.JoinedChannel += ClientOnJoinedChannel;
            _client.LeftChannel += ClientOnLeftChannel;
            _client.ProtocolError += ClientOnProtocolError;
            _client.WhoIsReplyReceived += ClientOnWhoIsReplyReceived;
            _client.WhoReplyReceived += ClientOnWhoReplyReceived;
            _client.WhoWasReplyReceived += ClientOnWhoIsReplyReceived;

            MessageBarViewModel = new MessageBarViewModel
            {
                SendCommand = new DelegateCommand<string>(SendCommand)
            };
        }

        private void SendCommand(string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                if (command.StartsWith('/'))
                {
                    var cmdArgs = command.Split(' ');
                    _client.SendCommand(cmdArgs[0].Remove(0, 1), cmdArgs.Skip(1).ToArray());
                }
                else
                {
                    var nickName = _client.CurrentServer.UserProfile.Nickname ?? "@You";
                    _client.SendMessage(_currentChannel, command);
                    if (_channelBoxInstances.TryGetValue(_currentChannel, out var vm))
                    {
                        vm.AddUserMessage(nickName, command);
                    }
                }
            }
        }

        #region Event Handling
        private void ClientOnJoinedChannel(IrcChannel channel)
        {
            if (_channelBoxInstances.ContainsKey(channel) || _userPanelInstances.ContainsKey(channel))
                return;

            _channelBoxInstances.Add(channel, new ChannelMessageBoxViewModel(channel, _eventAggregator));
            _userPanelInstances.Add(channel, new UsersPanelViewModel(channel, _eventAggregator));
        }

        private void ClientOnLeftChannel(IrcChannel channel)
        {
            if (_channelBoxInstances.ContainsKey(channel))
            {
                _channelBoxInstances.Remove(channel);
            }

            if (_userPanelInstances.ContainsKey(channel))
            {
                _channelBoxInstances.Remove(channel);
            }

            if (_currentChannel != null && _currentChannel.Equals(channel))
            {
                ExecuteOnUiThread(() =>
                {
                    if (_navService.CanGoBack())
                    {
                        _navService.GoBack();
                    }
                    else
                    {
                        _navService.ClearHistory();
                        _navService.Navigate(PageTokens.Home.ToString(), null);
                    }
                });
            }
        }

        private void ClientOnDisconnected(object sender, EventArgs e)
        {
            _channelBoxInstances.Clear();
            _userPanelInstances.Clear();
        }

        private void ClientOnWhoReplyReceived(object sender, IrcNameEventArgs e)
        {
            if (_currentChannel != null)
            {
                ExecuteOnUiThread(() =>
                {
                    if (_channelBoxInstances.TryGetValue(_currentChannel, out var vm))
                    {
                        vm.Messages.Add(new NoticeViewModel
                        {
                            Notice = e.Name
                        });
                    }
                });
            }
        }

        private void ClientOnWhoIsReplyReceived(object sender, IrcUserEventArgs e)
        {
            if (_currentChannel != null)
            {
                ExecuteOnUiThread(() =>
                {
                    if (_channelBoxInstances.TryGetValue(_currentChannel, out var vm))
                    {
                        var whoIsMsg = new StringBuilder()
                            .AppendLine($"Nickname: {e.User.NickName}")
                            .AppendLine($"Username: {e.User.UserName}")
                            .AppendLine($"Online: {e.User.IsOnline.ToString()}")
                            .AppendLine($"Operator: {e.User.IsOperator.ToString()}")
                            .AppendLine($"Away: {e.User.IsAway.ToString()}")
                            .AppendLine($"Idle duration: {e.User.IdleDuration:g}");

                        if (e.User.IsAway)
                        {
                            whoIsMsg.AppendLine($"Away message: {e.User.AwayMessage}");
                        }

                        vm.Messages.Add(new NoticeViewModel
                        {
                            Notice = whoIsMsg.ToString()
                        });
                    }
                });
            }
        }

        private void ClientOnProtocolError(object sender, IrcProtocolErrorEventArgs e)
        {
            if (_currentChannel != null)
            {
                ExecuteOnUiThread(() =>
                {
                    if (_channelBoxInstances.TryGetValue(_currentChannel, out var vm))
                    {
                        vm.Messages.Add(new NoticeViewModel
                        {
                            Notice = $"{e.Message} ({e.Code})"
                        });
                    }
                });
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            if (e.Parameter is IrcChannel channel)
            {
                if (!_channelBoxInstances.ContainsKey(channel))
                {
                    _channelBoxInstances.Add(channel, new ChannelMessageBoxViewModel(channel, _eventAggregator));
                }

                if (!_userPanelInstances.ContainsKey(channel))
                {
                    _userPanelInstances.Add(channel, new UsersPanelViewModel(channel, _eventAggregator));
                }

                if (_channelBoxInstances.TryGetValue(channel, out var channelVm))
                {
                    ChannelMessageBoxViewModel = channelVm;
                }

                if (_userPanelInstances.TryGetValue(channel, out var userVm))
                {
                    UsersPanelViewModel = userVm;
                }

                _currentChannel = channel;
            }
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            base.OnNavigatingFrom(e, viewModelState, suspending);
            _currentChannel = null;
        }

        #endregion Event Handling

    }
}
