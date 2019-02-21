using System.Windows.Input;
using Prism.Commands;
using Prism.Windows.Mvvm;

namespace FluidIrc.ViewModels
{

    public class AddServerViewModel : ViewModelBase
    {

        public InputServerBoxViewModel InputServerBoxViewModel { get; set; } = new InputServerBoxViewModel();

        public ICommand AddCommand { get; set; }

        public AddServerViewModel()
        {
            AddCommand = new DelegateCommand(() =>
            {
                // Add server to the db context
            }, IsServerFormValid);
        }

        private bool IsServerFormValid()
        {
            return !string.IsNullOrEmpty(InputServerBoxViewModel.ServerName)
                   && !string.IsNullOrEmpty(InputServerBoxViewModel.ServerAddress)
                   && InputServerBoxViewModel.ServerPort > 0;
        }

    }
    
}
