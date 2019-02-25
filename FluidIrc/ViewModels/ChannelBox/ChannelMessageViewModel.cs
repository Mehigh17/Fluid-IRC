using Prism.Windows.Mvvm;
using System;

namespace FluidIrc.ViewModels.ChannelBox
{
    public class ChannelMessageViewModel : ViewModelBase
    {

        public DateTime ReceivedAt { get; set; } = DateTime.Now;

    }
}
