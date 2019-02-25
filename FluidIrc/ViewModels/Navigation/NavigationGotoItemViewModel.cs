using System.Windows.Input;

namespace FluidIrc.ViewModels.Navigation
{
    public class NavigationGotoItemViewModel : NavigationItemViewModel
    {

        public string FontIcon { get; set; }

        private ICommand _command;
        public ICommand Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

    }
}
