﻿using FluidIrc.ViewModels;
using Prism.Windows.Mvvm;
using System.ComponentModel;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FluidIrc.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChannelPage : SessionStateAwarePage, INotifyPropertyChanged
    {

        public ChannelPageViewModel ConcreteDataContext => DataContext as ChannelPageViewModel;
        public event PropertyChangedEventHandler PropertyChanged;

        public ChannelPage()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConcreteDataContext)));
        }
        
    }
}
