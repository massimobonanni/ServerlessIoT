using GalaSoft.MvvmLight.CommandWpf;
using ServerlessIoT.Core.Interfaces;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetryDashboard.WPF.Interfaces;
using TelemetryEntities.Models;

namespace TelemetryDashboard.WPF.ViewModels
{
    public class DeviceConfigurationViewModel : ViewModelBase, IInitializableObject, IContextSettableObject
    {
        private readonly IDeviceManager _deviceManager;

        public DeviceConfigurationViewModel(IDeviceManager deviceManager)
        {
            this._deviceManager = deviceManager;
        }



        #region [ Properties ]
        public string ViewTitle
        {
            get => $"Device Configuration - [{this.DeviceName}]";
        }

        private string _deviceName;
        public string DeviceName
        {
            get => this._deviceName;
            set
            {
                _deviceName = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(ViewTitle));
            }
        }

        private string _deviceId;
        public string DeviceId
        {
            get => this._deviceId;
            set
            {
                _deviceId = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(ViewTitle));
            }
        }

        private int _retentionHistory;
        public int RetentionHistory
        {
            get => this._retentionHistory;
            set
            {
                _retentionHistory = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion [ Properties ]

        #region [ Commands ]
        private RelayCommand _updateConfigurationCommand;
        public RelayCommand UpdateConfigurationCommand
        {
            get
            {
                if (_updateConfigurationCommand == null)
                {
                    _updateConfigurationCommand = new RelayCommand(
                        async () => await UpdateConfigurationDeviceAsync());
                }
                return _updateConfigurationCommand;
            }
        }
        #endregion [ Commands ]

        private async Task UpdateConfigurationDeviceAsync()
        {
            this.IsBusy = true;
            var configuration = new DeviceConfigurationModel()
            {
                HistoryRetention = TimeSpan.FromSeconds(this.RetentionHistory)
            };

            await this._deviceManager.SetDeviceConfigurationAsync(this.DeviceId, configuration, default);

            this.IsBusy = false;
        }

        public Task InitializeAsync(object sender, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<bool> SetContextAsync(object sender, object context, CancellationToken cancellationToken)
        {
            var device = context as DeviceInfoModel;
            if (device != null)
            {
                this.DeviceName = device.DeviceName;
                this.DeviceId = device.DeviceId;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}
