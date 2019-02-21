using System.Collections.ObjectModel;

namespace FluidIrc.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {

        public ObservableCollection<ServerCardViewModel> ServerCards { get; set; } = new ObservableCollection<ServerCardViewModel>();

        public HomePageViewModel()
        {
            
        }

    }
}
