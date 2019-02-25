using Prism.Windows.Mvvm;
using System.Windows.Input;

namespace FluidIrc.ViewModels
{
    public class ServerCardViewModel : ViewModelBase
    {

        private string _serverName;
        public string ServerName
        {
            get => _serverName;
            set => SetProperty(ref _serverName, value);
        }


        private string _serverAddress;
        public string ServerAddress
        {
            get => _serverAddress;
            set => SetProperty(ref _serverAddress, value);
        }

        private ICommand _command;
        public ICommand Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get => _deleteCommand;
            set => SetProperty(ref _deleteCommand, value);
        }

    }
}
