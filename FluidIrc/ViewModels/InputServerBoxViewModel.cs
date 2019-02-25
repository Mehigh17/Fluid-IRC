using Prism.Windows.Mvvm;

namespace FluidIrc.ViewModels
{
    public class InputServerBoxViewModel : ViewModelBase
    {

        public string ServerName { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public bool EnableSsl { get; set; }
        public string Nickname { get; set; }
        
    }
}
