using Prism.Windows.Mvvm;
using System.Collections.ObjectModel;

namespace FluidIrc.ViewModels
{
    public class UsersPanelViewModel : ViewModelBase
    {

        public ObservableCollection<string> ConnectedUsers { get; set; } = new ObservableCollection<string>();

    }
}
