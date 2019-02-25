using FluidIrc.ViewModels.ChannelBox;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluidIrc.TemplateSelectors
{
    public class ChannelMessageDataTemplateSelector : DataTemplateSelector
    {

        public DataTemplate ChatMessageDataTemplate { get; set; }
        public DataTemplate NoticeDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ChatMessageViewModel)
                return ChatMessageDataTemplate;
            if (item is NoticeViewModel)
                return NoticeDataTemplate;

            return base.SelectTemplateCore(item);
        }
    }
}
