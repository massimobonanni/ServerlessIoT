using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ServerlessIoT.Core.Interfaces;
using ServerlessIoT.Core.Models;
using System;
using System.ComponentModel;
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
                SetProperty(ref this._deviceName, value);
                OnPropertyChanged(nameof(ViewTitle));
            }
        }

        private string _deviceId;
        public string DeviceId
        {
            get => this._deviceId;
            set
            {
                SetProperty(ref this._deviceId, value);
                OnPropertyChanged(nameof(ViewTitle));
            }
        }

        private string _retentionHistory;
        public string RetentionHistory
        {
            get => this._retentionHistory;
            set => SetProperty(ref this._retentionHistory, value);
        }

        private string _temperatureHighThreshold;
        public string TemperatureHighThreshold
        {
            get => this._temperatureHighThreshold;
            set => SetProperty(ref this._temperatureHighThreshold, value);
        }

        private string _temperatureLowThreshold;
        public string TemperatureLowThreshold
        {
            get => this._temperatureLowThreshold;
            set => SetProperty(ref this._temperatureLowThreshold, value);
        }

        private string _temperatureDecimalPrecision;
        public string TemperatureDecimalPrecision
        {
            get => this._temperatureDecimalPrecision;
            set => SetProperty(ref this._temperatureDecimalPrecision, value);
        }

        private string _humidityDecimalPrecision;
        public string HumidityDecimalPrecision
        {
            get => this._humidityDecimalPrecision;
            set => SetProperty(ref this._humidityDecimalPrecision, value);
        }

        private string _notificationNumber;
        public string NotificationNumber
        {
            get => this._notificationNumber;
            set => SetProperty(ref this._notificationNumber, value);
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

            int temperatureDecimalPrecision;
            if (!int.TryParse(this.TemperatureDecimalPrecision, out temperatureDecimalPrecision)
                || temperatureDecimalPrecision < 0)
            {
                temperatureDecimalPrecision = 2;
            }

            int humidityDecimalPrecision;
            if (!int.TryParse(this.HumidityDecimalPrecision, out humidityDecimalPrecision)
                || humidityDecimalPrecision < 0)
            {
                humidityDecimalPrecision = 2;
            }

            var configuration = new DeviceConfigurationModel()
            {
                HistoryRetention = TimeSpan.FromSeconds(retentionHistory),
                TemperatureHighThreshold = temperatureHighThreshold,
                TemperatureLowThreshold = temperatureLowThreshold,
                NotificationNumber = this.NotificationNumber,
                TemperatureDecimalPrecision = temperatureDecimalPrecision,
                HumidityDecimalPrecision = humidityDecimalPrecision
            };

            var updateResult = await this._deviceManager.SetDeviceConfigurationAsync(this.DeviceId, configuration, default);

            this.IsBusy = false;
            if (updateResult)
            {
                WeakReferenceMessenger.Default.Send(new CloseWindowMessage()
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
                    this.TemperatureDecimalPrecision = config.TemperatureDecimalPrecision.ToString();
                    this.TemperatureDecimalPrecision = config.TemperatureDecimalPrecision.ToString();
                    this.HumidityDecimalPrecision = config.HumidityDecimalPrecision.ToString();
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
