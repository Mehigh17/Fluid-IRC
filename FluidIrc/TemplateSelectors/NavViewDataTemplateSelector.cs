using FluidIrc.ViewModels.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluidIrc.TemplateSelectors
{
    public class NavViewDataTemplateSelector : DataTemplateSelector
    {

        public DataTemplate GotoItemDataTemplate { get; set; }
        public DataTemplate ChannelDataTemplate { get; set; }
        public DataTemplate HeaderDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is NavigationGotoItemViewModel)
                return GotoItemDataTemplate;
            if (item is NavigationHeaderViewModel)
                return HeaderDataTemplate;
            if (item is NavigationChannelViewModel)
                return ChannelDataTemplate;

            return base.SelectTemplateCore(item);
        }
    }
}
