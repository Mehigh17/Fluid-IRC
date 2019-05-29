using FluidIrc.Events;
using IrcDotNet;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidIrc.ViewModels.ChannelBox
{
    public class ChannelMessageBoxViewModel : MessageBoxViewModel
    {

        public IrcChannel Channel { get; }

        private IEventAggregator _eventAggregator;
        private readonly List<IrcUser> _mutedUsers = new List<IrcUser>();

        public ChannelMessageBoxViewModel(IrcChannel channel, IEventAggregator eventAggregator)
        {
            Channel = channel ?? throw new ArgumentException(nameof(channel));
            _eventAggregator = eventAggregator ?? throw new ArgumentException(nameof(eventAggregator)); ;

            channel.UserJoined += ChannelOnUserJoined;
            channel.MessageReceived += ChannelOnMessageReceived;
            channel.NoticeReceived += ChannelOnNoticeReceived;
            channel.UserLeft += ChannelOnUserLeft;
            channel.UserKicked += ChannelOnUserKicked;
            channel.ModesChanged += ChannelOnModesChanged;
            channel.TopicChanged += ChannelOnTopicChanged;

            _eventAggregator.GetEvent<UserMutedEvent>().Subscribe((args) =>
            {
                if (!_mutedUsers.Contains(args.User))
                {
                    _mutedUsers.Add(args.User);
                }
            });
            _eventAggregator.GetEvent<UserUnmutedEvent>().Subscribe((args) =>
            {
                if (_mutedUsers.Contains(args.User))
                {
                    _mutedUsers.Remove(args.User);
                }
            });
        }

        public void AddUserMessage(string nickname, string message)
        {
            var msgBuilder = new StringBuilder();
            if (Messages.Count > 0 && Messages.Last() is ChatMessageViewModel vm && vm.SenderName.Equals(nickname))
            {
                msgBuilder = new StringBuilder(vm.Message);
                msgBuilder.AppendLine(message);
                vm.Message = msgBuilder.ToString();
            }
            else
            {
                msgBuilder.AppendLine(message);
                Messages.Add(new ChatMessageViewModel
                {
                    SenderName = nickname,
                    Message = msgBuilder.ToString(),
                    ReceivedAt = DateTime.Now
                });
            }
        }

        private void ChannelOnTopicChanged(object sender, IrcUserEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                var topicMsg = Channel.Topic ?? "No topic.";
                Messages.Add(new NoticeViewModel
                {
                    Notice = $"Channel topic: {topicMsg}"
                });
            });
        }

        private void ChannelOnModesChanged(object sender, IrcUserEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                var modes = string.Join(' ', Channel.Modes);
                Messages.Add(new NoticeViewModel
                {
                    Notice = $"The channel modes have been changed to: {modes}",
                    ReceivedAt = DateTime.Now
                });
            });
        }

        private void ChannelOnUserKicked(object sender, IrcChannelUserEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                Messages.Add(new NoticeViewModel
                {
                    Notice = $"{e.ChannelUser.User.NickName} has been kicked. (${e.Comment})",
                    ReceivedAt = DateTime.Now
                });
            });
        }

        private void ChannelOnUserLeft(object sender, IrcChannelUserEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                var noticeBuilder = new StringBuilder()
                    .Append($"{e.ChannelUser.User.NickName} has left the channel.");
                if (!string.IsNullOrEmpty(e.Comment))
                {
                    noticeBuilder.Append($" ({e.Comment})");
                }
                Messages.Add(new NoticeViewModel
                {
                    Notice = noticeBuilder.ToString(),
                    ReceivedAt = DateTime.Now
                });
            });
        }

        private void ChannelOnNoticeReceived(object sender, IrcMessageEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                Messages.Add(new NoticeViewModel
                {
                    Notice = e.Text,
                    ReceivedAt = DateTime.Now
                });
            });
        }

        private void ChannelOnMessageReceived(object sender, IrcMessageEventArgs e)
        {
            if (_mutedUsers.FirstOrDefault(u => u.NickName.Equals(e.Source.Name)) == null)
            {
                ExecuteOnUiThread(() =>
                {
                    AddUserMessage(e.Source.Name, e.Text);
                });
            }
        }

        private void ChannelOnUserJoined(object sender, IrcChannelUserEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                var noticeBuilder = new StringBuilder()
                    .Append($"{e.ChannelUser.User.NickName} has joined the channel.");

                if (!string.IsNullOrEmpty(e.Comment))
                {
                    noticeBuilder.Append($" ({e.Comment})");
                }

                Messages.Add(new NoticeViewModel
                {
                    Notice = noticeBuilder.ToString(),
                    ReceivedAt = DateTime.Now
                });
            });
        }
    }
}
