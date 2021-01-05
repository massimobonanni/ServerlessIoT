using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ServerlessIoT.Core.Interfaces;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TelemetryDashboard.WPF.ViewModels
{

    public class MainViewModel : ViewModelBase
    {
        private readonly BackgroundWorker _devicesUpdateWorker;
        private readonly IDeviceManager _deviceManager;

        public MainViewModel(IDeviceManager deviceManager)
        {
            this._deviceManager = deviceManager;
            this.SearchPanelVisible = !this.IsInDesignMode;
            _devicesUpdateWorker = new BackgroundWorker();
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
                _selectedDevice = value;
                this.RaisePropertyChanged();
                this.ShowTelemetryCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<DeviceInfoModel> _devices;
        public ObservableCollection<DeviceInfoModel> Devices
        {
            get => this._devices;
            set
            {
                _devices = value;
                this.RaisePropertyChanged();
            }
        }

        private ObservableCollection<DeviceTelemetryModel> _deviceTelemetries;
        public ObservableCollection<DeviceTelemetryModel> DeviceTelemetries
        {
            get => this._deviceTelemetries;
            set
            {
                _deviceTelemetries = value;
                this.RaisePropertyChanged();
            }
        }

        private string _telemetryDeviceID;
        public string TelemetryDeviceID
        {
            get => _telemetryDeviceID;
            set
            {
                _telemetryDeviceID = value;
                this.RaisePropertyChanged();
            }
        }

        private string _telemetryDeviceName;
        public string TelemetryDeviceName
        {
            get => _telemetryDeviceName;
            set
            {
                _telemetryDeviceName = value;
                this.RaisePropertyChanged();
            }
        }

        private DateTimeOffset _telemetryDeviceLastUpdate;
        public DateTimeOffset TelemetryDeviceLastUpdate
        {
            get => _telemetryDeviceLastUpdate;
            set
            {
                _telemetryDeviceLastUpdate = value;
                this.RaisePropertyChanged();
            }
        }

        private string _deviceNameFilter;
        public string DeviceNameFilter
        {
            get => _deviceNameFilter;
            set
            {
                _deviceNameFilter = value;
                this.RaisePropertyChanged();
            }
        }

        private bool _searchPanelVisible;
        public bool SearchPanelVisible
        {
            get => _searchPanelVisible;
            set
            {
                _searchPanelVisible = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(DevicePanelVisible));
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
                            this.TelemetryDeviceID = this.SelectedDevice.DeviceId;
                            this.TelemetryDeviceName = this.SelectedDevice.DeviceName;
                            this.TelemetryDeviceLastUpdate = this.SelectedDevice.LastTelemetry.Timestamp;
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
                    devices.OrderBy(d => d.DeviceId));
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

                var device = await this._deviceManager.GetDeviceAsync(SelectedDevice.DeviceId, default);

                Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        if (device != null)
                        {
                            this.TelemetryDeviceLastUpdate = device.LastTelemetry.Timestamp;
                            this.TelemetryDeviceName = device.DeviceName;

                            var orderedTelemetry = device.TelemetryHistory.OrderByDescending(t => t.Timestamp);
                            this.DeviceTelemetries = new ObservableCollection<DeviceTelemetryModel>(orderedTelemetry);
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

        private TimeSpan GetTelemetriesPolling = TimeSpan.FromSeconds(5);
        #endregion [ Background worker for telemetry ]

    }
}