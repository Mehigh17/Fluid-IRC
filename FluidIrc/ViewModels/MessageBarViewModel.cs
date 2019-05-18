using Prism.Commands;
using Prism.Windows.Mvvm;

namespace FluidIrc.ViewModels
{
    public class MessageBarViewModel : ViewModelBase
    {

        private DelegateCommand<string> _sendCommand;
        public DelegateCommand<string> SendCommand
        {
            get => _sendCommand;
            set => SetProperty(ref _sendCommand, value);
        }

    }
}
