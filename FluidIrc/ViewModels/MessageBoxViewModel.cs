using System.Collections.ObjectModel;
using FluidIrc.ViewModels.ChannelBox;

namespace FluidIrc.ViewModels
{
    public class MessageBoxViewModel : ExtendedViewModelBase
    {

        public ObservableCollection<ChannelMessageViewModel> Messages { get; set; } = new ObservableCollection<ChannelMessageViewModel>();

    }
}
