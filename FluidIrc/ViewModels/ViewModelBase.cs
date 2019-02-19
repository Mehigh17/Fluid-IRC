using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluidIrc.Annotations;

namespace FluidIrc.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool Set<T>(ref T prop, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(prop, value))
                return false;

            prop = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}
