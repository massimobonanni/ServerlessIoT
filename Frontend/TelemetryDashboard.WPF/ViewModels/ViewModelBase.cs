using System;

namespace TelemetryDashboard.WPF.ViewModels
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {

        public ViewModelBase()
        {
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }
    }
}
