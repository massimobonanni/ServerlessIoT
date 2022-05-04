using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ServerlessIoT.Core.Interfaces;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetryDashboard.WPF.Interfaces;
using TelemetryDashboard.WPF.Messages;
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

        private string _retentionHistory;
        public string RetentionHistory
        {
            get => this._retentionHistory;
            set
            {
                _retentionHistory = value;
                this.RaisePropertyChanged();
            }
        }

        private string _temperatureHighThreshold;
        public string TemperatureHighThreshold
        {
            get => this._temperatureHighThreshold;
            set
            {
                _temperatureHighThreshold = value;
                this.RaisePropertyChanged();
            }
        }

        private string _temperatureLowThreshold;
        public string TemperatureLowThreshold
        {
            get => this._temperatureLowThreshold;
            set
            {
                _temperatureLowThreshold = value;
                this.RaisePropertyChanged();
            }
        }

        private string _notificationNumber;
        public string NotificationNumber
        {
            get => this._notificationNumber;
            set
            {
                _notificationNumber = value;
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

            if (!int.TryParse(this.RetentionHistory, out var retentionHistory))
                retentionHistory = 60;

            double? temperatureHighThreshold = null;
            if (double.TryParse(this.TemperatureHighThreshold, out var threshold1))
            {
                temperatureHighThreshold = threshold1;
            }

            double? temperatureLowThreshold = null;
            if (double.TryParse(this.TemperatureLowThreshold, out var threshold2))
            {
                temperatureLowThreshold = threshold2;
            }

            var configuration = new DeviceConfigurationModel()
            {
                HistoryRetention = TimeSpan.FromSeconds(retentionHistory),
                TemperatureHighThreshold = temperatureHighThreshold,
                TemperatureLowThreshold = temperatureLowThreshold,
                NotificationNumber = this.NotificationNumber
            };

            var updateResult = await this._deviceManager.SetDeviceConfigurationAsync(this.DeviceId, configuration, default);

            this.IsBusy = false;
            if (updateResult)
            {
                Messenger.Default.Send(new CloseWindowMessage()
                {
                    WindowToClose = WindowNames.DeviceConfiguration,
                    Parameter = null
                });
            }
        }

        public Task InitializeAsync(object sender, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<bool> SetContextAsync(object sender, object context, CancellationToken cancellationToken)
        {
            var device = context as DeviceInfoModel;
            if (device != null)
            {
                this.DeviceName = device.DeviceName;
                this.DeviceId = device.DeviceId;

                var config = await this._deviceManager.GetDeviceConfigurationAsync(device.DeviceId, cancellationToken);

                if (config != null)
                {
                    this.NotificationNumber = config.NotificationNumber;
                    this.TemperatureHighThreshold = config.TemperatureHighThreshold.HasValue ? config.TemperatureHighThreshold.ToString() : null;
                    this.TemperatureLowThreshold = config.TemperatureLowThreshold.HasValue ? config.TemperatureLowThreshold.ToString() : null;
                    this.RetentionHistory = config.HistoryRetention.TotalSeconds.ToString();
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
