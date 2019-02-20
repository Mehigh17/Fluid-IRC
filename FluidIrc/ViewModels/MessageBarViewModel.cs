using System.Windows.Input;

namespace FluidIrc.ViewModels
{
    public class MessageBarViewModel : ViewModelBase
    {

        private string _commandText;
        public string CommandText
        {
            get => _commandText;
            set => Set(ref _commandText, value);
        }

        public ICommand SendCommand { get; set; }

        public MessageBarViewModel()
        {

        }

    }
}
