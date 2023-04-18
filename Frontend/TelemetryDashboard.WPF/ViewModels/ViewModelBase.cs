using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace TelemetryDashboard.WPF.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {

        public ViewModelBase()
        {
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
    }
}
