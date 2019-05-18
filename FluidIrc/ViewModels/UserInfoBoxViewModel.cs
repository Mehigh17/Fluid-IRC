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

        public UserInfoBoxViewModel()
        {

        }

    }
}
