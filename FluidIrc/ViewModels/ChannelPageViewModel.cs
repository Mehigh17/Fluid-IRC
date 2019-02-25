using FluidIrc.Services;
using FluidIrc.ViewModels.ChannelBox;
using IrcDotNet;
using IrcDotNet.Collections;
using Prism.Commands;
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
        private IrcChannel _currentChannel;

        public ChannelPageViewModel(INavigationService navService, IIrcClient client)
        {
            _navService = navService;
            _client = client;
            _client.Disconnected += ClientOnDisconnected;
            _client.JoinedChannel += ClientOnJoinedChannel;
            _client.LeftChannel += ClientOnLeftChannel;
            _client.ProtocolError += ClientOnProtocolError;
            _client.WhoIsReplyReceived += ClientOnWhoIsReplyReceived;
            _client.WhoReplyReceived += ClientOnWhoReplyReceived;
            _client.WhoWasReplyReceived += ClientOnWhoIsReplyReceived;

            ChannelMessageBoxViewModel = new ChannelMessageBoxViewModel();
            UsersPanelViewModel = new UsersPanelViewModel();
            MessageBarViewModel = new MessageBarViewModel
            {
                SendCommand = new DelegateCommand(SendCommand)
            };
        }

        private void SendCommand()
        {
            var msg = MessageBarViewModel.CommandText;
            if (!string.IsNullOrEmpty(msg))
            {
                if (msg.StartsWith('/'))
                {
                    var cmdArgs = msg.Split(' ');
                    _client.SendCommand(cmdArgs[0].Remove(0, 1), cmdArgs.Skip(1).ToArray());
                }
                else
                {
                    var nickName = _client.CurrentServer.UserProfile.Nickname ?? "@You";
                    _client.SendMessage(_currentChannel, msg);
                    AddChatMessage(_currentChannel, nickName, msg);
                }

                MessageBarViewModel.CommandText = string.Empty;
            }
        }

        private void AddChatMessage(IrcChannel channel, string sender, string message)
        {
            if (_channelBoxInstances.TryGetValue(channel, out var vm))
            {
                ExecuteOnUiThread(() =>
                {
                    if (vm.Messages.LastOrDefault() is ChatMessageViewModel msg
                        && msg.SenderName.Equals(sender, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var msgString = new StringBuilder(msg.Message)
                            .AppendLine(message);
                        msg.Message = msgString.ToString();
                    }
                    else
                    {
                        vm.Messages.Add(new ChatMessageViewModel
                        {
                            SenderName = sender,
                            Message = new StringBuilder().AppendLine(message).ToString()
                        });
                    }
                });
            }
        }

        #region Event Handling

        private void ChannelOnUserLeft(object sender, IrcChannelUserEventArgs e)
        {
            if (sender is IrcChannel channel)
            {
                if (e.ChannelUser.User == _client.LocalUser)
                {
                    ClientOnLeftChannel(channel);
                }
                else
                {
                    if (_userPanelInstances.TryGetValue(channel, out var vm))
                    {
                        ExecuteOnUiThread(() => { vm.ConnectedUsers.Remove(e.ChannelUser.User.NickName); });
                    }

                    if (_channelBoxInstances.TryGetValue(channel, out var chatVm))
                    {
                        ExecuteOnUiThread(() =>
                        {
                            chatVm.Messages.Add(new NoticeViewModel
                            {
                                Notice = $"{e.ChannelUser.User.NickName} left the channel.",
                                ReceivedAt = DateTime.Now
                            });
                        });
                    }
                }
            }
        }

        private void ChannelOnUserJoined(object sender, IrcChannelUserEventArgs e)
        {
            if (sender is IrcChannel channel)
            {
                if (_userPanelInstances.TryGetValue(channel, out var userVm))
                {
                    ExecuteOnUiThread(() =>
                    {
                        userVm.ConnectedUsers.Add(e.ChannelUser.User.NickName);
                    });
                }

                if (_channelBoxInstances.TryGetValue(channel, out var chatVm))
                {
                    ExecuteOnUiThread(() =>
                    {
                        chatVm.Messages.Add(new NoticeViewModel
                        {
                            Notice = $"{e.ChannelUser.User.NickName} joined the channel."
                        });
                    });
                }
            }
        }

        private void ChannelOnNoticeReceived(object sender, IrcMessageEventArgs e)
        {
            if (sender is IrcChannel channel)
            {
                if (_channelBoxInstances.TryGetValue(channel, out var vm))
                {
                    ExecuteOnUiThread(() =>
                    {
                        vm.Messages.Add(new NoticeViewModel
                        {
                            Notice = e.Text
                        });
                    });
                }
            }
        }

        private void ChannelOnUsersListReceived(object sender, EventArgs e)
        {
            if (sender is IrcChannel channel)
            {
                if (_userPanelInstances.TryGetValue(channel, out var vm))
                {
                    ExecuteOnUiThread(() =>
                    {
                        var users = channel.Users.Select(u => u.User.NickName);
                        vm.ConnectedUsers.Clear();
                        vm.ConnectedUsers.AddRange(users);
                    });
                }
            }
        }

        private void ChannelOnMessageReceived(object sender, IrcMessageEventArgs e)
        {
            if (sender is IrcChannel channel)
            {
                AddChatMessage(channel, e.Source.Name, e.Text);
            }
        }

        private void ChannelOnModesChanged(object sender, IrcUserEventArgs e)
        {
            if (sender is IrcChannel channel)
            {
                if(channel.Modes.Count == 0)
                    return;
                
                var modes = string.Join(' ', channel.Modes);
                ExecuteOnUiThread(() =>
                {
                    if (_channelBoxInstances.TryGetValue(channel, out var vm))
                    {
                        vm.Messages.Add(new NoticeViewModel
                        {
                            Notice = $"The channel modes have been changed to: {modes}"
                        });
                    }
                });
            }
        }

        private void ClientOnJoinedChannel(IrcChannel channel)
        {
            if (_channelBoxInstances.ContainsKey(channel) || _userPanelInstances.ContainsKey(channel))
                return;

            _channelBoxInstances.Add(channel, new ChannelMessageBoxViewModel());
            _userPanelInstances.Add(channel, new UsersPanelViewModel());

            channel.MessageReceived += ChannelOnMessageReceived;
            channel.NoticeReceived += ChannelOnNoticeReceived;
            channel.UserJoined += ChannelOnUserJoined;
            channel.UserLeft += ChannelOnUserLeft;
            channel.UserKicked += ChannelOnUserLeft;
            channel.UsersListReceived += ChannelOnUsersListReceived;
            channel.ModesChanged += ChannelOnModesChanged;
            channel.TopicChanged += ChannelOnTopicChanged;
        }

        private void ChannelOnTopicChanged(object sender, IrcUserEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                if (sender is IrcChannel channel)
                {
                    if (_channelBoxInstances.TryGetValue(channel, out var vm))
                    {
                        var topicMsg = channel.Topic ?? "No topic.";
                        vm.Messages.Add(new NoticeViewModel
                        {
                            Notice = $"Channel topic: {topicMsg}"
                        });
                    }
                }
            });
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

            channel.MessageReceived -= ChannelOnMessageReceived;
            channel.NoticeReceived -= ChannelOnNoticeReceived;
            channel.UserJoined -= ChannelOnUserJoined;
            channel.UserLeft -= ChannelOnUserLeft;
            channel.UserKicked -= ChannelOnUserLeft;
            channel.UsersListReceived -= ChannelOnUsersListReceived;
            channel.ModesChanged -= ChannelOnModesChanged;

            if (_currentChannel != null && _currentChannel.Equals(channel))
            {
                ExecuteOnUiThread(() =>
                {
                    _navService.Navigate(PageTokens.Home.ToString(), null);
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
                    _channelBoxInstances.Add(channel, new ChannelMessageBoxViewModel());
                }

                if (!_userPanelInstances.ContainsKey(channel))
                {
                    _userPanelInstances.Add(channel, new UsersPanelViewModel());
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
