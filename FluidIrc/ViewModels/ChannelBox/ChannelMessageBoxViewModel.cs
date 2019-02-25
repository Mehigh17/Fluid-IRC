using Prism.Windows.Mvvm;
using System.Collections.ObjectModel;

namespace FluidIrc.ViewModels.ChannelBox
{
    public class ChannelMessageBoxViewModel : ViewModelBase
    {

        public ObservableCollection<ChannelMessageViewModel> Messages { get; set; } = new ObservableCollection<ChannelMessageViewModel>();

        public ChannelMessageBoxViewModel()
        {
            
        }

    }
}
