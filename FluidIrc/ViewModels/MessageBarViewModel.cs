using Prism.Windows.Mvvm;
using System.Windows.Input;

namespace FluidIrc.ViewModels
{
    public class MessageBarViewModel : ViewModelBase
    {

        private string _commandText;
        public string CommandText
        {
            get => _commandText;
            set => SetProperty(ref _commandText, value);
        }

        private ICommand _sendCommand;
        public ICommand SendCommand
        {
            get => _sendCommand;
            set => SetProperty(ref _sendCommand, value);
        }

    }
}
