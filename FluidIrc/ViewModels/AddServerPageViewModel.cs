using FluidIrc.Model.Providers;
using Prism.Commands;
using Prism.Windows.Mvvm;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FluidIrc.Model.Models;
using Server = FluidIrc.Model.Models.Server;

namespace FluidIrc.ViewModels
{

    public class AddServerPageViewModel : ViewModelBase
    {

        private InputServerBoxViewModel _inputServerBoxViewModel;
        public InputServerBoxViewModel InputServerBoxViewModel
        {
            get => _inputServerBoxViewModel;
            set => SetProperty(ref _inputServerBoxViewModel, value);
        }

        private ICommand _addCommand;
        public ICommand AddCommand
        {
            get => _addCommand;
            set => SetProperty(ref _addCommand, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private readonly IDataProvider _dataService;

        public AddServerPageViewModel(IDataProvider dataService)
        {
            _dataService = dataService;

            InputServerBoxViewModel = new InputServerBoxViewModel();
            
            AddCommand = new DelegateCommand(async () => await AddServer(), () => true);
        }

        private async Task AddServer()
        {
            var serverName = InputServerBoxViewModel.ServerName;
            var serverAddress = InputServerBoxViewModel.ServerAddress;
            var port = InputServerBoxViewModel.ServerPort;
            var enableSsl = InputServerBoxViewModel.EnableSsl;
            var nickname = InputServerBoxViewModel.Nickname;

            if (string.IsNullOrEmpty(serverName))
            {
                StatusMessage = "Please provide a server name.";
                return;
            }

            if (string.IsNullOrEmpty(serverAddress))
            {
                StatusMessage = "Please provide a server address.";
                return;
            }

            if (string.IsNullOrEmpty(nickname))
            {
                StatusMessage = "Please provide a nickname.";
                return;
            }

            var identity = new UserProfile
            {
                Id = Guid.NewGuid(),
                Nickname = nickname
            };

            var server = new Server
            {
                Id = Guid.NewGuid(),
                Name = serverName,
                Address = serverAddress,
                Port = port,
                SslEnabled = enableSsl,
                UserProfile = identity
            };

            var servers = await _dataService.GetServers();

            if (!servers.Any(s => s.Address.Equals(serverAddress, StringComparison.InvariantCultureIgnoreCase)))
            {
                await _dataService.AddServer(server);
                StatusMessage = "Server added successfully.";
            }
            else
            {
                StatusMessage = "A server with the same address already exists.";
            }
        }
        
    }
    
}
