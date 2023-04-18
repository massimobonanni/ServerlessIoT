using ServerlessIoT.Core.Interfaces;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using TelemetryDashboard.WPF.Messages;
using TelemetryDashboard.WPF.Models;
using DeviceTelemetryModel = TelemetryDashboard.WPF.Models.DeviceTelemetryModel;
using DeviceInfoModel = TelemetryDashboard.WPF.Models.DeviceInfoModel;
using System.Configuration;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace TelemetryDashboard.WPF.ViewModels
{

    public class MainViewModel : ViewModelBase
    {
        private readonly BackgroundWorker _devicesUpdateWorker;
        private readonly IDeviceManager _deviceManager;

        public MainViewModel(IDeviceManager deviceManager)
        {
            this._deviceManager = deviceManager;
            _devicesUpdateWorker = new BackgroundWorker();
            this.SearchPanelVisible = true;
            _devicesUpdateWorker.WorkerSupportsCancellation = true;
            _devicesUpdateWorker.DoWork += DevicesUpdateWorker_DoWork;
        }

        #region [ Properties ]
        private DeviceInfoModel _selectedDevice;
        public DeviceInfoModel SelectedDevice
        {
            get => this._selectedDevice;
            set
            {
                SetProperty(ref _selectedDevice, value);
                this.ShowTelemetryCommand.NotifyCanExecuteChanged();
                this.ConfigureDeviceCommand.NotifyCanExecuteChanged();
                this.DeviceMethodCommand.NotifyCanExecuteChanged();
            }
        }

        private ObservableCollection<DeviceInfoModel> _devices;
        public ObservableCollection<DeviceInfoModel> Devices
        {
            get => this._devices;
            set => SetProperty(ref _devices, value);
        }

        private ObservableCollection<DeviceTelemetryModel> _deviceTelemetries;
        public ObservableCollection<DeviceTelemetryModel> DeviceTelemetries
        {
            get => this._deviceTelemetries;
            set => SetProperty(ref _deviceTelemetries, value);
        }

        public string ViewTitle
        {
            get => $"Telemetry Dashboard - [{App.Parameters.APIUrl}]";
        }

        private string _telemetryDeviceID;
        public string TelemetryDeviceID
        {
            get => _telemetryDeviceID;
            set => SetProperty(ref _telemetryDeviceID, value);
        }

        private string _telemetryDeviceName;
        public string TelemetryDeviceName
        {
            get => _telemetryDeviceName;
            set => SetProperty(ref _telemetryDeviceName, value);
        }

        private DateTimeOffset _telemetryDeviceLastUpdate;
        public DateTimeOffset TelemetryDeviceLastUpdate
        {
            get => _telemetryDeviceLastUpdate;
            set => SetProperty(ref _telemetryDeviceLastUpdate, value);
        }

        private string _deviceNameFilter;
        public string DeviceNameFilter
        {
            get => _deviceNameFilter;
            set => SetProperty(ref _deviceNameFilter, value);
        }

        private bool _searchPanelVisible;
        public bool SearchPanelVisible
        {
            get => _searchPanelVisible;
            set 
            {
                SetProperty(ref _searchPanelVisible, value);
                this.OnPropertyChanged(nameof(DevicePanelVisible));
            }

        }

        public bool DevicePanelVisible
        {
            get => !_searchPanelVisible;
        }

        #endregion [ Properties ]

        #region [ Commands ]
        private RelayCommand _searchDeviceCommand;
        public RelayCommand SearchDeviceCommand
        {
            get
            {
                if (_searchDeviceCommand == null)
                {
                    _searchDeviceCommand = new RelayCommand(
                        async () => await SearchDeviceAsync());
                }
                return _searchDeviceCommand;
            }
        }

        private RelayCommand _showTelemetryCommand;
        public RelayCommand ShowTelemetryCommand
        {
            get
            {
                if (_showTelemetryCommand == null)
                {
                    _showTelemetryCommand = new RelayCommand(
                        () =>
                        {
                            this.TelemetryDeviceID = this.SelectedDevice.DeviceInfo?.DeviceId;
                            this.TelemetryDeviceName = this.SelectedDevice.DeviceInfo?.DeviceName;
                            this.TelemetryDeviceLastUpdate = this.SelectedDevice.DeviceInfo?.LastTelemetry == null ? DateTimeOffset.MinValue : this.SelectedDevice.DeviceInfo.LastTelemetry.Timestamp;
                            this.SearchPanelVisible = false;
                            this._devicesUpdateWorker.RunWorkerAsync();
                        },
                        () =>
                        {
                            return this.SelectedDevice != null;
                        });
                }
                return _showTelemetryCommand;
            }
        }

        private RelayCommand _configureDeviceCommand;
        public RelayCommand ConfigureDeviceCommand
        {
            get
            {
                if (_configureDeviceCommand == null)
                {
                    _configureDeviceCommand = new RelayCommand(
                        () =>
                        {
                            WeakReferenceMessenger.Default.Send(new OpenWindowMessage()
                            {
                                WindowToOpen = WindowNames.DeviceConfiguration,
                                Parameter = this.SelectedDevice.DeviceInfo
                            });
                        },
                        () =>
                        {
                            return this.SelectedDevice != null;
                        });
                }
                return _configureDeviceCommand;
            }
        }

        private RelayCommand _deviceMethodCommand;
        public RelayCommand DeviceMethodCommand
        {
            get
            {
                if (_deviceMethodCommand == null)
                {
                    _deviceMethodCommand = new RelayCommand(
                        () =>
                        {
                            WeakReferenceMessenger.Default.Send(new OpenWindowMessage()
                            {
                                WindowToOpen = WindowNames.DeviceMethod,
                                Parameter = this.SelectedDevice.DeviceInfo
                            });
                        },
                        () =>
                        {
                            return this.SelectedDevice != null;
                        });
                }
                return _deviceMethodCommand;
            }
        }

        private RelayCommand _hideTelemetryCommand;
        public RelayCommand HideTelemetryCommand
        {
            get
            {
                if (_hideTelemetryCommand == null)
                {
                    _hideTelemetryCommand = new RelayCommand(
                        () =>
                        {
                            this._devicesUpdateWorker.CancelAsync();
                            this.SearchPanelVisible = true;

                        });
                }
                return _hideTelemetryCommand;
            }
        }
        #endregion [ Commands ]

        #region [ Internal Methods ]
        private async Task SearchDeviceAsync()
        {
            this.IsBusy = true;
            this.SelectedDevice = null;
            var devices = await this._deviceManager.GetDevicesAsync(this.DeviceNameFilter, null, default);
            if (devices != null)
            {
                this.Devices = new ObservableCollection<DeviceInfoModel>(
                        devices
                            .OrderBy(d => d.DeviceId)
                            .Select(d => new DeviceInfoModel(d))
                    );
            }
            else
            {
                this.Devices = null;
            }
            this.IsBusy = false;
        }

        #endregion [ Internal Methods ]

        #region [ Background worker for telemetry ]
        private async void DevicesUpdateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var selectedDevice = this.SelectedDevice;
            while (!worker.CancellationPending)
            {
                Dispatcher.CurrentDispatcher.Invoke(() => this.IsBusy = true);

                var device = await this._deviceManager.GetDeviceAsync(SelectedDevice.DeviceInfo?.DeviceId, default);

                Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        if (device != null)
                        {
                            this.TelemetryDeviceLastUpdate = device.LastTelemetry == null ? DateTimeOffset.MinValue.ToLocalTime() : device.LastTelemetry.Timestamp.ToLocalTime();
                            this.TelemetryDeviceName = device.DeviceName;

                            if (device.TelemetryHistory != null)
                            {
                                var orderedTelemetry = device.TelemetryHistory
                                    .Select(t => new DeviceTelemetryModel()
                                    {
                                        Timestamp = t.Timestamp.LocalDateTime,
                                        Humidity = t.Humidity,
                                        Temperature = t.Temperature
                                    })
                                    .OrderByDescending(t => t.Timestamp);

                                this.DeviceTelemetries = new ObservableCollection<DeviceTelemetryModel>(orderedTelemetry);
                            }
                            else
                                this.DeviceTelemetries = null;
                        }
                        this.IsBusy = false;
                    }
                );

                var startWait = DateTime.Now;
                while (DateTime.Now.Subtract(startWait) < GetTelemetriesPolling && !worker.CancellationPending)
                {
                    await Task.Delay(250);
                }
            }
            Dispatcher.CurrentDispatcher.Invoke(() => this.IsBusy = false);
        }

        private TimeSpan GetTelemetriesPolling = TimeSpan.FromSeconds(App.Parameters.PollingTimeInSec);
        #endregion [ Background worker for telemetry ]

    }
}