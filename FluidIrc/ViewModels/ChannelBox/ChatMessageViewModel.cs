namespace FluidIrc.ViewModels.ChannelBox
{
    public class ChatMessageViewModel : ChannelMessageViewModel
    {

        private string _senderName;
        public string SenderName
        {
            get => _senderName;
            set => SetProperty(ref _senderName, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

    }
}
