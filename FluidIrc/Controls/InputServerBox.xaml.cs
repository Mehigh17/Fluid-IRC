using System.Linq;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FluidIrc.Controls
{
    public sealed partial class InputServerBox : UserControl
    {
        public InputServerBox()
        {
            this.InitializeComponent();
        }

        /*
         * Prevent user to input any character besides numeric ones.
         */
        private void PortTextBox_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if(!string.IsNullOrEmpty(args.NewText))
                args.Cancel = !int.TryParse(args.NewText.Last().ToString(), out _);
        }
    }
}
