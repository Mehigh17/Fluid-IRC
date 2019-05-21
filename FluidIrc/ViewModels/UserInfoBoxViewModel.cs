using IrcDotNet;
using Prism.Commands;
using System;

namespace FluidIrc.ViewModels
{
    public class UserInfoBoxViewModel : ExtendedViewModelBase
    {
        private string _nickname;
        private bool _isMuted;

        public string Nickname
        {
            get => _nickname;
            set => SetProperty(ref _nickname, value);
        }

        public bool IsMuted
        {
            get => _isMuted;
            set => SetProperty(ref _isMuted, value);
        }

        private DelegateCommand<UserInfoBoxViewModel> _ignoreCommand;
        public DelegateCommand<UserInfoBoxViewModel> IgnoreCommand
        {
            get => _ignoreCommand;
            set => SetProperty(ref _ignoreCommand, value);
        }

        private DelegateCommand<UserInfoBoxViewModel> _whoisCommand;
        public DelegateCommand<UserInfoBoxViewModel> WhoisCommand
        {
            get => _whoisCommand;
            set => SetProperty(ref _whoisCommand, value);
        }

        public IrcUser User { get; }

        public UserInfoBoxViewModel(IrcUser user)
        {
            User = user;
            user.NickNameChanged += UserOnNickNameChanged;
        }

        private void UserOnNickNameChanged(object sender, EventArgs e)
        {
            if (sender is IrcUser user)
            {
                ExecuteOnUiThread(() => Nickname = user.NickName);
            }
        }
    }
}
