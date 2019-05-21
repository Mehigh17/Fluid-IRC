using IrcDotNet;
using IrcDotNet.Collections;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FluidIrc.ViewModels
{
    public class UsersPanelViewModel : ExtendedViewModelBase
    {

        public ObservableCollection<UserInfoBoxViewModel> ConnectedUsers { get; set; } = new ObservableCollection<UserInfoBoxViewModel>();
        public IrcChannel Channel { get; }

        public UsersPanelViewModel(IrcChannel channel)
        {
            Channel = channel ?? throw new ArgumentException(nameof(channel));
            ConnectedUsers.CollectionChanged += ConnectedUsersOnCollectionChanged;

            channel.UserJoined += ChannelOnUserJoined;
            channel.UserLeft += ChannelOnUserLeft;
            channel.UsersListReceived += ChannelOnUsersListReceived;
        }

        private void ChannelOnUserLeft(object sender, IrcChannelUserEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                var vm = ConnectedUsers.FirstOrDefault(vmu => vmu.User == e.ChannelUser.User);
                if (vm != null)
                {
                    ConnectedUsers.Remove(vm);
                }
            });
        }

        private void ChannelOnUsersListReceived(object sender, EventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                var users = Channel.Users.Select(u => GetUserViewModel(u.User));
                ConnectedUsers.Clear();
                ConnectedUsers.AddRange(users);
            });
        }

        private void ChannelOnUserJoined(object sender, IrcChannelUserEventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                var vm = ConnectedUsers.FirstOrDefault(vmu => vmu.User == e.ChannelUser.User);
                if (vm == null)
                {
                    ConnectedUsers.Add(GetUserViewModel(e.ChannelUser.User));
                }
            });
        }

        private void ConnectedUsersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is IList<UserInfoBoxViewModel> newViewModels)
            {
                foreach (var userVm in newViewModels)
                {
                    userVm.User.Quit += (o, args) =>
                    {
                        ExecuteOnUiThread(() => ConnectedUsers.Remove(userVm));
                    };
                }
            }
        }

        private static UserInfoBoxViewModel GetUserViewModel(IrcUser user)
        {
            return new UserInfoBoxViewModel(user)
            {
                Nickname = user.NickName,
                IgnoreCommand = new DelegateCommand<UserInfoBoxViewModel>(v => { v.IsMuted = !v.IsMuted; }),
                WhoisCommand = new DelegateCommand<UserInfoBoxViewModel>(v =>
                {

                })
            };
        }
    }
}
