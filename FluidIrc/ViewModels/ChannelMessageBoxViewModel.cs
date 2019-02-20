using System.Collections.ObjectModel;

namespace FluidIrc.ViewModels
{
    public class ChannelMessageBoxViewModel : ViewModelBase
    {

        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public ChannelMessageBoxViewModel()
        {
            
        }

    }
}
